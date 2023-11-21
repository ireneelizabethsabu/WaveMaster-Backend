using WaveMaster_Backend.Models;
using WaveMaster_Backend.Services;

namespace WaveMaster_Backend.Observers
{
    public class DbObserver : IObserver<List<PlotData>>
    {
        private IDisposable unsubscriber;
        private readonly WaveMasterDbContext _context;

        public DbObserver(WaveMasterDbContext context)
        {
            _context = context;
        }

        public virtual void Subscribe(ReadService provider)
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
            Console.WriteLine("Additional temperature data will not be transmitted.");
        }

        public virtual void OnError(Exception error)
        {
            // Do nothing.
        }

        public virtual void OnNext(List<PlotData> dataStore)
        {
            Console.WriteLine("Going to wait");
            Task.Delay(10000);
            Console.WriteLine("writing dataStore of count " + dataStore.Count());

            _context.plotDatas.AddRange(dataStore);
            _context.SaveChanges();
        }
    }
}
