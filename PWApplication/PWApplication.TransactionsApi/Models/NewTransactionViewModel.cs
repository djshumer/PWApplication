namespace PWApplication.TransactionApi.Models
{
    public class TransactionModel
    {
        public string CounteragentId { get; set; }
        public decimal TransactionAmount { get; set; }
        public string Description { get; set; }
    }
}
