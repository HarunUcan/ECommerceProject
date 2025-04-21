using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DtoLayer.Dtos.StaticPageDtos
{
    public class StaticPageDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
