namespace Witchcraft.Gifs;

public class Gif(List<Sprite> frames, GifData data)
{
    public List<Sprite> Frames { get; set; } = frames;
    public GifData Data { get; set; } = data;
}

public class GifData
{
    public byte Sig0, Sig1, Sig2;
    public byte Ver0, Ver1, Ver2;
    public ushort ScreenWidth;
    public ushort ScreenHeight;
    public bool GlobalColorTableFlag;
    public int ColorResolution;
    public bool SortFlag;
    public int SizeOfGlobalColorTable;
    public byte BGColorIndex;
    public byte PixelAspectRatio;
    public List<byte[]>? GlobalColorTable;
    public List<ImageBlock>? ImageBlockList;
    public List<GraphicControlExtension>? GraphicCtrlExList;
    public List<CommentExtension>? CommentExList;
    public List<PlainTextExtension>? PlainTextExList;
    public ApplicationExtension AppEx;
    public byte Trailer;
}

public struct ImageBlock
{
    public byte ImageSeparator, LzwMinimumCodeSize;
    public ushort ImageLeftPosition, ImageTopPosition, ImageWidth, ImageHeight;
    public bool LocalColorTableFlag, InterlaceFlag, SortFlag;
    public int SizeOfLocalColorTable;
    public List<byte[]> LocalColorTable;
    public List<DataBlock> ImageDataList;
}

public struct DataBlock
{
    public byte BlockSize;
    public byte[] Data;
}

public struct GraphicControlExtension
{
    public byte ExtensionIntroducer;
    public byte GraphicControlLabel;
    public byte BlockSize;
    public ushort DisposalMethod;
    public bool TransparentColorFlag;
    public ushort DelayTime;
    public byte TransparentColorIndex;
    public byte BlockTerminator;
}

public struct CommentExtension
{
    public byte ExtensionIntroducer;
    public byte CommentLabel;
    public List<DataBlock> CommentDataList;
}

public struct PlainTextExtension
{
    public byte ExtensionIntroducer;
    public byte PlainTextLabel;
    public byte BlockSize;
    public List<DataBlock> PlainTextDataList;
}

public struct ApplicationExtension
{
    public byte ExtensionIntroducer;
    public byte ExtensionLabel;
    public byte BlockSize;
    public byte AppId1, AppId2, AppId3, AppId4, AppId5, AppId6, AppId7, AppId8;
    public byte AppAuthCode1, AppAuthCode2, AppAuthCode3;
    public List<DataBlock> AppDataList;
}