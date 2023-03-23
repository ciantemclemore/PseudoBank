using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PseudoBank.Payments.DTOs;
using PseudoBank.Payments.Helpers;
using PseudoBank.Payments.Runner.Context;
using PseudoBank.Payments.Services;
using System;
using System.Threading.Tasks;

namespace PseudoBank.Payments.Runner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // registers our services for the application
            ServiceProvider serviceProvider = ServiceProviderHelper.ConfigureServices();

            // now we can seed our database and get our payment service to test
            IPaymentService paymentService = serviceProvider.GetRequiredService<IPaymentService>();
            DatabaseSeeder databaseSeeder = serviceProvider.GetRequiredService<DatabaseSeeder>();
            AccountContext accountContext = serviceProvider.GetRequiredService<AccountContext>();

            // open a connection to our database for a migration
            await accountContext.Database.OpenConnectionAsync();

            // apply the database migration to update the database schema
            await accountContext.Database.MigrateAsync();

            // Seed the database so that we have initial data
            await databaseSeeder.SeedDatabaseAsync();

            // This should fail
            var response = await paymentService.MakePaymentAsync(new MakeAutomatedPaymentRequest()
            {
                Amount = 100.0m,
                CreditorAccountNumber = "789",
                DebtorAccountNumber = "123",
                PaymentDate = DateTime.Now,
                PaymentScheme = new() { SchemeName = PaymentSchemeConstants.AutomatedPaymentSystem }
            });

            Console.WriteLine($"Response: {response.IsSuccess}{Environment.NewLine}ErrorMessage: {response.ErrorMessage}");
        }
    }
}
