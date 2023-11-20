using Microsoft.AspNetCore.SignalR;
using System.IO.Ports;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;

namespace WaveMaster_Backend.Services
{
    public interface ISharedVariableService
    {
        
        SerialPort serialPort { get; set; }
        public void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e);

        public void SendData(string command);

        public int DataAcquisitionRate {  get; set; }

    }
    public class CommunicationService : ISharedVariableService
    {
        private readonly WaveMasterDbContext _context;
        public SerialPort serialPort{ get; set; }

        public string ReceivedString { get; set; } = String.Empty;

        byte[] buf {  get; set; }
        public string Mode { get; set; } = "CAPTURE";

        List<PlotData> dataStore = new List<PlotData>();

        public int DataAcquisitionRate { get; set; } = 1;

        private readonly IHubContext<PlotDataHub> _hub;



        public int count = 0;
        public CommunicationService(WaveMasterDbContext context ,IHubContext<PlotDataHub> hub)
        {
            _hub = hub;
            _context = context;
        }
        public void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            Random r = new Random();
            //Console.WriteLine("Received Hex Data: " );
            if (Mode.Equals("CAPTURE"))
            {
                byte[] buffer = new byte[2];
                //Console.WriteLine($"..........................{DataAcquisitionRate}......................");
                //Thread.Sleep(DataAcquisitionRate);
                sp.Read(buffer, 0, 2);
                PlotData pd = new PlotData();
                string hexData = BitConverter.ToString(buffer).Replace("-", ""); 
                //Console.WriteLine("Received Hex Data: " + hexData);
                pd.voltage = Convert.ToInt32("0x" + hexData, 16);
                pd.time = DateTime.Now;
                //Console.WriteLine($"{pd.voltage} - {pd.time} ");
                               
                dataStore.Add(pd);
                
                if (dataStore.Count() > 200)
                {
                    Console.WriteLine(".....Hii...........");
                    _hub.Clients.All.SendAsync("transferPlotData", dataStore);
                    //_context.plotDatas.AddRangeAsync(dataStore);
                    
                    //foreach (var item in dataStore)
                    //{
                    //    _context.plotDatas.AddAsync(item);
                    //}
                    //try
                    //{
                    //    _context.SaveChangesAsync();
                    //}catch(Exception ex)
                    //{
                    //    Console.WriteLine(ex);
                    //}
                    dataStore.Clear();
                }

            }
            //else
            //{
            //    ReceivedString = sp.ReadTo("\n");
            //    Console.WriteLine("Data Received : {0}", ReceivedString);
            //    ReceivedString = String.Empty;
            //    serialPort.DiscardInBuffer();
            //}

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
