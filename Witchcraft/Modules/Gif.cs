namespace Witchcraft.Modules;

// A slightly modified version of the gif loader found here: https://github.com/DigiWorm0/LevelImposter
public class Gif(string name) : UObject, IDisposable
{
    private static readonly Color[] DEFAULT_COLOR_TABLE =
    [
        new(0, 0, 0, 0),
        new(1, 1, 1, 1)
    ];

    // LZW Decoder
    private static readonly ushort[][] _codeTable = new ushort[1 << 12][]; // Table of "code"s to color indexes
    private readonly Color _backgroundColor = Color.clear; // Background color

    // Logical Screen Descriptor
    private Color[] _globalColorTable = DEFAULT_COLOR_TABLE; // Table of indexes to colors
    private int _globalColorTableSize; // Size of the global color table
    private bool _hasGlobalColorTable; // True if there is a global color table

    // Other Data
    private Color[]? _pixelBuffer; // Buffer of pixel colors

    // GIF File
    public bool IsLoaded { get; private set; }
    public string Name { get; private set; } = name;

    // Graphic Control Extension
    private GIFGraphicsControl? _lastGraphicsControl { get; set; }

    // Image Descriptor
    public ushort Width { get; private set; }
    public ushort Height { get; private set; }
    public List<GIFFrame> Frames { get; private set; } = [];

    public void Dispose()
    {
        _pixelBuffer = null;

        foreach (var frame in Frames)
        {
            if (frame.RenderedSprite?.texture)
                frame.RenderedSprite!.texture.Destroy();

            if (frame.RenderedSprite)
                frame.RenderedSprite!.Destroy();

            frame.IndexStream = null;
        }
    }

    public void Load(Stream dataStream)
    {
        using var reader = new BinaryReader(dataStream);
        IsLoaded = false;
        ReadHeader(reader);
        ReadDescriptor(reader);
        ReadGlobalColorTable(reader);

        while (ReadBlock(reader));

        _pixelBuffer = null;
        IsLoaded = true;
    }

    public Sprite GetFrameSprite(int frameIndex)
    {
        if (!IsLoaded)
            throw new Exception("GIF file is not loaded");

        if (frameIndex < 0 || frameIndex >= Frames.Count)
            throw new IndexOutOfRangeException("Frame index out of range");

        var frame = Frames[frameIndex];

        if (!frame.IsRendered)
            RenderFrame(frameIndex);

        if (!frame.RenderedSprite)
            throw new NullReferenceException("Frame sprite is null");

        return frame.RenderedSprite!;
    }

    private void ReadHeader(BinaryReader reader)
    {
        // Header
        var isGIF = new string(reader.ReadChars(3)) == "GIF";

        if (!isGIF)
            throw new Exception("File is not a GIF");

        // Version
        var version = new string(reader.ReadChars(3));

        if (version is not ("89a" or "87a"))
            throw new Exception("File is not a GIF89a or GIF87a");
    }

    private void ReadDescriptor(BinaryReader reader)
    {
        // Logical Screen Descriptor
        var width = reader.ReadUInt16();
        var height = reader.ReadUInt16();

        var packedField = reader.ReadByte();
        var hasGlobalColorTable = (packedField & 0b10000000) != 0;
        var globalColorTableSize = 1 << ((packedField & 0b00000111) + 1);

        reader.ReadByte(); // Background Color Index
        reader.ReadByte(); // Pixel Aspect Ratio

        // GIFData
        _hasGlobalColorTable = hasGlobalColorTable;
        _globalColorTableSize = globalColorTableSize;

        Width = width;
        Height = height;
        Frames = [];
    }

    private void ReadGlobalColorTable(BinaryReader reader)
    {
        if (!_hasGlobalColorTable)
            return;

        // Global Color Table
        var globalColorTable = new Color[_globalColorTableSize];

        for (var i = 0; i < _globalColorTableSize; i++)
        {
            var r = reader.ReadByte();
            var g = reader.ReadByte();
            var b = reader.ReadByte();

            globalColorTable[i] = new Color(r / 255f, g / 255f, b / 255f);
        }

        _globalColorTable = globalColorTable;
    }

