
using bhargavitest.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bhargavitest.Service
{
    public interface ICreditTransactionRepository
    {
        Task<IEnumerable<CreditTransactionModel>> GetAllAsync();
        Task<CreditTransactionModel> GetByIdAsync(int id);
        Task AddAsync(CreditTransactionModel transaction);
        Task UpdateAsync(CreditTransactionModel transaction);
        Task DeleteAsync(int id);
    }
}
