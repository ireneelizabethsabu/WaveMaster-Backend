using Microsoft.AspNetCore.SignalR;
using Serilog;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;
using WaveMaster_Backend.Services;

namespace WaveMaster_Backend.Observers
{
    /// <summary>
    /// Represents an observer for sending data to SignalR Hub using IHubContext
    /// </summary>
    public class HubObserver : IObserver<List<PlotData>>
    {
        private static IDisposable? _unsubscriber;
        private readonly IHubContext<PlotDataHub> _hub;

        /// <summary>
        /// Initializes a new instance of the HubObserver class with an IHubContext.
        /// </summary>
        /// <param name="hub">The IHubContext instance.</param>
        public HubObserver(IHubContext<PlotDataHub> hub)
        {
            _hub = hub ?? throw new ArgumentNullException(nameof(hub));
        }

        /// <summary>
        /// Subscribes the HubObserver to an IReadService provider.
        /// </summary>
        /// <param name="provider">The IReadService provider.</param>
        public virtual void Subscribe(IDataService provider)
        {
            if (provider != null)
            {
                _unsubscriber = provider.Subscribe(this);
            }
        }

        /// <summary>
        /// Unsubscribes the HubObserver.
        /// </summary>
        public virtual void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }

        /// <summary>
        /// Handles completion of data transmission.
        /// </summary>
        public virtual void OnCompleted()
        {
            Console.WriteLine("Writing to hub complete.");
        }

        /// <summary>
        /// Handles errors during data transmission.
        /// </summary>
        /// <param name="error">The exception occurred during data transmission.</param>
        public virtual void OnError(Exception error)
        {
            Log.Error($"Error while writing to hub : {error.Message}");
        }

        /// <summary>
        /// Handles new data received from the observable and sends it to SignalR Hub.
        /// </summary>
        /// <param name="dataStore">List of PlotData instances</param>
        public virtual void OnNext(List<PlotData> dataStore)
        {
            //sending data to signal R hub via the transferPlotData event
            _hub.Clients.All.SendAsync("transferPlotData", dataStore);
        }
    }
}