    private bool ReadBlock(BinaryReader reader)
    {
        var blockType = reader.ReadByte();

        switch (blockType)
        {
            case 0x21:
            {
                ReadExtension(reader);
                return true;
            }
            case 0x2C:
            {
                ReadImageBlock(reader);
                return true;
            }
            case 0x3B:
            {
                // End of File
                return false;
            }
            default:
            {
                throw new Exception("Invalid block type " + blockType);
            }
        }
    }

    private void ReadExtension(BinaryReader reader)
    {
        var extensionLabel = reader.ReadByte();

        switch (extensionLabel)
        {
            case 0xF9: // Graphic Control Extension
            {
                // Block Size
                var blockSize = reader.ReadByte();

                if (blockSize != 4)
                    throw new Exception("Invalid block size " + blockSize);

                var packedField = reader.ReadByte();
                var disposalMethod = (FrameDisposalMethod)((packedField & 0b00011100) >> 2);
                var transparentColorFlag = (packedField & 0b00000001) != 0;
                var delay = reader.ReadUInt16() / 100f;
                var transparentColorIndex = reader.ReadByte();

                // Block Terminator
                var blockTerminator = reader.ReadByte();

                if (blockTerminator != 0)
                    throw new Exception("Invalid block terminator " + blockTerminator);

                // GIFGraphicsControl
                _lastGraphicsControl = new()
                {
                    Delay = delay,
                    DisposalMethod = disposalMethod,
                    TransparentColorFlag = transparentColorFlag,
                    TransparentColorIndex = transparentColorIndex
                };
                break;
            }
            case 0xFF: // Application Extension
            case 0xFE: // Comment Extension
            case 0x01: // Plain Text Extension
            {
                while (true)
                {
                    var subBlockSize = reader.ReadByte();

                    if (subBlockSize == 0)
                        break;

                    reader.BaseStream.Position += subBlockSize; // Skip Over Data
                }

                break;
            }
            default:
            {
                throw new Exception("Invalid extension label " + extensionLabel);
            }
        }
    }

    private void ReadImageBlock(BinaryReader reader)
    {
        // Image Descriptor
        var imageLeftPosition = reader.ReadUInt16();
        var imageTopPosition = reader.ReadUInt16();
        var imageWidth = reader.ReadUInt16();
        var imageHeight = reader.ReadUInt16();

        var packedField = reader.ReadByte();
        var hasLocalColorTable = (packedField & 0b10000000) != 0;
        var interlaceFlag = (packedField & 0b01000000) != 0;
        var sortFlag = (packedField & 0b00100000) != 0;
        var localColorTableSize = 1 << ((packedField & 0b00000111) + 1);

        if (interlaceFlag)
            throw new NotImplementedException("Interlaced GIFs are not implemented");

        // Local Color Table
        var localColorTable = new Color[localColorTableSize];

        if (hasLocalColorTable)
        {
            for (var i = 0; i < localColorTableSize; i++)
            {
                var r = reader.ReadByte();
                var g = reader.ReadByte();
                var b = reader.ReadByte();

                var color = new Color(r / 255f, g / 255f, b / 255f);
                localColorTable[i] = color;
            }
        }

        // Image Data
        var minCodeSize = reader.ReadByte();

        // Get Block Length
        long byteLength = 0;
        var bytePosition = reader.BaseStream.Position;

        while (true)
        {
            var subBlockSize = reader.ReadByte(); // Read Sub Block

            if (subBlockSize == 0) // End of Image Data
                break;

            byteLength += subBlockSize;
            reader.BaseStream.Position += subBlockSize;
        }

        // Get Block Data
        var byteData = new byte[byteLength];
        reader.BaseStream.Position = bytePosition;
        bytePosition = 0;

        while (true)
        {
            var subBlockSize = reader.ReadByte(); // Read Sub Block

            if (subBlockSize == 0) // End of Image Data
                break;

            reader.Read(byteData, (int)bytePosition, subBlockSize);
            bytePosition += subBlockSize;
        }

        // Decode LZW
        var indexStream = DecodeLZW(byteData, minCodeSize, imageWidth * imageHeight);

        // GIFFrame
        var frame = new GIFFrame()
        {
            GraphicsControl = _lastGraphicsControl,
            HasLocalColorTable = hasLocalColorTable,
            LocalColorTable = localColorTable,
            InterlaceFlag = interlaceFlag,
            SortFlag = sortFlag,

            LeftPosition = imageLeftPosition,
            TopPosition = imageTopPosition,
            Width = imageWidth,
            Height = imageHeight,

            IndexStream = indexStream
        };

        Frames.Add(frame);
        _lastGraphicsControl = null;
    }

