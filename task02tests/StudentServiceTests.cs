using task02;

namespace task02tests;

public class StudentServiceTests
{
    private readonly StudentService service;


    public StudentServiceTests()
    {
        var students = new List<Student>
        {
            new Student
            {
                Name = "Иван",
                Faculty = "ФИТ",
                Grades = new List<int> {5, 4, 5}
            },

            new Student
            {
                Name = "Анна",
                Faculty = "ФИТ",
                Grades = new List<int> {4, 4, 5}
            },

            new Student
            {
                Name = "Петр",
                Faculty = "Экономика",
                Grades = new List<int> {5, 5, 5}
            }
        };


        service = new StudentService(students);
    }


    [Fact]
    public void GetStudentsByFaculty_ReturnsCorrectStudents()
    {
        var result = service.GetStudentsByFaculty("ФИТ");

        Assert.Equal(2, result.Count());
    }


    [Fact]
    public void GetStudentsByFaculty_NoStudents_ReturnsEmpty()
    {
        var result = service.GetStudentsByFaculty("Несуществующий факультет");

        Assert.Empty(result);
    }


    [Fact]
    public void GetStudentsWithMinAverageGrade_ReturnsCorrectStudents()
    {
        var result = service.GetStudentsWithMinAverageGrade(5);

        Assert.Single(result);
    }


    [Fact]
    public void GetStudentsWithMinAverageGrade_ReturnsStudentsWithExactAverage()
    {
        var result = service.GetStudentsWithMinAverageGrade(4.5);

        Assert.Equal(2, result.Count());
    }


    [Fact]
    public void GetStudentsOrderedByName_ReturnsCorrectOrder()
    {
        var result = service.GetStudentsOrderedByName().ToList();

        Assert.Equal("Анна", result[0].Name);
        Assert.Equal("Иван", result[1].Name);
        Assert.Equal("Петр", result[2].Name);
    }


    [Fact]
    public void GetStudentsOrderedByName_ReturnsAllStudents()
    {
        var result = service.GetStudentsOrderedByName();

        Assert.Equal(3, result.Count());
    }


    [Fact]
    public void GroupStudentsByFaculty_ReturnsCorrectGroups()
    {
        var result = service.GroupStudentsByFaculty();

        Assert.Equal(2, result.Count);
        Assert.True(result.Contains("ФИТ"));
        Assert.True(result.Contains("Экономика"));
    }


    [Fact]
    public void GetFacultyWithHighestAverageGrade_ReturnsCorrectFaculty()
    {
        var result = service.GetFacultyWithHighestAverageGrade();

        Assert.Equal("Экономика", result);
    }


    [Fact]
    public void GetFacultyWithHighestAverageGrade_ReturnsNotEmpty()
    {
        var result = service.GetFacultyWithHighestAverageGrade();

        Assert.False(string.IsNullOrEmpty(result));
    }
}