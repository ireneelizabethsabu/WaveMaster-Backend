using Microsoft.EntityFrameworkCore;

namespace WaveMaster_Backend.Models
{
    public class WaveMasterDbContext : DbContext
    {
        public WaveMasterDbContext(DbContextOptions options)
            : base(options)
        { }

        public DbSet<PlotData> plotDatas { get; set; } = default!;
    }
}
