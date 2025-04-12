namespace CodeGenPlayground;

public abstract class BuilderBase<T, TBuilder> where TBuilder : BuilderBase<T, TBuilder>
{
    protected T Instance { get; private set; }

    public T Build() => Instance;
}