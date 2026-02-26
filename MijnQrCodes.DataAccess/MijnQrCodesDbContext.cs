using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MijnQrCodes.DataAccess.Entities;

namespace MijnQrCodes.DataAccess;

public class MijnQrCodesDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<ShortUrl> ShortUrls { get; set; }

    public MijnQrCodesDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration["CONNECTION_STRING"]);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortUrl>(entity =>
        {
            entity.ToTable("SHORT_URLS");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SysId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.SysId).IsClustered(false).IsUnique();
            entity.HasIndex(e => e.ShortCode).IsUnique();
            entity.Property(e => e.Title).HasMaxLength(256).IsRequired();
            entity.Property(e => e.OriginalUrl).HasMaxLength(2048).IsRequired();
            entity.Property(e => e.ShortCode).HasMaxLength(10).IsRequired();
        });
    }
}
