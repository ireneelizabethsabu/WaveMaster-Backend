using Microsoft.AspNetCore.SignalR;
using System.IO.Ports;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.ViewModels;

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
        public void Connect(ConenctionParamsModel value);
        public void Disconnect();

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
        /// Open serial port connection
        /// </summary>
        /// <param name="value">ConnectionParamsModel instance</param>
        public void Connect(ConenctionParamsModel value)
        {
            try
            {
                serialPort = new SerialPort
                {
                    PortName = value.portName,
                    BaudRate = value.baudRate,
                    Parity = (Parity)Enum.Parse(typeof(Parity), value.parity, true),
                    DataBits = value.dataBit,
                    StopBits = (StopBits)value.stopBit,
                    Handshake = (Handshake)Enum.Parse(typeof(Handshake), "None", true),
                    ReadTimeout = 500,
                    WriteTimeout = 500
                };
                serialPort.Open();
                SendData(Commands.RESET);
            }catch(Exception ex) { }
            
        }

        /// <summary>
        /// Disconnect serial port connection
        /// </summary>
        public void Disconnect()
        {
            try
            {
                SendData(Commands.CONNECTION_STOP);
                serialPort.Close();
            }catch (Exception ex) { }
            
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
