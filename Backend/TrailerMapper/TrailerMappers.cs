using AutoMapper;
using Backend.Models;
using Backend.Models.ModelsDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.TrailerMapper
{
    public class TrailerMappers : Profile
    {
        public TrailerMappers()
        {
            CreateMap<User, UserRegisterDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();
            CreateMap<Trailer, TrailerDto>().ReverseMap();
            CreateMap<Trailer, TrailerUpdateDto>().ReverseMap();

        }

    }
}
