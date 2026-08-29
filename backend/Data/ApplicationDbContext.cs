using Microsoft.EntityFrameworkCore;
using FinancialTracker.Models;

namespace FinancialTracker.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserDetail> UserDetails { get; set; }
}