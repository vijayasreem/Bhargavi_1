


using System.Threading.Tasks;

public interface ITransactionService
{
    Task ProcessTransaction(double creditAmount);
}
