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
        private readonly IReadService _readService;
        public CaptureController(ISharedVariableService sharedVariableService, IReadService readService)
        {
            _sharedVariableService = sharedVariableService;
            _readService = readService;
        }
       

        
        [HttpPost("plotcommand")]
        public IActionResult PostCommand([FromBody] string value)
        {
            
            _sharedVariableService.SendData($"{value} CAPTURE;");
            _readService.Mode = "CAPTURE";
            return Ok(new { message = "ConfigurationController : PostCommand() -  Successful!" });
        }


        [HttpGet("signaldata")]
        public SignalDataModel GetSignalData()
        {
            SignalDataModel dataModel = new SignalDataModel();
            Random r = new Random();
            _readService.Mode = "FETCH";
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


        [HttpPost("rate")]
        public IActionResult PostRate([FromBody] int rate)
        {
            Console.WriteLine(rate);
            _sharedVariableService.DataAcquisitionRate = rate;
            return Ok(new { message = "ConfigurationController : PostRate() -  Successful!" });
        }
    }
}