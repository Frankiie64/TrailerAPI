using Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repository.IRepository
{
    public interface IRepositoryUser
    {
        public Task<bool> ExistUser(string Username);
        public Task<User> Login(string password, string Username);
        public Task<User> Register(User user, string password);
        public Task<bool> Save();

    }
}
