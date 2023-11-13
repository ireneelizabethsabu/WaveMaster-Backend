namespace WaveMaster_Backend.ViewModels
{
    public class ConenctionParamsModel
    {
        public string portName {get; set;}
        public int stopBit { get; set;}
        public int baudRate { get; set; }
        public int dataBit { get; set; }
        public string parity { get; set; }
    }                
}
