using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.IO.Ports;
using WaveMaster_Backend.Services;
using WaveMaster_Backend.ViewModels;

namespace WaveMaster_Backend.Controllers
{
    /// <summary>
    /// API controller to configure,connect and disconnect serial port.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly ISerialPortService _serialportService;
        private static Thread RxThread { get; set; }
        public ConfigurationController(ISerialPortService serialportService)
        {
            _serialportService = serialportService;
        }

        /// <summary>
        /// Retrieves available port names
        /// </summary>
        /// <returns>Returns list of available port names on success and 
        /// error message in case of failure</returns>
        [HttpGet]
        public IActionResult GetAvailablePortName()
        {
            try
            {
                var ports = SerialPort.GetPortNames();
                return Ok(ports);
            }
            catch (Exception ex)
            {
                Log.Error("GetAvailablePortName() " + ex.ToString());
                return StatusCode(500, new { message = "Unable to fetch ports" });
            }
        }

        /// <summary>
        /// Connects to the serial port based on provided parameters and starts a thread to
        /// listen to the serial port.
        /// </summary>
        /// <param name="value">Connection parameters.</param>
        /// <returns>Returns status indicating successful connection or error.</returns>
        [HttpPost("connect")]
        public IActionResult ConnectSerialPort(ConenctionParamsModel value)
        {
            try
            {
                //Connect to serial port
                _serialportService.Connect(value);
                //listen to the incoming data on serial port
                RxThread = new Thread(_serialportService.DataReceivedHandler);
                RxThread.Start();

                return Ok(new { message = "ESTABLISHED CONNECTION" });
            }
            catch (ArgumentException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, new { message = "Unable to connect to serial port, check if connected elsewhere." });
            }
        }

        /// <summary>
        /// Disconnects from the serial port.
        /// </summary>
        /// <returns>
        /// Returns status indicating successful disconnection or error.
        /// </returns>
        [HttpPost("disconnect")]
        public IActionResult DisconnectSerialPort()
        {
            //_serialportService.SendData(Commands.CONNECTION_STOP);
            try
            {
                _serialportService.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return StatusCode(500, $"Error disconnecting from serial port");
            }
            finally
            {
                RxThread.Join();
            }
            return Ok(new { message = "ConfigurationController : DisconnectSerialPort() - Connection Disconnected Successfully!" });
        }
    }
}
