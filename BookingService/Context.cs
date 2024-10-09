using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
        
    }
    
    public DbSet<Place> Places { get; set; }
}