    private List<ushort> DecodeLZW(byte[] byteBuffer, byte minCodeSize, int expectedSize)
    {
        var clearCode = 1 << minCodeSize; // Code used to clear the code table
        var endOfInformationCode = clearCode + 1; // Code used to signal the end of the image data

        var codeTableIndex = endOfInformationCode + 1; // The next index in the code table
        var codeSize = minCodeSize + 1; // The size of the codes in bits
        var previousCode = -1; // The previous code

        var indexStream = new List<ushort>(expectedSize); // The index stream

        // Initialize Code Table
        for (ushort k = 0; k < codeTableIndex; k++)
            _codeTable[k] = k < clearCode ? [ k ] : [];

        // Decode LZW
        var i = 0;

        while (i + codeSize < byteBuffer.Length * 8)
        {
            // Read Code
            var code = 0;

            for (var j = 0; j < codeSize; j++)
                code |= GetBit(byteBuffer, i + j) ? 1 << j : 0;

            i += codeSize;

            // Special Codes
            if (code == clearCode)
            {
                // Reset LZW
                codeTableIndex = endOfInformationCode + 1;
                codeSize = minCodeSize + 1;
                previousCode = -1;
                continue;
            }

            if (code == endOfInformationCode)
                // End of Information
                break;

            if (previousCode == -1)
            {
                // Initial Code
                indexStream.AddRange(_codeTable[code]);
                previousCode = code;
                continue;
            }

            // Compare to Code Table
            if (code < codeTableIndex)
            {
                // Get New Code
                var currentCodeArray = _codeTable[code];
                var previousCodeArray = _codeTable[previousCode];
                var newCode = new ushort[previousCodeArray.Length + 1];
                previousCodeArray.CopyTo(newCode, 0);
                newCode[^1] = currentCodeArray[0];

                // Add to Index Stream
                indexStream.AddRange(currentCodeArray);

                // Add to Code Table
                if (codeTableIndex < _codeTable.Length)
                    _codeTable[codeTableIndex] = newCode;
            }
            else
            {
                // Get New Code
                var previousCodeArray = _codeTable[previousCode];
                var newCode = new ushort[previousCodeArray.Length + 1];
                previousCodeArray.CopyTo(newCode, 0);
                newCode[^1] = previousCodeArray[0];

                // Add to Index Stream
                indexStream.AddRange(newCode);

                // Add to Code Table
                if (codeTableIndex < _codeTable.Length)
                    _codeTable[codeTableIndex] = newCode;
            }

            // Increase Code Table Index
            codeTableIndex++;

            // Update Previous Code
            previousCode = code;

            // Increase Code Size
            if (codeTableIndex >= 1 << codeSize && codeSize < 12)
                codeSize++;
        }

        // Fill in the rest of the index stream
        while (indexStream.Count < expectedSize)
            indexStream.Add(0);

        // Free Memory
        for (var k = endOfInformationCode + 1; k < _codeTable.Length; k++)
            _codeTable[k] = null!;

        return indexStream;
    }

    private static bool GetBit(byte[] arr, int index)
    {
        var byteIndex = index / 8;
        var bitIndex = index % 8;
        return (arr[byteIndex] & (1 << bitIndex)) != 0;
    }

    public void RenderAllFrames() => RenderFrame(Frames.Count - 1);

