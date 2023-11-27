using Microsoft.AspNetCore.Mvc;
using Serilog;
using WaveMaster_Backend.Services;

namespace WaveMaster_Backend.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CaptureController : ControllerBase
    {
        private readonly ISerialPortService _serialportService;
        private readonly IReadService _readService;
        private readonly IObserverService _observerService;

        public CaptureController(ISerialPortService serialportService, IReadService readService
            ,IObserverService observerService)
        {
            _serialportService = serialportService;
            _readService = readService;
            _observerService = observerService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>        
        [HttpPost("plotcommand")]
        public IActionResult PostCommand([FromBody] string value)
        {
            _serialportService.SendData($"CAPTURE {value};");
            if (value.Equals("START"))
            {
               
                _observerService.HubObserver.Subscribe(_readService);
                Log.Information("Hub Observer Subscribed");
                _observerService.DbObserver.Subscribe(_readService);
                Log.Information("Db Observer Subscribed");
                _readService.Mode = "CAPTURE";
            }
            else if (value.Equals("STOP"))
            {
                _readService.Mode = "READ";
                //UNSUBSCRIBE OBSERVERS HERE
                _observerService.HubObserver.Unsubscribe();
                Log.Information("Hub Observer UnSubscribed");
                _observerService.DbObserver.Unsubscribe();
                Log.Information("Db Observer UnSubscribed");
            }
            return Ok(new { message = "ConfigurationController : PostCommand() -  Successful!" });
        }


        [HttpGet("signaldata")]
        public void GetSignalData()
        {
            _serialportService.SendData("GET CAPTURE FREQUENCY;");
            _serialportService.SendData("GET CAPTURE PEAKTOPEAK;");
            //_readService.Mode = "FETCH";           
        }


        [HttpPost("rate")]
        public IActionResult PostRate([FromBody] int rate)
        {
            Console.WriteLine(rate);
            _serialportService.DataAcquisitionRate = rate;
            return Ok(new { message = "ConfigurationController : PostRate() -  Successful!" });
        }
    }
}