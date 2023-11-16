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
            Console.WriteLine(DateTime.Now);
            Random r = new Random();
            var timerManager = new TimerManager(() => _hub.Clients.All.SendAsync("transferPlotData", new { voltage = r.NextDouble(), timestamp = DateTime.Now }));

            return Ok(new { Message = "Request Completed" });
        }
    }
}
