using Microsoft.EntityFrameworkCore;

namespace WaveMaster_Backend.Models
{
    public class WaveMasterDbContext : DbContext
    {
        public WaveMasterDbContext(DbContextOptions<WaveMasterDbContext> options)
            : base(options)
        {

        }
    }
}
