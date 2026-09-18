namespace PaymentOrders.Domain.Orders.Patterns;

public sealed class PaymentOrderFactory : IPaymentOrderFactory
{
    public IPaymentOrderBuilder CreateBuilder(OrderType orderType)
    {
        return new PaymentOrderBuilder(orderType);
    }
}