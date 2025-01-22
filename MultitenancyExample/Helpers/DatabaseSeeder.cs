using Microsoft.EntityFrameworkCore;
using MultitenancyExample.Contexts;
using MultitenancyExample.EntityModels.Main;
using MultitenancyExample.EntityModels.Shared;

namespace MultitenancyExample.Helpers;

public static class DatabaseSeeder
{
    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var mainContext = scope.ServiceProvider.GetRequiredService<MainContext>();
        // This is where you seed your database
        #region Seed customers
        // Check if Customer with DatabaseName "CustomerA" exists
        var customerA = await mainContext.Customers.FirstOrDefaultAsync(c => c.DatabaseName == "CustomerA.db");
        if (customerA == null)
        {
            customerA = new Customer
            {
                Name = "Customer A",
                DatabaseName = "CustomerA.db"
            };
            mainContext.Customers.Add(customerA);
        }

        // Check if Customer with DatabaseName "CustomerB" exists
        var customerB = await mainContext.Customers.FirstOrDefaultAsync(c => c.DatabaseName == "CustomerB.db");
        if (customerB == null)
        {
            customerB = new Customer
            {
                Name = "Customer B",
                DatabaseName = "CustomerB.db"
            };
            mainContext.Customers.Add(customerB);
        }

        #endregion

        #region Seed users
        var userAdmin = await mainContext.Users.FirstOrDefaultAsync(user => user.Username == "admin");
        if (userAdmin == null)
        {
            userAdmin = new User
            {
                Username = "admin",
                Email = "admin@example.com"
            };
            mainContext.Users.Add(userAdmin);
            // Add userAdmin to UserCustomers for CustomerA and CustomerB
            mainContext.UserCustomers.Add(new UserCustomer
            {
                UserId = userAdmin.Id,
                Customer = customerA
            });
            mainContext.UserCustomers.Add(new UserCustomer
            {
                UserId = userAdmin.Id,
                Customer = customerB
            });
        }


        // Make sure userA exists and is associated with CustomerA
        var userA = await mainContext.Users.FirstOrDefaultAsync(user => user.Username == "userA");
        if (userA == null)
        {
            userA = new User
            {
                Username = "userA",
                Email = "userA@example.com"
            };
            mainContext.Users.Add(userA);
            // Add userA to UserCustomers for CustomerA
            mainContext.UserCustomers.Add(new UserCustomer
            {
                UserId = userA.Id,
                Customer = customerA
            });

        }
        else
        {
            var userCustomer = await mainContext.UserCustomers.FirstOrDefaultAsync(uc => uc.UserId == userA.Id && uc.Customer.DatabaseName == "CustomerA.db");
            if (userCustomer == null)
            {
                mainContext.UserCustomers.Add(new UserCustomer
                {
                    UserId = userA.Id,
                    Customer = customerA
                });
            }
        }

        // Make sure userB exists and is associated with CustomerB
        var userB = await mainContext.Users.FirstOrDefaultAsync(user => user.Username == "userB");
        if (userB == null)
        {
            userB = new User
            {
                Username = "userB",
                Email = "userB@example.com"
            };
            mainContext.Users.Add(userB);
            // Add userB to UserCustomers for CustomerB
            mainContext.UserCustomers.Add(new UserCustomer
            {
                UserId = userB.Id,
                Customer = customerB
            });
        }
        else
        {
            var userCustomer = await mainContext.UserCustomers.FirstOrDefaultAsync(uc => uc.UserId == userB.Id && uc.Customer.DatabaseName == "CustomerB.db");
            if (userCustomer == null)
            {
                mainContext.UserCustomers.Add(new UserCustomer
                {
                    UserId = userB.Id,
                    Customer = customerB
                });
            }
        }
        #endregion

        await mainContext.SaveChangesAsync();

        await SeedSharedContextCustomerAAsync(customerA.DatabaseName);
        await SeedSharedContextCustomerBAsync(customerB.DatabaseName);
    }

    public static async Task SeedSharedContextCustomerAAsync(string databaseName)
    {
        var connectionString = ConnectionStringHelper.GetConnectionString(databaseName);
        var options = new DbContextOptionsBuilder<SharedContext>()
            .UseSqlite(connectionString)
            .Options;

        using var context = new SharedContext(options);

        // Apply migrations (if needed)
        //await context.Database.MigrateAsync();

        // Seed specific data for Database A
        if (!await context.Documents.AnyAsync())
        {
            await context.Documents.AddRangeAsync(
                new Document { Title = "Document for customer A", Content = "Content for customer A." },
                new Document { Title = "Document 2 for customer A", Content = "Content 2 for customer A." }
            );
            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedSharedContextCustomerBAsync(string databaseName)
    {
        var connectionString = ConnectionStringHelper.GetConnectionString(databaseName);
        var options = new DbContextOptionsBuilder<SharedContext>()
            .UseSqlite(connectionString)
            .Options;

        using var context = new SharedContext(options);

        // Apply migrations (if needed)
        // await context.Database.MigrateAsync();

        // Seed specific data for Database B
        if (!await context.Documents.AnyAsync())
        {
            await context.Documents.AddRangeAsync(
                new Document { Title = "Document for customer B", Content = "Content for customer B." },
                new Document { Title = "Document 2 for customer B", Content = "Content 2 for customer B." }
            );
            await context.SaveChangesAsync();
        }
    }

}