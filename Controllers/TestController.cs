using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WaveMaster_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpPost]
        public void TestComponent([FromBody] string value)
        {
            Console.WriteLine(value);
        }
    }
}
