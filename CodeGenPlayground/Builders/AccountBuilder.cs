using CodeGenPlayground.Models;

namespace CodeGenPlayground.Builders;

public partial class AccountBuilder : BuilderBase<Account, AccountBuilder>
{
    public AccountBuilder WithSomeState()
    {
        return WithId(Guid.NewGuid()).WithDateCreated(DateTime.UtcNow).WithDateClosed(null);
    }

    // Since the WithId generated method has different parameter types this one can also be created
    public AccountBuilder WithId(Guid id) => WithId(id.ToString());

}