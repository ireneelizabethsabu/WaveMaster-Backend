using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;

namespace WaveMaster_Backend.Services
{
    public interface IReadService
    {
        void DataReceivedHandler();
        public IDisposable Subscribe(IObserver<List<PlotData>> observer);
        string Mode { get; set; }
    }
    public class ReadService : IObservable<List<PlotData>>,IReadService
    {
        public string ReceivedString { get; set; } = "READ";
        public string Mode { get; set; } = "";
        private readonly IHubContext<PlotDataHub> _hub;
        private readonly ISharedVariableService _sharedVariableService;
        List<PlotData> dataStore = new();
        List<IObserver<List<PlotData>>> observers;

        public ReadService(IHubContext<PlotDataHub> hub, ISharedVariableService sharedVariableService)
        { 
            observers = new List<IObserver<List<PlotData>>>();
            _hub = hub;
           _sharedVariableService = sharedVariableService;
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

        public async void DataReceivedHandler()
        {
            SerialPort sp = _sharedVariableService.serialPort;
            while (sp.IsOpen)
            {
                var buffer = new byte[1024];
                try
                {  
                    if (sp.BytesToRead == 0)
                        continue;

                    int cnt = sp.BaseStream.Read(buffer, 0, 1024);
                    string asciiString = Encoding.ASCII.GetString(buffer, 0, cnt);
                    Console.WriteLine(asciiString);
                    if(asciiString.Contains("Capture Stopped;"))
                    {
                        Log.Information("Capture Stopped; received");
                        await _hub.Clients.All.SendAsync("captureControl", "STOP CAPTURE");
                        Mode = "";
                    }else if (asciiString.Contains("DATA"))
                    {
                        await _hub.Clients.All.SendAsync("fetchData", asciiString);
                    }
                    else if (asciiString.Contains("Capture Started;"))
                    {
                        Log.Information("Capture Started; received");
                        Mode = "CAPTURE";
                    }
                    else
                    {
                        if (Mode.Equals("CAPTURE"))
                        {
                            for (int i = 0; i < cnt; i++)
                            {
                                if ((char)buffer[i] == ';')
                                {
                                    Console.WriteLine("semiiii");
                                }
                                else
                                {
                                    Console.Write((int)buffer[i]);
                                }
                            }
                            Console.WriteLine();
                        }
                    }        
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} RxCommandsAsync: Exception {ex}");
                }
                //Console.WriteLine(ReceivedString);
                //byte[] buffer = new byte[2];
                //if (Mode.Equals("CAPTURE"))
                //{
                //    try
                //    {
                //        sp.Read(buffer, 0, 2);
                //        string buf = Encoding.ASCII.GetString(buffer);
                //        Console.WriteLine(buf);
                //        if (buf.Equals("Ca"))
                //        {
                //            Console.WriteLine("HELLLLLLLLLLLLLLLLLLLLLLLLLLLOOOOOOOOOOOOOOOOOOOOOOOOOOO");
                //        }
                //        int hexData = BitConverter.ToInt16(buffer, 0);
                //        Console.WriteLine(hexData);
                //        //string hexData = BitConverter.ToString(buffer).Replace("-", "");
                //        //string hexData = BitConverter.ToString(buffer).Replace("-", "");
                //        PlotData pd = new PlotData();
                //        pd.voltage = hexData * (3.3 / 4096);
                //        pd.time = DateTime.Now;
                //        dataStore.Add(pd);
                //        Console.WriteLine($"{pd.voltage} - {pd.time} ");

                //        if (dataStore.Count() > 200)
                //        {
                //            //_hub.Clients.All.SendAsync("transferPlotData", dataStore);
                //            NotifyObservers();
                //            dataStore.Clear();
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        //Mode = "READ";
                //        //_hub.Clients.All.SendAsync("captureControl", "STOP CAPTURE");
                //        //ReceivedString = sp.ReadTo(";");
                //        //Console.WriteLine(ReceivedString);
                //    }
                //}
                
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
