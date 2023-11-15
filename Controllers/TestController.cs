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
        public void TestComponent([FromBody] string value)
        {
            _sharedVariableService.serialPort.WriteLine(String.Format(value));

        }
    }
}
