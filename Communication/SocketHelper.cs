using System.Net.Sockets;

namespace Communication;

public class SocketHelper
{

    private readonly TcpClient _client;
    private readonly NetworkStream _stream;

    public SocketHelper(TcpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _stream = _client.GetStream();
    }

    public async Task SendMessage(string message)
    {
        try
        {
            byte[] data = await ConversionHandler.ConvertStringToByte(message);
            byte[] dataLength = await ConversionHandler.ConvertIntToByte(data.Length);
            Send(dataLength);
            Send(data);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Error sending data to the server: " + ex.Message);
        }
    }

    public async Task<string> ReceiveMessage()
    {
        byte[] responseLength = Receive(FilePartitionerCalculator.FixedDataSize);
        int length = await ConversionHandler.ConvertByteToInt(responseLength);
        byte[] response = Receive(length);
        string message = await ConversionHandler.ConvertByteToString(response);
        return message;
    }

    public async Task SendFileName(string fileName, int fileNameLength)
    {
        Send(await ConversionHandler.ConvertIntToByte(fileNameLength));
        Send(await ConversionHandler.ConvertStringToByte(fileName));
    }

    public async Task SendFileSize(long fileSize)
    {
        byte[] size = await ConversionHandler.ConvertLongToByte(fileSize);
        Send(size);
    }

    public void SendFile(byte[] data)
    {
        Send(data);
    }

    public async Task<string> ReceiveFileName()
    {
        byte[] fileNameLength = Receive(FilePartitionerCalculator.FixedDataSize);
        byte[] fileName = Receive(await ConversionHandler.ConvertByteToInt(fileNameLength));
        return await ConversionHandler.ConvertByteToString(fileName);
    }

    public async Task<long> ReceiveFileSize()
    {
        byte[] fileSize = Receive(FilePartitionerCalculator.FixedFileSize);
        return await ConversionHandler.ConvertByteToLong(fileSize);
    }

    public byte[] ReceiveFileData(int length)
    {
        return Receive(length);
    }

    public void CloseConnection()
    {
        if (_client != null)
        {
            _stream.Close();
            _client.Close();
            Console.WriteLine("Connection closed successfully.");
        }
    }

    public void CloseConnectionShutdown()
    {
        if (_client != null)
        {
            _stream.Close();
            _client.Close();
            Console.WriteLine("Connection closed successfully.");
            Environment.Exit(0);
        }
    }

    private void Send(byte[] data)
    {
        int offset = 0;
        int size = data.Length;

        while (offset < size)
        {
            _stream.Write(data, offset, size - offset);
            offset += size;
        }
    }

    public byte[] Receive(int length)
    {
        byte[] data = new byte[length];
        int bytesRead = 0;
        int offset = 0;

        while (offset < length)
        {
            bytesRead = _stream.Read(data, offset, length - offset);
            if (bytesRead == 0)
            {
                throw new SocketException();
            }
            offset += bytesRead;
        }

        return data;
    }
}