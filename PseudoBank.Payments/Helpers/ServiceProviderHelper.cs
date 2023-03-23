using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PseudoBank.Payments.Factories;
using PseudoBank.Payments.Runner.Context;
using PseudoBank.Payments.Services;
using PseudoBank.Payments.Validators;

namespace PseudoBank.Payments.Helpers
{
    public static class ServiceProviderHelper
    {
        public static ServiceProvider ConfigureServices()
        {
            // Create a service collection so that our application can use dependency inject
            ServiceCollection services = new ServiceCollection();

            // Now we can begin to register our dependencies by adding them to the service collection
            // Since .net 6 removed the use of the startup file, we can clean up our program.cs through the use of extensions if needed
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<PaymentValidatorBase, AutomatedPaymentSystemValidator>();
            services.AddTransient<PaymentValidatorBase, ExpeditedPaymentsValidator>();
            services.AddTransient<PaymentValidatorBase, BankToBankTransferValidator>();

            services.AddSingleton<IPaymentValidatorFactory, PaymentValidatorFactory>();
            services.AddSingleton<DatabaseSeeder>();


            // Add the database context to the service collection so that we can easily inject it into other services
            services.AddDbContext<AccountContext>(options =>
            {
                options.UseSqlite("Data Source=:memory:");
            });

            // Build the service provider once all the dependencies have been registered
            return services.BuildServiceProvider();
        }
    }
}
