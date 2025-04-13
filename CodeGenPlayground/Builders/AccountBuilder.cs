using CodeGenPlayground.Models;

namespace CodeGenPlayground.Builders;

public partial class AccountBuilder : BuilderBase<Account, AccountBuilder>
{
    public AccountBuilder WithSomeState()
    {
        WithDateCreated(DateTime.UtcNow).WithDateClosed(null);
        return this;
    }
}