using AutoMapper;
using BusinessCard.Core.Data;
using BusinessCard.Core.DTO;
using BusinessCard.Core.IRepository;
using BusinessCard.Core.IService;
using BusinessCard.Infra.Repository;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BusinessCard.Infra.Service
{
    public class BusinessCardsService: BusinessCardsRepository, IBusinessCardsService
    {
        private readonly IBusinessCardsRepository _repository;
        private readonly IMapper _mapper;

        
        public BusinessCardsService(IBusinessCardsRepository repository, IMapper mapper, BusinessCardDbContext context) : base(context)
        {
            _repository= repository;
            _mapper= mapper;
        }
        public async Task<IEnumerable<FillterBusinessCardsDTo>> GetFilteredBusinessCardsAsync(FillterBusinessCardsDTo filter)
        {
            var businessCards = await _repository.GetFilteredBusinessCardsAsync(filter);
            return _mapper.Map<IEnumerable<FillterBusinessCardsDTo>>(businessCards);
        }


        public async Task<byte[]> ExportToCsvAsync()
        {
            var businessCards = await _repository.GetAllAsync();
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Id,Name,Gender,DateOfBirth,Email,Phone");

            foreach (var card in businessCards)
            {
                csvBuilder.AppendLine($"{card.Id},{card.Name},{card.Gender},{card.DateOfBirth.ToString("yyyy-MM-dd")},{card.Email},{card.Phone}");
            }

            return Encoding.UTF8.GetBytes(csvBuilder.ToString());
        }

        public async Task<byte[]> ExportToXmlAsync()
        {
            var businessCards = await _repository.GetAllAsync();

            using (var memoryStream = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(List<BusinessCards>));
                serializer.Serialize(memoryStream, businessCards);
                return memoryStream.ToArray();
            }
        }



        public async Task CreateBusinessCardAsync(BusinessCards businessCard,IFormFile file)
        {
            // If a file is provided, process it
            if (file != null)
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    if (file.FileName.EndsWith(".csv"))
                    {
                        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                        {
                            var records = csv.GetRecords<BusinessCards>();
                            foreach (var record in records)
                            {
                                // Save each record to the database
                                await _repository.AddAsync(record);
                            }
                        }
                    }
                    else if (file.FileName.EndsWith(".xml"))
                    {
                        var serializer = new XmlSerializer(typeof(BusinessCards));
                        var record = (BusinessCards)serializer.Deserialize(reader);
                        await _repository.AddAsync(record);
                    }
                }
            }

            // Save the business card created from the form
            await _repository.AddAsync(businessCard);
        }
    }
}
