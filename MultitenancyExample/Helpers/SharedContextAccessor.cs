using Microsoft.EntityFrameworkCore;
using MultitenancyExample.Contexts;

namespace MultitenancyExample.Helpers;

public interface ISharedContextAccessor
{
    Task<SharedContext> GetSharedContextAsync();
}
public class SharedContextAccessor : ISharedContextAccessor
{
    private readonly IConfiguration _configuration;
    private readonly ICurrentUserDataAccessor _currentUserDataAccessor;
    private readonly Lazy<Task<SharedContext>> _sharedContext;


    public SharedContextAccessor(
        IConfiguration configuration,
        ICurrentUserDataAccessor currentUserDataAccessor
    )
    {
        _configuration = configuration;
        _currentUserDataAccessor = currentUserDataAccessor;

        // This is a lazy initialization of the SharedContext
        this._sharedContext = new Lazy<Task<SharedContext>>(async () =>
        {
            var currentUserData = await _currentUserDataAccessor.GetCurrentUserDataAsync();
            if (currentUserData == null)
            {
                throw new Exception("CustomerContext can't be created. No customer selected/ or user has no access to any customer");
            }
            
            if (string.IsNullOrEmpty(currentUserData.DatabaseName))
            {
                throw new Exception("CustomerContext can't be created. Missing database information");
            }
            
            var connectionstring = GetConnectionString(
                currentUserData.DatabaseName
                );

            var optionsBuilder = new DbContextOptionsBuilder<SharedContext>();
            // This is the important part, it tells your code which database to use
            optionsBuilder.UseSqlServer(connectionstring, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            return new SharedContext(optionsBuilder.Options);
        });
    }

    // This method is used to create the connection string
    // Right now it is set up for Sqlite3, but you can adjust it to your needs.
    // Maybe you want to move this method to a helper class
    public static string GetConnectionString(
            string databaseName
        ) =>
            $"Data Source=Databases/{databaseName};";

    public async Task<SharedContext> GetSharedContextAsync()
    {
        return await _sharedContext.Value;
    }
}
