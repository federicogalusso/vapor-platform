using Entities;
using Microsoft.AspNetCore.Mvc;
using Statistics.Data;
using WebApiRabbitMQ.Service;

namespace Statistics.Controllers;

[ApiController]
[Route("reports")]
public class ReportController : ControllerBase
{
    private readonly ILogger<ReportController> _logger;
    private readonly GameDataAccess _gameDataAccess;
    private static Dictionary<string, int>? _salesByPublisher;
    private static bool _isReportReady = false;

    public ReportController(ILogger<ReportController> logger, GameDataAccess gameDataAccess)
    {
        _logger = logger;
        _gameDataAccess = gameDataAccess;
    }

    [HttpPost]
    public async Task<IActionResult> GenerateReport()
    {
        _isReportReady = false;
        var gamesEvents = _gameDataAccess.GetAllGameEvents();
        _salesByPublisher = await ReportGeneratorService.GetSalesByPublisher(gamesEvents);
        _isReportReady = true;
        return Ok("Report generation started.");
    }

    [HttpGet("status")]
    public IActionResult GetReportStatus()
    {
        return Ok(new ReportStatus { IsReady = _isReportReady });
    }

    [HttpGet]
    public IActionResult GetReport()
    {
        if (!_isReportReady)
        {
            return BadRequest("Report is not ready yet.");
        }
        return Ok(_salesByPublisher);
    }
}