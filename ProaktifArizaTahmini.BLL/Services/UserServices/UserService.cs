using AutoMapper;
using ProaktifArizaTahmini.CORE.Entities;
using ProaktifArizaTahmini.CORE.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<bool> CheckUserByUsername(string username)
        {
            bool result = await userRepository.Any(u => u.Username.ToLower() == username.ToLower());
            return result;
        }

        public async Task<bool> CreateUser(User domainUser)
        {
            User user = new User();
            user = mapper.Map(domainUser, user);
            bool result = await userRepository.Create(user);
            return result;
        }

        public async Task<bool> DeleteUser(int ID)
        {
            User user = await userRepository.GetWhere(x => x.ID == ID);
            if (user == null) return false;
            bool result = userRepository.Delete(user);
            return result;
        }

        public async Task<User> GetChangedUser(string username, string password)
        {
            var user = await userRepository.GetWhere(u => u.Username.ToLower() == username.ToLower() && u.Password != password);
            return user;
        }

        public async Task<User> GetUser(string username, string password)
        {
            var user = await userRepository.GetWhere(u=>u.Username.ToLower() == username.ToLower() &&  u.Password == password);
            return user;
        }

        public async Task<bool> UpdateUser(User user)
        {
            var updateUser = await userRepository.GetWhere(x => x.ID == user.ID);
            updateUser = mapper.Map(user, updateUser);
            bool result = userRepository.Update(updateUser);
            return result;
        }
    }
}
