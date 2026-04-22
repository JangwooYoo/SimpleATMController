namespace SimpleATMController;

using SimpleATMController.Exceptions;
using SimpleATMController.Interfaces;
using SimpleATMController.Models;

/// <summary>
/// Handles the main ATM workflow so that a separate UI layer can drive it.
/// </summary>
public class ATMController
{
    private readonly IBankSystem _bankSystem;
    private readonly ICardReader _cardReader;
    private readonly ICashBin _cashBin;
    private readonly ATMSession _session;

    public ATMController(IBankSystem bankSystem, ICardReader cardReader, ICashBin cashBin)
    {
        _bankSystem = bankSystem ?? throw new ArgumentNullException(nameof(bankSystem));
        _cardReader = cardReader ?? throw new ArgumentNullException(nameof(cardReader));
        _cashBin = cashBin ?? throw new ArgumentNullException(nameof(cashBin));
        _session = new ATMSession();
    }

    /// <summary>
    /// Starts a new session by reading the inserted card.
    /// </summary>
    public void InsertCard()
    {
        var card = _cardReader.ReadCard();
        if (card == null)
            throw new ATMException("Failed to read card");

        _session.InsertedCard = card;
        _session.IsAuthenticated = false;
        _session.SelectedAccount = null;
    }

    /// <summary>
    /// Validates the PIN using the bank system.
    /// </summary>
    public bool EnterPin(string pin)
    {
        EnsureCardInserted();

        if (string.IsNullOrWhiteSpace(pin))
            throw new ArgumentException("PIN cannot be empty", nameof(pin));

        _session.IsAuthenticated = _bankSystem.ValidatePin(_session.InsertedCard, pin);

        if (!_session.IsAuthenticated)
        {
            throw new InvalidPinException();
        }

        return true;
    }

    /// <summary>
    /// Returns the list of accounts available for the current card.
    /// </summary>
    public IEnumerable<Account> GetAvailableAccounts()
    {
        EnsureAuthenticated();

        return _bankSystem.GetAccounts(_session.InsertedCard);
    }

    /// <summary>
    /// Selects the account to use for subsequent transactions.
    /// </summary>
    public void SelectAccount(string accountNumber)
    {
        EnsureAuthenticated();

        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number cannot be empty", nameof(accountNumber));

        var account = _bankSystem.GetAccount(accountNumber);
        if (account == null)
            throw new ATMException("Account not found");

        var availableAccounts = _bankSystem.GetAccounts(_session.InsertedCard);
        if (!availableAccounts.Any(a => a.AccountNumber == accountNumber))
            throw new ATMException("Account does not belong to this card");

        _session.SelectedAccount = account;
    }

    /// <summary>
    /// Returns the current balance of the selected account.
    /// </summary>
    public int GetBalance()
    {
        EnsureAccountSelected();

        var currentAccount = _bankSystem.GetAccount(_session.SelectedAccount.AccountNumber);
        return currentAccount?.Balance ?? _session.SelectedAccount.Balance;
    }

    /// <summary>
    /// Deposits whole-dollar cash into the selected account.
    /// </summary>
    public void Deposit(int amount)
    {
        EnsureAccountSelected();

        if (amount <= 0)
            throw new InvalidAmountException("Deposit amount must be greater than zero");

        // A real implementation would forward this to a cash acceptor device.
        _cashBin.AcceptCash(amount);

        if (!_bankSystem.Deposit(_session.SelectedAccount, amount))
            throw new ATMException("Deposit failed");

        var updatedAccount = _bankSystem.GetAccount(_session.SelectedAccount.AccountNumber);
        if (updatedAccount != null)
            _session.SelectedAccount = updatedAccount;
    }

    /// <summary>
    /// Withdraws whole-dollar cash from the selected account.
    /// </summary>
    public void Withdraw(int amount)
    {
        EnsureAccountSelected();

        if (amount <= 0)
            throw new InvalidAmountException("Withdrawal amount must be greater than zero");

        // Fail early if the ATM hardware cannot pay out the requested amount.
        if (!_cashBin.HasSufficientCash(amount))
            throw new InsufficientCashException();

        var currentAccount = _bankSystem.GetAccount(_session.SelectedAccount.AccountNumber);
        if (currentAccount == null || currentAccount.Balance < amount)
            throw new InsufficientFundsException();

        if (!_bankSystem.Withdraw(_session.SelectedAccount, amount))
            throw new ATMException("Withdrawal failed");

        if (!_cashBin.DispenseCash(amount))
            throw new ATMException("Failed to dispense cash");

        var updatedAccount = _bankSystem.GetAccount(_session.SelectedAccount.AccountNumber);
        if (updatedAccount != null)
            _session.SelectedAccount = updatedAccount;
    }

    /// <summary>
    /// Ends the session and ejects the current card.
    /// </summary>
    public void EjectCard()
    {
        _cardReader.EjectCard();
        _session.InsertedCard = null;
        _session.IsAuthenticated = false;
        _session.SelectedAccount = null;
    }

    /// <summary>
    /// Returns the in-memory session state.
    /// </summary>
    public ATMSession GetCurrentSession()
    {
        return _session;
    }

    private void EnsureCardInserted()
    {
        if (_session.InsertedCard == null)
            throw new NoCardInsertedException();
    }

    private void EnsureAuthenticated()
    {
        EnsureCardInserted();

        if (!_session.IsAuthenticated)
            throw new NotAuthenticatedException();
    }

    private void EnsureAccountSelected()
    {
        EnsureAuthenticated();

        if (_session.SelectedAccount == null)
            throw new NoAccountSelectedException();
    }
}
