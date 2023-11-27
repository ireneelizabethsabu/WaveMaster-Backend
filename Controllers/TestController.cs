using Microsoft.AspNetCore.Mvc;
using WaveMaster_Backend.Services;

namespace WaveMaster_Backend.Controllers
{
    /// <summary>
    /// Controller responsible for testing components by sending test data.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ISerialPortService _serialportService;

        /// <summary>
        /// Initializes a new instance of the TestController class.
        /// </summary>
        /// <param name="serialportService">The serial port service instance.</param>
        public TestController(ISerialPortService serialportService)
        {
            _serialportService = serialportService;
        }

        /// <summary>
        /// Sends test data to a component for testing purposes.
        /// </summary>
        /// <param name="command">The command to be sent for testing.</param>
        /// <returns>Returns a status indicating success or failure of the test.</returns>
        [HttpPost]
        public IActionResult TestComponents([FromBody] string command)
        {
            _serialportService.SendData(command);
            return Ok(new { message = "TestController : TestComponent() - Test data send success"});
        }
    }
}
