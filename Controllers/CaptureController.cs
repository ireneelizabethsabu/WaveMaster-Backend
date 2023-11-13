using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Ports;
using WaveMaster_Backend.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WaveMaster_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CaptureController : ControllerBase
    {
        [HttpGet("plotdata")]
        public PlotDataModel GetPlotData()
        {
            PlotDataModel dataModel = new PlotDataModel();
            dataModel.Timestamp = DateTime.Now;
            Random r =  new Random();
            dataModel.Voltage = r.NextDouble();
            //command to fetch plot data from mcb
            return dataModel;
        }

        [HttpGet("signaldata")]
        public SignalDataModel GetSignalData()
        {
            SignalDataModel dataModel = new SignalDataModel();
            Random r = new Random();
            dataModel.PeakToPeak = r.NextDouble();
            dataModel.Frequency = r.Next(100, 1000);
            //command to fetch signal data from mcb
            return dataModel;
        }

        
        [HttpPost("plotcommand")]
        public void PostCommand([FromBody] string value)
        {
            Console.WriteLine(value);
            if (value.Equals("START"))
            {
                //send command to start capturing
            }else if (value.Equals("STOP"))
            {
                //send command to stop capturing
            }
        }
    }
}
