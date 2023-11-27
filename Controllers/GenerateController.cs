using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WaveMaster_Backend.Services;
using WaveMaster_Backend.ViewModels;

namespace WaveMaster_Backend.Controllers
{
    /// <summary>
    /// Controller for signal generation
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class GenerateController : ControllerBase
    {
        private readonly ISerialPortService _serialportService;

        /// <summary>
        /// Initializes a new instance of the GenerateController class.
        /// </summary>
        /// <param name="serialportService">The serial port service instance.</param>
        public GenerateController(ISerialPortService serialportService)
        {
            _serialportService = serialportService;
        }

        /// <summary>
        /// Retrieves signal generation settings from a JSON file.
        /// </summary>
        /// <returns>
        /// Returns a status indicating success or failure of retrieving settings file. In case of success returns the retrieved settings
        /// </returns>
        [HttpGet]
        public IActionResult GetSignalSettings()
        {           
            try
            {
                string filePath = "settings.json";
                string jsonString = System.IO.File.ReadAllText(filePath);
                var settings = JsonSerializer.Deserialize<dynamic>(jsonString);
                return Ok(settings);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound($"SignalGenerationController : GetSignalSettings() - File not found: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return StatusCode(500, $"SignalGenerationController : GetSignalSettings() - JSON parsing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"GenerateController : Get() - Error reading JSON from file: {ex}");
            }
        }

        /// <summary>
        /// Starts signal generation based on provided signal data.
        /// </summary>
        /// <param name="signalData">contains signal type, frequency and peak to peak of the wave to be generated</param>
        /// <returns>
        /// Returns a status indicating success or failure of signal generation
        /// </returns>
        [HttpPost("start")]
        public IActionResult StartSignalGeneration([FromBody] SignalDataModel signalData)
        {           
            try
            {
                string jsonString = JsonSerializer.Serialize(signalData);
                string filePath = "settings.json";
                _serialportService.SendData($"GENERATE START;");
                _serialportService.SendData($"GENERATE {signalData.SignalType.ToUpper()} {signalData.Frequency} {signalData.PeakToPeak};");
                System.IO.File.WriteAllText(filePath, jsonString);
                return Ok(new { message = "GenerateController : StartSignalGeneration() -JSON data has been written to the file." });
            }
            catch(IOException ex)
            {         
                return StatusCode(500, $"GenerateController : StartSignalGeneration() - {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"GenerateController : StartSignalGeneration() - Error writing JSON to file - {ex.Message}");
            }           
        }

        /// <summary>
        /// Stops the signal generation process.
        /// </summary>
        [HttpPost("stop")]
        public IActionResult StopSignalGeneration()
        {
            try
            {
                _serialportService.SendData($"GENERATE STOP;");
                return Ok(new { message = "GenerateController : StopSignalGeneration() - success." });
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"SignalGenerationController : StopSignalGeneration() - Error: {ex.Message}");
            }                       
        }
    }
}
