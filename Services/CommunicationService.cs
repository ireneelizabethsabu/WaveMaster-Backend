using System.IO.Ports;

namespace WaveMaster_Backend.Services
{
    public interface ISharedVariableService
    {
        
        SerialPort serialPort { get; set; }
        public void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e);
        
    }
    public class CommunicationService : ISharedVariableService
    {
        public SerialPort serialPort{ get; set; }

        //public void Read()
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            string message = serialPort.ReadLine();

        //            Console.WriteLine(message);
        //        }
        //        catch (TimeoutException) { }
        //    }
        //}

        public void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Console.WriteLine("Data Received:");
            Console.Write(indata);
            Console.WriteLine(indata);
        }
    }
}
