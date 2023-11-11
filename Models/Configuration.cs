using System.IO.Ports;

namespace WaveMaster_Backend.Models
{
    public class Configuration
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBit { get; set; }
        public string Parity { get; set; }

        public int StopBit { get; set; }

    }
}
