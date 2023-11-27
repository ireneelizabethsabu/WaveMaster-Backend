using Microsoft.AspNetCore.Mvc;
using System.IO.Ports;
using WaveMaster_Backend.Services;
using WaveMaster_Backend.ViewModels;
using Serilog;

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
        private readonly IReadService _readService;
        
        public ConfigurationController(ISerialPortService serialportService,IReadService readService)
        {
            _serialportService = serialportService;
            _readService = readService;
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
                return StatusCode(500, $"Error retrieving port names: {ex.Message}");
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
                SerialPort _serialPort = new SerialPort
                {
                    PortName = value.portName,
                    BaudRate = value.baudRate,
                    Parity = (Parity)Enum.Parse(typeof(Parity), value.parity, true),
                    DataBits = value.dataBit,
                    StopBits = (StopBits)value.stopBit,
                    Handshake = (Handshake)Enum.Parse(typeof(Handshake), "None", true),
                    ReadTimeout = 500,
                    WriteTimeout = 500
                };
                _serialPort.Open();
                _serialportService.serialPort = _serialPort;

                Thread rxThread = new Thread(_readService.DataReceivedHandler);
                rxThread.Start();

                return Ok(new { message = "ConfigurationController : ConnectSerialPort() - Connected Successfully!" });
            }
            catch(ArgumentException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch(Exception ex) {
                return StatusCode(500, $"ConfigurationController : ConnectSerialPort() : {ex}");
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
            _serialportService.SendData("STOP CONNECTION;");
            try
            {
                _serialportService.serialPort.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return StatusCode(500, $"ConfigurationController : DisconnectSerialPort() : {ex.Message}");
            }
            return Ok(new { message = "ConfigurationController : DisconnectSerialPort() - Connection Disconnected Successfully!" });
        }
    }
}
