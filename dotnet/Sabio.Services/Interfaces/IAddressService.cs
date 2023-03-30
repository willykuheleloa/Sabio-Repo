using Sabio.Models.Domain.Addresses;
using Sabio.Models.Requests.Addresses;
using System.Collections.Generic;

namespace Sabio.Services.Interfaces
{
    public interface IAddressService
    {
        int Add(AddressAddRequest model, int userId);
        void Delete(int Id);
        Address Get(int Id);
        List<Address> GetRandomAddresses();
        void Update(AddressUpdateRequest model);
    }
}