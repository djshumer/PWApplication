using System;
using PWApplication.TransactionApi.Infrastructure.Data.DataModels;

namespace PWApplication.TransactionApi.Models
{
    public class TransactionViewModel
    {

        public TransactionViewModel(PWTransaction transaction)
        {
            if (transaction.Сounteragent == null) throw new NullReferenceException("TransactionViewModel - PWTransaction has invalid property value of Сounteragent");
            Id = transaction.Id;
            AgentId = transaction.AgentId;
            СounteragentId = transaction.СounteragentId;
            СounteragentFullName = transaction.Сounteragent.FullName;
            СounteragentUserName = transaction.Сounteragent.UserName;
            TransactionAmount = transaction.TransactionAmount;
            AgentBalance = transaction.AgentBalance;
            OperationDateTime = transaction.OperationDateTime;
            Description = transaction.Description;
        }

        public Guid Id { get; set; }

        public string AgentId { get; set; }

        public string СounteragentId { get; set; }

        public string СounteragentFullName { get; set; }

        public string СounteragentUserName { get; set; }

        public DateTime OperationDateTime { get; set; }

        public Decimal TransactionAmount { get; set; } = 0;

        public Decimal AgentBalance { get; set; } = 0;

        public String Description { get; set; } = "";
}
}
