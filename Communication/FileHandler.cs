namespace Communication;

public class FileHandler
{
    public Task<bool> FileExists(string path)
    {
        return Task.FromResult(File.Exists(path));
    }

    public async Task<string> GetFileName(string path)
    {
        bool exists = await FileExists(path);
        if (exists)
        {
            return new FileInfo(path).Name;
        }
        throw new FileNotFoundException();
    }

    public async Task<long> GetFileSize(string path)
    {
        bool exists = await FileExists(path);
        if (exists)
        {
            return new FileInfo(path).Length;
        }
        throw new FileNotFoundException();
    }
}