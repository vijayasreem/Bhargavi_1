using System;
using System.Threading.Tasks;

public interface ITransactionService
{
    Task ProcessTransaction(double creditAmount);
}

public class TransactionService : ITransactionService
{
    private double dailyCreditLimit = 50000;
    private int maxTransactionCount = 3;
    private double dailyCreditSpent = 0;
    private int transactionCount = 0;

    public async Task ProcessTransaction(double creditAmount)
    {
        try
        {
            await CheckCreditLimit(creditAmount);
            await CheckTransactionCount();

            // Process the transaction here
            Console.WriteLine("Transaction processed successfully");

            // Update daily credit spent and transaction count
            dailyCreditSpent += creditAmount;
            transactionCount++;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task CheckCreditLimit(double creditAmount)
    {
        if (dailyCreditSpent + creditAmount > dailyCreditLimit)
        {
            throw new Exception("Your daily credit limit is exceeded");
        }
    }

    private async Task CheckTransactionCount()
    {
        if (transactionCount >= maxTransactionCount)
        {
            throw new Exception("Transaction limit exceeded. You cannot perform more than 3 transactions in a day");
        }
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        // Test the TransactionService
        var service = new TransactionService();
        await service.ProcessTransaction(20000);
        await service.ProcessTransaction(30000);
        await service.ProcessTransaction(10000);
        await service.ProcessTransaction(5000);
    }
}