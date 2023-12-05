using Microsoft.AspNetCore.SignalR;
using Serilog;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;
using WaveMaster_Backend.Observers;

namespace WaveMaster_Backend.Services
{
    /// <summary>
    /// Interface defining subscribing and unsubscribing functions of the observers
    /// </summary>
    public interface IObserverService
    {
        public void SubscribeObservers();
        public void UnsubscribeObservers();
    }
    /// <summary>
    /// Handles subscribe and unsibscribe operations by implementing IObserverService interface
    /// </summary>
    /// <remarks>
    /// For the observers to subscribe and unsubscribe to the read service.
    /// </remarks>
    public class ObserverService : IObserverService
    {
        private readonly IHubContext<PlotDataHub> _hubContext;
        private readonly WaveMasterDbContext _context;
        private readonly IReadService _readService;
        public DbObserver DbObserver { get; set; }
        public HubObserver HubObserver { get; set; }

        /// <summary>
        /// Initializes an instance of Observer service by injecting required dependencies. 
        /// </summary>
        /// <param name="hubContext"> The hub service instance</param>
        /// <param name="context">DB Context instance</param>
        /// <param name="readService">The read service instance</param>
        public ObserverService(IHubContext<PlotDataHub> hubContext,
            WaveMasterDbContext context,
            IReadService readService)
        {
            _hubContext = hubContext;
            _context = context;
            _readService = readService;
            DbObserver = new DbObserver(_context);
            HubObserver = new HubObserver(_hubContext);
        }

        /// <summary>
        /// Subscribe observers to the read service. 
        /// </summary>
        public void SubscribeObservers()
        {
            HubObserver.Subscribe(_readService); // Subscribe HubObserver
            Log.Information("Hub Observer Subscribed");
            DbObserver.Subscribe(_readService); // Subscribe DbObserver
            Log.Information("Db Observer Subscribed");
        }

        /// <summary>
        /// Unsubscribe observers from the read service. 
        /// </summary>
        public void UnsubscribeObservers()
        {
            HubObserver.Unsubscribe(); // Unsubscribe HubObserver
            Log.Information("Hub Observer Unsubscribed");
            DbObserver.Unsubscribe(); // Unsubscribe DbObserver
            Log.Information("Db Observer Unsubscribed");
        }
    }
}
