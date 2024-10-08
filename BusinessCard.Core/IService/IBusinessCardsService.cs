using BusinessCard.Core.Data;
using BusinessCard.Core.DTO;
using BusinessCard.Core.IRepository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCard.Core.IService
{
    public interface IBusinessCardsService : IBusinessCardsRepository
    {
   
        new  Task<byte[]> ExportToCsvAsync();

        public Task<byte[]> ExportToXmlAsync();
      new Task<IEnumerable<FillterBusinessCardsDTo>> GetFilteredBusinessCardsAsync(FillterBusinessCardsDTo filter);

        Task ImportFromXmlAsync(IFormFile file);

        Task ImportFromCsvAsync(IFormFile file);







    }
}
