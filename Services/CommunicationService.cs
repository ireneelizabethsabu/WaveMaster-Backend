using System.IO.Ports;

namespace WaveMaster_Backend.Services
{
    public interface ISharedVariableService
    {
        
        SerialPort serialPort { get; set; }

        public void Read();
    }
    public class CommunicationService : ISharedVariableService
    {
        public SerialPort serialPort{ get; set; }

        public void Read()
        {
            while (true)
            {
                try
                {
                    string message = serialPort.ReadLine();
                    Console.WriteLine(message);
                }
                catch (TimeoutException) { }
            }
        }
    }
}