    public void RenderFrame(int frameIndex)
    {
        if (!IsLoaded)
            throw new Exception("GIF is not loaded");

        if (frameIndex < 0 || frameIndex >= Frames.Count)
            throw new Exception($"Frame index {frameIndex} is out of range");

        // Create pixel buffer
        if (_pixelBuffer == null)
        {
            _pixelBuffer = new Color[Width * Height];

            for (var i = 0; i < _pixelBuffer.Length; i++)
                _pixelBuffer[i] = _backgroundColor;
        }

        // Render all frames up to the target frame
        for (var i = 0; i <= frameIndex; i++)
        {
            // Frame
            var frame = Frames[i];

            if (frame.IsRendered) // Skip rendered frames
                continue;

            if (frame.IndexStream == null)
                throw new Exception($"Frame {i} index stream is null");

            var graphicsControl = frame.GraphicsControl;

            // Create temp pixel buffer
            Color[]? tempBuffer = null;

            if (frame.DisposalMethod == FrameDisposalMethod.RestoreToPrevious)
            {
                tempBuffer = new Color[_pixelBuffer.Length];
                _pixelBuffer.CopyTo(tempBuffer, 0);
            }

            // Create frame texture
            var texture = new Texture2D(Width, Height, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
                requestedMipmapLevel = 0
            }.DontDestroyOrUnload();

            // Get frame data
            var colorTable = frame.HasLocalColorTable ? frame.LocalColorTable : _globalColorTable;
            var x = frame.LeftPosition;
            var y = frame.TopPosition;
            var w = frame.Width;
            var h = frame.Height;

            // Loop through pixels
            for (var o = 0; o < w * h; o++)
            {
                var colorIndex = frame.IndexStream[o];

                // Skip transparent pixels
                if (graphicsControl != null && graphicsControl.TransparentColorFlag && colorIndex == graphicsControl.TransparentColorIndex)
                    continue;

                // Calculate pixel index based on frame position
                var newX = o % w;
                var newY = o / w;
                var pixelIndex = (Height - 1 - (y + newY)) * Width + x + newX;

                // Set pixel color
                var color = colorTable![colorIndex];
                _pixelBuffer[pixelIndex] = color;
            }

            // Free memory
            frame.IndexStream = null;

            // Apply Texture
            texture.SetPixels(_pixelBuffer);
            texture.Apply(false, true); // Remove from CPU memory

            // Handle frame disposal
            switch (frame.DisposalMethod)
            {
                case FrameDisposalMethod.RestoreToPrevious:
                {
                    // Restore previous buffer
                    if (tempBuffer != null)
                        _pixelBuffer = tempBuffer;

                    break;
                }
                case FrameDisposalMethod.RestoreToBackgroundColor:
                {
                    // Fill pixel buffer with background color
                    for (var o = 0; o < w * h; o++)
                    {
                        // Calculate pixel index based on frame position
                        var newX = o % w;
                        var newY = o / w;
                        var pixelIndex = (Height - 1 - (y + newY)) * Width + x + newX;

                        // Set pixel color
                        _pixelBuffer[pixelIndex] = _backgroundColor;
                    }

                    break;
                }
                // Do nothing otherwise
            }

            // Generate Sprite
            var sprite = Sprite.Create(texture, new(0, 0, texture.width, texture.height), new(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect).DontDestroyOrUnload();

            // Apply to frame
            frame.RenderedSprite = sprite;
        }

        // If last frame, free memory
        if (frameIndex >= Frames.Count - 1)
        {
            _pixelBuffer = null;
            _lastGraphicsControl = null;
            _globalColorTable = DEFAULT_COLOR_TABLE;
        }
    }
}

public class GIFGraphicsControl
{
    public float Delay { get; set; } // seconds
    public FrameDisposalMethod DisposalMethod { get; set; }
    public bool TransparentColorFlag { get; set; }
    public int TransparentColorIndex { get; set; }
}

public class GIFFrame : UObject
{
    // Graphic Control Extension
    public GIFGraphicsControl? GraphicsControl { get; set; }

    public float Delay => GraphicsControl?.Delay ?? 0;

    public FrameDisposalMethod DisposalMethod => GraphicsControl?.DisposalMethod ?? FrameDisposalMethod.DoNotDispose;

    public bool IsRendered => RenderedSprite;

    // Image Descriptor
    public Color[]? LocalColorTable { get; set; }
    public bool HasLocalColorTable { get; set; }
    public bool InterlaceFlag { get; set; }
    public bool SortFlag { get; set; }

    public int LeftPosition { get; set; }
    public int TopPosition { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public List<ushort>? IndexStream { get; set; }
    public Sprite? RenderedSprite { get; set; }
}

public enum FrameDisposalMethod
{
    NoDisposal = 0,
    DoNotDispose = 1,
    RestoreToBackgroundColor = 2,
    RestoreToPrevious = 3
}