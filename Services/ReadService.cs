using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.IO.Ports;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;

namespace WaveMaster_Backend.Services
{
    public interface IReadService
    {
        public void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e);
    }

    public class ReadService : IReadService
    {
        private readonly IHubContext<PlotDataHub> _hub;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public string ReceivedString { get; set; } = String.Empty;
        public string Mode { get; set; } = "CAPTURE";
        List<PlotData> dataStore = new List<PlotData>();
        public ReadService(
            IServiceScopeFactory serviceScopeFactory
            , IHubContext<PlotDataHub> hub)
        {

            //_context = context;
            _hub = hub;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            if (Mode.Equals("CAPTURE"))
            {
                byte[] buffer = new byte[2];
                //Console.WriteLine($"..........................{DataAcquisitionRate}......................");

                sp.Read(buffer, 0, 2);
                string hexData = BitConverter.ToString(buffer).Replace("-", "");

                PlotData pd = new PlotData();
                pd.voltage = Convert.ToInt32("0x" + hexData, 16);
                pd.time = DateTime.Now;
                dataStore.Add(pd);
                //Console.WriteLine($"{pd.voltage} - {pd.time} ");

                if (dataStore.Count() > 200)
                {
                    Log.Information("Data STore count - 200");
                    _hub.Clients.All.SendAsync("transferPlotData", dataStore);
                    // Task.Run(() => WriteToDB(dataStore,_serviceScopeFactory));

                    List<PlotData> points = new List<PlotData>(dataStore);
                    await WriteToDB(points, _serviceScopeFactory);

                    dataStore.Clear();
                }

            }
            else
            {
                ReceivedString = sp.ReadTo("\n");
                Console.WriteLine("Data Received : {0}", ReceivedString);
                ReceivedString = String.Empty;
                //serialPort.DiscardInBuffer();
            }
        }
        public async Task WriteToDB(List<PlotData> dataStore, IServiceScopeFactory serviceScopeFactory)
        {
            
            //await Task.Delay(30000);
           
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<WaveMasterDbContext>();
                await dbContext.plotDatas.AddRangeAsync(dataStore).ContinueWith(x => dbContext.SaveChangesAsync());
            }
        }
    }
}
