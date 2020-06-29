using Kubernetes.TransferObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kubernetes.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int Id);
    }
}
