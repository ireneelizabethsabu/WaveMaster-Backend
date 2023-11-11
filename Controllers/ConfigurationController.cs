using Microsoft.AspNetCore.Mvc;
using System.IO.Ports;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WaveMaster_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        // GET: api/<ConfigurationController>
        [HttpGet]
        public IEnumerable<string> GetPortName()
        {
            var ports = SerialPort.GetPortNames();
            return new string[] { "value1", "value2" };
        }

        // GET api/<ConfigurationController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ConfigurationController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ConfigurationController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ConfigurationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
