namespace SimpleATMController.Interfaces;

/// <summary>
/// Contract for cash handling hardware.
/// </summary>
public interface ICashBin
{
    bool HasSufficientCash(int amount);
    bool DispenseCash(int amount);
    void AcceptCash(int amount);
}
