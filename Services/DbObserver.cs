using WaveMaster_Backend.Models;

namespace WaveMaster_Backend.Services
{
    public class DbObserver : IObserver<List<PlotData>>
    {
        private IDisposable unsubscriber;
        private readonly IReadService _readService;
        public DbObserver(IReadService readService)
        {
            _readService = readService;
            _readService.Subscribe(this);
        }
        public virtual void Subscribe(IObservable<List<PlotData>> provider)
        {
            if(provider != null)
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
            Console.WriteLine("Additional temperature data will not be transmitted.");
        }

        public virtual void OnError(Exception error)
        {
            // Do nothing.
        }

        public virtual void OnNext(List<PlotData> value)
        {
            Console.WriteLine("writing");

        }
    }
}
