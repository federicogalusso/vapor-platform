using System.Text;

namespace Communication;

public static class ConversionHandler
{
    public static Task<byte[]> ConvertStringToByte(string data)
    {
        return Task.FromResult(Encoding.UTF8.GetBytes(data));
    }

    public static Task<string> ConvertByteToString(byte[] data)
    {
        return Task.FromResult(Encoding.UTF8.GetString(data));
    }

    public static Task<byte[]> ConvertIntToByte(int data)
    {
        return Task.FromResult(BitConverter.GetBytes(data));
    }

    public static Task<int> ConvertByteToInt(byte[] data)
    {
        return Task.FromResult(BitConverter.ToInt32(data));
    }

    public static Task<byte[]> ConvertLongToByte(long data)
    {
        return Task.FromResult(BitConverter.GetBytes(data));
    }

    public static Task<long> ConvertByteToLong(byte[] data)
    {
        return Task.FromResult(BitConverter.ToInt64(data));
    }
}