using CodeGenPlayground.Models;

namespace CodeGenPlayground.Builders;

public partial class CustomerBuilder : BuilderBase<Customer, CustomerBuilder>
{
    public CustomerBuilder WithRandomIdAndName()
    {
        return WithId(Guid.NewGuid().ToString()).WithName(Guid.NewGuid().ToString()).WithAccounts([]).WithMainAccount(new AccountBuilder().Build());
    }

    public CustomerBuilder WithId(string id)
    {
        // Since this method is explicitly created here it shouldn't be auto-generated
        Instance.Id = id;
        return this;
    }
}