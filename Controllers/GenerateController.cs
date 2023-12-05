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
        private readonly IFileService _fileService;
        /// <summary>
        /// Initializes a new instance of the GenerateController class.
        /// </summary>
        /// <param name="serialportService">The serial port service instance.</param>
        public GenerateController(ISerialPortService serialportService, IFileService fileService)
        {
            _serialportService = serialportService;
            _fileService = fileService;
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
                var settings = _fileService.FileRead();
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
                _serialportService.SendData(Commands.GENERATE_START);
                _serialportService.SendData($"GENERATE {signalData.SignalType.ToUpper()} {signalData.Frequency} {signalData.PeakToPeak};");
                //file write method
                _fileService.FileWrite(signalData);


                return Ok(new { message = $"DEVICE GENERATING {signalData.SignalType.ToUpper()} SIGNAL" });
            }
            catch (IOException ex)
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
                _serialportService.SendData(Commands.GENERATE_STOP);
                return Ok(new { message = "GenerateController : StopSignalGeneration() - success." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"SignalGenerationController : StopSignalGeneration() - Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Save signal params to eeprom
        /// </summary>
        /// <param name="signalData">contains signal type, frequency and peak to peak of the wave to be generated</param>
        /// <returns>Returns a status indicating success or failure of signal generation</returns>
        [HttpPost("eepromsave")]
        public IActionResult SaveToEEPROM([FromBody] SignalDataModel signalData)
        {
            try
            {

                _serialportService.SendData(Commands.SET_GENERATOR_CONFIG);
                _serialportService.SendData($"GENERATE {signalData.SignalType.ToUpper()} {signalData.Frequency} {signalData.PeakToPeak};");
                return Ok(new { message = "GenerateController : SaveToEEPROM() - success." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"SignalGenerationController : SaveToEEPROM() - Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Read signal params from eeprom
        /// </summary>
        /// <returns>Returns a status indicating success or failure of signal generation</returns>
        [HttpGet("eepromread")]
        public IActionResult ReadFromEEPROM()
        {
            try
            {
                _serialportService.SendData(Commands.GET_GENERATOR_CONFIG);

                return Ok(new { message = "GenerateController : SaveToEEPROM() - success." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"SignalGenerationController : SaveToEEPROM() - Error: {ex.Message}");
            }
        }
    }
}
