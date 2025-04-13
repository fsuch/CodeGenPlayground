using CodeGenPlayground.Models;

namespace CodeGenPlayground.Builders;

public partial class CustomerBuilder : BuilderBase<Customer, CustomerBuilder>
{
    public CustomerBuilder WithRandomIdAndName() => WithId(Guid.NewGuid().ToString()).WithName(Guid.NewGuid().ToString());
}