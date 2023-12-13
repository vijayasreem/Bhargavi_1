
using System;

public interface ITransactionService
{
    Task ProcessTransaction(decimal creditAmount);
}

public class TransactionService : ITransactionService
{
    private decimal dailyCreditSpent;
    private int transactionCount;

    public async Task ProcessTransaction(decimal creditAmount)
    {
        try
        {
            await CheckCreditLimit(creditAmount);
            await CheckTransactionCount();

            // Perform the transaction processing here

            UpdateDailyCreditSpent(creditAmount);
            UpdateTransactionCount();

            Console.WriteLine("Transaction completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    private async Task CheckCreditLimit(decimal creditAmount)
    {
        if (dailyCreditSpent + creditAmount > 50000)
        {
            throw new Exception("Your daily credit limit is exceeded.");
        }
    }

    private async Task CheckTransactionCount()
    {
        if (transactionCount >= 3)
        {
            throw new Exception("Transaction limit exceeded. You cannot perform more than 3 transactions in a day.");
        }
    }

    private void UpdateDailyCreditSpent(decimal creditAmount)
    {
        dailyCreditSpent += creditAmount;
    }

    private void UpdateTransactionCount()
    {
        transactionCount++;
    }
}
