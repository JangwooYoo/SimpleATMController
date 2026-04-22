namespace SimpleATMController.Interfaces;

using SimpleATMController.Models;

/// <summary>
/// Contract for card reader hardware.
/// </summary>
public interface ICardReader
{
    Card? ReadCard();
    void EjectCard();
}
