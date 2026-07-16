using System.ComponentModel.DataAnnotations;

namespace task13;

public class Subject : IValidatableObject
{
    public string? Name { get; set; }

    public int Grade { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            yield return new ValidationResult(
                "Название предмета не может быть пустым.",
                [nameof(Name)]);
        }

        if (Grade is < 0 or > 100)
        {
            yield return new ValidationResult(
                "Оценка должна находиться в диапазоне от 0 до 100.",
                [nameof(Grade)]);
        }
    }
}

public class Student : IValidatableObject
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime BirthDate { get; set; }

    public List<Subject>? Grades { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(FirstName))
        {
            yield return new ValidationResult(
                "Имя студента не может быть пустым.",
                [nameof(FirstName)]);
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            yield return new ValidationResult(
                "Фамилия студента не может быть пустой.",
                [nameof(LastName)]);
        }

        if (BirthDate == default)
        {
            yield return new ValidationResult(
                "Дата рождения обязательна.",
                [nameof(BirthDate)]);
        }
        else if (BirthDate.Date > DateTime.Today)
        {
            yield return new ValidationResult(
                "Дата рождения не может быть датой в будущем.",
                [nameof(BirthDate)]);
        }

        if (Grades is null)
        {
            yield return new ValidationResult(
                "Список оценок обязателен.",
                [nameof(Grades)]);
        }
        else
        {
            for (var index = 0; index < Grades.Count; index++)
            {
                var subject = Grades[index];

                if (subject is null)
                {
                    yield return new ValidationResult(
                        "Список оценок не может содержать null.",
                        [$"{nameof(Grades)}[{index}]"]);

                    continue;
                }

                foreach (var validationResult in subject.Validate(
                             new ValidationContext(subject)))
                {
                    yield return new ValidationResult(
                        validationResult.ErrorMessage,
                        validationResult.MemberNames.Select(
                            memberName => $"{nameof(Grades)}[{index}].{memberName}"));
                }
            }
        }
    }
}