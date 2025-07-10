using Microsoft.EntityFrameworkCore;

namespace PostgresVm.contexts;


public class VmContext : DbContext
{
    public VmContext(DbContextOptions<VmContext> options) : base(options)
    {

    }

    public DbSet<User> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasKey(u => u.guid)
                                    .HasName("pk_user");
    }
}