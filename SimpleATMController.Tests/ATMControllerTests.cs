using SimpleATMController.Exceptions;
using SimpleATMController.Models;
using SimpleATMController.Tests.Mocks;
using Xunit;

namespace SimpleATMController.Tests;

/// <summary>
/// Tests the controller independently from any real bank system or hardware.
/// </summary>
public class ATMControllerTests
{
    private readonly MockBankSystem _bankSystem;
    private readonly MockCardReader _cardReader;
    private readonly MockCashBin _cashBin;
    private readonly ATMController _controller;
    private readonly Card _testCard;
    private readonly Account _checkingAccount;
    private readonly Account _savingsAccount;

    public ATMControllerTests()
    {
        _bankSystem = new MockBankSystem();
        _cardReader = new MockCardReader();
        _cashBin = new MockCashBin(10000);

        _testCard = new Card("1234-5678-9012-3456");
        _checkingAccount = new Account("CHK001", "Checking Account", 1000);
        _savingsAccount = new Account("SAV001", "Savings Account", 5000);

        _bankSystem.AddCard(_testCard.CardNumber, "1234");
        _bankSystem.AddAccount(_testCard.CardNumber, _checkingAccount);
        _bankSystem.AddAccount(_testCard.CardNumber, _savingsAccount);

        _controller = new ATMController(_bankSystem, _cardReader, _cashBin);
    }

    [Fact]
    public void InsertCard_ShouldSetCardInSession()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();

