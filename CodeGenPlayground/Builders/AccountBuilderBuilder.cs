using CodeGenPlayground.Models;

namespace CodeGenPlayground.Builders;

public partial class AccountBuilderBuilder : BuilderBase<Account, AccountBuilderBuilder>
{
    public AccountBuilderBuilder WithSomeState()
    {
        WithDateCreated(DateTime.UtcNow);
        return this;
    }
}