

using DocumentShare.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentShare.Contexts;
public class FileContext : DbContext
{
    public FileContext(DbContextOptions<FileContext> options) : base(options)
    {
    }

    public DbSet<FileModel> Files { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileModel>().HasKey(f => f.guid)
                                        .HasName("PK_Files");

        modelBuilder.Entity<FileModel>()
            .Property(f => f.FileName)
            .IsRequired()
            .HasMaxLength(255);
        modelBuilder.Entity<FileModel>()
            .Property(f => f.FileType)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<FileModel>()
            .Property(f => f.Uploader)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<FileModel>()
            .Property(f => f.UploadDate)
            .IsRequired();
        modelBuilder.Entity<FileModel>()
            .Property(f => f.FileData)
            .IsRequired();

        modelBuilder.Entity<User>().HasKey(u => u.Username)
                                    .HasName("PK_Users");


    }
}