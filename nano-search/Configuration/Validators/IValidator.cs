namespace NanoSearch.Configuration.Validators;

public interface IValidator<in TOptions>
{
    IEnumerable<string> Validate(TOptions options);
}