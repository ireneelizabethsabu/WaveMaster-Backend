using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.IO.Ports;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;

namespace WaveMaster_Backend.Services
{
    public interface ISharedVariableService
    {
        SerialPort serialPort { get; set; }
        
        public void SendData(string command);

        public int DataAcquisitionRate {  get; set; }

    }
    public class CommunicationService : ISharedVariableService
    {
        public SerialPort serialPort{ get; set; }
        public string ReceivedString { get; set; } = String.Empty;
        public string Mode { get; set; } = "CAPTURE";
        public int DataAcquisitionRate { get; set; } = 1;

        private readonly IHubContext<PlotDataHub> _hub;

        public int count = 0;
        public CommunicationService(
            IHubContext<PlotDataHub> hub)
        {
            _hub = hub;
        }

        public void SendData(string command)
        {
            try 
            {
                //SocketCommunication.SendData(command);
                Console.WriteLine(command);
                if(command.Equals("GET CAPTURE DATA;"))
                {
                    Mode = "CAPTURE";
                }
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
