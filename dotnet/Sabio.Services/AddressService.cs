using Sabio.Data.Providers;
using Sabio.Models.Domain.Addresses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sabio.Data;
using System.Net;
using Sabio.Models.Requests.Addresses;
using Sabio.Services.Interfaces;

namespace Sabio.Services
{
    public class AddressService : IAddressService
        {
        IDataProvider _data = null;
        public AddressService(IDataProvider data)
            {
            _data = data;
            }
        public void Delete(int Id)
            {
            _data.ExecuteNonQuery("[dbo].[Sabio_Addresses_DeleteById]",
                inputParamMapper: delegate (SqlParameterCollection col)
                    {
                        col.AddWithValue("@Id", Id);

                        },
                returnParameters: null);
            }
        public void Update(AddressUpdateRequest model)
            {

            _data.ExecuteNonQuery("[dbo].[Sabio_Addresses_Update]",
                     inputParamMapper: delegate (SqlParameterCollection col)
                     {
                         col.AddWithValue("@Id", model.Id);
                         AddCommonParams(model, col);
                         },
                     returnParameters: null);
            }
        public int Add(AddressAddRequest model, int userId)
            {
            int id = 0;

            _data.ExecuteNonQuery("[dbo].[Sabio_Addresses_Insert]",

                inputParamMapper: delegate (SqlParameterCollection col)
                    {
                        SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                        idOut.Direction = ParameterDirection.Output;
                        col.Add(idOut);

                        AddCommonParams(model, col);
                        },
                returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;

                    int.TryParse(oId.ToString(), out id);

                    }
            );

            return id;
            }
        public Address Get(int Id)
            {
            Address address = null;

            _data.ExecuteCmd("[dbo].[Sabio_Addresses_SelectById]",
                delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Id", Id);
                    },
                delegate (IDataReader reader, short set)
                {
                    address = MapSingleAddress(reader);
                    }
            );

            return address;
            }
        public List<Address> GetRandomAddresses()
            {
            List<Address> list = null;

            _data.ExecuteCmd("[dbo].[Sabio_Addresses_SelectRandom50]",
                inputParamMapper: null,
               singleRecordMapper: delegate (IDataReader reader, short set)
                   {
                       Address anAddress = MapSingleAddress(reader);

                       if (list == null)
                           {
                           list = new List<Address>();
                           }
                       list.Add(anAddress);
                       }
           );

            return list;
            }
        private static Address MapSingleAddress(IDataReader reader)
            {
            Address anAddress = new Address();
            int startIndex = 0;
            anAddress.Id = reader.GetSafeInt32(startIndex++);
            anAddress.LineOne = reader.GetSafeString(startIndex++);
            anAddress.SuiteNumber = reader.GetSafeInt32(startIndex++);
            anAddress.City = reader.GetSafeString(startIndex++);
            anAddress.State = reader.GetSafeString(startIndex++);
            anAddress.PostalCode = reader.GetSafeString(startIndex++);
            anAddress.IsActive = reader.GetSafeBool(startIndex++);
            anAddress.Lat = reader.GetSafeDouble(startIndex++);
            anAddress.Long = reader.GetSafeDouble(startIndex++);
            return anAddress;
            }
        private static void AddCommonParams(AddressAddRequest model, SqlParameterCollection col)
            {
            col.AddWithValue("@LineOne", model.LineOne);
            col.AddWithValue("@SuiteNumber", model.SuiteNumber);
            col.AddWithValue("@City", model.City);
            col.AddWithValue("@State", model.State);
            col.AddWithValue("@PostalCode", model.PostalCode);
            col.AddWithValue("@IsActive", model.IsActive);
            col.AddWithValue("@Lat", model.Lat);
            col.AddWithValue("@Long", model.Long);
            }

        }
    }