using System;
using System.Collections.Generic;
using System.Text;

namespace PWApplication.MobileShared.Models
{
    public class NewTransactionModel
    {
       public string CounteragentId { get; set; }
       public decimal TransactionAmount { get; set; }
       public string Description { get; set; }
    }
}
