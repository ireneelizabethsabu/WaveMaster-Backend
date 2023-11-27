using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;
using WaveMaster_Backend.Observers;
using WaveMaster_Backend.Services;
using WaveMaster_Backend.ViewModels;

namespace WaveMaster_Backend.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CaptureController : ControllerBase
    {
        private readonly ISharedVariableService _sharedVariableService;
        private readonly IReadService _readService;
        private readonly IObserverService _observerService;

        public CaptureController(ISharedVariableService sharedVariableService, IReadService readService
            ,IObserverService observerService)
        {
            _sharedVariableService = sharedVariableService;
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
            _sharedVariableService.SendData($"CAPTURE {value};");
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
            _sharedVariableService.SendData("GET CAPTURE FREQUENCY;");
            _sharedVariableService.SendData("GET CAPTURE PEAKTOPEAK;");
            //_readService.Mode = "FETCH";           
        }


        [HttpPost("rate")]
        public IActionResult PostRate([FromBody] int rate)
        {
            Console.WriteLine(rate);
            _sharedVariableService.DataAcquisitionRate = rate;
            return Ok(new { message = "ConfigurationController : PostRate() -  Successful!" });
        }
    }
}