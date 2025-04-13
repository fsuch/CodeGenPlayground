
using CodeGenPlayground.Models;

namespace CodeGenPlayground.Builders;

public partial class CustomerBuilder : BuilderBase<Customer, CustomerBuilder>
{
    public CustomerBuilder WithSomeState()
    {
        Foo();
        return this;
    }
}