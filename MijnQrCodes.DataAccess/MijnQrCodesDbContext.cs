using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MijnQrCodes.DataAccess.Entities;

namespace MijnQrCodes.DataAccess;

public class MijnQrCodesDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<ShortUrl> ShortUrls { get; set; }
    public DbSet<ShortUrlVisit> ShortUrlVisits { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ShortUrlTag> ShortUrlTags { get; set; }
    public DbSet<User> Users { get; set; }

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
            entity.Property(e => e.BackgroundColor).HasMaxLength(10).HasDefaultValue("#FFFFFF");
            entity.Property(e => e.ForegroundColor).HasMaxLength(10).HasDefaultValue("#212121");
            entity.Property(e => e.FinderPatternColor).HasMaxLength(10).HasDefaultValue("#212121");
            entity.Property(e => e.CenterImageColor).HasMaxLength(10);
            entity.Property(e => e.CenterImageContentType).HasMaxLength(50);
        });

        modelBuilder.Entity<ShortUrlVisit>(entity =>
        {
            entity.ToTable("SHORT_URL_VISITS");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SysId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.SysId).IsClustered(false).IsUnique();
            entity.HasIndex(e => e.ShortUrlId);
            entity.HasOne(e => e.ShortUrl)
                .WithMany()
                .HasForeignKey(e => e.ShortUrlId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("USERS");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SysId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.SysId).IsClustered(false).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(256).IsRequired();
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("TAGS");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SysId).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.SysId).IsClustered(false).IsUnique();
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Color).HasMaxLength(10).HasDefaultValue("#6366f1");
        });

        modelBuilder.Entity<ShortUrlTag>(entity =>
        {
            entity.ToTable("SHORT_URL_TAGS");
            entity.HasKey(e => new { e.ShortUrlId, e.TagId });
            entity.HasOne(e => e.ShortUrl)
                .WithMany(s => s.ShortUrlTags)
                .HasForeignKey(e => e.ShortUrlId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Tag)
                .WithMany(t => t.ShortUrlTags)
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
