using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.Diagnostics;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;
using WaveMaster_Backend.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WaveMaster_Backend.Services
{
    /// <summary>
    /// Interface defining serial port read operation 
    /// </summary>
    public interface IReadService
    {
        void DataReceivedHandler();
        public IDisposable Subscribe(IObserver<List<PlotData>> observer);
        string Mode { get; set; }
    }
    /// <summary>
    /// Handles read operation from serial port by implementing IReadService and IObservable interfaces 
    /// </summary>
    /// <remarks>
    /// For adding observers for read service and also reading and processing data from the serial port.
    /// </remarks>
    public class ReadService : IObservable<List<PlotData>>,IReadService
    {
        public string ReceivedString { get; set; } = "READ";
        public string Mode { get; set; } = "";
        private readonly IHubContext<PlotDataHub> _hub;
        private readonly ISerialPortService _serialPortService;
        List<PlotData> dataStore = new();
        List<IObserver<List<PlotData>>> observers;

        /// <summary>
        /// Initializes an instance of Read service by injecting required dependencies. 
        /// </summary>
        /// <param name="hub"> The hub service instance</param>
        /// <param name="serialPortService">The serial port instance</param>
        public ReadService(IHubContext<PlotDataHub> hub, ISerialPortService sharedVariableService)
        { 
            observers = new List<IObserver<List<PlotData>>>();
            _hub = hub;
           _serialPortService = sharedVariableService;
        }
        /// <summary>
        /// Implementation of IDisposable for unsubscribing observers from ReadService.
        /// </summary>
        private class Unsubscriber : IDisposable
        {
            private List<IObserver<List<PlotData>>> _observers;
            private IObserver<List<PlotData>> _observer;
            /// <summary>
            /// Initializes an instance of Unsubscriber with the given list of observers and the observer to unsubscribe.
            /// </summary>
            /// <param name="observers">The list of observers.</param>
            /// <param name="observer">The observer to unsubscribe.</param>
            public Unsubscriber(List<IObserver<List<PlotData>>> observers, IObserver<List<PlotData>> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }
            /// <summary>
            /// Disposes the observer from the list of observers.
            /// </summary>
            public void Dispose()
            {
                if (! (_observer == null)) _observers.Remove(_observer);
            }
        }

        /// <summary>
        /// Subscribes an observer to the read service.
        /// </summary>
        /// <param name="observer">The observer to subscribe.</param>
        /// <returns>An IDisposable to allow unsubscribing.</returns>
        public IDisposable Subscribe(IObserver<List<PlotData>> observer)
        {
            if (! observers.Contains(observer))
                observers.Add(observer);
            Console.WriteLine("added observer");
            return new Unsubscriber(observers, observer);
        }


        public async void WriteToHub(string hubEvent,string data){
            await _hub.Clients.All.SendAsync(hubEvent, data);
        }

        public void AddToDataStore(int data)
        {
            PlotData pd = new PlotData
            {
                voltage = data * (3.3 / 4096),
                time = DateTime.Now
            };
            dataStore.Add(pd);
        }

        /// <summary>
        /// Handles the read operation from the serial port.
        /// </summary>
        public async void DataReceivedHandler()
        {
            SerialPort sp = _serialPortService.serialPort;
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
                    Console.WriteLine(asciiString);
                    if (Mode.Equals("TEST"))
                    {
                        if (asciiString.Contains("User Mode Started;"))
                        {
                            Mode = "";
                        }
                        Console.WriteLine(asciiString);
                        WriteToHub("test", asciiString);
                     
                    }
                    else if(asciiString.Contains("Capture Stopped;"))
                    {
                        Log.Information("Capture Stopped; received");
                        WriteToHub("captureControl", "STOP CAPTURE");
                        Mode = "";
                    }else if (asciiString.Contains("Test Mode Started;"))
                    {
                        Mode = "TEST";
                    }
                    else if (asciiString.Contains("DATA"))
                    {
                        Log.Information("Peak to peak and frequency data; received");
                        WriteToHub("fetchData", asciiString);
                    }
                    else if (asciiString.Contains("Capture Started;"))
                    {
                        Log.Information("Capture Started; received");
                        WriteToHub("captureControl", "START CAPTURE");
                        Mode = "CAPTURE";
                    }
                    else if (asciiString.Contains("EEPROM"))
                    {                        
                        WriteToHub("defaultData", asciiString);
                    }
                    else if(Mode.Equals("CAPTURE"))
                    {                                                             
                        var byteStore = new byte[2];
                        bool hasSync = asciiString.Contains("SYNC;");
                        int startIndex = hasSync ? asciiString.IndexOf(";SYNC;") : 0;
                        int bufferLength = hasSync ? startIndex : bytesRead;

                        if (flag == 1)
                        {
                            byteStore[1] = buffer[0];
                            int data = BitConverter.ToUInt16(byteStore, 0);
                            AddToDataStore(data);
                        }
                        
                        
                        for (int i = flag; i < bufferLength ; i+=2)
                        {                                
                            if(i+1 < bufferLength)
                            {
                                flag = 0;
                                int data = BitConverter.ToUInt16(buffer, i);
                                AddToDataStore(data);                               
                            }
                            else
                            {
                                flag =  1;
                                byteStore[0] = buffer[i];
                            }

                            if(dataStore.Count() > 200)
                            {
                                NotifyObservers();
                                dataStore.Clear();
                            }
                        }
                        if(hasSync)
                        {

                            for (int i = startIndex + 6 ; i < bytesRead; i = i + 2)
                            {
                                if (i + 1 < bytesRead)
                                {
                                    flag = 0;                                   
                                    int data = BitConverter.ToUInt16(buffer, i);
                                    AddToDataStore(data);                                    
                                }
                                else
                                {
                                    flag = 1;
                                    byteStore[0] = buffer[i];
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
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} RxCommandsAsync: Exception {ex}");
                }
            }           
        }


        public void processData(int startIndex,int bufferLength)
        {

        }
        /// <summary>
        /// Notifies all observers with the current plot data.
        /// </summary>
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
