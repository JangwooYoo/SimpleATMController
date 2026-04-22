namespace SimpleATMController.Models;

/// <summary>
/// Keeps the current ATM session state.
/// </summary>
public class ATMSession
{
    public Card? InsertedCard { get; set; }
    public Account? SelectedAccount { get; set; }
    public bool IsAuthenticated { get; set; }
}
