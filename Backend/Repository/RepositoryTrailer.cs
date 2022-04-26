using Backend.Data;
using Backend.Models;
using Backend.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repository
{
    public class RepositoryTrailer : IRepositoryTrailer
    {
        private readonly ApplicationDbContext _db;
        public RepositoryTrailer(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> CreateTrailer(Trailer trailer)
        {
            await _db.trailers.AddAsync(trailer);
            return await Save();
        }

        public async Task<bool> DeleteTrailer(Trailer trailer)
        {
             _db.trailers.Remove(trailer);
            return await Save();
        }

        public async Task<bool> UpdateTrailer(Trailer trailer)
        {
            _db.trailers.Update(trailer);
            return  await Save();
        }      
    
        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() >= 0 ? true : false;
        }

        public async Task<List<Trailer>> GetTrailer()
        {
            return await _db.trailers.OrderBy(x => x.Titulo).ToListAsync();
        }

        public async Task<IEnumerable<Trailer>> GetTraileraByKeyword(string keyword)
        {
            IQueryable<Trailer> query = _db.trailers;

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(e => e.Titulo.Contains(keyword) || e.Reseña.Contains(keyword));
            }
            return await query.ToListAsync();
        }

        public async Task<bool> ExisteTrailer(string trailer)
        {
            return await _db.trailers.AnyAsync(p =>
             p.Titulo.Trim().ToLower() == trailer.Trim().ToLower());
        }

        public async Task<Trailer> GetTrailerByName(string name)
        {
            return await _db.trailers.FirstOrDefaultAsync(x => x.Titulo.Trim().ToLower() == name.Trim().ToLower());
        }

        public async Task<bool> ExisteTrailer(int id)
        {
            return await _db.trailers.AnyAsync(p =>
             p.Id== id);
        }

        public async Task<Trailer> GetTrailerById(int Id)
        {
            return await _db.trailers.FirstOrDefaultAsync(x => x.Id == Id);
        }
    }
}
