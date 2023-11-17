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
        List<Object> dataStore = new List<Object>();

        public PlotController(IHubContext<PlotDataHub> hub)
        {
            _hub = hub;
        }

        [HttpGet]
        public IActionResult Get()
        {
            Console.WriteLine(DateTime.Now);

            

            Random r = new Random();

            double amplitude = 1.0;
            double frequency = 1.0; // Adjust frequency as needed
            double phaseShift = 0.0;
            int i = 0;

            // Number of data points
            int numPoints = 200;

            // Time interval between points
            double timeInterval = 0.1;

            var timerManager = new TimerManager(() => {
                double time = i++ * timeInterval;
                double sineValue = (amplitude * Math.Sin(2 * Math.PI * frequency * time + phaseShift)) + 1.0;
                dataStore.Add(new { voltage = sineValue, timestamp = DateTime.Now });
                //dataStore.Add(new { voltage = r.NextDouble(), timestamp = DateTime.Now });
                Console.WriteLine(dataStore.Count());
                if (dataStore.Count() > 200) {
                    Console.WriteLine("Hii");
                    _hub.Clients.All.SendAsync("transferPlotData", dataStore);
                    dataStore.Clear();
                }                
            });

            //var timerManager = new TimerManager(() => _hub.Clients.All.SendAsync("transferPlotData", new { voltage = r.NextDouble(), timestamp = DateTime.Now }));

            return Ok(new { Message = "Request Completed" });
        }
    }
}
