


using System.Collections.Generic;
using System.Threading.Tasks;

namespace bhargavitest.Service
{
    public interface ICreditTransactionService
    {
        Task<bool> CreateTransaction(CreditTransactionModel transaction);
        Task<CreditTransactionModel> GetTransaction(int id);
        Task<List<CreditTransactionModel>> GetAllTransactions();
        Task<bool> UpdateTransaction(CreditTransactionModel transaction);
        Task<bool> DeleteTransaction(int id);
    }
}
