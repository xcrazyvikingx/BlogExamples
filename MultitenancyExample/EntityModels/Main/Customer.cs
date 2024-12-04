namespace MultitenancyExample.EntityModels.Main;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DatabaseName { get; set; }
    public string ServerName { get; set; }
    public string DatabaseUsername { get; set; }
}
