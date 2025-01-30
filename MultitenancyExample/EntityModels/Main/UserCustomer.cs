namespace MultitenancyExample.EntityModels.Main;
public class UserCustomer
{
    public string UserId { get; set; }
    public User User { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
}
