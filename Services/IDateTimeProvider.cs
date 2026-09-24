namespace ProvaPub.Services;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
