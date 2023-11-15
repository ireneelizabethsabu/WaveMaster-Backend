using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WaveMaster_Backend.Services;

namespace WaveMaster_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ISharedVariableService _sharedVariableService;

        public TestController(ISharedVariableService sharedVariableService)
        {
            _sharedVariableService = sharedVariableService;
        }

        [HttpPost]
        public IActionResult TestComponent([FromBody] string value)
        {
            try
            {
                _sharedVariableService.serialPort.WriteLine(String.Format(value));
                return Ok("TestController : TestComponent() - Command send Successfully!");
            }
            catch (NullReferenceException ex)
            {
                return StatusCode(500, $"TestController : TestComponent() - NULL REFERENCE EXCEPTION - {ex}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"TestController : TestComponent() - {ex}");
            }


        }
    }
}
