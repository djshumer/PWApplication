using System;
using System.ComponentModel.DataAnnotations;

namespace PWApplication.MobileAppService.Models.DataModels
{
    public abstract class BaseEntitySimpleModel
    {
        [Key]
        public Guid Id { get; set; }
    }
}
