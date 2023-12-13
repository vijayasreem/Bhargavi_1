
using System.Threading.Tasks;
using bhargavitest.DTO;
using bhargavitest.Service;
using Microsoft.AspNetCore.Mvc;

namespace bhargavitest.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessTransaction([FromBody] double creditAmount)
        {
            try
            {
                await _transactionService.ProcessTransaction(creditAmount);

                return Ok("Transaction completed successfully");
            }
            catch (CreditLimitExceededException ex)
            {
                return BadRequest("Your daily credit limit is exceeded");
            }
            catch (TransactionLimitExceededException ex)
            {
                return BadRequest("Transaction limit exceeded. You cannot perform more than 3 transactions in a day");
            }
        }
    }
}
