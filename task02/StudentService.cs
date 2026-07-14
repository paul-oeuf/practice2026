namespace task02;

public class StudentService
{
    private readonly List<Student> students;

    public StudentService(List<Student> students)
    {
        this.students = students;
    }


    public IEnumerable<Student> GetStudentsByFaculty(string faculty)
    {
        return students
            .Where(student => student.Faculty == faculty);
    }


    public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade)
    {
        return students
            .Where(student => student.Grades.Any())
            .Where(student => student.Grades.Average() >= minAverageGrade);
    }


    public IEnumerable<Student> GetStudentsOrderedByName()
    {
        return students
            .OrderBy(student => student.Name);
    }


    public ILookup<string, Student> GroupStudentsByFaculty()
    {
        return students
            .ToLookup(student => student.Faculty);
    }


    public string GetFacultyWithHighestAverageGrade()
    {
        return students
            .GroupBy(student => student.Faculty)
            .Select(group => new
            {
                Faculty = group.Key,
                AverageGrade = group
                    .Where(student => student.Grades.Any())
                    .Average(student => student.Grades.Average())
            })
            .OrderByDescending(result => result.AverageGrade)
            .First()
            .Faculty;
    }
}