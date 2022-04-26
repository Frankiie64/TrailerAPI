using Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repository.IRepository
{
    public interface IRepositoryTrailer
    {
        public Task<List<Trailer>> GetTrailer();
        public Task<Trailer> GetTrailerByName(string name);
        public Task<Trailer> GetTrailerById(int Id);
        public Task<IEnumerable<Trailer>> GetTraileraByKeyword(string keyword);
        public Task<bool> CreateTrailer(Trailer trailer);
        public Task<bool> DeleteTrailer(Trailer trailer);
        public Task<bool> UpdateTrailer(Trailer trailer);
        public Task<bool> ExisteTrailer(int id);
        public Task<bool> ExisteTrailer(string trailer);

        public Task<bool> Save();
    }
}
