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
    public class RepositoryUser : IRepositoryUser
    {
        private readonly ApplicationDbContext _db;
        public RepositoryUser(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<User> Register(User user, string password)
        {
            await _db.AddAsync(user);

            await Save();

            return user;
        }
        public async Task<User> Login(string password, string Username)
        {
            User item = await _db.users.Include(sp => sp.Rol).FirstOrDefaultAsync(x => x.Username == Username);

            if (item == null)
            {
                return null;
            }

           if(item.Password != password)
            {
                return null;
            }

            return item;
        }
        public async Task<bool> ExistUser(string Username)
        {
            return await _db.users.AnyAsync(u => u.Username == Username);
        }
        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() >= 0 ? true : false;
        }
    }
}
