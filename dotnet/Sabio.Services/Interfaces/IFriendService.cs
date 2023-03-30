using Sabio.Models;
using Sabio.Models.Domain.Friends;
using Sabio.Models.Requests.Friends;
using System.Collections.Generic;

namespace Sabio.Services.Interfaces
{
    public interface IFriendService
    {
        int Add(FriendAddRequest model, int userId);
        void Delete(int Id);
        Friend Get(int id);
        List<Friend> GetAll();
        void Update(FriendUpdateRequest model, int userId);
        FriendV2 GetV2(int id);
        List <FriendV2> GetAllV2();
        int AddV2(FriendAddRequestV2 model, int userId);
        void UpdateV2(FriendUpdateRequestV2 model, int userId);
        void DeleteV2(int id);
        Paged<FriendV2> Pagination(int pageIndex, int pageSize);
        Paged<FriendV2> SearchPagination(int pageIndex, int pageSize, string query);
        FriendV3 GetV3(int id);
        List<FriendV3> GetAllV3();
        Paged<FriendV3> PaginationV3(int pageIndex, int pageSize);
        Paged<FriendV3> SearchPaginationV3(int pageIndex, int pageSize, string query);
        int AddV3(FriendAddRequestV3 model, int userId);
        void FriendUpdateV3(FriendUpdateRequestV3 model, int userId);

        }
    }