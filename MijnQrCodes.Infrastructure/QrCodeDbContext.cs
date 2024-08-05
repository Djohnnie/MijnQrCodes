using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MijnQrCodes.Domain;

namespace MijnQrCodes.Infrastructure;

public class QrCodeDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<Link> Links { get; set; }

    public QrCodeDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetValue<string>("CONNECTION_STRING"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Link>(entityBuilder =>
        {
            entityBuilder.ToTable("LINK_ENTRIES");
            entityBuilder.HasKey(x => x.Id).IsClustered(false);
            entityBuilder.Property(x => x.SysId).ValueGeneratedOnAdd();
            entityBuilder.HasIndex(x => x.SysId).IsClustered();
            entityBuilder.Property(p => p.Title)
                .IsRequired();
            entityBuilder.Property(p => p.Url)
                .IsRequired();
        });
    }
}