namespace MultitenancyExample.EntityModels.Main;
public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string SecurityStamp { get; set; }
    public string ConcurrencyStamp { get; set; }
}
