using System.Net.Sockets;

namespace Communication;

public class FileCommunicationHandler
{
    private readonly FileHandler _fileHandler;
    private readonly FileStreamHandler _fileStreamHandler;
    private readonly SocketHelper _socketHelper;

    public FileCommunicationHandler(TcpClient client)
    {
        _fileHandler = new FileHandler();
        _fileStreamHandler = new FileStreamHandler();
        _socketHelper = new SocketHelper(client);
    }

    public async Task SendFile(string path)
    {
        if (await _fileHandler.FileExists(path))
        {
            string fileName = await _fileHandler.GetFileName(path);
            int nameLength = fileName.Length;
            await Task.Run(() => _socketHelper.SendFileName(fileName, nameLength));

            long fileSize = await _fileHandler.GetFileSize(path);
            await Task.Run(() => _socketHelper.SendFileSize(fileSize));

            await SendFileWithStream(fileSize, path);
        }
        else
        {
            throw new FileNotFoundException("File not found");
        }
    }

    public async Task ReceiveFile()
    {
        string fileName = await Task.Run(() => _socketHelper.ReceiveFileName());
        long fileSize = await Task.Run(() => _socketHelper.ReceiveFileSize());
        string filePath = storagePath(fileName);

        await ReceiveFileWithStream(filePath, fileSize);
    }

    private async Task SendFileWithStream(long fileSize, string path)
    {
        long fileParts = await FilePartitionerCalculator.CalculateFileParts(fileSize);
        long offset = 0;
        long current = 1;
        byte[]? data = null;
        while (offset < fileSize)
        {
            if (current == fileParts)
            {
                var lastPartSize = (int)(fileSize - offset);
                data = await Task.Run(() => _fileStreamHandler.Read(path, offset, lastPartSize));
                offset += lastPartSize;
            }
            else
            {
                data = await Task.Run(() => _fileStreamHandler.Read(path, offset, FilePartitionerCalculator.MaxPartitionSize));
                offset += FilePartitionerCalculator.MaxPartitionSize;
            }
            await Task.Run(() => _socketHelper.SendFile(data));
            current++;
        }
    }

    private async Task ReceiveFileWithStream(string fileName, long fileSize)
    {
        long fileParts = await FilePartitionerCalculator.CalculateFileParts(fileSize);
        long offset = 0;
        long current = 1;
        while (offset < fileSize)
        {
            byte[] data;
            if (current == fileParts)
            {
                var lastPartSize = (int)(fileSize - offset);
                data = await Task.Run(() => _socketHelper.ReceiveFileData(lastPartSize));
                offset += lastPartSize;
            }
            else
            {
                data = await Task.Run(() => _socketHelper.ReceiveFileData(FilePartitionerCalculator.MaxPartitionSize));
                offset += FilePartitionerCalculator.MaxPartitionSize;
            }
            await Task.Run(() => _fileStreamHandler.Write(fileName, data));
            current++;
        }
    }

    private string storagePath(string fileName)
    {
        string newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImageFiles");
        if (!Directory.Exists(newPath))
        {
            Directory.CreateDirectory(newPath);
        }
        return newPath + "\\" + fileName;
    }
}