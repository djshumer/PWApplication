using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transaction.Api.Models
{
    public class TransactionModel
    {
        public string CounteragentId { get; set; }
        public decimal TransactionAmount { get; set; }
        public string Description { get; set; }
    }
}
