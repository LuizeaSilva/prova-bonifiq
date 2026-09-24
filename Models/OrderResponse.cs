namespace ProvaPub.Models;

public class OrderResponse
{
    private static readonly TimeZoneInfo BrazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    public int Id { get; set; }
    public decimal Value { get; set; }
    public int CustomerId { get; set; }
    public DateTimeOffset OrderDate { get; set; }

    public static OrderResponse FromOrder(Order order)
    {
        var utcDate = DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc);

        return new OrderResponse
        {
            Id = order.Id,
            Value = order.Value,
            CustomerId = order.CustomerId,
            OrderDate = TimeZoneInfo.ConvertTime(new DateTimeOffset(utcDate), BrazilTimeZone)
        };
    }
}
