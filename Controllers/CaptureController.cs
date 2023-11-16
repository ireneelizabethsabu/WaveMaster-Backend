﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Ports;
using WaveMaster_Backend.Services;
using WaveMaster_Backend.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WaveMaster_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CaptureController : ControllerBase
    {
        private readonly ISharedVariableService _sharedVariableService;

        public CaptureController(ISharedVariableService sharedVariableService)
        {
            _sharedVariableService = sharedVariableService;
        }

        [HttpGet("plotdata")]
        public PlotDataModel GetPlotData()
        {
            PlotDataModel dataModel = new PlotDataModel();
            dataModel.Timestamp = DateTime.Now;
            Random r =  new Random();
            dataModel.Voltage = r.NextDouble();

            _sharedVariableService.SendData("GET CAPTURE DATA;");        
            return dataModel;
        }

        [HttpGet("signaldata")]
        public SignalDataModel GetSignalData()
        {
            SignalDataModel dataModel = new SignalDataModel();
            Random r = new Random();
            
            _sharedVariableService.SendData("GET CAPTURE DATA;");
            
            while (true)
            {
                try
                {
                    string message = _sharedVariableService.serialPort.ReadLine();
                    Console.WriteLine(message);
                                        
                    //logic to get data 
                    dataModel.PeakToPeak = r.NextDouble();
                    dataModel.Frequency = r.Next(100, 1000);
                    break;
                }
                catch (TimeoutException) { }
            }

            return dataModel;
        }

        
        [HttpPost("plotcommand")]
        public IActionResult PostCommand([FromBody] string value)
        {

            _sharedVariableService.SendData($"{value} CAPTURE;");           
            return Ok(new { message = "ConfigurationController : PostCommand() -  Successful!" });
        }
    }
}