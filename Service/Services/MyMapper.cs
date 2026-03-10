using Repository.models;
using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class MyMapper:AutoMapper.Profile
    {
        public MyMapper()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<CandidateProfiles, CandidateProfileDto>().ReverseMap();
            CreateMap<Categories, CategoriesDto>().ReverseMap();
            CreateMap<JobListings, JobListingsDto>().ReverseMap();
            CreateMap<Employer, EmployerDto>().ReverseMap();
            CreateMap<Match, MatchDto>().ReverseMap();
        }
    }
}
