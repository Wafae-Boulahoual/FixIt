using FixIT.Application.DTOs;
using FixIT.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FixIT.Application.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        public async Task<int> CreateServiceRequestAsync(CreateServiceRequestDto dto, int clientId)
        {
            if(string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new ArgumentException("Titeln får inte vara tom!");

            }
            if(string.IsNullOrWhiteSpace(dto.Description))
            {
                throw new ArgumentException("Beskrivningen får inte vara tom!");
            }
            if (string.IsNullOrWhiteSpace(dto.Adress))
            {
                throw new ArgumentException("Adressen får inte vara tom!");
            }

            return 1; // temporär
        }
       

    }
}
