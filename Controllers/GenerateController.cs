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
            string filePath = "settings.json";
            try
            {
                string jsonString = System.IO.File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<dynamic>(jsonString);
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
            string jsonString = JsonSerializer.Serialize(signalData);
            string filePath = "settings.json";
            _sharedVariableService.SendData($"GENERATE {signalData.SignalType} {signalData.Frequency} {signalData.PeakToPeak};");
            try
            {
                System.IO.File.WriteAllText(filePath, jsonString); 
            }catch(IOException ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, $"GenerateController : Post() - {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, $"GenerateController : Post() - Error writing JSON to file - {ex.Message}");
            }
            return Ok("GenerateController : Post() -JSON data has been written to the file.");
        }
    }
}
