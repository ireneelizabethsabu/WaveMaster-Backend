using Microsoft.AspNetCore.SignalR;
using System.IO.Ports;
using WaveMaster_Backend.HubConfig;

namespace WaveMaster_Backend.Services
{
    /// <summary>
    /// Interface defining method to send data to serial port
    /// </summary>
    public interface ISerialPortService
    {
        SerialPort serialPort { get; set; }
        
        public void SendData(string command);

        public int DataAcquisitionRate {  get; set; }

    }
    public class SerialPortService : ISerialPortService
    {
        
        public SerialPort serialPort{ get; set; }
        public string ReceivedString { get; set; } = String.Empty;

        public int DataAcquisitionRate { get; set; } = 1;

        private readonly IHubContext<PlotDataHub> _hub;

        public int count = 0;
        /// <summary>
        /// Initializes an instance of Serial Port service by injecting required dependencies. 
        /// </summary>
        /// <param name="hub"> The hub service instance</param>
        public SerialPortService(
            IHubContext<PlotDataHub> hub)
        {
            _hub = hub;
           
        }
        /// <summary>
        /// Sends commands to the serial port. 
        /// </summary>
        /// <param name="command"> The command string to be sent</param>
        public void SendData(string command)
        {
            try 
            {
                //SocketCommunication.SendData(command);
                Console.WriteLine("sent command : " + command);
                
                serialPort.Write(command);
                serialPort.DiscardOutBuffer();
            }catch(Exception ex)
            {
                _hub.Clients.All.SendAsync("captureControl", "DEVICE DISCONNECTED");
            }            
        }
    }
}
