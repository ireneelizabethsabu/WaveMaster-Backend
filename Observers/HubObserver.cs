using Microsoft.AspNetCore.SignalR;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;
using WaveMaster_Backend.Services;

namespace WaveMaster_Backend.Observers
{
    public class HubObserver : IObserver<List<PlotData>>
    {
        private IDisposable unsubscriber;

        private readonly IHubContext<PlotDataHub> _hub;

        public HubObserver(IHubContext<PlotDataHub> hub)
        {
            _hub = hub;
        }

        public virtual void Subscribe(IReadService provider)
        {
            if (provider != null)
            {
                unsubscriber = provider.Subscribe(this);
            }
        }
        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }

        public virtual void OnCompleted()
        {
            Console.WriteLine("Writing to hub complete.");
        }

        public virtual void OnError(Exception error)
        {
            // Do nothing.
        }

        public virtual void OnNext(List<PlotData> dataStore)
        {
            Console.WriteLine("writing to hub");
            _hub.Clients.All.SendAsync("transferPlotData", dataStore);
        }
    }
}
