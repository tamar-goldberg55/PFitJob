using AutoMapper;
using Repository.Interfaces;
using Repository.models;
using Service.Dto;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class MatchService: IMatch
    {
        private readonly IRepository<Match> _repository;
        private readonly IMapper mapper;

        public  async Task<MatchDto> AddItem(MatchDto item)
        {
            return mapper.Map<Match, MatchDto>(

            await _repository.AddItem(mapper.Map<MatchDto, Match>(item)));
        }

        public Task<double> CalculateMatchScore(int candidateId, int jobId)
        {
            throw new NotImplementedException();
        }

        public  async Task DeleteItem(int id)
        {
            await _repository.DeleteItem(id);
        }

        public async Task<List<MatchDto>> GetAll()
        {
            return mapper.Map<List<Match>, List<MatchDto>>(await
                                _repository.GetAll()); throw new NotImplementedException();
        }

        public async Task<MatchDto> GetById(int id)
        {
            return mapper.Map<Match, MatchDto>(await _repository.GetById(id));
        }

        public Task<List<MatchDto>> GetTopMatchesForCandidate(int candidateId, int topCount)
        {
            throw new NotImplementedException();
        }

        public Task<List<MatchDto>> RunMatchingAlgorithm(int jobId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateItem(int id, MatchDto item)
        {
            throw new NotImplementedException();
        }
    }
}
