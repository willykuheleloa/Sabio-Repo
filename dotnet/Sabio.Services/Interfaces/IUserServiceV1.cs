using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Users;
using System.Collections.Generic;

namespace Sabio.Services.Interfaces
{
    public interface IUserServiceV1
    {
        int Add(UserAddRequest model);
        void Delete(int Id);
        User Get(int Id);
        List<User> GetAll();
        void Update(UserUpdateRequest model);
    }
}