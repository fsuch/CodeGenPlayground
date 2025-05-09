namespace CodeGenPlayground.Models;

public class Account
{
    public string Id { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateClosed { get; set; } // TODO Implement support for nullables
    public SubAccount[] SubAccounts { get; set; }

    public class SubAccount
    {
        public string Id { get; set; }
    }
}