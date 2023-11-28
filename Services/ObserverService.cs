using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;
using WaveMaster_Backend.Observers;

namespace WaveMaster_Backend.Services
{
    public interface IObserverService
    {
        public void SubscribeObservers();
        public void UnsubscribeObservers();
    }
    public class ObserverService : IObserverService
    {
        private readonly IHubContext<PlotDataHub> _hubContext;
        private readonly WaveMasterDbContext _context;
        private readonly IReadService _readService;
        public DbObserver DbObserver { get; set; }
        public HubObserver HubObserver { get; set; }

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

        public void SubscribeObservers()
        {
            HubObserver.Subscribe(_readService); // Subscribe HubObserver
            Log.Information("Hub Observer Subscribed");
            DbObserver.Subscribe(_readService); // Subscribe DbObserver
            Log.Information("Db Observer Subscribed");
        }

        public void UnsubscribeObservers()
        {
            HubObserver.Unsubscribe(); // Unsubscribe HubObserver
            Log.Information("Hub Observer Unsubscribed");
            DbObserver.Unsubscribe(); // Unsubscribe DbObserver
            Log.Information("Db Observer Unsubscribed");
        }
    }
}
