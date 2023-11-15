﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WaveMaster_Backend.Services;
using WaveMaster_Backend.ViewModels;

namespace WaveMaster_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GenerateController : ControllerBase
    {

        private readonly ISharedVariableService _sharedVariableService;

        public GenerateController(ISharedVariableService sharedVariableService)
        {
            _sharedVariableService = sharedVariableService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
               
                string filePath = "settings.json";
                string jsonString = System.IO.File.ReadAllText(filePath);

                // Deserialize the JSON string to an object
                var data = JsonSerializer.Deserialize<dynamic>(jsonString);
                Console.WriteLine(data);

                // Work with the deserialized data
                return Ok("GenerateController : Get() - " + data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"GenerateController : Get() - Error reading JSON from file: {ex}");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] SignalDataModel signalData)
        {
            Console.WriteLine(signalData);
            string jsonString = JsonSerializer.Serialize(signalData);
            string filePath = "settings.json";

            try
            {
                _sharedVariableService.serialPort.WriteLine(String.Format("GENERATE {0} {1} {2}", signalData.SignalType, signalData.Frequency, signalData.PeakToPeak));
                // Write the JSON string to the file
                System.IO.File.WriteAllText(filePath, jsonString);
                return Ok("GenerateController : Post() -JSON data has been written to the file.");
            }catch(NullReferenceException ex)
            {
                return StatusCode(500, $"GenerateController : Post() - NULL REFERENCE EXCEPTION - {ex}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"GenerateController : Post() - Error writing JSON to file - {ex}");
            }
        }
    }
}
