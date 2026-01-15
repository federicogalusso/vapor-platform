namespace Communication;

public static class FilePartitionerCalculator
{
    public static readonly int FixedDataSize = 4;   
    public const int FixedFileSize = 8;
    public const int MaxPartitionSize = 32768; 

    public static Task<long> CalculateFileParts(long fileSize)
    {
        long parts = fileSize / MaxPartitionSize;
        long remainder = fileSize % MaxPartitionSize;

        if (remainder > 0)
        {
            parts++;
        }

        return Task.FromResult(parts);
    }
}