using Microsoft.AspNetCore.Mvc;
using System.IO.Ports;
using WaveMaster_Backend.ViewModels;

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
            return ports;
        }

        // GET api/<ConfigurationController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ConfigurationController>
        [HttpPost]
<<<<<<< Updated upstream
        public void Post(ConenctionParamsModel value)
        {
            Console.WriteLine($"Port Name : {value.portName}");
            Console.WriteLine($"Stop Bit : {value.stopBit}");
            Console.WriteLine($"Data Bit : {value.dataBit}");
            Console.WriteLine($"Baud Rate : {value.baudRate}");
            Console.WriteLine($"Parity : {value.parity}");
=======
        public void Post(Object value)
        {
            Console.WriteLine($"Port Name : {value}");
>>>>>>> Stashed changes
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
