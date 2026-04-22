using SimpleATMController.Interfaces;
using SimpleATMController.Models;

namespace SimpleATMController.Tests.Mocks;

public class MockBankSystem : IBankSystem
{
    private readonly Dictionary<string, string> _cardPins = new();
    private readonly Dictionary<string, List<Account>> _cardAccounts = new();
    private readonly Dictionary<string, Account> _accounts = new();

    public void AddCard(string cardNumber, string pin)
    {
        _cardPins[cardNumber] = pin;
        _cardAccounts[cardNumber] = new List<Account>();
    }

    public void AddAccount(string cardNumber, Account account)
    {
        if (!_cardAccounts.ContainsKey(cardNumber))
            _cardAccounts[cardNumber] = new List<Account>();

        _cardAccounts[cardNumber].Add(account);
        _accounts[account.AccountNumber] = account;
    }

    public bool ValidatePin(Card card, string pin)
    {
        return _cardPins.TryGetValue(card.CardNumber, out var correctPin) && correctPin == pin;
    }

    public IEnumerable<Account> GetAccounts(Card card)
    {
        return _cardAccounts.TryGetValue(card.CardNumber, out var accounts) 
            ? accounts 
            : Enumerable.Empty<Account>();
    }

    public Account? GetAccount(string accountNumber)
    {
        return _accounts.TryGetValue(accountNumber, out var account) ? account : null;
    }

    public bool Withdraw(Account account, int amount)
    {
        if (!_accounts.TryGetValue(account.AccountNumber, out var acc))
            return false;

        if (acc.Balance < amount)
            return false;

        acc.Balance -= amount;
        return true;
    }

    public bool Deposit(Account account, int amount)
    {
        if (!_accounts.TryGetValue(account.AccountNumber, out var acc))
            return false;

        acc.Balance += amount;
        return true;
    }
}
