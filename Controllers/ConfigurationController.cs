using Microsoft.AspNetCore.Mvc;
using System.IO.Ports;
using System.Net;
using WaveMaster_Backend.Services;
using WaveMaster_Backend.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WaveMaster_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {

        private readonly ISharedVariableService _sharedVariableService;
        
        public ConfigurationController(ISharedVariableService sharedVariableService)
        {
            _sharedVariableService = sharedVariableService;
        }

        // GET: api/<ConfigurationController>
        [HttpGet]
        public IEnumerable<string> GetPortName()
        {
            var ports = SerialPort.GetPortNames();
            return ports;
        }
              
        // POST api/<ConfigurationController>
        [HttpPost("connect")]
        public IActionResult PostConnect(ConenctionParamsModel value)
        {
            Console.WriteLine($"Port Name : {value.portName}");
            Console.WriteLine($"Stop Bit : {value.stopBit}");
            Console.WriteLine($"Data Bit : {value.dataBit}");
            Console.WriteLine($"Baud Rate : {value.baudRate}");
            Console.WriteLine($"Parity : {value.parity}");

            SerialPort _serialPort = new SerialPort();

            // Allow the user to set the appropriate properties.
            _serialPort.PortName = value.portName;
            _serialPort.BaudRate = value.baudRate;
            _serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), value.parity, true);
            _serialPort.DataBits = value.dataBit;
            _serialPort.StopBits = (StopBits)value.stopBit;
            _serialPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), "None", true);

            try
            {
                _serialPort.Open();
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(_sharedVariableService.DataReceivedHandler);
            }
            catch(Exception ex) {
                Console.WriteLine(ex);
                return NotFound("Error");
            }

            _sharedVariableService.serialPort = _serialPort;
            //Thread readThread = new Thread(_sharedVariableService.Read);
            //readThread.Start();
            return Ok();
        }

        [HttpPost("disconnect")]
        public IActionResult PostDisconnect(ConenctionParamsModel value)
        {
            try
            {
                _sharedVariableService.serialPort.WriteLine(
                    System.String.Format("STOP CONNECTION;"));
                _sharedVariableService.serialPort.DataReceived -= _sharedVariableService.DataReceivedHandler;
                _sharedVariableService.serialPort.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return NotFound(ex);
            }

            
            return Ok();
        }

    }
}
