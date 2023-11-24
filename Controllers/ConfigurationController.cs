using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.IO.Ports;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;
using WaveMaster_Backend.Observers;
using WaveMaster_Backend.Services;
using WaveMaster_Backend.ViewModels;

namespace WaveMaster_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly ISharedVariableService _sharedVariableService;
        private readonly IReadService _readService;
        
        public ConfigurationController(ISharedVariableService sharedVariableService,IReadService readService)
        {
            _sharedVariableService = sharedVariableService;
            _readService = readService;
        }

        [HttpGet]
        public IEnumerable<string> GetPortName()
        {
            var ports = SerialPort.GetPortNames();
            return ports;
        }
      
        [HttpPost("connect")]
        public IActionResult PostConnect(ConenctionParamsModel value)
        {
            SerialPort _serialPort = new SerialPort();
            _serialPort.PortName = value.portName;
            _serialPort.BaudRate = value.baudRate;
            _serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), value.parity, true);
            _serialPort.DataBits = value.dataBit;
            _serialPort.StopBits = (StopBits)value.stopBit;
            _serialPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), "None", true);
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            try
            {
                //_serialPort.DataReceived += new SerialDataReceivedEventHandler(_readService.DataReceivedHandler);
                _serialPort.Open();
                Thread rxThread = new Thread(_readService.DataReceivedHandler);
                rxThread.Start();
            }
            catch(Exception ex) {
                Console.WriteLine(ex);
                return StatusCode(500, $"ConfigurationController : PostConnect() : {ex}");
            }

            _sharedVariableService.serialPort = _serialPort;
            return Ok(new { message = "ConfigurationController : PostConnect() - Connected Successfully!" });
        }

        [HttpPost("disconnect")]
        public IActionResult PostDisconnect()
        {
            _sharedVariableService.SendData("STOP CONNECTION;");
            try
            {
                //_sharedVariableService.serialPort.DataReceived -= _readService.DataReceivedHandler;
                _sharedVariableService.serialPort.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, $"ConfigurationController : PostDisconnect() : {ex.Message}");
            }
            return Ok(new { message = "ConfigurationController : PostDisconnect() - Connection Disconnected Successfully!" });
        }
    }
}
