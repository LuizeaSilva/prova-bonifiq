namespace ProvaPub.Services.Payments;

public interface IPaymentMethod
{
    string Name { get; }
    Task PayAsync(decimal value, int customerId);
}
