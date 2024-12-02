using Microsoft.EntityFrameworkCore;
using MultitenancyExample.Contexts;

namespace MultitenancyExample.Helpers;

// This accessor is a bit more advanced. It has more of what you need in a real-world application.

public class SharedContextAccessor_Advanced : ISharedContextAccessor
{
    private readonly IConfiguration _configuration;
    private readonly IConfigurationRoot _configurationRoot;
    private readonly ICurrentUserDataAccessor _currentUserDataAccessor;
    private readonly Lazy<Task<SharedContext>> _sharedContext;


    public SharedContextAccessor_Advanced(
        IConfiguration configuration,
        ICurrentUserDataAccessor currentUserDataAccessor
    )
    {
        _configuration = configuration;
        _configurationRoot = (IConfigurationRoot)_configuration;
        _currentUserDataAccessor = currentUserDataAccessor;

        // This is a lazy initialization of the SharedContext
        this._sharedContext = new Lazy<Task<SharedContext>>(async () =>
        {
            var currentUserData = await _currentUserDataAccessor.GetCurrentUserDataAsync();
            if (currentUserData == null)
            {
                throw new Exception("CustomerContext can't be created. No customer selected/ or user has no access to any customer");
            }
            
            if (string.IsNullOrEmpty(currentUserData.DatabaseName) || string.IsNullOrEmpty(currentUserData.ServerName) || string.IsNullOrEmpty(currentUserData.DatabaseUsername))
            {
                throw new Exception("CustomerContext can't be created. Missing database information");
            }
            
            var sharedDbPassword = _configuration[currentUserData.DatabaseUsername];
            if (string.IsNullOrEmpty(sharedDbPassword))
            {
                try
                {
                    /*  This is just a backup in case the password can't be retrieved from the configuration on first try.
                        It could be due to the fact that you get passwords from Azure key vault at application startup, 
                        and there is a new customer added to the system after the application has started.
                    */
                    _configurationRoot.Reload();
                    sharedDbPassword = _configuration[currentUserData.DatabaseUsername];
                    if (string.IsNullOrEmpty(sharedDbPassword))
                    {
                        throw new Exception("CustomerContext can't be created");
                    }
                }
                catch (Exception ex)
                {
                    // Throw an exception if the password can't be retrieved
                    // Maybe you want to log the exception
                    throw new Exception("CustomerContext can't be created", ex);
                }
            }
            var connectionstring = GetConnectionString(
                currentUserData.ServerName,
                currentUserData.DatabaseName,
                currentUserData.DatabaseUsername,
                sharedDbPassword,
                connectionTimeout: 300
                );

            var optionsBuilder = new DbContextOptionsBuilder<SharedContext>();
            // This is the important part, it tells your code which database to use
            optionsBuilder.UseSqlServer(connectionstring, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            return new SharedContext(optionsBuilder.Options);
        });
    }

    // This method is used to create the connection string
    // Right now it is set up for Azure SQL, but you can adjust it to your needs.
    // Maybe you want to move this method to a helper class
    public static string GetConnectionString(
            string serverName,
            string databaseName,
            string username,
            string password,
            int connectionTimeout = 30
        ) =>
            $"Server=tcp:{serverName}.database.windows.net,1433;Initial Catalog={databaseName};Persist Security Info=False;User ID={username};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout={connectionTimeout};";

    public async Task<SharedContext> GetSharedContextAsync()
    {
        return await _sharedContext.Value;
    }
}
