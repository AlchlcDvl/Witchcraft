using System.Collections;

// Using a modified version of the gif loader from here: https://github.com/Turninator95/UniGif, this fellow's a life saver man
namespace Witchcraft.Gifs;

public static class GifLoader
{
    public static List<Sprite> LoadGifFromResources(string path) => LoadGifFromResources(path, Assembly.GetCallingAssembly());

    public static List<Sprite> LoadGifFromResources(string path, Assembly assembly) => LoadGif(ReadFully(assembly.GetManifestResourceStream(path)));

    private static byte[] ReadFully(Stream input)
    {
        using var memoryStream = new MemoryStream();
        input.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    public static List<Sprite> LoadGifFromExternal(string path) => LoadGif(File.ReadAllBytes(path));

    private static List<Sprite> LoadGif(byte[] data)
    {
        var result = new List<Sprite>();
        var gifData = SetGifData(data);

        if (gifData == null || gifData.m_imageBlockList == null || gifData.m_imageBlockList.Count < 1)
            return [];

        var width = gifData.ScreenWidth;
        var height = gifData.ScreenHeight;
        var imgIndex = 0;
        var rawTextures = new List<Color32[]>();

        foreach (var img in gifData.m_imageBlockList)
        {
            var decodedData = GetDecodedData(img);
            var graphicCtrlEx = GetGraphicCtrlExt(gifData, imgIndex);
            var transparentIndex = GetTransparentIndex(graphicCtrlEx);

            var colorTable = GetColorTableAndSetBgColor(gifData, img, transparentIndex, out var bgColor);
            rawTextures.Add(GetTextureData(decodedData, bgColor, colorTable, transparentIndex, img));
            imgIndex++;
        }

        for (var i = 0; i < rawTextures.Count; i++)
        {
            var gifFrame = new Texture2D(width, height, TextureFormat.ARGB32, false, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            gifFrame.SetPixels32(rawTextures[i], 0);
            gifFrame.Apply();

            result.Add(Sprite.Create(gifFrame, new(0, 0, width, height), new(0.5f, 0.5f), 100));
        }

        return result;
    }

    private static GifData SetGifData(byte[] gifBytes)
    {
        if (gifBytes == null || gifBytes.Length <= 0)
        {
            Logging.LogError("Byte data is empty.");
            return null!;
        }

        var byteIndex = 0;
        var gifData = new GifData();

        if (!SetGifHeader(gifBytes, ref byteIndex, ref gifData))
        {
            Logging.LogError("Problem setting header data.");
            return null!;
        }

        if (!SetGifBlock(gifBytes, ref byteIndex, ref gifData))
        {
            Logging.LogError("Problem setting gif block data.");
            return null!;
        }

        return gifData;
    }

    private static bool SetGifHeader(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
    {
        if (gifBytes[0] != 'G' || gifBytes[1] != 'I' || gifBytes[2] != 'F')
        {
            Logging.LogError("Header data is not valid for a GIF file.");
            return false;
        }

        gifData.m_sig0 = gifBytes[0];
        gifData.m_sig1 = gifBytes[1];
        gifData.m_sig2 = gifBytes[2];

        if (gifBytes[3] != '8' || (gifBytes[4] != '7' && gifBytes[4] != '9') || gifBytes[5] != 'a')
        {
            Logging.LogError("GIF version error. Only GIF87a or GIF89a are supported.");
            return false;
        }

        gifData.m_ver0 = gifBytes[3];
        gifData.m_ver1 = gifBytes[4];
        gifData.m_ver2 = gifBytes[5];

        gifData.ScreenWidth = BitConverter.ToUInt16(gifBytes, 6);
        gifData.ScreenHeight = BitConverter.ToUInt16(gifBytes, 8);

        gifData.GlobalColorTableFlag = (gifBytes[10] & 128) == 128;

        gifData.m_colorResolution = (gifBytes[10] & 112) switch
        {
            112 => 8,
            96 => 7,
            80 => 6,
            64 => 5,
            48 => 4,
            32 => 3,
            16 => 2,
            _ => 1,
        };

        gifData.m_sortFlag = (gifBytes[10] & 8) == 8;
        gifData.m_sizeOfGlobalColorTable = (int)Math.Pow(2, (gifBytes[10] & 7) + 1);
        gifData.m_bgColorIndex = gifBytes[11];
        gifData.m_pixelAspectRatio = gifBytes[12];
        byteIndex = 13;

        if (gifData.GlobalColorTableFlag)
        {
            gifData.m_globalColorTable = [];

            for (var i = byteIndex; i < byteIndex + (gifData.m_sizeOfGlobalColorTable * 3); i += 3)
                gifData.m_globalColorTable.Add([ gifBytes[i], gifBytes[i + 1], gifBytes[i + 2] ]);

            byteIndex = byteIndex + (gifData.m_sizeOfGlobalColorTable * 3);
        }

        return true;
    }

    private static bool SetGifBlock(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
    {
        try
        {
            var lastIndex = 0;

            while (true)
            {
                var nowIndex = byteIndex;

                if (gifBytes[nowIndex] == 0x2c)
                    SetImageBlock(gifBytes, ref byteIndex, ref gifData);
                else if (gifBytes[nowIndex] == 0x21)
                {
                    switch (gifBytes[nowIndex + 1])
                    {
                        case 0xf9:
                            SetGraphicControlExtension(gifBytes, ref byteIndex, ref gifData);
                            break;

                        case 0xfe:
                            SetCommentExtension(gifBytes, ref byteIndex, ref gifData);
                            break;

                        case 0x01:
                            SetPlainTextExtension(gifBytes, ref byteIndex, ref gifData);
                            break;

                        case 0xff:
                            SetApplicationExtension(gifBytes, ref byteIndex, ref gifData);
                            break;

                        default:
                            break;
                    }
                }
                else if (gifBytes[nowIndex] == 0x3b)
                {
                    gifData.m_trailer = gifBytes[byteIndex];
                    byteIndex++;
                    break;
                }

                if (lastIndex == nowIndex)
                {
                    Logging.LogError($"Infinite loop detected in GIF block data.");
                    return false;
                }

                lastIndex = nowIndex;
            }
        }
        catch (Exception ex)
        {
            Logging.LogError($"{ex.Message}");
            return false;
        }

        return true;
    }

    private static void SetImageBlock(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
    {
        var ib = new ImageBlock() { m_imageSeparator = gifBytes[byteIndex] };
        byteIndex++;

        ib.m_imageLeftPosition = BitConverter.ToUInt16(gifBytes, byteIndex);
        byteIndex += 2;

        ib.m_imageTopPosition = BitConverter.ToUInt16(gifBytes, byteIndex);
        byteIndex += 2;

        ib.m_imageWidth = BitConverter.ToUInt16(gifBytes, byteIndex);
        byteIndex += 2;

        ib.m_imageHeight = BitConverter.ToUInt16(gifBytes, byteIndex);
        byteIndex += 2;

        ib.m_localColorTableFlag = (gifBytes[byteIndex] & 128) == 128;
        ib.m_interlaceFlag = (gifBytes[byteIndex] & 64) == 64;
        ib.m_sortFlag = (gifBytes[byteIndex] & 32) == 32;

        ib.m_sizeOfLocalColorTable = (int)Math.Pow(2, (gifBytes[byteIndex] & 7) + 1);
        byteIndex++;

        if (ib.m_localColorTableFlag)
        {
            ib.m_localColorTable = [];

            for (var i = byteIndex; i < byteIndex + (ib.m_sizeOfLocalColorTable * 3); i += 3)
                ib.m_localColorTable.Add([ gifBytes[i], gifBytes[i + 1], gifBytes[i + 2] ]);

            byteIndex = byteIndex + (ib.m_sizeOfLocalColorTable * 3);
        }

        ib.m_lzwMinimumCodeSize = gifBytes[byteIndex];
        byteIndex++;

        while (true)
        {
            var blockSize = gifBytes[byteIndex];
            byteIndex++;

            if (blockSize == 0x00)
                break;

            var imageDataBlock = new DataBlock()
            {
                m_blockSize = blockSize,
                m_data = new byte[blockSize]
            };

            for (var i = 0; i < imageDataBlock.m_data.Length; i++)
            {
                imageDataBlock.m_data[i] = gifBytes[byteIndex];
                byteIndex++;
            }

            if (ib.m_imageDataList == null)
                ib.m_imageDataList = [];

            ib.m_imageDataList.Add(imageDataBlock);
        }

        if (gifData.m_imageBlockList == null)
            gifData.m_imageBlockList = [];

        gifData.m_imageBlockList.Add(ib);
    }

    private static void SetGraphicControlExtension(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
    {
        var gcEx = new GraphicControlExtension() {  m_extensionIntroducer = gifBytes[byteIndex] };
        byteIndex++;

        gcEx.m_graphicControlLabel = gifBytes[byteIndex];
        byteIndex++;

        gcEx.m_blockSize = gifBytes[byteIndex];
        byteIndex++;

        gcEx.m_disposalMethod = (gifBytes[byteIndex] & 28) switch
        {
            4 => 1,
            8 => 2,
            12 => 3,
            _ => 0,
        };
        gcEx.m_transparentColorFlag = (gifBytes[byteIndex] & 1) == 1;
        byteIndex++;

        gcEx.m_delayTime = BitConverter.ToUInt16(gifBytes, byteIndex);
        byteIndex += 2;

        gcEx.m_transparentColorIndex = gifBytes[byteIndex];
        byteIndex++;

        gcEx.m_blockTerminator = gifBytes[byteIndex];
        byteIndex++;

        if (gifData.m_graphicCtrlExList == null)
            gifData.m_graphicCtrlExList = [];

        gifData.m_graphicCtrlExList.Add(gcEx);
    }

    private static void SetCommentExtension(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
    {
        var commentEx = new CommentExtension() { m_extensionIntroducer = gifBytes[byteIndex] };
        byteIndex++;

        commentEx.m_commentLabel = gifBytes[byteIndex];
        byteIndex++;

        while (true)
        {
            var blockSize = gifBytes[byteIndex];
            byteIndex++;

            if (blockSize == 0x00)
                break;

            var commentDataBlock = new DataBlock()
            {
                m_blockSize = blockSize,
                m_data = new byte[blockSize]
            };

            for (var i = 0; i < commentDataBlock.m_data.Length; i++)
            {
                commentDataBlock.m_data[i] = gifBytes[byteIndex];
                byteIndex++;
            }

            if (commentEx.m_commentDataList == null)
                commentEx.m_commentDataList = [];

            commentEx.m_commentDataList.Add(commentDataBlock);
        }

        if (gifData.m_commentExList == null)
            gifData.m_commentExList = [];

        gifData.m_commentExList.Add(commentEx);
    }

    private static void SetPlainTextExtension(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
    {
        var plainTxtEx = new PlainTextExtension() { m_extensionIntroducer = gifBytes[byteIndex] };
        byteIndex++;

        plainTxtEx.m_plainTextLabel = gifBytes[byteIndex];
        byteIndex++;

        plainTxtEx.m_blockSize = gifBytes[byteIndex];
        byteIndex += 13;

        while (true)
        {
            var blockSize = gifBytes[byteIndex];
            byteIndex++;

            if (blockSize == 0x00)
            {
                break;
            }

            var plainTextDataBlock = new DataBlock()
            {
                m_blockSize = blockSize,
                m_data = new byte[blockSize]
            };

            for (var i = 0; i < plainTextDataBlock.m_data.Length; i++)
            {
                plainTextDataBlock.m_data[i] = gifBytes[byteIndex];
                byteIndex++;
            }

            if (plainTxtEx.m_plainTextDataList == null)
                plainTxtEx.m_plainTextDataList = [];

            plainTxtEx.m_plainTextDataList.Add(plainTextDataBlock);
        }

        if (gifData.m_plainTextExList == null)
            gifData.m_plainTextExList = [];

        gifData.m_plainTextExList.Add(plainTxtEx);
    }

    private static void SetApplicationExtension(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
    {
        gifData.m_appEx.m_extensionIntroducer = gifBytes[byteIndex];
        byteIndex++;

        gifData.m_appEx.m_extensionLabel = gifBytes[byteIndex];
        byteIndex++;

        gifData.m_appEx.m_blockSize = gifBytes[byteIndex];
        byteIndex++;

        gifData.m_appEx.m_appId1 = gifBytes[byteIndex];
        byteIndex++;

        gifData.m_appEx.m_appId2 = gifBytes[byteIndex];
        byteIndex++;

        gifData.m_appEx.m_appId3 = gifBytes[byteIndex];
        byteIndex++;

        gifData.m_appEx.m_appId4 = gifBytes[byteIndex];
        byteIndex++;

        gifData.m_appEx.m_appId5 = gifBytes[byteIndex];
        byteIndex++;

        gifData.m_appEx.m_appId6 = gifBytes[byteIndex];
        byteIndex++;

        gifData.m_appEx.m_appId7 = gifBytes[byteIndex];
        byteIndex++;

        gifData.m_appEx.m_appId8 = gifBytes[byteIndex];
        byteIndex++;

        gifData.m_appEx.m_appAuthCode1 = gifBytes[byteIndex];
        byteIndex++;

        gifData.m_appEx.m_appAuthCode2 = gifBytes[byteIndex];
        byteIndex++;

        gifData.m_appEx.m_appAuthCode3 = gifBytes[byteIndex];
        byteIndex++;

        while (true)
        {
            var blockSize = gifBytes[byteIndex];
            byteIndex++;

            if (blockSize == 0x00)
                break;

            var appDataBlock = new DataBlock()
            {
                m_blockSize = blockSize,
                m_data = new byte[blockSize]
            };

            for (var i = 0; i < appDataBlock.m_data.Length; i++)
            {
                appDataBlock.m_data[i] = gifBytes[byteIndex];
                byteIndex++;
            }

            if (gifData.m_appEx.m_appDataList == null)
                gifData.m_appEx.m_appDataList = [];

            gifData.m_appEx.m_appDataList.Add(appDataBlock);
        }
    }

    private static byte[] GetDecodedData(ImageBlock imgBlock)
    {
        var lzwData = new List<byte>();

        foreach (var delList in imgBlock.m_imageDataList)
        {
            foreach (var imgDelta in delList.m_data)
                lzwData.Add(imgDelta);
        }

        var decodedData = DecodeGifLZW(lzwData, imgBlock.m_lzwMinimumCodeSize, imgBlock.m_imageHeight * imgBlock.m_imageWidth);

        if (imgBlock.m_interlaceFlag)
            decodedData = SortInterlaceGifData(decodedData, imgBlock.m_imageWidth);

        return decodedData;
    }

    private static List<byte[]> GetColorTableAndSetBgColor(GifData gifData, ImageBlock imgBlock, int transparentIndex, out Color32 bgColor)
    {
        var colorTable = imgBlock.m_localColorTableFlag ? imgBlock.m_localColorTable : (gifData.GlobalColorTableFlag ? gifData.m_globalColorTable : null);

        if (colorTable != null)
        {
            bgColor = new()
            {
                r = colorTable[gifData.m_bgColorIndex][0],
                g = colorTable[gifData.m_bgColorIndex][1],
                b = colorTable[gifData.m_bgColorIndex][2],
                a = (byte)(transparentIndex == gifData.m_bgColorIndex ? 0 : 255)
            };
        }
        else
        {
            bgColor = new()
            {
                r = 0,
                g = 0,
                b = 0,
                a = 255
            };
        }

        return colorTable!;
    }

    private static GraphicControlExtension? GetGraphicCtrlExt(GifData gifData, int imgBlockIndex)
    {
        if (gifData.m_graphicCtrlExList != null && gifData.m_graphicCtrlExList.Count > imgBlockIndex)
            return gifData.m_graphicCtrlExList[imgBlockIndex];

        return null;
    }

    private static int GetTransparentIndex(GraphicControlExtension? graphicCtrlEx)
    {
        var transparentIndex = -1;

        if (graphicCtrlEx != null && graphicCtrlEx.Value.m_transparentColorFlag)
            transparentIndex = graphicCtrlEx.Value.m_transparentColorIndex;

        return transparentIndex;
    }

    // private static float GetDelaySec(GraphicControlExtension? graphicCtrlEx)
    // {
    //     var delaySec = graphicCtrlEx != null ? graphicCtrlEx.Value.m_delayTime / 100f : (1f / 60f);

    //     if (delaySec <= 0f)
    //         delaySec = 0.1f;

    //     return delaySec;
    // }

    // private static ushort GetDisposalMethod(GraphicControlExtension? graphicCtrlEx) => graphicCtrlEx != null ? graphicCtrlEx.Value.m_disposalMethod : (ushort)2;

    private static Color32[] GetTextureData(byte[] gifData, Color32 backgroundColor, List<byte[]> colorTable, int transparentIndex, ImageBlock imageBlock)
    {
        var dataIndex = 0;
        var pixelData = new Color32[imageBlock.m_imageHeight * imageBlock.m_imageWidth];

        for (var y = imageBlock.m_imageHeight - 1; y >= 0; y--)
        {
            for (var x = 0; x < imageBlock.m_imageWidth; x++)
            {
                if (y < imageBlock.m_imageTopPosition || y >= imageBlock.m_imageTopPosition + imageBlock.m_imageHeight || x < imageBlock.m_imageLeftPosition || x >=
                    imageBlock.m_imageLeftPosition + imageBlock.m_imageWidth)
                {
                    pixelData[x + y * imageBlock.m_imageWidth] = new(backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a);
                    continue;
                }

                if (dataIndex >= gifData.Length)
                {
                    pixelData[x + y * imageBlock.m_imageWidth] = new(backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a);

                    if (dataIndex == gifData.Length)
                        Logging.LogError($"dataIndex exceeded the size of decodedData. dataIndex: {dataIndex} decodedData.Length: {gifData.Length} y: {y} x: {x}");

                    dataIndex++;
                    continue;
                }

                var colorIndex = gifData[dataIndex];

                if (colorTable == null || colorTable.Count <= colorIndex)
                {
                    pixelData[x + y * imageBlock.m_imageWidth] = new(backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a);

                    if (colorTable == null)
                        Logging.LogError($"colorIndex exceeded the size of colorTable. colorTable is null. colorIndex: {colorIndex}");
                    else
                        Logging.LogError($"colorIndex exceeded the size of colorTable. colorTable.Count: {colorTable.Count}. colorIndex: {colorIndex}");

                    dataIndex++;
                    continue;
                }

                var rgb = colorTable[colorIndex];
                var alpha = transparentIndex >= 0 && transparentIndex == colorIndex ? (byte)0 : (byte)255;
                pixelData[x + y * imageBlock.m_imageWidth] = new(rgb[0], rgb[1], rgb[2], alpha);
                dataIndex++;
            }
        }

        return pixelData;
    }

    private static byte[] DecodeGifLZW(List<byte> compData, int lzwMinimumCodeSize, int needDataSize)
    {
        var dic = new Dictionary<int, string>();
        InitDictionary(dic, lzwMinimumCodeSize, out var lzwCodeSize, out var clearCode, out var finishCode);

        var compDataArr = compData.ToArray();
        var bitData = new BitArray(compDataArr);

        var output = new byte[needDataSize];
        var outputAddIndex = 0;
        var prevEntry = "";
        var dicInitFlag = false;
        var bitDataIndex = 0;

        while (bitDataIndex < bitData.Length)
        {
            if (dicInitFlag)
            {
                InitDictionary(dic, lzwMinimumCodeSize, out lzwCodeSize, out clearCode, out finishCode);
                dicInitFlag = false;
            }

            var key = bitData.GetNumeral(bitDataIndex, lzwCodeSize);
            var entry = "";

            if (key == clearCode)
            {
                dicInitFlag = true;
                bitDataIndex += lzwCodeSize;
                prevEntry = null;
                continue;
            }
            else if (key == finishCode)
            {
                Logging.LogWarning($"LZW Decoding anomaly. Early stop code detected. bitDataIndex: {bitDataIndex} lzwCodeSize: {lzwCodeSize} key: {key} dic.Count: {dic.Count}");
                break;
            }
            else if (dic.ContainsKey(key))
                entry = dic[key];
            else if (key >= dic.Count)
            {
                if (prevEntry != null)
                    entry = prevEntry + prevEntry[0];
                else
                {
                    Logging.LogWarning($"LZW Decoding anomaly. Current key did not match previous conditions. bitDataIndex: {bitDataIndex} lzwCodeSize: {lzwCodeSize} key: {key} dic.Count: {dic.Count}");
                    bitDataIndex += lzwCodeSize;
                    continue;
                }
            }
            else
            {
                Logging.LogWarning($"LZW Decoding anomaly. Current key did not match previous conditions. bitDataIndex: {bitDataIndex} lzwCodeSize: {lzwCodeSize} key: {key} dic.Count: {dic.Count}");
                bitDataIndex += lzwCodeSize;
                continue;
            }

            var  temp = Encoding.Unicode.GetBytes(entry);

            for (var i = 0; i < temp.Length; i++)
            {
                if (i % 2 == 0)
                {
                    output[outputAddIndex] = temp[i];
                    outputAddIndex++;
                }
            }

            if (outputAddIndex >= needDataSize)
                break;

            if (prevEntry != null)
                dic.Add(dic.Count, prevEntry + entry[0]);

            prevEntry = entry;
            bitDataIndex += lzwCodeSize;

            if (lzwCodeSize == 3 && dic.Count >= 8)
                lzwCodeSize = 4;
            else if (lzwCodeSize == 4 && dic.Count >= 16)
                lzwCodeSize = 5;
            else if (lzwCodeSize == 5 && dic.Count >= 32)
                lzwCodeSize = 6;
            else if (lzwCodeSize == 6 && dic.Count >= 64)
                lzwCodeSize = 7;
            else if (lzwCodeSize == 7 && dic.Count >= 128)
                lzwCodeSize = 8;
            else if (lzwCodeSize == 8 && dic.Count >= 256)
                lzwCodeSize = 9;
            else if (lzwCodeSize == 9 && dic.Count >= 512)
                lzwCodeSize = 10;
            else if (lzwCodeSize == 10 && dic.Count >= 1024)
                lzwCodeSize = 11;
            else if (lzwCodeSize == 11 && dic.Count >= 2048)
                lzwCodeSize = 12;
            else if (lzwCodeSize == 12 && dic.Count >= 4096)
            {
                if (bitData.GetNumeral(bitDataIndex, lzwCodeSize) != clearCode)
                    dicInitFlag = true;
            }
        }

        return output;
    }

    private static void InitDictionary(Dictionary<int, string> dic, int lzwMinimumCodeSize, out int lzwCodeSize, out int clearCode, out int finishCode)
    {
        var dicLength = (int)Math.Pow(2, lzwMinimumCodeSize);

        clearCode = dicLength;
        finishCode = clearCode + 1;
        dic.Clear();

        for (var i = 0; i < dicLength + 2; i++)
            dic.Add(i, ((char)i).ToString());

        lzwCodeSize = lzwMinimumCodeSize + 1;
    }

    private static byte[] SortInterlaceGifData(byte[] decodedData, int xNum)
    {
        var rowNo = 0;
        var dataIndex = 0;
        var newArr = new byte[decodedData.Length];

        for (var i = 0; i < newArr.Length; i++)
        {
            if (rowNo % 8 == 0)
            {
                newArr[i] = decodedData[dataIndex];
                dataIndex++;
            }

            if (i != 0 && i % xNum == 0)
                rowNo++;
        }

        rowNo = 0;

        for (var i = 0; i < newArr.Length; i++)
        {
            if (rowNo % 8 == 4)
            {
                newArr[i] = decodedData[dataIndex];
                dataIndex++;
            }

            if (i != 0 && i % xNum == 0)
                rowNo++;
        }

        rowNo = 0;

        for (var i = 0; i < newArr.Length; i++)
        {
            if (rowNo % 4 == 2)
            {
                newArr[i] = decodedData[dataIndex];
                dataIndex++;
            }

            if (i != 0 && i % xNum == 0)
                rowNo++;
        }

        rowNo = 0;

        for (var i = 0; i < newArr.Length; i++)
        {
            if (rowNo % 8 != 0 && rowNo % 8 != 4 && rowNo % 4 != 2)
            {
                newArr[i] = decodedData[dataIndex];
                dataIndex++;
            }

            if (i != 0 && i % xNum == 0)
                rowNo++;
        }

        return newArr;
    }

    public static int GetNumeral(this BitArray array, int startIndex, int bitLength)
    {
        var newArray = new BitArray(bitLength);

        for (var i = 0; i < bitLength; i++)
            newArray[i] = array.Length > startIndex + i && array.Get(startIndex + i);

        return newArray.ToNumeral();
    }

    public static int ToNumeral(this BitArray array)
    {
        if (array == null)
        {
            Debug.LogError("Array is nothing.");
            return 0;
        }

        if (array.Length > 32)
        {
            Debug.LogError("Must be at most 32 bits long.");
            return 0;
        }

        var result = new int[1];
        array.CopyTo(result, 0);
        return result[0];
    }
}