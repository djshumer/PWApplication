using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWApplication.TransactionApi.Infrastructure.Data.DataModels
{
    public class PWTransaction
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string AgentId { get; set; }

        public ApplicationUser Agent { get; set; }

        [Required]
        public string СounteragentId { get; set; }

        public ApplicationUser Сounteragent { get; set; }

        [Required]
        public DateTime OperationDateTime { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal TransactionAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal AgentBalance { get; set; } = 0;

        [MaxLength(500)]
        public String Description { get; set; } = "";


    }
}
