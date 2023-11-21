using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.IO.Ports;
using System.Net.Sockets;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;

namespace WaveMaster_Backend.Services
{
    public interface IReadService
    {
        void Subscribe(IObserver<List<PlotData>> observer, string name = "");
        void Unsubscribe(IObserver<List<PlotData>> observer);

        void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e);
    }
    public class ReadService : IObservable<List<PlotData>>,IReadService
    {
        private readonly IHubContext<PlotDataHub> _hub;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public string ReceivedString { get; set; } = String.Empty;
        public string Mode { get; set; } = "CAPTURE";
        
        List<PlotData> dataStore = new List<PlotData>();
        List<IObserver<List<PlotData>>> observers;

        public ReadService(IHubContext<PlotDataHub> hub)
        {
            //_context = context;
            _hub = hub;
            observers = new List<IObserver<List<PlotData>>>();
        }
        private class Unsubscriber : IDisposable
        {
            private List<IObserver<List<PlotData>>> _observers;
            private IObserver<List<PlotData>> _observer;

            public Unsubscriber(List<IObserver<List<PlotData>>> observers, IObserver<List<PlotData>> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (! (_observer == null)) _observers.Remove(_observer);
            }
        }

        public void Subscribe(IObserver<List<PlotData>> observer,string name = "")
        {
            Console.WriteLine(name);
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }

        public void Unsubscribe(IObserver<List<PlotData>> observer)
        {
            observers.Remove(observer);
        }
        public IDisposable Subscribe(IObserver<List<PlotData>> observer)
        {
            if (! observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }

        

        public void DataReceivedHandler(
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
                    //await WriteToDB(points, _serviceScopeFactory);
                    NotifyObservers();
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

        public void NotifyObservers()
        {
            foreach (var observer in observers)
            {
                //By Calling the Update method, we are sending notifications to observers
                observer.OnNext(dataStore);
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
