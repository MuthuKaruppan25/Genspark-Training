
using BankApi.Models;
using Microsoft.EntityFrameworkCore;

public class BankContext : DbContext
{
    public BankContext(DbContextOptions<BankContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Transaction> Transactions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.CustomerId);

        modelBuilder.Entity<Account>().HasKey(a => a.AccountNo);

        modelBuilder.Entity<Account>().HasOne(u => u.user)
                                        .WithMany(a => a.Accounts)
                                        .HasForeignKey(au => au.CustomerId)
                                        .HasConstraintName("FK_Account_User")
                                        .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Account>().HasOne(b => b.Branch)
                                .WithMany(a => a.Accounts)
                                .HasForeignKey(au => au.BranchId)
                                .HasConstraintName("FK_Branch_User")
                                .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Branch>().HasKey(b => b.IFSCCode);

        modelBuilder.Entity<Transaction>()
                    .HasKey(t => t.TransactionId);

        modelBuilder.Entity<Transaction>()
                    .HasOne(t => t.FromAccount)
                    .WithMany(a => a.Transactions)
                    .HasForeignKey(t => t.FromAccountId)
                    .HasConstraintName("FK_Transaction_FromAccount")
                    .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Transaction>()
                    .HasOne(t => t.ToAccount)
                    .WithMany()
                    .HasForeignKey(t => t.ToAccountId)
                    .HasConstraintName("FK_Transaction_ToAccount")
                    .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}