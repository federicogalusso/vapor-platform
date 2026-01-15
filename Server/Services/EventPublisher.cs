using System.Text;
using System.Text.Json;
using Entities;
using RabbitMQ.Client;

namespace Server.Services;

public class EventPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _gameExchange = "game-events";
    private readonly string _userExchange = "user-events";
    private readonly string _adminExchange = "admin-notifications";

    public EventPublisher()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: _gameExchange, durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: _userExchange, durable: true, exclusive: false, autoDelete: false, arguments: null);

        _channel.ExchangeDeclare(exchange: _adminExchange, type: "fanout");
    }

    public void PublishGameEvent(GameEvent gameEvent)
    {
        var message = JsonSerializer.Serialize(gameEvent);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "", routingKey: _gameExchange, basicProperties: null, body: body);
    }

    public void PublishUserEvent(UserEvent userEvent)
    {
        var message = JsonSerializer.Serialize(userEvent);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "", routingKey: _userExchange, basicProperties: null, body: body);
    }

    public void NotifyAdmins(GameEvent gameEvent)
    {
        var message = JsonSerializer.Serialize(gameEvent);
        var body = Encoding.UTF8.GetBytes(message);
            
        _channel.BasicPublish(exchange: _adminExchange, routingKey: "", basicProperties: null, body: body);
    }

    public void Close()
    {
        _channel.Close();
        _connection.Close();
    }
}
