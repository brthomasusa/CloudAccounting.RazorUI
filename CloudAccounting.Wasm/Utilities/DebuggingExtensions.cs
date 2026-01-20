using System.Text.Json;

namespace CloudAccounting.Wasm.Utilities
{
    public static class DebuggingExtensions
    {
        private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };
        public static string ToJson(this object obj) => JsonSerializer.Serialize(obj, _options);
    }
}
