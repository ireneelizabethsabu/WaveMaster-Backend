using Microsoft.AspNetCore.Mvc;
using Serilog;
using WaveMaster_Backend.Services;

namespace WaveMaster_Backend.Controllers
{
    /// <summary>
    /// Controller for signal capturing 
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CaptureController : ControllerBase
    {
        private readonly ISerialPortService _serialportService;
        private readonly IReadService _readService;
        private readonly IObserverService _observerService;

        /// <summary>
        /// Initializes an instance of Capture controller by injecting required dependencies. 
        /// </summary>
        /// <param name="serialportService"> The serial port service instance</param>
        /// <param name="readService">The read service instance</param>
        /// <param name="observerService">observer service instance</param>
        public CaptureController(ISerialPortService serialportService, IReadService readService
            ,IObserverService observerService)
        {
            _serialportService = serialportService;
            _readService = readService;
            _observerService = observerService;
        }

        /// <summary>
        /// Subscribe and unsubscribe observers based on board status
        /// </summary>
        /// <param name="command">board_start or board_stop command</param>
        /// <returns>Returns an HTTP action result indicating the status of the operation.</returns>
        [HttpPost("observers")]
        public IActionResult HandleObservers([FromBody] string command)
        {
            try
            {

                if (command.Equals("BOARD_START"))
                {
                    _observerService.SubscribeObservers();

                }
                else if (command.Equals("BOARD_STOP"))
                {
                    _observerService.UnsubscribeObservers();

                }
                return Ok(new { message = "ConfigurationController : HandleObservers() -  Successful!" });
            }
            catch (Exception ex)
            {
                Log.Error($"Error with observers: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }

        }

        /// <summary>
        /// Handles sending commands to the serial port based on the received value.
        /// </summary>
        /// <param name="command">start or stop command</param>
        /// <returns>Returns an HTTP action result indicating the status of the operation.</returns>        
        [HttpPost("plotcommand")]
        public IActionResult PostCommand([FromBody] string command)
        {
            try
            {
                _serialportService.SendData($"CAPTURE {command};");               
                return Ok(new { message = "ConfigurationController : PostCommand() -  Successful!" });
            }
            catch (Exception ex)
            {
                Log.Error($"Error sending command: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
            
        }

        /// <summary>
        /// Requests capture frequency and peak-to-peak data from the serial port.
        /// </summary>
        /// /// <returns>
        /// Returns an HTTP action result indicating the status of the operation.
        /// </returns>
        [HttpGet("signaldata")]
        public IActionResult GetSignalData()
        {
            try
            {
                _serialportService.SendData("GET CAPTURE FREQUENCY;");
                _serialportService.SendData("GET CAPTURE PEAKTOPEAK;");

                return Ok(new { message = "Signal data requested successfully!" });
            }
            catch(Exception ex)
            {
                Log.Error($"Error getting signal data: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
                    
        }

        /// <summary>
        /// Sets the data acquisition rate for the serial port.
        /// </summary>
        /// <param name="rate">The data acquisition rate to be set.</param>
        /// <returns>Returns an HTTP action result indicating the status of the operation.</returns>
        [HttpPost("rate")]
        public IActionResult SetDataAcquisitionRate([FromBody] int rate)
        {
            _serialportService.DataAcquisitionRate = rate;
            return Ok(new { message = "ConfigurationController : PostRate() -  Successful!" });
        }
    }
}