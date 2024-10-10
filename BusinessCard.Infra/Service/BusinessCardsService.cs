using AutoMapper;
using BusinessCard.Core.Data;
using BusinessCard.Core.DTO;
using BusinessCard.Core.IRepository;
using BusinessCard.Core.IService;
using BusinessCard.Infra.Repository;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CsvHelper;

using System.IO;

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
      new  public async Task<IEnumerable<FillterBusinessCardsDTo>> GetFilteredBusinessCardsAsync(FillterBusinessCardsDTo filter)
        {
            var businessCards = await _repository.GetFilteredBusinessCardsAsync(filter);
            return _mapper.Map<IEnumerable<FillterBusinessCardsDTo>>(businessCards);
        }

        //Export To Csvc
        //public async Task<byte[]> ExportToCsvAsync()
        //{
        //    var businessCards = await GetAllAsync();
        //    var csvBuilder = new StringBuilder();
        //    csvBuilder.AppendLine("Name,Gender,DateOfBirth,Email,Phone,Address,Photo,CreatedAt");

        //    foreach (var card in businessCards)
        //    {
        //        csvBuilder.AppendLine($"{card.Name},{card.Gender},{card.DateOfBirth.ToString("yyyy-MM-dd")},{card.Email},{card.Phone},{card.Address},{card.Photo},{card.CreatedAt}");
        //    }

        //    return Encoding.UTF8.GetBytes(csvBuilder.ToString());
        //}

        public async Task<byte[]> ExportToCsvAsync()
        {
            var businessCards = await GetAllAsync();
            var csvBuilder = new StringBuilder();

            // Write CSV header
            csvBuilder.AppendLine("Name,Gender,DateOfBirth,Email,Phone,Address,Photo,CreatedAt");

            foreach (var card in businessCards)
            {
                // Escape values containing special characters (like commas or quotes)
                var name = EscapeForCsv(card.Name);
                var gender = EscapeForCsv(card.Gender);
                var email = EscapeForCsv(card.Email);
                var phone = EscapeForCsv(card.Phone);
                var address = EscapeForCsv(card.Address);

                // If Photo is already a string, no need to convert to Base64 again
                var photo = EscapeForCsv(card.Photo); // Use string directly

                var dateOfBirth = card.DateOfBirth.ToString("yyyy-MM-dd");
                var createdAt = card.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss");

                csvBuilder.AppendLine($"{name},{gender},{dateOfBirth},{email},{phone},{address},{photo},{createdAt}");
            }

            return Encoding.UTF8.GetBytes(csvBuilder.ToString());
        }

        private string EscapeForCsv(string field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            // Escape fields with quotes, commas, or newlines
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                // Escape quotes by doubling them
                field = field.Replace("\"", "\"\"");
                // Wrap the field in double quotes
                return $"\"{field}\"";
            }

            return field;
        }
    


        public async Task<byte[]> ExportToXmlAsync()
        {
            var businessCards = await _repository.GetAllAsync();

            // Map the BusinessCards to BusinessCardsDTo
            var businessCardsDto = _mapper.Map<List<BusinessCardsDTo>>(businessCards);

            using (var memoryStream = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(List<BusinessCardsDTo>));
                serializer.Serialize(memoryStream, businessCardsDto);
                return memoryStream.ToArray();
            }
        }

        public async Task ImportFromCsvAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or not provided.");

            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvHelper.CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                var records = csv.GetRecords<BusinessCardsDTo>().ToList();
                var businessCards = _mapper.Map<List<BusinessCards>>(records);

                foreach (var card in businessCards)
                {
                    await _repository.AddAsync(card); // Assuming you have an AddAsync method in your repository
                }
            }
        }



        public async Task ImportFromXmlAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or not provided.");

            using (var stream = file.OpenReadStream())
            {
                try
                {
                    // Adjusted to expect the correct root element
                    var serializer = new XmlSerializer(typeof(List<BusinessCardsDTo>), new XmlRootAttribute("ArrayOfBusinessCardsDTo"));
                    var businessCards = (List<BusinessCardsDTo>)serializer.Deserialize(stream);
                    var mappedCards = _mapper.Map<List<BusinessCards>>(businessCards);

                    foreach (var card in mappedCards)
                    {
                        await _repository.AddAsync(card); // Assuming you have an AddAsync method in your repository
                    }
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException("Error during XML deserialization", ex.InnerException);
                }
            }
        }




          public async Task AddBusinessCardAsync(BusinessCardsDTo businessCardDto, IFormFile photoFile)
    {
        // Convert the uploaded photo file to Base64 if provided
        if (photoFile != null && photoFile.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await photoFile.CopyToAsync(memoryStream);
                var photoBytes = memoryStream.ToArray();
                businessCardDto.Photo = Convert.ToBase64String(photoBytes); // Convert to Base64 string
            }
        }

        // Map the DTO to the business card entity
        var businessCard = _mapper.Map<BusinessCards>(businessCardDto);

        // Add the new business card to the repository
        await _repository.AddAsync(businessCard);
    }












    }
}
