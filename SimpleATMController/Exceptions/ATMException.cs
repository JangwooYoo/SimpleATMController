namespace SimpleATMController.Exceptions;

/// <summary>
/// Base exception type for ATM-related failures.
/// </summary>
public class ATMException : Exception
{
    public ATMException(string message) : base(message) { }
}

public class InvalidPinException : ATMException
{
    public InvalidPinException() : base("Invalid PIN number") { }
}

public class NoCardInsertedException : ATMException
{
    public NoCardInsertedException() : base("No card has been inserted") { }
}

public class NotAuthenticatedException : ATMException
{
    public NotAuthenticatedException() : base("User is not authenticated") { }
}

public class NoAccountSelectedException : ATMException
{
    public NoAccountSelectedException() : base("No account has been selected") { }
}

public class InsufficientFundsException : ATMException
{
    public InsufficientFundsException() : base("Insufficient funds in account") { }
}

public class InsufficientCashException : ATMException
{
    public InsufficientCashException() : base("ATM does not have sufficient cash") { }
}

public class InvalidAmountException : ATMException
{
    public InvalidAmountException(string message) : base(message) { }
}
