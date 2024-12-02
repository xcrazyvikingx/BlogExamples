using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MultitenancyExample.Contexts;

namespace MultitenancyExample.Helpers;

public interface ICurrentUserDataAccessor
{
    Task<CurrentUserData> GetCurrentUserDataAsync();
}

public class CurrentUserDataAccessor(IHttpContextAccessor httpContextAccessor, MainContext mainContext) : ICurrentUserDataAccessor
{
    public async Task<CurrentUserData> GetCurrentUserDataAsync()
    {
        // Get the User ID from the current user, adjust to your needs
        var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var customerId = getSelectedCustomerId();

        // Gets the customer relation. 
        // Throws an exception if none or more than one is found
        var customer = await mainContext.UserCustomers
            .Where(uc => uc.UserId == userId && (!customerId.HasValue || uc.CustomerId == customerId))
            .Select(uc => uc.Customer)
            .SingleAsync();


        return new CurrentUserData
        {
            UserId = userId,
            CustomerId = customer.Id,
            DatabaseName = customer.DatabaseName,
            // Add the other properties you need to fit your use case
            // ServerName = customer.ServerName,
            // DatabaseUsername = customer.DatabaseUsername
        };
    }

    private int? getSelectedCustomerId()
    {
        var context = httpContextAccessor.HttpContext;
        // Check if the user is authenticated
        if (context == null || !context.User.Identity.IsAuthenticated)
        {
            throw new KeyNotFoundException("Userinfo data could not be resolved. User not authenticated.");
        }

        // Get the selected customer ID from the request headers
        context.Request.Headers.TryGetValue("selected-customer-id", out var selectedCustomerId);

        // Try to parse the selected customer ID, and return it if successful
        if (int.TryParse(selectedCustomerId, out var currentCustomerId))
        {
            return currentCustomerId;
        }

        return null;
    }
}