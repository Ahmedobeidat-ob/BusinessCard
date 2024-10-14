using AutoMapper.Configuration.Conventions;
using BusinessCard.Core.Data;
using BusinessCard.Core.DTO;
using BusinessCard.Core.IRepository;
using BusinessCard.Core.Migrations;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using BusinessCards = BusinessCard.Core.Data.BusinessCards;

namespace BusinessCard.Infra.Repository
{
    public class BusinessCardsRepository:GenericRepository<BusinessCards>,IBusinessCardsRepository
    {
        private readonly BusinessCardDbContext _context;
        public BusinessCardsRepository(BusinessCardDbContext context):base(context)
        {
            _context = context;
        }



        public async Task<IEnumerable<BusinessCards>> GetFilteredBusinessCardsAsync(FillterBusinessCardsDTo filter)
        {
            IQueryable<BusinessCards> query = _context.BusinessCards;

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(b => b.Name.ToLower().Contains(filter.Name.ToLower()));
            }
            if (!string.IsNullOrEmpty(filter.Gender))
            {
                // Perform case-insensitive filtering for Gender
                string genderFilter = filter.Gender.ToLower();
                query = query.Where(b => b.Gender.ToLower().Contains(genderFilter));
            }
            if (filter.DateOfBirth.HasValue)
            {
                query = query.Where(b => b.DateOfBirth.Date == filter.DateOfBirth.Value.Date);
            }
            if (!string.IsNullOrEmpty(filter.Email))
            {
                query = query.Where(b => b.Email.ToLower().Contains(filter.Email.ToLower()));
            }
            if (!string.IsNullOrEmpty(filter.Phone))
            {
                query = query.Where(b => b.Phone.ToLower().Contains(filter.Phone.ToLower()));
            }

            return await query.ToListAsync();
        }









    }




}
