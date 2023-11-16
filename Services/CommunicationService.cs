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

        public string ReceivedString { get; set; } = String.Empty;
        public void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            //ReceivedString += sp.ReadExisting();
            //if (ReceivedString.Trim().EndsWith("\n"))
            //{
            //    Console.WriteLine("Data Received:");
            //    Console.Write(ReceivedString);
            //    ReceivedString = String.Empty;
            //}
            ReceivedString = sp.ReadTo("\n");
            //var indata = sp.ReadExisting();            
            Console.WriteLine("Data Received : {0}",ReceivedString);
            serialPort.DiscardInBuffer();
        }

        public void SendData(string command)
        {
            try 
            {
                //SocketCommunication.SendData(command);
                Console.WriteLine(command);

                serialPort.Write(command);
                serialPort.DiscardOutBuffer();
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }            
        }
    }
}
