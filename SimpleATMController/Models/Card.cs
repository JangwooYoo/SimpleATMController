namespace SimpleATMController.Models;

/// <summary>
/// Represents a card that can be inserted into the ATM.
/// </summary>
public class Card
{
    public string CardNumber { get; }

    public Card(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new ArgumentException("Card number cannot be empty", nameof(cardNumber));

        CardNumber = cardNumber;
    }
}
