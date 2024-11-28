using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MultitenancyExample.Contexts;

namespace MultitenancyExample.Helpers;

public class CurrentUserDataAccessor(IHttpContextAccessor httpContextAccessor, MainContext mainContext)
{
    public async Task<CurrentUserData> GetCurrentUserData()
    {
        // Get the User ID from the current user, adjust to your needs
        var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var customerId = getSelectedCustomerId();

        // Gets the customer. Throws an exception if none is found, or if more than one is found
        var customer = await mainContext.UserCustomers
            .Where(uc => uc.UserId == userId && (!customerId.HasValue || uc.CustomerId == customerId))
            .Select(uc => uc.Customer)
            .SingleAsync();


        return new CurrentUserData
        {
            UserId = userId,
            CustomerId = customer.Id,
            ServerName = customer.ServerName,
            DatabaseName = customer.DatabaseName,
            DatabaseUsername = customer.DatabaseUsername
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