        var session = _controller.GetCurrentSession();
        Assert.NotNull(session.InsertedCard);
        Assert.Equal(_testCard.CardNumber, session.InsertedCard.CardNumber);
        Assert.False(session.IsAuthenticated);
    }

    [Fact]
    public void EnterPin_WithCorrectPin_ShouldAuthenticate()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();

        var result = _controller.EnterPin("1234");

        Assert.True(result);
        Assert.True(_controller.GetCurrentSession().IsAuthenticated);
    }

    [Fact]
    public void EnterPin_WithIncorrectPin_ShouldThrowInvalidPinException()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();

        Assert.Throws<InvalidPinException>(() => _controller.EnterPin("9999"));
        Assert.False(_controller.GetCurrentSession().IsAuthenticated);
    }

    [Fact]
    public void EnterPin_WithoutCard_ShouldThrowNoCardInsertedException()
    {
        Assert.Throws<NoCardInsertedException>(() => _controller.EnterPin("1234"));
    }

    [Fact]
    public void GetAvailableAccounts_AfterAuthentication_ShouldReturnAccounts()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");

        var accounts = _controller.GetAvailableAccounts().ToList();

        Assert.Equal(2, accounts.Count);
        Assert.Contains(accounts, a => a.AccountNumber == "CHK001");
        Assert.Contains(accounts, a => a.AccountNumber == "SAV001");
    }

    [Fact]
    public void GetAvailableAccounts_WithoutAuthentication_ShouldThrowNotAuthenticatedException()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();

        Assert.Throws<NotAuthenticatedException>(() => _controller.GetAvailableAccounts());
    }

    [Fact]
    public void SelectAccount_WithValidAccount_ShouldSetSelectedAccount()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");

        _controller.SelectAccount("CHK001");

        var session = _controller.GetCurrentSession();
        Assert.NotNull(session.SelectedAccount);
        Assert.Equal("CHK001", session.SelectedAccount.AccountNumber);
    }

    [Fact]
    public void SelectAccount_WithoutAuthentication_ShouldThrowNotAuthenticatedException()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();

        Assert.Throws<NotAuthenticatedException>(() => _controller.SelectAccount("CHK001"));
    }

    [Fact]
    public void SelectAccount_WithAccountThatDoesNotBelongToCard_ShouldThrowATMException()
    {
        var otherCard = new Card("9999-8888-7777-6666");
        _bankSystem.AddCard(otherCard.CardNumber, "4321");
        _bankSystem.AddAccount(otherCard.CardNumber, new Account("EXT001", "External Account", 100));

        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");

        Assert.Throws<ATMException>(() => _controller.SelectAccount("EXT001"));
    }

    [Fact]
    public void GetBalance_AfterSelectingAccount_ShouldReturnCorrectBalance()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");
        _controller.SelectAccount("CHK001");

        var balance = _controller.GetBalance();

        Assert.Equal(1000, balance);
    }

    [Fact]
    public void GetBalance_WithoutSelectingAccount_ShouldThrowNoAccountSelectedException()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");

        Assert.Throws<NoAccountSelectedException>(() => _controller.GetBalance());
    }

    [Fact]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");
        _controller.SelectAccount("CHK001");

        _controller.Deposit(500);

        var balance = _controller.GetBalance();
        Assert.Equal(1500, balance);
    }

    [Fact]
    public void Deposit_WithZeroAmount_ShouldThrowInvalidAmountException()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");
        _controller.SelectAccount("CHK001");

        Assert.Throws<InvalidAmountException>(() => _controller.Deposit(0));
    }

    [Fact]
    public void Deposit_WithNegativeAmount_ShouldThrowInvalidAmountException()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");
        _controller.SelectAccount("CHK001");

        Assert.Throws<InvalidAmountException>(() => _controller.Deposit(-100));
    }

    [Fact]
    public void Withdraw_WithValidAmount_ShouldDecreaseBalance()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");
        _controller.SelectAccount("CHK001");

        _controller.Withdraw(300);

        var balance = _controller.GetBalance();
        Assert.Equal(700, balance);
    }

    [Fact]
    public void Withdraw_WithInsufficientFunds_ShouldThrowInsufficientFundsException()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");
        _controller.SelectAccount("CHK001");

        Assert.Throws<InsufficientFundsException>(() => _controller.Withdraw(2000));
    }

    [Fact]
    public void Withdraw_WithInsufficientCash_ShouldThrowInsufficientCashException()
    {
        var lowCashBin = new MockCashBin(100);
        var controller = new ATMController(_bankSystem, _cardReader, lowCashBin);

        _cardReader.SetCard(_testCard);
        controller.InsertCard();
        controller.EnterPin("1234");
        controller.SelectAccount("CHK001");

        Assert.Throws<InsufficientCashException>(() => controller.Withdraw(500));
    }

    [Fact]
    public void Withdraw_WithZeroAmount_ShouldThrowInvalidAmountException()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");
        _controller.SelectAccount("CHK001");

        Assert.Throws<InvalidAmountException>(() => _controller.Withdraw(0));
    }

    [Fact]
    public void Withdraw_WithoutSelectingAccount_ShouldThrowNoAccountSelectedException()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");

        Assert.Throws<NoAccountSelectedException>(() => _controller.Withdraw(100));
    }

    [Fact]
    public void Deposit_WithoutSelectingAccount_ShouldThrowNoAccountSelectedException()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");

        Assert.Throws<NoAccountSelectedException>(() => _controller.Deposit(100));
    }

    [Fact]
    public void EjectCard_ShouldClearSession()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");
        _controller.SelectAccount("CHK001");

        _controller.EjectCard();

        var session = _controller.GetCurrentSession();
        Assert.Null(session.InsertedCard);
        Assert.False(session.IsAuthenticated);
        Assert.Null(session.SelectedAccount);
    }

    [Fact]
    public void CompleteFlow_InsertCardToPinToAccountToBalance_ShouldWork()
    {
        _cardReader.SetCard(_testCard);

        _controller.InsertCard();
        Assert.NotNull(_controller.GetCurrentSession().InsertedCard);

        _controller.EnterPin("1234");
        Assert.True(_controller.GetCurrentSession().IsAuthenticated);

        _controller.SelectAccount("SAV001");
        Assert.Equal("SAV001", _controller.GetCurrentSession().SelectedAccount?.AccountNumber);

        var balance = _controller.GetBalance();
        Assert.Equal(5000, balance);
    }

    [Fact]
    public void CompleteFlow_WithDeposit_ShouldWork()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");
        _controller.SelectAccount("SAV001");

        var initialBalance = _controller.GetBalance();
        _controller.Deposit(1000);
        var newBalance = _controller.GetBalance();

        Assert.Equal(initialBalance + 1000, newBalance);
        Assert.Equal(6000, newBalance);
    }

    [Fact]
    public void CompleteFlow_WithWithdrawal_ShouldWork()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");
        _controller.SelectAccount("SAV001");

        var initialBalance = _controller.GetBalance();
        _controller.Withdraw(2000);
        var newBalance = _controller.GetBalance();

        Assert.Equal(initialBalance - 2000, newBalance);
        Assert.Equal(3000, newBalance);
    }

    [Fact]
    public void CompleteFlow_MultipleTransactions_ShouldWork()
    {
        _cardReader.SetCard(_testCard);
        _controller.InsertCard();
        _controller.EnterPin("1234");
        _controller.SelectAccount("CHK001");

        Assert.Equal(1000, _controller.GetBalance());

        _controller.Deposit(500);
        Assert.Equal(1500, _controller.GetBalance());

        _controller.Withdraw(200);
        Assert.Equal(1300, _controller.GetBalance());

        _controller.Deposit(700);
        Assert.Equal(2000, _controller.GetBalance());
    }
}
