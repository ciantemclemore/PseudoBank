using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PseudoBank.Payments.Runner.Entities;

namespace PseudoBank.Payments.Runner.Context
{
    /// <summary>
    /// The database context for Accounts. 
    /// We can use dbContext instead of account store. DbContext can serve as the unit of work and the dbSets can be seen as the repositories
    /// </summary>
    public class AccountContext : DbContext
    {
        public DbSet<Account> Accounts => Set<Account>();

        public DbSet<AccountStatus> AccountStatuses => Set<AccountStatus>();

        public DbSet<PaymentScheme> PaymentSchemes => Set<PaymentScheme>();

        public AccountContext(DbContextOptions<AccountContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // an account is dependent on a status, configure a relationship where an account status can belong to many accounts and an account can have a single status
            modelBuilder.Entity<Account>()
                .HasOne(a => a.AccountStatus)
                .WithMany(accStatus => accStatus.Accounts)
                .HasForeignKey(a => a.AccountStatusId);

            // create a many-to-many relationship between an 'Account' and 'PaymentSchemes'
            // An account can have many allowed payment schemes and a payment scheme can belong to many accounts
            modelBuilder.Entity<Account>()
                .HasMany(a => a.AllowedPaymentSchemes)
                .WithMany(ps => ps.Accounts);
        }
    }

    public class AccountContextFactory : IDesignTimeDbContextFactory<AccountContext>
    {
        public AccountContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AccountContext>();
            optionsBuilder.UseSqlite("Data Source=:memory:");

            return new AccountContext(optionsBuilder.Options);
        }
    }
}
