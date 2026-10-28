namespace TemporalCouplingDemo;

public static class PaymentGateways
{
    public static Func<decimal, string> CreateFake(Action<decimal> onCharge)
    {
        return amount =>
        {
            onCharge(amount);
            return $"FAKE-{Guid.NewGuid()}";
        };
    }

    public static Func<decimal, string> CreateReal()
    {
        return amount =>
        {
            Console.WriteLine($"Real gateway: charged {amount:F2}");
            return $"REAL-{Guid.NewGuid()}";
        };
    }
}
