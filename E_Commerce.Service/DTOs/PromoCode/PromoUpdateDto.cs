using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Service.DTOs.PromoCode
{
    public class PromoUpdateDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int DiscountPercent { get; set; }
        public int ExpireAfterMinutes { get; set; }
    }
}
