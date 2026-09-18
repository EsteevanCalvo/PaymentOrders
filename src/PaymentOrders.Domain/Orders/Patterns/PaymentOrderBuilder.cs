namespace PaymentOrders.Domain.Orders.Patterns;

public sealed class PaymentOrderBuilder : IPaymentOrderBuilder
{
    private Guid _id;
    private string? _sourceAccount;
    private string? _targetAccount;
    private decimal _amount;
    private string? _currency;
    private string? _swiftCode;
    private DateTimeOffset? _scheduledFor;
    private DateTimeOffset _createdAt;


    private readonly OrderType _orderType;

    internal PaymentOrderBuilder(OrderType orderType)
    {
        _orderType = orderType;
    }

    public IPaymentOrderBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public IPaymentOrderBuilder WithSourceAccount(string sourceAccount)
    {
        _sourceAccount = sourceAccount;
        return this;
    }

    public IPaymentOrderBuilder WithTargetAccount(string targetAccount)
    {
        _targetAccount = targetAccount;
        return this;
    }

    public IPaymentOrderBuilder WithAmount(decimal amount)
    {
        _amount = amount;
        return this;
    }

    public IPaymentOrderBuilder WithCurrency(string currency)
    {
        _currency = currency;
        return this;
    }

    public IPaymentOrderBuilder WithSwiftCode(string? swiftCode)
    {
        _swiftCode = swiftCode;
        return this;
    }

    public IPaymentOrderBuilder WithScheduledFor(DateTimeOffset? scheduledFor)
    {
        _scheduledFor = scheduledFor;
        return this;
    }

    public IPaymentOrderBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public PaymentOrder Build()
    {
        var draft = new PaymentOrderDraft(
            _id,
            _sourceAccount!,
            _targetAccount!,
            _amount,
            _currency!,
            _swiftCode,
            _scheduledFor,
            _createdAt);

        return _orderType switch
        {
            OrderType.National => new NationalPaymentOrder(draft),
            OrderType.International => new InternationalPaymentOrder(draft),
            OrderType.Scheduled => new ScheduledPaymentOrder(draft),
            _ => throw new ArgumentOutOfRangeException(nameof(_orderType))
        };
    }
}
