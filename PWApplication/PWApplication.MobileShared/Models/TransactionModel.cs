using System;

namespace PWApplication.MobileShared.Models
{
    public class TransactionModel
    {

        public Guid Id { get; set; }

        public string AgentId { get; set; }

        public string СounteragentId { get; set; }

        public string СounteragentFullName { get; set; }

        public string СounteragentUserName { get; set; }

        public DateTime OperationDateTime { get; set; }

        public Decimal TransactionAmount { get; set; }

        public Decimal AgentBalance { get; set; }

        public String Description { get; set; } 

    }
}
