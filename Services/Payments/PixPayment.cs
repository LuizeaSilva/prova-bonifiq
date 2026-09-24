namespace ProvaPub.Services.Payments;

public class PixPayment : IPaymentMethod
{
    public string Name => "pix";

    public Task PayAsync(decimal value, int customerId)
    {
        //Faz pagamento...
        return Task.CompletedTask;
    }
}
