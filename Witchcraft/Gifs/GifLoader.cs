using System.Collections;

namespace Witchcraft.Gifs;

public static class GifLoader
{
    public static GifData GetGifData(byte[] gifBytes)
    {
        if (gifBytes == null || gifBytes.Length <= 0)
        {
            Witchcraft.Instance!.Error("Byte data is empty.");
            return null!;
        }

        var byteIndex = 0;
        var gifData = new GifData();

        if (!GetGifHeader(gifBytes, ref byteIndex, ref gifData))
        {
            Witchcraft.Instance!.Error("Problem getting header data.");
            return null!;
        }

        if (!GetGifBlock(gifBytes, ref byteIndex, ref gifData))
        {
            Witchcraft.Instance!.Error("Problem getting gif block data.");
            return null!;
        }

        return gifData;
    }

    public static bool GetGifHeader(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
    {
        if (gifBytes[0] != 'G' || gifBytes[1] != 'I' || gifBytes[2] != 'F')
        {
            Witchcraft.Instance!.Error("Header data is not valid for a GIF file.");
            return false;
        }

        gifData.Sig0 = gifBytes[0];
        gifData.Sig1 = gifBytes[1];
        gifData.Sig2 = gifBytes[2];

        if (gifBytes[3] != '8' || (gifBytes[4] != '7' && gifBytes[4] != '9') || gifBytes[5] != 'a')
        {
            Witchcraft.Instance!.Error("GIF version error. Only GIF87a or GIF89a are supported.");
            return false;
        }

        gifData.Ver0 = gifBytes[3];
        gifData.Ver1 = gifBytes[4];
        gifData.Ver2 = gifBytes[5];

        gifData.ScreenWidth = BitConverter.ToUInt16(gifBytes, 6);
        gifData.ScreenHeight = BitConverter.ToUInt16(gifBytes, 8);

        gifData.GlobalColorTableFlag = (gifBytes[10] & 128) == 128;

        gifData.ColorResolution = (gifBytes[10] & 112) switch
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

        gifData.SortFlag = (gifBytes[10] & 8) == 8;
        gifData.SizeOfGlobalColorTable = (int)Math.Pow(2, (gifBytes[10] & 7) + 1);
        gifData.BGColorIndex = gifBytes[11];
        gifData.PixelAspectRatio = gifBytes[12];
        byteIndex = 13;

        if (gifData.GlobalColorTableFlag)
        {
            gifData.GlobalColorTable = [];

            for (var i = byteIndex; i < byteIndex + (gifData.SizeOfGlobalColorTable * 3); i += 3)
                gifData.GlobalColorTable.Add([ gifBytes[i], gifBytes[i + 1], gifBytes[i + 2] ]);

            byteIndex = byteIndex + (gifData.SizeOfGlobalColorTable * 3);
        }

        return true;
    }

    public static bool GetGifBlock(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
    {
        try
        {
            var lastIndex = 0;

            while (true)
            {
                var nowIndex = byteIndex;

                if (gifBytes[nowIndex] == 0x2c)
                    GetImageBlock(gifBytes, ref byteIndex, ref gifData);
                else if (gifBytes[nowIndex] == 0x21)
                {
                    switch (gifBytes[nowIndex + 1])
                    {
                        case 0xf9:
                            GetGraphicControlExtension(gifBytes, ref byteIndex, ref gifData);
                            break;

                        case 0xfe:
                            GetCommentExtension(gifBytes, ref byteIndex, ref gifData);
                            break;

                        case 0x01:
                            GetPlainTextExtension(gifBytes, ref byteIndex, ref gifData);
                            break;

                        case 0xff:
                            GetApplicationExtension(gifBytes, ref byteIndex, ref gifData);
                            break;

                        default:
                            break;
                    }
                }
                else if (gifBytes[nowIndex] == 0x3b)
                {
                    gifData.Trailer = gifBytes[byteIndex];
                    byteIndex++;
                    break;
                }

                if (lastIndex == nowIndex)
                {
                    Witchcraft.Instance!.Error($"Infinite loop detected in GIF block data.");
                    return false;
                }

                lastIndex = nowIndex;
            }
        }
        catch (Exception ex)
        {
            Witchcraft.Instance!.Error(ex);
            return false;
        }

        return true;
    }

    public static void GetImageBlock(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
    {
        var ib = new ImageBlock() { ImageSeparator = gifBytes[byteIndex] };
        byteIndex++;

        ib.ImageLeftPosition = BitConverter.ToUInt16(gifBytes, byteIndex);
        byteIndex += 2;

        ib.ImageTopPosition = BitConverter.ToUInt16(gifBytes, byteIndex);
        byteIndex += 2;

        ib.ImageWidth = BitConverter.ToUInt16(gifBytes, byteIndex);
        byteIndex += 2;

        ib.ImageHeight = BitConverter.ToUInt16(gifBytes, byteIndex);
        byteIndex += 2;

        ib.LocalColorTableFlag = (gifBytes[byteIndex] & 128) == 128;
        ib.InterlaceFlag = (gifBytes[byteIndex] & 64) == 64;
        ib.SortFlag = (gifBytes[byteIndex] & 32) == 32;

        ib.SizeOfLocalColorTable = (int)Math.Pow(2, (gifBytes[byteIndex] & 7) + 1);
        byteIndex++;

        if (ib.LocalColorTableFlag)
        {
            ib.LocalColorTable = [];

            for (var i = byteIndex; i < byteIndex + (ib.SizeOfLocalColorTable * 3); i += 3)
                ib.LocalColorTable.Add([ gifBytes[i], gifBytes[i + 1], gifBytes[i + 2] ]);

            byteIndex = byteIndex + (ib.SizeOfLocalColorTable * 3);
        }

        ib.LzwMinimumCodeSize = gifBytes[byteIndex];
        byteIndex++;

        while (true)
        {
            var blockSize = gifBytes[byteIndex];
            byteIndex++;

            if (blockSize == 0x00)
                break;

            var imageDataBlock = new DataBlock()
            {
                BlockSize = blockSize,
                Data = new byte[blockSize],
            };

            for (var i = 0; i < imageDataBlock.Data.Length; i++)
            {
                imageDataBlock.Data[i] = gifBytes[byteIndex];
                byteIndex++;
            }

            if (ib.ImageDataList == null)
                ib.ImageDataList = [];

            ib.ImageDataList.Add(imageDataBlock);
        }

        if (gifData.ImageBlockList == null)
            gifData.ImageBlockList = [];

        gifData.ImageBlockList.Add(ib);
    }

    public static void GetGraphicControlExtension(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
    {
        var gcEx = new GraphicControlExtension() { ExtensionIntroducer = gifBytes[byteIndex] };
        byteIndex++;

        gcEx.GraphicControlLabel = gifBytes[byteIndex];
        byteIndex++;

        gcEx.BlockSize = gifBytes[byteIndex];
        byteIndex++;

        gcEx.DisposalMethod = (gifBytes[byteIndex] & 28) switch
        {
            4 => 1,
            8 => 2,
            12 => 3,
            _ => 0,
        };
        gcEx.TransparentColorFlag = (gifBytes[byteIndex] & 1) == 1;
        byteIndex++;

        gcEx.DelayTime = BitConverter.ToUInt16(gifBytes, byteIndex);
        byteIndex += 2;

        gcEx.TransparentColorIndex = gifBytes[byteIndex];
        byteIndex++;

        gcEx.BlockTerminator = gifBytes[byteIndex];
        byteIndex++;

        if (gifData.GraphicCtrlExList == null)
            gifData.GraphicCtrlExList = [];

        gifData.GraphicCtrlExList.Add(gcEx);
    }

    public static void GetCommentExtension(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
    {
        var commentEx = new CommentExtension() { ExtensionIntroducer = gifBytes[byteIndex] };
        byteIndex++;

        commentEx.CommentLabel = gifBytes[byteIndex];
        byteIndex++;

        while (true)
        {
            var blockSize = gifBytes[byteIndex];
            byteIndex++;

            if (blockSize == 0x00)
                break;

            var commentDataBlock = new DataBlock()
            {
                BlockSize = blockSize,
                Data = new byte[blockSize],
            };

            for (var i = 0; i < commentDataBlock.Data.Length; i++)
            {
                commentDataBlock.Data[i] = gifBytes[byteIndex];
                byteIndex++;
            }

            if (commentEx.CommentDataList == null)
                commentEx.CommentDataList = [];

            commentEx.CommentDataList.Add(commentDataBlock);
        }

        if (gifData.CommentExList == null)
            gifData.CommentExList = [];

        gifData.CommentExList.Add(commentEx);
    }

    public static void GetPlainTextExtension(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
    {
        var plainTxtEx = new PlainTextExtension() { ExtensionIntroducer = gifBytes[byteIndex] };
        byteIndex++;

        plainTxtEx.PlainTextLabel = gifBytes[byteIndex];
        byteIndex++;

        plainTxtEx.BlockSize = gifBytes[byteIndex];
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
                BlockSize = blockSize,
                Data = new byte[blockSize],
            };

            for (var i = 0; i < plainTextDataBlock.Data.Length; i++)
            {
                plainTextDataBlock.Data[i] = gifBytes[byteIndex];
                byteIndex++;
            }

            if (plainTxtEx.PlainTextDataList == null)
                plainTxtEx.PlainTextDataList = [];

            plainTxtEx.PlainTextDataList.Add(plainTextDataBlock);
        }

        if (gifData.PlainTextExList == null)
            gifData.PlainTextExList = [];

        gifData.PlainTextExList.Add(plainTxtEx);
    }

    public static void GetApplicationExtension(byte[] gifBytes, ref int byteIndex, ref GifData gifData)
    {
        gifData.AppEx.ExtensionIntroducer = gifBytes[byteIndex];
        byteIndex++;

        gifData.AppEx.ExtensionLabel = gifBytes[byteIndex];
        byteIndex++;

        gifData.AppEx.BlockSize = gifBytes[byteIndex];
        byteIndex++;

        gifData.AppEx.AppId1 = gifBytes[byteIndex];
        byteIndex++;

        gifData.AppEx.AppId2 = gifBytes[byteIndex];
        byteIndex++;

        gifData.AppEx.AppId3 = gifBytes[byteIndex];
        byteIndex++;

        gifData.AppEx.AppId4 = gifBytes[byteIndex];
        byteIndex++;

        gifData.AppEx.AppId5 = gifBytes[byteIndex];
        byteIndex++;

        gifData.AppEx.AppId6 = gifBytes[byteIndex];
        byteIndex++;

        gifData.AppEx.AppId7 = gifBytes[byteIndex];
        byteIndex++;

        gifData.AppEx.AppId8 = gifBytes[byteIndex];
        byteIndex++;

        gifData.AppEx.AppAuthCode1 = gifBytes[byteIndex];
        byteIndex++;

        gifData.AppEx.AppAuthCode2 = gifBytes[byteIndex];
        byteIndex++;

        gifData.AppEx.AppAuthCode3 = gifBytes[byteIndex];
        byteIndex++;

        while (true)
        {
            var blockSize = gifBytes[byteIndex];
            byteIndex++;

            if (blockSize == 0x00)
                break;

            var appDataBlock = new DataBlock()
            {
                BlockSize = blockSize,
                Data = new byte[blockSize],
            };

            for (var i = 0; i < appDataBlock.Data.Length; i++)
            {
                appDataBlock.Data[i] = gifBytes[byteIndex];
                byteIndex++;
            }

            if (gifData.AppEx.AppDataList == null)
                gifData.AppEx.AppDataList = [];

            gifData.AppEx.AppDataList.Add(appDataBlock);
        }
    }

    public static byte[] GetDecodedData(ImageBlock imgBlock)
    {
        var lzwData = new List<byte>();

        foreach (var delList in imgBlock.ImageDataList)
        {
            foreach (var imgDelta in delList.Data)
                lzwData.Add(imgDelta);
        }

        var decodedData = DecodeGifLZW(lzwData, imgBlock.LzwMinimumCodeSize, imgBlock.ImageHeight * imgBlock.ImageWidth);

        if (imgBlock.InterlaceFlag)
            decodedData = SortInterlaceGifData(decodedData, imgBlock.ImageWidth);

        return decodedData;
    }

    public static List<byte[]> GetColorTableAndGetBgColor(GifData gifData, ImageBlock imgBlock, int transparentIndex, out Color32 bgColor)
    {
        var colorTable = imgBlock.LocalColorTableFlag ? imgBlock.LocalColorTable : (gifData.GlobalColorTableFlag ? gifData.GlobalColorTable : null);

        if (colorTable != null)
        {
            bgColor = new(colorTable[gifData.BGColorIndex][0], colorTable[gifData.BGColorIndex][1], colorTable[gifData.BGColorIndex][2],
                (byte)(transparentIndex == gifData.BGColorIndex ? 0 : 255));
        }
        else
            bgColor = new(0, 0, 0, 255);

        return colorTable!;
    }

    public static GraphicControlExtension? GetGraphicCtrlExt(GifData gifData, int imgBlockIndex)
    {
        if (gifData.GraphicCtrlExList != null && gifData.GraphicCtrlExList.Count > imgBlockIndex)
            return gifData.GraphicCtrlExList[imgBlockIndex];

        return null;
    }

    public static int GetTransparentIndex(GraphicControlExtension? graphicCtrlEx)
    {
        var transparentIndex = -1;

        if (graphicCtrlEx != null && graphicCtrlEx.Value.TransparentColorFlag)
            transparentIndex = graphicCtrlEx.Value.TransparentColorIndex;

        return transparentIndex;
    }

    /*public static float GetDelaySec(GraphicControlExtension? graphicCtrlEx)
    {
        var delaySec = graphicCtrlEx != null ? graphicCtrlEx.Value.DelayTime / 100f : (1f / 60f);

        if (delaySec <= 0f)
            delaySec = 0.1f;

        return delaySec;
    }

    public static ushort GetDisposalMethod(GraphicControlExtension? graphicCtrlEx) => graphicCtrlEx != null ? graphicCtrlEx.Value.DisposalMethod : (ushort)2;*/

    public static Color32[] GetTextureData(byte[] gifData, Color32 backgroundColor, List<byte[]> colorTable, int transparentIndex, ImageBlock imageBlock)
    {
        var dataIndex = 0;
        var pixelData = new Color32[imageBlock.ImageHeight * imageBlock.ImageWidth];

        for (var y = imageBlock.ImageHeight - 1; y >= 0; y--)
        {
            for (var x = 0; x < imageBlock.ImageWidth; x++)
            {
                if (y < imageBlock.ImageTopPosition || y >= imageBlock.ImageTopPosition + imageBlock.ImageHeight || x < imageBlock.ImageLeftPosition || x >=
                    imageBlock.ImageLeftPosition + imageBlock.ImageWidth)
                {
                    pixelData[x + y * imageBlock.ImageWidth] = new(backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a);
                    continue;
                }

                if (dataIndex >= gifData.Length)
                {
                    pixelData[x + y * imageBlock.ImageWidth] = new(backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a);

                    if (dataIndex == gifData.Length)
                        Witchcraft.Instance!.Error($"dataIndex exceeded the size of decodedData. dataIndex: {dataIndex} decodedData.Length: {gifData.Length} y: {y} x: {x}");

                    dataIndex++;
                    continue;
                }

                var colorIndex = gifData[dataIndex];

                if (colorTable == null || colorTable.Count <= colorIndex)
                {
                    pixelData[x + y * imageBlock.ImageWidth] = new(backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a);

                    if (colorTable == null)
                        Witchcraft.Instance!.Error($"colorIndex exceeded the size of colorTable. colorTable is null. colorIndex: {colorIndex}");
                    else
                        Witchcraft.Instance!.Error($"colorIndex exceeded the size of colorTable. colorTable.Count: {colorTable.Count}. colorIndex: {colorIndex}");

                    dataIndex++;
                    continue;
                }

                var rgb = colorTable[colorIndex];
                var alpha = transparentIndex >= 0 && transparentIndex == colorIndex ? (byte)0 : (byte)255;
                pixelData[x + y * imageBlock.ImageWidth] = new(rgb[0], rgb[1], rgb[2], alpha);
                dataIndex++;
            }
        }

        return pixelData;
    }

    public static byte[] DecodeGifLZW(List<byte> compData, int lzwMinimumCodeSize, int needDataSize)
    {
        var dic = new Dictionary<int, string>();
        InitDictionary(dic, lzwMinimumCodeSize, out var lzwCodeSize, out var clearCode, out var finishCode);

        var compDataArr = compData.ToArray();
        var bitData = new BitArray(compDataArr);

        var output = new byte[needDataSize];
        var outputAddIndex = 0;
        var prevEntry = string.Empty;
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
            var entry = string.Empty;

            if (key == clearCode)
            {
                dicInitFlag = true;
                bitDataIndex += lzwCodeSize;
                prevEntry = null;
                continue;
            }
            else if (key == finishCode)
            {
                Witchcraft.Instance!.Warning($"LZW Decoding anomaly. Early stop code detected. bitDataIndex: {bitDataIndex} lzwCodeSize: {lzwCodeSize} key: {key} dic.Count: {dic.Count}");
                break;
            }
            else if (dic.TryGetValue(key, out var value))
                entry = value;
            else if (key >= dic.Count)
            {
                if (prevEntry != null)
                    entry = prevEntry + prevEntry[0];
                else
                {
                    Witchcraft.Instance!.Warning($"LZW Decoding anomaly. Current key did not match previous conditions. bitDataIndex: {bitDataIndex} lzwCodeSize: {lzwCodeSize} key: {key} dic.Count: {dic.Count}");
                    bitDataIndex += lzwCodeSize;
                    continue;
                }
            }
            else
            {
                Witchcraft.Instance!.Warning($"LZW Decoding anomaly. Current key did not match previous conditions. bitDataIndex: {bitDataIndex} lzwCodeSize: {lzwCodeSize} key: {key} dic.Count: {dic.Count}");
                bitDataIndex += lzwCodeSize;
                continue;
            }

            var temp = Encoding.Unicode.GetBytes(entry);

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
            else if (lzwCodeSize == 12 && dic.Count >= 4096 && bitData.GetNumeral(bitDataIndex, lzwCodeSize) != clearCode)
                dicInitFlag = true;
        }

        return output;
    }

    public static void InitDictionary(Dictionary<int, string> dic, int lzwMinimumCodeSize, out int lzwCodeSize, out int clearCode, out int finishCode)
    {
        var dicLength = (int)Math.Pow(2, lzwMinimumCodeSize);

        clearCode = dicLength;
        finishCode = clearCode + 1;
        dic.Clear();

        for (var i = 0; i < dicLength + 2; i++)
            dic.Add(i, ((char)i).ToString());

        lzwCodeSize = lzwMinimumCodeSize + 1;
    }

    public static byte[] SortInterlaceGifData(byte[] decodedData, int xNum)
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
}