namespace Witchcraft.Gifs;

public class GifData
{
    public byte m_sig0, m_sig1, m_sig2;
    public byte m_ver0, m_ver1, m_ver2;
    public ushort ScreenWidth;
    public ushort ScreenHeight;
    public bool GlobalColorTableFlag;
    public int m_colorResolution;
    public bool m_sortFlag;
    public int m_sizeOfGlobalColorTable;
    public byte m_bgColorIndex;
    public byte m_pixelAspectRatio;
    public List<byte[]>? m_globalColorTable;
    public List<ImageBlock>? m_imageBlockList;
    public List<GraphicControlExtension>? m_graphicCtrlExList;
    public List<CommentExtension>? m_commentExList;
    public List<PlainTextExtension>? m_plainTextExList;
    public ApplicationExtension m_appEx;
    public byte m_trailer;

    public string signature => new([ (char)m_sig0, (char)m_sig1, (char)m_sig2 ]);
    public string version => new([ (char)m_ver0, (char)m_ver1, (char)m_ver2 ]);
}

public struct ImageBlock
{
    public byte m_imageSeparator, m_lzwMinimumCodeSize;
    public ushort m_imageLeftPosition, m_imageTopPosition, m_imageWidth, m_imageHeight;
    public bool m_localColorTableFlag, m_interlaceFlag, m_sortFlag;
    public int m_sizeOfLocalColorTable;
    public List<byte[]> m_localColorTable;
    public List<DataBlock> m_imageDataList;
}

public struct DataBlock
{
    public byte m_blockSize;
    public byte[] m_data;
}

public struct GraphicControlExtension
{
    public byte m_extensionIntroducer;
    public byte m_graphicControlLabel;
    public byte m_blockSize;
    public ushort m_disposalMethod;
    public bool m_transparentColorFlag;
    public ushort m_delayTime;
    public byte m_transparentColorIndex;
    public byte m_blockTerminator;
}

public struct CommentExtension
{
    public byte m_extensionIntroducer;
    public byte m_commentLabel;
    public List<DataBlock> m_commentDataList;

}

public struct PlainTextExtension
{
    public byte m_extensionIntroducer;
    public byte m_plainTextLabel;
    public byte m_blockSize;
    public List<DataBlock> m_plainTextDataList;
}

public struct ApplicationExtension
{
    public byte m_extensionIntroducer;
    public byte m_extensionLabel;
    public byte m_blockSize;
    public byte m_appId1, m_appId2, m_appId3, m_appId4, m_appId5, m_appId6, m_appId7, m_appId8;
    public byte m_appAuthCode1, m_appAuthCode2, m_appAuthCode3;
    public List<DataBlock> m_appDataList;

    public readonly string applicationIdentifier => new([ (char)m_appId1, (char)m_appId2, (char)m_appId3, (char)m_appId4, (char)m_appId5, (char)m_appId6, (char)m_appId7, (char)m_appId8 ]);
    public readonly string applicationAuthenticationCode => new([ (char)m_appAuthCode1, (char)m_appAuthCode2, (char)m_appAuthCode3 ]);

    public readonly int loopCount
    {
        get
        {
            if (m_appDataList == null || m_appDataList.Count < 1 || m_appDataList[0].m_data.Length < 3 || m_appDataList[0].m_data[0] != 0x01)
                return 0;

            return BitConverter.ToUInt16(m_appDataList[0].m_data, 1);
        }
    }
}