namespace ProvaPub.Services.Payments;

public class CreditCardPayment : IPaymentMethod
{
    public string Name => "creditcard";

    public Task PayAsync(decimal value, int customerId)
    {
        //Faz pagamento...
        return Task.CompletedTask;
    }
}
