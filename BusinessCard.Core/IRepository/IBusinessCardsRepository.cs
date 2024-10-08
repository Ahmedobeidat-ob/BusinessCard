using BusinessCard.Core.Data;
using BusinessCard.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCard.Core.IRepository
{
    public interface IBusinessCardsRepository:IGenericRepository<BusinessCards>
    {

        Task<IEnumerable<BusinessCards>> GetFilteredBusinessCardsAsync(FillterBusinessCardsDTo filter);

    
    }
}
