
using Twitterapi.Models;
using Microsoft.EntityFrameworkCore;

namespace Twitterapi.Contexts;

public class TwitterContext : DbContext
{
    public TwitterContext(DbContextOptions<TwitterContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Hashtag> Hashtags { get; set; }
    public DbSet<PostHashtag> PostHashtags { get; set; }
    public DbSet<Likes> Likes { get; set; }
    public DbSet<UserFollow> UserFollows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Password).IsRequired().HasMaxLength(255);
            entity.Property(u => u.Bio).HasMaxLength(500);
            entity.Property(u => u.Location).HasMaxLength(100);
            entity.Property(u => u.Website).HasMaxLength(100);
            entity.Property(u => u.DateOfBirth).HasColumnType("date");
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();


        });
  
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.HasOne(p => p.Author)
                  .WithMany(u => u.Posts)
                  .HasForeignKey(p => p.AuthorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.Property(p => p.Content).IsRequired().HasMaxLength(280);
            entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(p => p.AuthorId); 
        });

        modelBuilder.Entity<Likes>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(l => l.User)
                  .WithMany(u => u.Likes)
                  .HasForeignKey(l => l.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(l => l.Post)
                  .WithMany(p => p.Likes)
                  .HasForeignKey(l => l.PostId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(l => l.UserId);
            entity.HasIndex(l => l.PostId); 
        });

        modelBuilder.Entity<PostHashtag>(entity =>
        {
            entity.HasKey(ph => ph.Id);
            entity.Property(ph => ph.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne(ph => ph.Post)
                  .WithMany(p => p.PostHashtags)
                  .HasForeignKey(ph => ph.PostId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ph => ph.Hashtag)
                  .WithMany(h => h.PostHashtags)
                  .HasForeignKey(ph => ph.HashtagId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(ph => ph.PostId);    
            entity.HasIndex(ph => ph.HashtagId); 
        });

        modelBuilder.Entity<UserFollow>(entity =>
        {
            entity.HasKey(uf => uf.Id);
            entity.HasOne(uf => uf.Follower)
                  .WithMany(u => u.Following)
                  .HasForeignKey(uf => uf.FollowerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(uf => uf.Following)
                  .WithMany(u => u.Followers)
                  .HasForeignKey(uf => uf.FollowingId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(uf => uf.FollowerId);  
            entity.HasIndex(uf => uf.FollowingId); 
        });
      
    }


}