namespace SimpleATMController.Interfaces;

using SimpleATMController.Models;

/// <summary>
/// Contract for bank-side operations used by the ATM controller.
/// </summary>
public interface IBankSystem
{
    bool ValidatePin(Card card, string pin);
    IEnumerable<Account> GetAccounts(Card card);
    Account? GetAccount(string accountNumber);
    bool Withdraw(Account account, int amount);
    bool Deposit(Account account, int amount);
}
