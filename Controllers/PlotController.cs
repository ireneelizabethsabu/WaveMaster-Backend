using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.TimerFunctions;

namespace WaveMaster_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlotController : ControllerBase
    {
        private readonly IHubContext<PlotDataHub> _hub;

        public PlotController(IHubContext<PlotDataHub> hub)
        {
            _hub = hub;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var timerManager = new TimerManager(() => _hub.Clients.All.SendAsync("transferPlotData", 3));

            return Ok(new { Message = "Request Completed" });
        }
    }
}
