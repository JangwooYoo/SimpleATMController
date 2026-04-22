using SimpleATMController.Interfaces;
using SimpleATMController.Models;

namespace SimpleATMController.Tests.Mocks;

public class MockCardReader : ICardReader
{
    private Card? _cardToRead;

    public void SetCard(Card card)
    {
        _cardToRead = card;
    }

    public Card? ReadCard()
    {
        return _cardToRead;
    }

    public void EjectCard()
    {
        _cardToRead = null;
    }
}
