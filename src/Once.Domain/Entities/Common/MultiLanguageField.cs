using System.Text.Json;
using System.Text.Json.Serialization;

namespace Once.Domain.Entities.Common;

public class MultiLanguageField
{
    private bool Equals(MultiLanguageField other)
    {
        return Uz == other.Uz &&
               Ru == other.Ru &&
               En == other.En &&
               Cyrl == other.Cyrl;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        
        return Equals((MultiLanguageField)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Uz, Ru, En, Cyrl);
    }

    /// <summary>
    /// In Uzbek
    /// </summary>
    [JsonPropertyName("uz")]
    public string Uz { get; set; } = default!;

    /// <summary>
    /// In Russian
    /// </summary>
    [JsonPropertyName("ru")]
    public string Ru { get; set; } = default!;

    /// <summary>
    /// In English
    /// </summary>
    [JsonPropertyName("en")]
    public string En { get; set; } = default!;
    
    /// <summary>
    /// In Crylic
    /// </summary>
    [JsonPropertyName("cyrl")]
    public string Cyrl { get; set; } = default!;

    public static implicit operator MultiLanguageField(string data) => new()
    {
        Ru = data, Uz = data, En = data, Cyrl = data
    };

    public static bool operator ==(MultiLanguageField a, string b)
    {
        return a.Ru == b ||
               a.En == b ||
               a.Uz == b ||
               a.Cyrl == b;
    }

    public static bool operator !=(MultiLanguageField a, string b)
    {
        return a.Ru != b &&
               a.En != b &&
               a.Uz != b &&
               a.Cyrl != b;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}