using System.IO.Ports;

namespace WaveMaster_Backend.Services
{
    public interface ISharedVariableService
    {
        
        SerialPort serialPort { get; set; }
        public void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e);

        public void SendData(string command);


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
            string indata = sp.ReadTo("\r");
            Console.WriteLine("Data Received:");
            Console.WriteLine(indata);
            serialPort.DiscardInBuffer();
        }

        public void SendData(string command)
        {
            try 

            { 
                serialPort.WriteLine(command);
                serialPort.DiscardOutBuffer();
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }            
        }
    }
}
