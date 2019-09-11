using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Transaction.Api.Infrastructure.Data.DataModels
{
    public class PWOperationPair
    {
        public PWTransaction TransactionOne { get; set; }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid TransactionOneId { get; set; }

        public PWTransaction TransactionTwo { get; set; }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid TransactionTwoId { get; set; }

    }
}
