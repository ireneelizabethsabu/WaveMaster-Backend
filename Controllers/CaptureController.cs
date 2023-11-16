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
            try
            {
                
                    //_sharedVariableService.serialPort.WriteLine(
                    //    System.String.Format("GET CAPTURE DATA;"));
            }catch(NullReferenceException ex)
            {
                Console.WriteLine(ex);

            }catch(Exception ex) 
            {
                Console.WriteLine(ex);
            }
            
            
            return dataModel;
        }

        [HttpGet("signaldata")]
        public SignalDataModel GetSignalData()
        {
            SignalDataModel dataModel = new SignalDataModel();
            Random r = new Random();
            
            
            //command to fetch signal data from mcb
            _sharedVariableService.serialPort.WriteLine(
                    System.String.Format("GET FREQUENCY;"));
            _sharedVariableService.serialPort.WriteLine(
                    System.String.Format("GET PEAKTOPEAK;"));

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
            Console.WriteLine(value);
            try
            {
                if (value.Equals("START"))
                {
                    //_sharedVariableService.serialPort.WriteLine(
                    //    System.String.Format("START CAPTURE;"));
                    return Ok(new { message = "ConfigurationController : PostDisconnect() - Connection Connected Successfully!" });
                }
                else if (value.Equals("STOP"))
                {
                    //_sharedVariableService.serialPort.WriteLine(
                    //     System.String.Format("STOP CAPTURE;"));
                    //var msg = new { "message" : "ConfigurationController : PostDisconnect() - Connection Disconnected Successfully!"};
                    return Ok(new{ message = "ConfigurationController : PostDisconnect() - Connection Disconnected Successfully!" });

                }
            }
            catch (NullReferenceException ex)
            {
                return StatusCode(500, $"CaptureController : PostCommand() - NULL REFERENCE EXCEPTION: {ex}");
            }

            return StatusCode(500, $"CaptureController : PostCommand() - not received either START or STOP");
        }
    }
}
