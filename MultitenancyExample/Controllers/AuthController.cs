using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MultitenancyExample.Contexts;

namespace MultitenancyExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly MainContext _mainContext;
    public AuthController(
        MainContext mainContext
    )
    {
        _mainContext = mainContext;
    }


    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest login)
    {
        var user = _mainContext.Users.FirstOrDefault(u => u.Username == login.Username);
        if (user == null)
        {
            return Unauthorized();
        }
        // This is a VERY simple example, you should use a secure way to store passwords and users.
        // You should also verify logins in a secure way. Here we are just checking if the user exists.
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("your-secure-key-that-is-long-enough-to-be-secure-and-should-be-stored-in-a-secure-place-and-random-enough-to-be-secure-and-should-be-kept-secret");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, login.Username), new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddHours(10),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { token = tokenString });
    }

    [HttpGet("customer-logins")]
    public async Task<IActionResult> GetCustomerLogins()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var customerLogins = await _mainContext.UserCustomers
            .Where(ucl => ucl.UserId == userId)
            .Select(ucl => new UserCustomerLogin
            {
                CustomerId = ucl.CustomerId,
                CustomerName = ucl.Customer.Name
            }).ToListAsync();

        return Ok(customerLogins);
    }
}

public class LoginRequest
{
    public string Username { get; set; }
}

public class UserCustomerLogin
{
    public string CustomerName { get; set; }
    public int CustomerId { get; set; }
}
