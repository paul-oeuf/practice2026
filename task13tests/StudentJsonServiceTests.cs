using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using task13;

namespace task13tests;

public class StudentJsonServiceTests
{
    private static Student CreateStudent()
    {
        return new Student
        {
            FirstName = "Павел",
            LastName = "Журавский",
            BirthDate = new DateTime(2000, 5, 15),
            Grades =
            [
                new Subject
                {
                    Name = "Математический анализ",
                    Grade = 90
                },
                new Subject
                {
                    Name = "Программирование",
                    Grade = 100
                }
            ]
        };
    }

    [Fact]
    public void Serialize_UsesCustomDateFormat()
    {
        var student = CreateStudent();

        var json = StudentJsonService.Serialize(student);

        Assert.Contains(
            "\"BirthDate\": \"15.05.2000\"",
            json);
    }

    [Fact]
    public void Serialize_IgnoresNullProperties()
    {
        var student = CreateStudent();
        student.LastName = null;

        var json = StudentJsonService.Serialize(student);

        Assert.DoesNotContain("LastName", json);
    }

    [Fact]
    public void Deserialize_ReturnsStudentWithCorrectData()
    {
        const string json = """
        {
          "FirstName": "Павел",
          "LastName": "Журавский",
          "BirthDate": "15.05.2000",
          "Grades": [
            {
              "Name": "Программирование",
              "Grade": 100
            }
          ]
        }
        """;

        var student = StudentJsonService.Deserialize(json);

        Assert.Equal("Павел", student.FirstName);
        Assert.Equal("Журавский", student.LastName);
        Assert.Equal(
            new DateTime(2000, 5, 15),
            student.BirthDate);

        Assert.NotNull(student.Grades);
        Assert.Single(student.Grades);

        var subject = student.Grades[0];

        Assert.Equal(
            "Программирование",
            subject.Name);

        Assert.Equal(
            100,
            subject.Grade);
    }

    [Fact]
    public void Deserialize_InvalidStudent_ThrowsValidationException()
    {
        const string json = """
        {
          "FirstName": "",
          "LastName": "Журавский",
          "BirthDate": "15.05.2000",
          "Grades": [
            {
              "Name": "Программирование",
              "Grade": 150
            }
          ]
        }
        """;

        Assert.Throws<ValidationException>(
            () => StudentJsonService.Deserialize(json));
    }

    [Fact]
    public void Deserialize_InvalidDate_ThrowsJsonException()
    {
        const string json = """
        {
          "FirstName": "Павел",
          "LastName": "Журавский",
          "BirthDate": "2000-05-15",
          "Grades": []
        }
        """;

        Assert.Throws<JsonException>(
            () => StudentJsonService.Deserialize(json));
    }

    [Fact]
    public void SaveAndLoadFile_PreservesStudentData()
    {
        var student = CreateStudent();

        var filePath = Path.Combine(
            Path.GetTempPath(),
            $"student-{Guid.NewGuid():N}.json");

        try
        {
            StudentJsonService.SaveToFile(
                student,
                filePath);

            Assert.True(File.Exists(filePath));

            var loadedStudent =
                StudentJsonService.LoadFromFile(filePath);

            Assert.Equal(
                student.FirstName,
                loadedStudent.FirstName);

            Assert.Equal(
                student.LastName,
                loadedStudent.LastName);

            Assert.Equal(
                student.BirthDate,
                loadedStudent.BirthDate);

            Assert.NotNull(loadedStudent.Grades);
            Assert.Equal(
                student.Grades!.Count,
                loadedStudent.Grades.Count);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}