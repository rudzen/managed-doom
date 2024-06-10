using System.Text.Json.Serialization;

namespace ManagedDoom.Config;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ConfigValues))]
public partial class ConfigValuesContext : JsonSerializerContext;