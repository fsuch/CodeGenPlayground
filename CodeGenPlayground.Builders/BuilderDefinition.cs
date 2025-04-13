namespace CodeGenPlayground.Builders;

internal class BuilderDefinition
{
    public string BuilderName { get; set; }
    public string BuilderNamespace { get; set; }
    public PropertyDefinition[] Properties { get; set; }

    internal class PropertyDefinition
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Type { get; set; }
        public string TypeNamespace { get; set; }
        public bool TypeIsNullable { get; set; }
        public bool IsEnumerable { get; set; }
        public string EnumerableType { get; set; }
    }
}