using Kubernetes.Repository;
using Kubernetes.TransferObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kubernetes.Business
{
    public class UserManager:IUserManager
    {
        readonly IUserRepository _userRepository;
        public UserManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByIdAsync(int Id)
        {
            if (Id <= 0)
                throw new Exception("Invalid User Id!");

            return await _userRepository.GetUserByIdAsync(Id);
        }

        public async Task<User> GetFakeUserByIdAsync(int Id)
        {
            if (Id <= 0)
                throw new Exception("Invalid User Id!");

            return await _userRepository.GetUserByIdAsync(Id);
        }

    }
}
