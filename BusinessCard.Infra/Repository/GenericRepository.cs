﻿using BusinessCard.Core.Data;
using BusinessCard.Core.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCard.Infra.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly BusinessCardDbContext _context;

        public GenericRepository(BusinessCardDbContext context)
        {
            _context = context;
        }
        public async Task<T> AddAsync(T entity)
        {
          await  _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> GetAsync(int? id)
        {
            if(id == null)
            {
                return null;
            }


            return await _context.Set<T>().FindAsync(id);
        }
        public async Task DeleteAsync(int id)
        {
           var entity=await GetAsync(id);
            if(entity == null)
            {
                return;
            }
            _context.Set<T>().Remove(entity);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(int id)
        {
            var entity = await GetAsync(id);
            return  entity != null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }


        public async Task UpdateAsync(T entity)
        {
           _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
