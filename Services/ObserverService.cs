using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;
using WaveMaster_Backend.Observers;

namespace WaveMaster_Backend.Services
{
    public interface IObserverService
    {
        public DbObserver DbObserver { get; set; }
        public HubObserver HubObserver { get; set; }
    }
    public class ObserverService : IObserverService
    {
        private readonly IHubContext<PlotDataHub> _hubContext;
        private readonly WaveMasterDbContext _context;
        public DbObserver DbObserver { get; set; }
        public HubObserver HubObserver { get; set; }

        public ObserverService(IHubContext<PlotDataHub> hubContext, WaveMasterDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
            DbObserver = new DbObserver(_context);
            HubObserver = new HubObserver(_hubContext);
        }
    }
}
