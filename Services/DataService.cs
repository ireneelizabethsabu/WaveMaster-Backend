using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;

namespace WaveMaster_Backend.Services
{
    public interface IDataService
    {
        void handleData(string data, byte[] buffer,int bytesRead);
        IDisposable Subscribe(IObserver<List<PlotData>> observer);
    }
    public class DataService : IDataService
    {
        int flag = 0;
        List<PlotData> dataStore = new();
        private readonly IHubContext<PlotDataHub> _hub;
        List<IObserver<List<PlotData>>> observers = new List<IObserver<List<PlotData>>>();
        public static string Mode { get; set; } = "";
        public DataService(IHubContext<PlotDataHub> hub) {
            _hub = hub;
        }

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
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }

        /// <summary>
        /// Subscribes an observer to the read service.
        /// </summary>
        /// <param name="observer">The observer to subscribe.</param>
        /// <returns>An IDisposable to allow unsubscribing.</returns>
        public IDisposable Subscribe(IObserver<List<PlotData>> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            Console.WriteLine("added observer");
            return new Unsubscriber(observers, observer);
        }
        public async void WriteToHub(string hubEvent, string data)
        {
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

            if (dataStore.Count() > 200)
            {
                NotifyObservers();
                dataStore.Clear();
            }
        }

        public void processData(int initialValue, int length, ref int flag, byte[] buffer, byte[] byteStore)
        {
            for (int i = initialValue; i < length; i += 2)
            {
                if (i + 1 < length)
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
            }
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

        public void handleData(string asciiString, byte[] buffer, int bytesRead)
        {
            if (Mode.Equals("TEST"))
            {
                if (asciiString.Contains("User Mode Started;"))
                {
                    Mode = "";
                }

                WriteToHub("test", asciiString);

            }
            else if (asciiString.Contains("Capture Stopped;"))
            {
                Log.Information("Capture Stopped; received");
                WriteToHub("captureControl", "STOP CAPTURE");
                Mode = "";
            }
            else if (asciiString.Contains("Test Mode Started;"))
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
            else if (Mode.Equals("CAPTURE"))
            {
                byte[] byteStore = new byte[2];
                bool hasSync = asciiString.Contains("SYNC;");
                int startIndex = hasSync ? asciiString.IndexOf(";SYNC;") : 0;
                int bufferLength = hasSync ? startIndex : bytesRead;

                if (flag == 1)
                {
                    byteStore[1] = buffer[0];
                    int data = BitConverter.ToUInt16(byteStore, 0);
                    AddToDataStore(data);
                }

                processData(flag, bufferLength, ref flag, buffer, byteStore);
                if (hasSync)
                {
                    processData(startIndex + 6, bytesRead, ref flag, buffer, byteStore);
                }
            }
        }
    }
}
