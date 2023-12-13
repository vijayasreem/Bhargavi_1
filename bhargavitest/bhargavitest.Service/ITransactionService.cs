public interface ITransactionService
{
    Task ProcessTransaction(decimal creditAmount);
}