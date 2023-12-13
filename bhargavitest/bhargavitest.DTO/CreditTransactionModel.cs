
namespace bhargavitest
{
    public class CreditTransactionModel
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsSuccessful { get; set; }
    }
}
