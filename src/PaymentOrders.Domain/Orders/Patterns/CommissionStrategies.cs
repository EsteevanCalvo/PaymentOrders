namespace PaymentOrders.Domain.Orders.Patterns;

public sealed class NationalCommissionStrategy : ICommissionStrategy
{
    public decimal Calculate(PaymentOrder order)
    {
        return Math.Round(order.Amount * 0.01m, 2);
    }
}

public sealed class InternationalCommissionStrategy : ICommissionStrategy
{
    public decimal Calculate(PaymentOrder order)
    {
        return Math.Round(order.Amount * 0.03m, 2);
    }
}

public sealed class ScheduledCommissionStrategy : ICommissionStrategy
{
    public decimal Calculate(PaymentOrder order)
    {
        return Math.Round(order.Amount * 0.005m, 2);
    }
}
