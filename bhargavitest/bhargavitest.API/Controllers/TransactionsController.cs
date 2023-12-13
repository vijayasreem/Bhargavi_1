
using System;
using System.Threading.Tasks;
using bhargavitest.DTO;
using bhargavitest.Service;
using Microsoft.AspNetCore.Mvc;

namespace bhargavitest.API
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessTransaction(TransactionRequestDTO request)
        {
            try
            {
                // Check if credit amount exceeds limit
                if (request.CreditAmount > 50000)
                {
                    return BadRequest("Your daily credit limit is exceeded.");
                }

                // Check if transaction count exceeds limit
                int transactionCount = await _transactionService.GetTransactionCountForDay();
                if (transactionCount >= 3)
                {
                    return BadRequest("Transaction limit exceeded. You cannot perform more than 3 transactions in a day.");
                }

                // Process the transaction
                await _transactionService.ProcessTransaction(request.CreditAmount);

                // Update daily credit spent and transaction count
                await _transactionService.UpdateDailyCreditAndCount(request.CreditAmount);

                return Ok("Transaction completed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
