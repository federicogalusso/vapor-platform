namespace Communication;

public class FileStreamHandler
{
    private readonly FileHandler _fileHandler;

    public FileStreamHandler()
    {
        _fileHandler = new FileHandler();
    }

    public async Task<byte[]> Read(string path, long offset, int length)
    {
        if (await _fileHandler.FileExists(path))
        {
            using var fileStream = new FileStream(path, FileMode.Open) { Position = offset };
            var data = new byte[length];
            var bytesRead = 0;

            while (bytesRead < length)
            {
                var read = fileStream.Read(data, bytesRead, length - bytesRead);
                if (read == 0)
                {
                    throw new EndOfStreamException("Error reading file");
                }
                bytesRead += read;
            }

            return data;

        }
        throw new FileNotFoundException();
    }

    public async void Write(string path, byte[] data)
    {
        var fileMode = await _fileHandler.FileExists(path) ? FileMode.Append : FileMode.Create;
        using var fileStream = new FileStream(path, fileMode);
        fileStream.Write(data, 0, data.Length);
    }
}