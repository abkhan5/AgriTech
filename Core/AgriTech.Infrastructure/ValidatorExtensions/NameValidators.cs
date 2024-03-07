using FluentValidation;


namespace EveryEng;
public static class StringValidator
{
    public static IRuleBuilderOptions<T, DateTime> DateMustBeGreaterThanNowRule<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    => ruleBuilder
            .Must(x => x > DateTime.UtcNow).WithMessage("{PropertyName} must be greater ");


    public static IRuleBuilderOptions<T, DateTime?> DateMustBeGreaterThanNowRule<T>(this IRuleBuilder<T, DateTime?> ruleBuilder)
    => ruleBuilder.NotNull()
            .Must(x => x > DateTime.UtcNow).WithMessage("{PropertyName} must be greater ");


    public static IRuleBuilderOptions<T, string> EnglishOnlyRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    => ruleBuilder
            .NotEmpty().NotNull().WithMessage("{PropertyName} should be not empty");
    //.Matches(@"^[a-zA-Z0-9$@$!%*?&#^-_. +]+$").WithMessage("{PropertyName} Only English allowed");




    public static IRuleBuilderOptions<T, string> AlphabetOnlyRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    => ruleBuilder
            .NotEmpty().NotNull().WithMessage("{PropertyName} should be not empty")
            .Matches(@"^[a-zA-Z]+$").WithMessage("{PropertyName} Only alphabets allowed");


    public static IRuleBuilderOptions<T, string> FileNameRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    => ruleBuilder
            .Matches(@"^[a-z0-9]+(-[a-z0-9]+)*$").WithMessage("{PropertyName} special characters not allowed");




    public static IRuleBuilderOptions<T, string> NotEmptyOrNullRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    => ruleBuilder.NotEmpty().WithMessage("{PropertyName} should be not empty");


    public static IRuleBuilderOptions<T, string> PasswordRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    => ruleBuilder
            .NotEmpty().WithMessage("Your password cannot be empty")
            .MinimumLength(8).WithMessage("{PropertyName} Your password length must be at least 8.")
            .MaximumLength(16).WithMessage("{PropertyName} Your password length must not exceed 16.")
            .Matches(@"[A-Z]+").WithMessage("{PropertyName} Your password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("{PropertyName} Your password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("{PropertyName} Your password must contain at least one number.")
            .Matches(@"[\!\?\*\.\@]+").WithMessage("{PropertyName} Your password must contain at least one (!? *.).");



}