using AutoMapper;
using BusinessCard.Core.Data;
using BusinessCard.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCard.Core.Configrations
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            CreateMap<BusinessCards, BusinessCardsDTo>().ReverseMap();

            CreateMap<BusinessCards, FillterBusinessCardsDTo>().ReverseMap();


        }
    }
}
