using System;
using System.Collections.Generic;
using System.Text;

namespace FixIT.Application.DTOs
{
    public class CreateServiceRequestDto // det som behövs för kunder för att skapa en ny felanmälan
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Adress { get; set; }
    }
}
