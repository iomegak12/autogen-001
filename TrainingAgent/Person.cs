using Json.Schema.Generation;

namespace TrainingAgent;

[Title("Person")]
[Description("Represents a person with various attributes.")]
public class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string? Address { get; set; }

    public string? City { get; set; }

    public List<string>? Hobbies { get; set; } = new List<string>();

    public override string ToString()
    {
        return $"{Name}, {Age}, {Address}, {City}, Hobbies: {string.Join(", ", Hobbies ?? new List<string>())}";
    }

    public static Person Parse(string input)
    {
        var parts = input.Split(',');
        if (parts.Length < 2)
            throw new ArgumentException("Input must contain at least a name and an age.");

        return new Person
        {
            Name = parts[0].Trim(),
            Age = int.Parse(parts[1].Trim()),
            Address = parts.Length > 2 ? parts[2].Trim() : null
        };
    }
}