namespace CodeGenPlayground.Tests;

public abstract class BuilderBase<T, TBuilder> where TBuilder : BuilderBase<T, TBuilder>
{
    protected T Instance { get; private set; }

    public T Build() => Instance;
}

public class Customer
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public List<Account> Accounts { get; set; }
}

public class Account
{
    public string Id { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateClosed { get; set; }
}

public partial class CustomerBuilder : BuilderBase<Customer, CustomerBuilder>{

}