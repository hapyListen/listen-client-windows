namespace ListenTools.Media.FFmpegHelper;

internal static class Aac
{
    /// <summary>
    /// 写入AAC头
    /// </summary>
    public static byte[] GenerateAdtsHeader(int sampleRate,int channels, int aacFrameLength)
    {
        int profile = 1; // AAC LC (Low Complexity)
        int freqIdx = GetFreqIdx(sampleRate);
        int fullFrameLength = aacFrameLength + 7; // ADTS header length is 7 bytes

        byte[] adtsHeader = new byte[7];
        adtsHeader[0] = 0xFF; // Sync word 0xFFF (12 bits)
        adtsHeader[1] = 0xF1; // Sync word and MPEG-4, Layer 00
        adtsHeader[2] = (byte)(((profile << 6) & 0xC0) | ((freqIdx << 2) & 0x3C) | ((channels >> 2) & 0x01));
        adtsHeader[3] = (byte)(((channels << 6) & 0xC0) | ((fullFrameLength >> 11) & 0x03));
        adtsHeader[4] = (byte)((fullFrameLength >> 3) & 0xFF);
        adtsHeader[5] = (byte)(((fullFrameLength << 5) & 0xE0) | 0x1F);
        adtsHeader[6] = 0xFC;

        return adtsHeader;
    }

    /// <summary>
    /// 根据码率获取对应的编号
    /// </summary>
    /// <param name="sampleRate"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static int GetFreqIdx(int sampleRate)
    {
        switch (sampleRate)
        {
            case 96000: return 0;
            case 88200: return 1;
            case 64000: return 2;
            case 48000: return 3;
            case 44100: return 4;
            case 32000: return 5;
            case 24000: return 6;
            case 22050: return 7;
            case 16000: return 8;
            case 12000: return 9;
            case 11025: return 10;
            case 8000: return 11;
            case 7350: return 12;
            default: throw new ArgumentOutOfRangeException(nameof(sampleRate), "Invalid sample rate");
        }
    }
}