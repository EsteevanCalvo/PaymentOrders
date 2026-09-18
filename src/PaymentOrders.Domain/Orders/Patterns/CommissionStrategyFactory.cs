namespace PaymentOrders.Domain.Orders.Patterns;

public sealed class CommissionStrategyFactory : ICommissionStrategyFactory
{
    public ICommissionStrategy GetFor(OrderType orderType)
    {
        return orderType switch
        {
            OrderType.National => new NationalCommissionStrategy(),
            OrderType.International => new InternationalCommissionStrategy(),
            OrderType.Scheduled => new ScheduledCommissionStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(orderType))
        };
    }
}