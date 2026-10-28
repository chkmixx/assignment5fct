# Cosmetics Delivery System: Handling Temporal Coupling
name:Tomiris Chekalin
group:IT-2503

# Project context

This project is a small checkout example for an online cosmetics shop. It focuses on the order-processing steps that happen before a cosmetics order is ready for delivery: validating the order, applying a discount, and charging the customer.

This assignment demonstrates how to make the order-processing sequence safer. It does not calculate shipping costs or implement delivery logistics; delivery is the broader system context for this checkout workflow.

# Checkout stages

The valid workflow is:

`Draft` → `Validated` → `Priced` → `Receipt`

- `Draft` stores the original cosmetics order amount.
- `Validate` checks that the amount is greater than zero and returns `Validated`.
- `ApplyDiscount` takes `Validated` and returns `Priced` with the discounted amount.
- `Charge` takes `Priced`, calls a payment gateway delegate, and returns a `Receipt` containing a transaction ID and charged amount.
- <img width="1878" height="906" alt="image" src="https://github.com/user-attachments/assets/3f9041eb-af57-43e9-a56f-8449f640b05f" />


# Payment gateways

`Charge` accepts a `Func<decimal, string>` delegate. The real gateway prints a charge message and returns a transaction ID. The fake gateway counts its calls and returns a transaction ID, making it useful for demonstrating and checking payment behavior without a real bank connection.

# Demonstration

`Program.cs` demonstrates four problems in the original mutable checkout design:

1. Applying the discount twice charges 810.00 instead of 900.00.
2. Forgetting the discount charges the full 1000.00.
3. Calling `Charge` twice prints two charges.
4. Calling `Charge` before `Validate` compiles but throws at runtime.

It then runs the typed checkout workflow with both fake and real gateways. The fake gateway is called once for the first charge and twice after deliberately charging the same `Priced` value again. 
<img width="1542" height="572" alt="image" src="https://github.com/user-attachments/assets/1c74b0e1-7c02-4500-8e67-f87aac0484a0" />
