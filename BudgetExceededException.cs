// Thrown when the list would exceed the allowed total budget.
public class BudgetExceededException : Exception
{
    public BudgetExceededException(string message)
        : base(message)
    {
    }
}
