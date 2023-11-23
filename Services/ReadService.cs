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
        void DataReceivedHandler(object sender,SerialDataReceivedEventArgs e);
        public IDisposable Subscribe(IObserver<List<PlotData>> observer);
        string Mode { get; set; }
    }
    public class ReadService : IObservable<List<PlotData>>,IReadService
    {
        public string ReceivedString { get; set; } = "READ";
        public string Mode { get; set; } = String.Empty;
        private readonly IHubContext<PlotDataHub> _hub;

        List<PlotData> dataStore = new();
        List<IObserver<List<PlotData>>> observers;

        public ReadService(IHubContext<PlotDataHub> hub)
        { 
            observers = new List<IObserver<List<PlotData>>>();
            _hub = hub;
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
        
        public IDisposable Subscribe(IObserver<List<PlotData>> observer)
        {
            if (! observers.Contains(observer))
                observers.Add(observer);
            Console.WriteLine("added observer");
            return new Unsubscriber(observers, observer);
        }

        public void DataReceivedHandler(object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            if (Mode.Equals("CAPTURE"))
            {
                byte[] buffer = new byte[2];
                

                sp.Read(buffer, 0, 2);
                
                string hexData = BitConverter.ToString(buffer).Replace("-", "");
                //string data = BitConverter.ToString(buffer);
                //string hexData = data.Split("-")[1] + data.Split("-")[0];
                //Console.WriteLine(System.Text.Encoding.Unicode.GetString(buffer));
                
 
                try
                {
                    PlotData pd = new PlotData();
                    pd.voltage = Convert.ToInt32(hexData);
                    pd.time = DateTime.Now;
                    dataStore.Add(pd);
                    //Console.WriteLine($"{pd.voltage} - {pd.time} ");

                    if (dataStore.Count() > 200)
                    {
                        //_hub.Clients.All.SendAsync("transferPlotData", dataStore);
                        NotifyObservers();
                        dataStore.Clear();
                    }
                }
                catch(FormatException ex)
                {
                    Mode = "READ";
                    _hub.Clients.All.SendAsync("captureControl", "STOP CAPTURE");
                    ReceivedString = sp.ReadTo("\n");
                }
                
                
            }
            else if(Mode == "FETCH")
            {
                ReceivedString = sp.ReadTo("\n");
                //ReceivedString = "0.85 3.33";
                Console.WriteLine("Data Received : {0}", ReceivedString);
                _hub.Clients.All.SendAsync("fetchData", ReceivedString);
                Mode = "READ";
            }
            else if (Mode == "READ")
            {
                ReceivedString = sp.ReadTo("\n");
                Console.WriteLine("Data Received : {0}", ReceivedString);
                ReceivedString = String.Empty;
            }
        }

        public void NotifyObservers()
        {
            Console.WriteLine(observers.Count());
            foreach (var observer in observers)
            {
                observer.OnNext(new List<PlotData>(dataStore));                
            }
        }
    }
}
