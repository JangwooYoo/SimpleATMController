namespace SimpleATMController.Models;

/// <summary>
/// Represents a bank account accessible through the ATM.
/// </summary>
public class Account
{
    public string AccountNumber { get; }
    public string AccountName { get; }

    // Balance is stored as whole dollars only.
    public int Balance { get; set; }

    public Account(string accountNumber, string accountName, int balance)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number cannot be empty", nameof(accountNumber));

        AccountNumber = accountNumber;
        AccountName = accountName ?? string.Empty;
        Balance = balance;
    }
}
