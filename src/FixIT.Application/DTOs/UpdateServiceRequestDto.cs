using System;
using System.Collections.Generic;
using System.Text;

namespace FixIT.Application.DTOs
{
    public class UpdateServiceRequestDto
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Adress { get; set; } = "";
    }
}
