using Microsoft.EntityFrameworkCore;
using MultitenancyExample.Contexts;

namespace MultitenancyExample.Helpers;

public interface ISharedContextAccessor
{
    Task<SharedContext> GetSharedContextAsync();
}
public class SharedContextAccessor : ISharedContextAccessor
{
    private readonly ICurrentUserDataAccessor _currentUserDataAccessor;
    private readonly Lazy<Task<SharedContext>> _sharedContext;


    public SharedContextAccessor(
        ICurrentUserDataAccessor currentUserDataAccessor
    )
    {
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

            var connectionstring = ConnectionStringHelper.GetConnectionString(
                currentUserData.DatabaseName
                );

            var optionsBuilder = new DbContextOptionsBuilder<SharedContext>();
            // This is the important part, it tells your code which database to use
            optionsBuilder.UseSqlite(connectionstring, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            return new SharedContext(optionsBuilder.Options);
        });
    }

    public async Task<SharedContext> GetSharedContextAsync()
    {
        return await _sharedContext.Value;
    }
}
