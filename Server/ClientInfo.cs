using System.Net.Sockets;

namespace Server;

public class ClientInfo
{
    public TcpClient Client { get; init; }
    public bool MarkedForShutdown { get; set; }
    public string Username { get; set; } = string.Empty;

    public ClientInfo(TcpClient client)
    {
        Client = client;
        MarkedForShutdown = false;
    }

}
