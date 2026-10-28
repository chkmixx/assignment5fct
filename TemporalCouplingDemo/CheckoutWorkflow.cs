namespace TemporalCouplingDemo;

public static class CheckoutWorkflow
{
    public static Validated Validate(Draft draft)
    {
        if (draft.Amount <= 0m)
            throw new ArgumentOutOfRangeException(nameof(draft), "Order amount must be greater than zero.");

        return new Validated(draft.Amount);
    }

    public static Priced ApplyDiscount(Validated validated, decimal percent)
    {
        if (percent < 0m || percent > 1m)
            throw new ArgumentOutOfRangeException(nameof(percent), "Discount rate must be between 0 and 1.");

        return new Priced(validated.Amount * (1m - percent));
    }

    public static Receipt Charge(Priced priced, Func<decimal, string> gateway)
    {
        var transactionId = gateway(priced.Amount);
        return new Receipt(transactionId, priced.Amount);
    }
}
