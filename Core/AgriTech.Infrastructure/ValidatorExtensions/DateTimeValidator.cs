using FluentValidation;

namespace EveryEng;

public static class DateTimeValidator
{
    public static IRuleBuilderOptions<T, DateTime> DateOfBirthRule<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        return ruleBuilder.NotEmpty().WithMessage("{PropertyName} should be not empty")
            .LessThan(p => DateTime.Now.AddYears(18)).WithMessage("Min age should be 18");
    }
}