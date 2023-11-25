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
            int flag = 0;
            while (sp.IsOpen)
            {                
                try
                {
                    var buffer = new byte[1024];
                    if (sp.BytesToRead == 0)
                        continue;

                    int bytesRead = sp.BaseStream.Read(buffer, 0, 1024);
                    string asciiString = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    //Console.WriteLine(asciiString);
                    if(asciiString.Contains("Capture Stopped;"))
                    {
                        Log.Information("Capture Stopped; received");
                        await _hub.Clients.All.SendAsync("captureControl", "STOP CAPTURE");
                        Mode = "";
                    }else if (asciiString.Contains("DATA"))
                    {
                        Log.Information("Peak to peak and frequency data; received");
                        await _hub.Clients.All.SendAsync("fetchData", asciiString);
                    }
                    else if (asciiString.Contains("Capture Started;"))
                    {
                        Log.Information("Capture Started; received");
                        Mode = "CAPTURE";
                    }
                    else
                    {
                        var dataBuffer = new byte[1024];
                        var indexOfSync = 0;
                        var lengthOfBuffer = bytesRead;
                        bool sync = false;
                        var byteStore = new byte[2];
                        if (asciiString.Contains("SYNC;"))
                        {
                            sync = true;
                            indexOfSync = asciiString.IndexOf(";SYNC;");
                            Log.Information($"SYNC read at index {indexOfSync}");
                            Array.Copy(buffer, 0, dataBuffer, 0, indexOfSync);
                            lengthOfBuffer = indexOfSync;
                        }
                        else
                        {
                            sync = false;
                            dataBuffer = buffer;
                            lengthOfBuffer = bytesRead;
                        }
                        if (Mode.Equals("CAPTURE"))
                        {
                            if (flag == 1)
                            {
                                byteStore[1] = dataBuffer[0];
                                int data = BitConverter.ToUInt16(byteStore, 0);
                                //Console.Write(data);
                                PlotData pd = new PlotData();
                                pd.voltage = data * (3.3 / 4096);
                                pd.time = DateTime.Now;
                                dataStore.Add(pd);
                            }
                            for (int i = flag; i < lengthOfBuffer ; i+=2)
                            {                                
                                if(i+1 < lengthOfBuffer)
                                {
                                    flag = 0;
                                    //var temp = dataBuffer[i];
                                    //dataBuffer[i] = dataBuffer[i + 1];
                                    //dataBuffer[i + 1] = temp;
                                    int data = BitConverter.ToUInt16(dataBuffer, i);
                                    //Console.Write(data);
                                    PlotData pd = new PlotData();
                                    pd.voltage = data * (3.3/4096);
                                    pd.time = DateTime.Now;
                                    dataStore.Add(pd);
                                    Console.WriteLine($"{pd.voltage} - {pd.time} ");
                                }
                                else
                                {
                                    flag =  1;
                                    byteStore[0] = dataBuffer[i];
                                }

                                if(dataStore.Count() > 200)
                                {
                                    NotifyObservers();
                                    dataStore.Clear();
                                }
                            }
                            if(sync)
                            {
                                for (int i = indexOfSync + 6 ; i < bytesRead; i = i + 2)
                                {
                                    //Console.WriteLine((char)buffer[indexOfSync + 5]);
                                    if (i + 1 < bytesRead)
                                    {
                                        flag = 0;
                                        //var temp = buffer[i];
                                        //buffer[i] = buffer[i + 1];
                                        //buffer[i + 1] = temp; 
                                        int data = BitConverter.ToUInt16(buffer, i);
                                        PlotData pd = new PlotData();
                                        pd.voltage = data * (3.3 / 4096);
                                        pd.time = DateTime.Now;
                                        dataStore.Add(pd);
                                        Console.WriteLine($"{pd.voltage} - {pd.time} ");
                                    }
                                    else
                                    {
                                        flag = 1;
                                    }
                                    if (dataStore.Count() > 200)
                                    {
                                        NotifyObservers();
                                        dataStore.Clear();
                                    }
                                }
                            }                               
                        }
                    }        
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} RxCommandsAsync: Exception {ex}");
                }       
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
