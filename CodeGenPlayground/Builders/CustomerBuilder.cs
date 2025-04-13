using CodeGenPlayground.Models;

namespace CodeGenPlayground.Builders;

public partial class CustomerBuilder : BuilderBase<Customer, CustomerBuilder>
{
    public CustomerBuilder WithRandomId() => WithId(Guid.NewGuid().ToString());
}


public partial class AccountBuilderBuilder : BuilderBase<Account, AccountBuilderBuilder>
{
    public AccountBuilderBuilder WithSomeState()
    {
        WithDateCreated(DateTime.UtcNow);
        return this;
    }
}