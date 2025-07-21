

using Microsoft.EntityFrameworkCore;
using StreamingApp.Models;

namespace StreamingApp.Contexts;
public class VideoDbContext : DbContext
{
    public VideoDbContext(DbContextOptions<VideoDbContext> options) : base(options)
    {

    }
    public DbSet<VideoModel> Videos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VideoModel>()
                    .HasKey(v => v.Id)
                    .HasName("PK-Videos");
    }
}