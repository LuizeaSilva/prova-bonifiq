namespace ProvaPub.Services.Payments;

public class PaypalPayment : IPaymentMethod
{
    public string Name => "paypal";

    public Task PayAsync(decimal value, int customerId)
    {
        //Faz pagamento...
        return Task.CompletedTask;
    }
}
