using System.ComponentModel;

namespace CodeGenPlayground.Builders;

public class BuilderDefinition
{
    public string BuilderName { get; set; }
    public string BuilderNamespace { get; set; }
    public PropertyDefinition[] Properties { get; set; }

    public class PropertyDefinition
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Namespace { get; set; }
        public string TypeNamespace { get; set; }
    }
}

