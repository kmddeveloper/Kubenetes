using Kubernetes.TransferObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kubernetes.Business
{
    public interface IUserManager
    {
        Task<User> GetUserByIdAsync(int Id);
        Task<User> GetFakeUserByIdAsync(int Id);
    }
}
