using EFCore.BulkExtensions;
using Serilog;
using WaveMaster_Backend.Models;
using WaveMaster_Backend.Services;

namespace WaveMaster_Backend.Observers
{
    /// <summary>
    /// Represents an observer for writing data to database using WaveMasterDbContext 
    /// </summary>
    public class DbObserver : IObserver<List<PlotData>>
    {
        private IDisposable? _unsubscriber;
        private readonly WaveMasterDbContext _context;
        private readonly Mutex _mutex = new(false);

        /// <summary>
        /// Initializes a new instance of the DbObserver class with a WaveMasterDbContext.
        /// </summary>
        /// <param name="context">The WaveMasterDbContext instance.</param>
        public DbObserver(WaveMasterDbContext context)
        {
            _context = context ?? 
                throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Subscribes the DbObserver to an IReadService provider.
        /// </summary>
        /// <param name="provider">The IReadService provider.</param>
        public virtual void Subscribe(IReadService provider)
        {
            if (provider != null)
            {
                _unsubscriber = provider.Subscribe(this);
            }
        }

        /// <summary>
        /// Unsubscribes the DbObserver.
        /// </summary>
        public virtual void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }

        /// <summary>
        /// Handles completion of data transmission.
        /// </summary>
        public virtual void OnCompleted() { }

        /// <summary>
        /// Handles errors during data transmission.
        /// </summary>
        /// <param name="error">The exception occurred during data transmission.</param>
        public virtual void OnError(Exception error)
        {
            Log.Error($"Error occurred in DbObserver: {error.Message}");
        }

        /// <summary>
        /// Handles new data received from the observable and writes it to the database asynchronously.
        /// </summary>
        /// <param name="dataStore">list of PlotData objects</param>
        public async virtual void OnNext(List<PlotData> dataStore)
        {
            try
            {
                await Task.Run(() => {
                    _mutex.WaitOne();
                    _context.BulkInsert(dataStore);
                    _mutex.ReleaseMutex();
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Error writing to db : {ex.Message}");
            }
        }        
    }
}
