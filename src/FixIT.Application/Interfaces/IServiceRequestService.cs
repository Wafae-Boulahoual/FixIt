using FixIT.Application.DTOs;
using FixIT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FixIT.Application.Interfaces
{
    public interface IServiceRequestService
    {
        Task<int> CreateServiceRequestAsync(CreateServiceRequestDto dto, int clientId); //Skapa en ny felanmälan och returnera sin ID 
        
    }
}
