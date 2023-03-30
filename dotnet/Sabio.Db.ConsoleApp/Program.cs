using Sabio.Data;
using Sabio.Models.Domain.Addresses;
using Sabio.Models.Domain.Friends;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Addresses;
using Sabio.Models.Requests.Friends;
using Sabio.Models.Requests.Users;
using Sabio.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Sabio.Db.ConsoleApp
{
    internal class Program
        {
        private static void Main(string[] args)
            {
            //Here are two example connection strings. Please check with the wiki and video courses to help you pick an option

            //string connString = @"Data Source=ServerName_Or_IpAddress;Initial Catalog=DB_Name;User ID=SabioUser;Password=Sabiopass1!";
            string connString = @"Data Source=104.42.194.102;Initial Catalog=C126_willy_kuheleloa_yahoo;User ID=C126_willy_kuheleloa_yahoo_User;Password=C126_willy_kuheleloa_yahoo_User75F7F9B7";

            TestConnection(connString);
            //TestAddressService(connString);
            //TestFriendService(connString);
            //TestUserService(connString);

            //Console.ReadLine();
            }
        private static void TestFriendService(string myConn)
            {
            #region - Constructor Calls - OK
            SqlDataProvider provider = new SqlDataProvider(myConn);
            FriendService friendService = new FriendService(provider);
            #endregion
            Friend aFriend = friendService.Get(1);

            List<Friend> friends = friendService.GetAll();

            FriendAddRequest addFriendRequest = new FriendAddRequest();
            addFriendRequest.Title = "John Smith"; 
            addFriendRequest.Bio = "This is his Bio"; 
            addFriendRequest.Summary = "John"; 
            addFriendRequest.Headline = "John"; 
            addFriendRequest.Slug = "John"; 
            addFriendRequest.StatusId = 1;
            addFriendRequest.PrimaryImageUrl = "https://image.png";
            
            int newFriendId = friendService.Add(addFriendRequest, 123);

            FriendUpdateRequest updateRequest = new FriendUpdateRequest();
            updateRequest.Title = "Mike Jones";
            updateRequest.Bio = "This is his Bio";
            updateRequest.Summary = "Summary";
            updateRequest.Headline = "Headline";
            updateRequest.Slug = "SlugUpdate";
            updateRequest.StatusId = 0;
            updateRequest.PrimaryImageUrl = "https://imageChanged.png";
            updateRequest.Id = newFriendId;

            friendService.Update(updateRequest, 123);

            friendService.Delete(123);

            Console.ReadLine();
            }
        private static void TestUserService(string myConn)
            {
            #region - Constructor Calls - OK
            SqlDataProvider provider = new SqlDataProvider(myConn);
            UserServiceV1 userServiceV1 = new UserServiceV1(provider);
            #endregion
            User aUser = userServiceV1.Get(29);

            List<User> users = userServiceV1.GetAll();

            UserAddRequest userRequest = new UserAddRequest();
            userRequest.FirstName = "Will";
            userRequest.LastName = "Kuheleloa";
            userRequest.Email = "www.email.com";
            userRequest.AvatarUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_1280.png";
            userRequest.Password = "Password1!";
            userRequest.TenantId = "1234568789";

            int newUserId = userServiceV1.Add(userRequest);

            UserUpdateRequest updateUser = new UserUpdateRequest();
            
            updateUser.FirstName = "John";
            updateUser.LastName = "Doe";
            updateUser.Email = "www.JDemail.com";
            updateUser.AvatarUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_1280.png";
            updateUser.Password = "Password1!";
            updateUser.TenantId = "1234568789";
            updateUser.Id = newUserId;

            userServiceV1.Update(updateUser);

            User updatedUser = userServiceV1.Get(newUserId);
            }
        private static void TestAddressService(string myConn)
            {

            #region - Constructor Calls - OK
            SqlDataProvider provider = new SqlDataProvider(myConn);

            AddressService addressService = new AddressService(provider);
            #endregion

            #region Gets/Selects

            Address anAddress = addressService.Get(7);

            List<Address> addresses = addressService.GetRandomAddresses();
            #endregion

            #region Insert/Updates
            AddressAddRequest request = new AddressAddRequest();

            request.City = "San Diego";
            request.State = "CA";
            request.PostalCode = "91902";
            request.LineOne = "2345 A St";
            request.SuiteNumber = 123;
            request.IsActive = true;
            request.Lat = 33.5;
            request.Long = -117.85;

            //int newId = addressService.Add(request);
            int newId = 0;

            AddressUpdateRequest updateRequest = new AddressUpdateRequest();

            updateRequest.City = "San Diego";
            updateRequest.State = "CA";
            updateRequest.PostalCode = "91902";
            updateRequest.LineOne = "2345 A St";
            updateRequest.SuiteNumber = 200;
            updateRequest.IsActive = false;
            updateRequest.Lat = 33.5;
            updateRequest.Long = -117.85;
            updateRequest.Id = newId;

            addressService.Update(updateRequest);

            Address updatedAddress = addressService.Get(newId);

            addressService.Delete(1034);
            #endregion

            Console.WriteLine(updatedAddress.Id.ToString());
            }

        private static void TestConnection(string connString)
            {
            bool isConnected = IsServerConnected(connString);
            Console.WriteLine("DB isConnected = {0}", isConnected);
            }
        //
        private static bool IsServerConnected(string connectionString)
            {
            using (SqlConnection connection = new SqlConnection(connectionString))
                {
                try
                    {
                    connection.Open();
                    return true;
                    }
                catch (SqlException ex)
                    {
                    Console.WriteLine(ex.Message);
                    return false;
                    }
                }
            }
        }
    }
