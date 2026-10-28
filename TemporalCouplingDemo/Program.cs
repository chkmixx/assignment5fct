using TemporalCouplingDemo;

Console.WriteLine("--- Old design: wrong sequences still compile ---");

// Discount applied twice: charges 810 instead of 900.
var doubleDiscount = new LegacyCheckout();
doubleDiscount.Start(1000m);
doubleDiscount.Validate();
doubleDiscount.ApplyDiscount(0.10m);
doubleDiscount.ApplyDiscount(0.10m);
doubleDiscount.Charge();

// Discount forgotten: charges the full 1000.
var forgottenDiscount = new LegacyCheckout();
forgottenDiscount.Start(1000m);
forgottenDiscount.Validate();
forgottenDiscount.Charge();

// Charge called twice: prints the same charge twice.
var doubleCharge = new LegacyCheckout();
doubleCharge.Start(1000m);
doubleCharge.Validate();
doubleCharge.ApplyDiscount(0.10m);
doubleCharge.Charge();
doubleCharge.Charge();

// Charge before Validate compiles, then throws at runtime.
var beforeValidate = new LegacyCheckout();
beforeValidate.Start(1000m);
try
{
    beforeValidate.Charge();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Charge before Validate failed: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("--- New design: typed stages ---");

var draft = new Draft(1000m);
var validated = CheckoutWorkflow.Validate(draft);
var priced = CheckoutWorkflow.ApplyDiscount(validated, 0.10m);

var fakeCalls = 0;
Func<decimal, string> fakeGateway = PaymentGateways.CreateFake(_ => fakeCalls++);
var fakeReceipt = CheckoutWorkflow.Charge(priced, fakeGateway);
Console.WriteLine($"Fake receipt: {fakeReceipt.TransactionId}, amount {fakeReceipt.Amount:F2}");
Console.WriteLine($"Fake gateway calls: {fakeCalls}");

Func<decimal, string> realGateway = PaymentGateways.CreateReal();
var realReceipt = CheckoutWorkflow.Charge(priced, realGateway);
Console.WriteLine($"Real receipt: {realReceipt.TransactionId}, amount {realReceipt.Amount:F2}");

// Compiler checks: each call below is invalid and intentionally commented out.
// CS1503: ApplyDiscount requires Validated, but draft is Draft.
// CheckoutWorkflow.ApplyDiscount(draft, 0.10m);

// CS1503: Charge requires Priced, but validated is Validated.
// CheckoutWorkflow.Charge(validated, fakeGateway);

// CS1503: Validate requires Draft, but priced is Priced.
// CheckoutWorkflow.Validate(priced);

// Repeating Charge is still possible: Priced can be reused as an immutable value.
CheckoutWorkflow.Charge(priced, fakeGateway);
Console.WriteLine($"Fake gateway calls after charging again: {fakeCalls}");
