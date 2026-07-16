using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public static class StudentJsonService
{
    public static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };

        options.Converters.Add(new DateTimeJsonConverter());

        return options;
    }

    public static string Serialize(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        return JsonSerializer.Serialize(
            student,
            CreateOptions());
    }

    public static Student Deserialize(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        var student = JsonSerializer.Deserialize<Student>(
            json,
            CreateOptions());

        if (student is null)
        {
            throw new JsonException(
                "Не удалось десериализовать объект Student.");
        }

        Validate(student);

        return student;
    }

    public static void SaveToFile(
        Student student,
        string filePath)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var json = Serialize(student);

        File.WriteAllText(filePath, json);
    }

    public static Student LoadFromFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var json = File.ReadAllText(filePath);

        return Deserialize(json);
    }

    private static void Validate(Student student)
    {
        var validationResults = new List<ValidationResult>();

        var validationContext = new ValidationContext(student);

        Validator.TryValidateObject(
            student,
            validationContext,
            validationResults,
            validateAllProperties: true);

        if (student.Grades is not null)
        {
            foreach (var subject in student.Grades)
            {
                if (subject is not null)
                {
                    Validator.TryValidateObject(
                        subject,
                        new ValidationContext(subject),
                        validationResults,
                        validateAllProperties: true);
                }
            }
        }

        if (validationResults.Count == 0)
        {
            return;
        }

        var errors = string.Join(
            Environment.NewLine,
            validationResults.Select(
                result => result.ErrorMessage));

        throw new ValidationException(
            $"Данные Student не прошли валидацию:{Environment.NewLine}{errors}");
    }
}