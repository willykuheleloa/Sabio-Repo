using Sabio.Data;
using Sabio.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Sabio.Models.Requests.Users;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests;
using Sabio.Services.Interfaces;

namespace Sabio.Services
{
    public class UserServiceV1 : IUserServiceV1
    {
        IDataProvider _data = null;
        public UserServiceV1(IDataProvider data)
        {
            _data = data;
        }
        public void Delete(int Id)
        {
            _data.ExecuteNonQuery("[dbo].[Users_Delete]",
                inputParamMapper: delegate (SqlParameterCollection col)
                    {
                        col.AddWithValue("@Id", Id);
                    },
                returnParameters: null);
        }
        public void Update(UserUpdateRequest model)
        {
            _data.ExecuteNonQuery("[dbo].[Users_Update]",
                inputParamMapper: delegate (SqlParameterCollection col)
                    {
                        col.AddWithValue("@Id", model.Id);
                        AddCommonParams(model, col);
                    },
                returnParameters: null);
        }
        public int Add(UserAddRequest model)
        {
            int id = 0;

            _data.ExecuteNonQuery("[dbo].[Users_Insert]",
                inputParamMapper: delegate (SqlParameterCollection col)
                    {
                        SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                        idOut.Direction = ParameterDirection.Output;
                        col.Add(idOut);

                        AddCommonParams(model, col);
                    },
                returnParameters: delegate (SqlParameterCollection returnCol)
                    {
                        object oId = returnCol["@Id"].Value;

                        int.TryParse(oId.ToString(), out id);
                    }
                );
            return id;
        }
        public User Get(int Id)
        {

            User user = null;

            _data.ExecuteCmd("[dbo].[Users_SelectById]",
                delegate (SqlParameterCollection col)
                    {
                        col.AddWithValue("@Id", Id);
                    },
                delegate (IDataReader reader, short set)
                    {
                        user = MapSingleUser(reader);
                    }

                );
            return user;
        }
        public List<User> GetAll()
        {
            List<User> list = null;

            _data.ExecuteCmd("[dbo].[Users_SelectAll]",
                inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                    {
                        User aUser = MapSingleUser(reader);

                        if (list == null)
                        {
                            list = new List<User>();
                        }
                        list.Add(aUser);
                    }
                );
            return list;
        }
        private static void AddCommonParams(UserAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@FirstName", model.FirstName);
            col.AddWithValue("@LastName", model.LastName);
            col.AddWithValue("@Email", model.Email);
            col.AddWithValue("@Password", model.Password);
            col.AddWithValue("@AvatarUrl", model.AvatarUrl);
            col.AddWithValue("@TenantId", model.TenantId);
        }
        private static User MapSingleUser(IDataReader reader)
        {
            User aUser = new User();
            int startIndex = 0;
            aUser.Id = reader.GetInt32(startIndex++);
            aUser.FirstName = reader.GetSafeString(startIndex++);
            aUser.LastName = reader.GetSafeString(startIndex++);
            aUser.Email = reader.GetSafeString(startIndex++);
            aUser.AvatarUrl = reader.GetSafeString(startIndex++);
            aUser.TenantId = reader.GetSafeString(startIndex++);
            aUser.DateCreated = reader.GetSafeDateTime(startIndex++);
            aUser.DateModified = reader.GetSafeDateTime(startIndex++);
            return aUser;
        }

    }
}
