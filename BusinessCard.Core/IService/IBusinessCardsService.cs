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

        public  Task<byte[]> ExportToCsvAsync();

        public Task<byte[]> ExportToXmlAsync();
        Task<IEnumerable<FillterBusinessCardsDTo>> GetFilteredBusinessCardsAsync(FillterBusinessCardsDTo filter);

        public Task CreateBusinessCardAsync(BusinessCards businessCard, IFormFile file);
    }
}
