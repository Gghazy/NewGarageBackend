namespace Garage.Contracts.Examinations;

public sealed record AddPaymentRequest(
    decimal Amount,
    string  Method,   // "Cash" | "Card" | "BankTransfer" | "Cheque"
    string? Currency, // defaults to "EGP" if null
    string? Notes
);
