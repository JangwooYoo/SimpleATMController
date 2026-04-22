using SimpleATMController.Interfaces;

namespace SimpleATMController.Tests.Mocks;

public class MockCashBin : ICashBin
{
    private int _cashAmount;

    public MockCashBin(int initialAmount)
    {
        _cashAmount = initialAmount;
    }

    public bool HasSufficientCash(int amount)
    {
        return _cashAmount >= amount;
    }

    public bool DispenseCash(int amount)
    {
        if (!HasSufficientCash(amount))
            return false;

        _cashAmount -= amount;
        return true;
    }

    public void AcceptCash(int amount)
    {
        _cashAmount += amount;
    }

    public int GetCurrentAmount()
    {
        return _cashAmount;
    }
}
