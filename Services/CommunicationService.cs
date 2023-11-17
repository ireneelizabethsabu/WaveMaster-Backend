using Microsoft.AspNetCore.SignalR;
using System.IO.Ports;
using WaveMaster_Backend.HubConfig;

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

        byte[] buf {  get; set; }
        public string Mode { get; set; } = "";

        List<Object> dataStore = new List<Object>();

        private readonly IHubContext<PlotDataHub> _hub;



        public int count = 0;
        public CommunicationService(IHubContext<PlotDataHub> hub)
        {
            _hub = hub;
        }
        public void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            Random r = new Random();
            if (Mode.Equals("CAPTURE"))
            {
                int bytesToRead = sp.BytesToRead;
                byte[] buffer = new byte[2];
                byte[] res = new byte[2];

                sp.Read(buffer, 0, 2);

                // Process the received hex data
                //foreach(byte b in buffer)
                //{
                //if(count < 2)
                //{
                //    res.Append(b);
                //}
                string hexData = BitConverter.ToString(buffer).Replace("-", ""); // Convert bytes to hex string
                //Console.WriteLine("Received Hex Data: " + hexData);
                    var voltage = Convert.ToInt32("0x" + hexData, 16);
                    Console.WriteLine(voltage);
                    dataStore.Add(new { voltage, timestamp = DateTime.Now });
                    if (dataStore.Count() > 200)
                    {
                        Console.WriteLine("Hii");
                        _hub.Clients.All.SendAsync("transferPlotData", dataStore);
                        dataStore.Clear();
                    }

                //}


                //if(buf is null)
                //{
                //    buf = new byte[sp.BytesToRead];
                //}
                //if (count < 2)
                //{

                //    sp.Read(buf, 0, sp.BytesToRead);
                //    count++;
                //}
                //else
                //{
                //    Console.WriteLine(buf);
                //    Console.WriteLine("-----------------------------------");

                //    buf = new byte[sp.BytesToRead];
                //    sp.Read(buf, 0, sp.BytesToRead);
                //    count = 1;

                //    //var int32Val = Convert.ToInt32(ReceivedString, 16);

                //    //Console.WriteLine(int32Val);
                //}

                //var rs = "";
                //ReceivedString = serialPort.ReadExisting();
                //foreach(var c in ReceivedString)
                //{
                //    if(count < 4)
                //    {
                //        rs += c;
                //        count++;
                //    }
                //    else
                //    {
                //        Console.WriteLine(rs);
                //        Console.WriteLine("--------");
                //        rs = String.Empty;
                //        rs += c;
                //        count = 1;
                //    }
                //}
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
