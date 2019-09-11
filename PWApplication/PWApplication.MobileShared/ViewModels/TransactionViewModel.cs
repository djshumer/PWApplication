using System;
using PWApplication.MobileShared.Models;
using PWApplication.MobileShared.ViewModels.Base;

namespace PWApplication.MobileShared.ViewModels
{
    public class TransactionViewModel : ExtendedBindableObject
    {

        public Transaction Transaction { get; set; }

        public TransactionViewModel(Transaction transaction)
        {
            Transaction = transaction;
        }

        public Guid Id { get { return Transaction.Id; } }

        public string AgentId
        {
            get { return Transaction.AgentId;  }
            set { Transaction.AgentId = value; RaisePropertyChanged(() => AgentId); }
        }

        public string СounteragentId
        {
            get { return Transaction.СounteragentId; }
            set { Transaction.СounteragentId = value; RaisePropertyChanged(() => СounteragentId); }
        }

        public string СounteragentFullName
        {
            get { return Transaction.СounteragentFullName; }
            set { Transaction.СounteragentFullName = value; RaisePropertyChanged(() => СounteragentFullName); }
        }

        public string СounteragentUserName
        {
            get { return Transaction.СounteragentUserName; }
            set { Transaction.СounteragentUserName = value; RaisePropertyChanged(() => СounteragentUserName); }
        }

        public DateTime OperationDateTime
        {
            get { return Transaction.OperationDateTime; }
            set { Transaction.OperationDateTime = value; RaisePropertyChanged(() => OperationDateTime); }
        }

        public decimal TransactionAmount
        {
            get { return Transaction.TransactionAmount; }
            set
            {
                Transaction.TransactionAmount = value;
                RaisePropertyChanged(() => TransactionAmount);
                RaisePropertyChanged(() => TransactionAmountView);
                RaisePropertyChanged(() => IsIncoming);
            }
        }

        public decimal AgentBalance
        {
            get { return Transaction.AgentBalance; }
            set { Transaction.AgentBalance = value; RaisePropertyChanged(() => AgentBalance); }
        }

        public string Description
        {
            get { return Transaction.Description; }
            set { Transaction.Description = value; RaisePropertyChanged(() => Description); }
        }

        public bool IsDescriptionEmpty
        {
            get { return String.IsNullOrEmpty(Description); }
        }

        public string TransactionAmountView 
        {
            get
            {
                if (Transaction.TransactionAmount > 0)
                    return String.Format("+{0:N2}", Transaction.TransactionAmount);
                else
                    return String.Format("{0:N2}", Transaction.TransactionAmount);
            }
        }

        public string OperationTimeView
        {
            get { return Transaction.OperationDateTime.ToString("hh:mm:ss"); }
        }

        public string OperationDateView
        {
            get { return Transaction.OperationDateTime.ToString("ddd d MMM yyyy"); }
        }

        public string OperationDateTimeView
        {
            get { return Transaction.OperationDateTime.ToString("ddd d MMM yyyy - hh:mm:ss"); }
        }

        public bool IsIncoming
        {
            get { return TransactionAmount > 0; }
        }
    }

    
}
