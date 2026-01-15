using Entities;
using Interfaces.Statistics.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Statistics.Data;
using System.Text;
using System.Text.Json;
public class EventProcessor
{
    private readonly IStatisticsService _statisticsService;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly GameDataAccess _gameEventRepository;
    private readonly UserDataAccess _userEventRepository;

    public EventProcessor(IStatisticsService statisticsService, GameDataAccess gameEventRepository, UserDataAccess userEventRepository)
    {
        _statisticsService = statisticsService;
        _gameEventRepository = gameEventRepository;
        _userEventRepository = userEventRepository;

        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq", // Configurar según tu entorno
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "game-events",
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        _channel.QueueDeclare(queue: "user-events",
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);
    }

    public void StartListening()
    {
        ListenToQueue("game-events", message =>
        {
            var gameEvent = JsonSerializer.Deserialize<GameEvent>(message);

            _gameEventRepository.Add(gameEvent);

            Console.WriteLine($"Game Event Received: {JsonSerializer.Serialize(gameEvent)}");

            _statisticsService.ProcessGameEvent(gameEvent);
        });

        ListenToQueue("user-events", message =>
        {
            var userEvent = JsonSerializer.Deserialize<UserEvent>(message);

            _userEventRepository.Add(userEvent);

            _statisticsService.ProcessUserEvent(userEvent);
        });
    }

    private void ListenToQueue(string queueName, Action<string> onMessageReceived)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            onMessageReceived(message);
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }
}