namespace CodeGenPlayground.Models;

public class Customer
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public List<Account> Accounts { get; set; }
    public Account[] AccountsArray { get; set; }
    public Account MainAccount { get; set; }
}