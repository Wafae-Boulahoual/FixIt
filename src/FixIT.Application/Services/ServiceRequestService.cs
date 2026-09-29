using FixIT.Application.DTOs;
using FixIT.Application.Interfaces;
using FixIT.Domain.Interfaces;
using FixIT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FixIT.Application.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _repository;
        public ServiceRequestService(IServiceRequestRepository repository)
        {
            _repository = repository;
        }
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

            var request = new ServiceRequests
            {
                Title = dto.Title,
                Description = dto.Description,
                Adress = dto.Adress,
                ClientId = clientId
            };
            await _repository.AddAsync(request);
            return request.Id;
            //return 1; // temporär
        }
       

    }
}
