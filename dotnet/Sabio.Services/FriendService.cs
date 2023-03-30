using Sabio.Data.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sabio.Models.Domain.Friends;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Data;
using Sabio.Data;
using Sabio.Models.Requests.Friends;
using Sabio.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using Image = Sabio.Models.Domain.Friends.Image;
using Sabio.Models;
using System.Net.NetworkInformation;
using System.Reflection;
using Sabio.Models.Domain;

namespace Sabio.Services
    {
    public class FriendService : IFriendService
        {
        IDataProvider _data = null;

        public FriendService(IDataProvider data)
            {
            _data = data;
            }
        #region ----FriendV1 CRUD----
        public void Delete(int Id)
            {
            _data.ExecuteNonQuery("[dbo].[Friends_Delete]",
                inputParamMapper: delegate (SqlParameterCollection col)
                    {
                        col.AddWithValue("@Id", Id);
                        },
                returnParameters: null
                );
            }
        public void Update(FriendUpdateRequest model, int userId)
            {
            _data.ExecuteNonQuery("[dbo].[Friends_Update]",
                inputParamMapper: delegate (SqlParameterCollection col)
                    {
                        col.AddWithValue("@Id", model.Id);
                        AddCommonParams(model, col);
                        col.AddWithValue("@UserId", userId);
                        }, returnParameters: null);

            }
        public int Add(FriendAddRequest model, int userId)
            {
            int id = 0;
            _data.ExecuteNonQuery("[dbo].[Friends_Insert]",
                inputParamMapper: delegate (SqlParameterCollection col)
                    {
                        SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                        idOut.Direction = ParameterDirection.Output;
                        col.Add(idOut);

                        AddCommonParams(model, col);
                        col.AddWithValue("@UserId", userId);
                        },
                returnParameters: delegate (SqlParameterCollection returnCol)
                    {
                        object oId = returnCol["@Id"].Value;

                        int.TryParse(oId.ToString(), out id);
                        }
                );
            return id;
            }
        public Friend Get(int id)
            {
            Friend friend = null;

            _data.ExecuteCmd("[dbo].[Friends_SelectById]",
                delegate (SqlParameterCollection col)
                    {
                        col.AddWithValue("@Id", id);
                        },
                delegate (IDataReader reader, short set)
                    {
                        friend = MapSingleFriend(reader);
                        }
                );
            return friend;
            }
        public List<Friend> GetAll()
            {
            List<Friend> list = null;
            _data.ExecuteCmd("[dbo].[Friends_SelectAll]",
                inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                    {
                        Friend aFriend = MapSingleFriend(reader);
                        if (list == null)
                            {
                            list = new List<Friend>();
                            }
                        list.Add(aFriend);
                        }
                );
            return list;
            }

        private static void AddCommonParams(FriendAddRequest model, SqlParameterCollection col)
            {
            col.AddWithValue("@Title", model.Title);
            col.AddWithValue("@Bio", model.Bio);
            col.AddWithValue("@Summary", model.Summary);
            col.AddWithValue("@Headline", model.Headline);
            col.AddWithValue("@Slug", model.Slug);
            col.AddWithValue("@StatusId", model.StatusId);
            col.AddWithValue("@PrimaryImageUrl", model.PrimaryImageUrl);

            }
        private static Friend MapSingleFriend(IDataReader reader)
            {
            Friend aFriend = new Friend();
            int i = 0;
            aFriend.Id = reader.GetInt32(i++);
            aFriend.Title = reader.GetSafeString(i++);
            aFriend.Bio = reader.GetSafeString(i++);
            aFriend.Summary = reader.GetSafeString(i++);
            aFriend.Headline = reader.GetSafeString(i++);
            aFriend.Slug = reader.GetSafeString(i++);
            aFriend.StatusId = reader.GetInt32(i++);
            aFriend.PrimaryImageUrl = reader.GetSafeString(i++);
            aFriend.UserId = reader.GetInt32(i++);
            aFriend.DateCreated = reader.GetDateTime(i++);
            aFriend.DateModified = reader.GetDateTime(i++);
            return aFriend;
            }
        #endregion

        #region --- FriendV2 CRUD ---
        public FriendV2 GetV2(int id)
            {
            FriendV2 friend = null;

            _data.ExecuteCmd("[dbo].[Friends_SelectByIdV2]",
                delegate (SqlParameterCollection col)
                    {
                        col.AddWithValue("@Id", id);
                        },
                delegate (IDataReader reader, short set)
                    {
                        friend = MapperV2(reader);
                        }
                );
            return friend;
            }
        public List<FriendV2> GetAllV2()
            {
            List<FriendV2> list = null;
            _data.ExecuteCmd("[dbo].[Friends_SelectAllV2]",
               inputParamMapper: null,
               singleRecordMapper: delegate (IDataReader reader, short set)
                   {
                       FriendV2 aFriend = MapperV2(reader);
                       if (list == null)
                           {
                           list = new List<FriendV2>();
                           }
                       list.Add(aFriend);
                       }
               );
            return list;
            }
        public Paged<FriendV2> Pagination(int pageIndex, int pageSize)
            {
            Paged<FriendV2> pagedList = null;
            List<FriendV2> list = null;
            int totalCount = 0;

            _data.ExecuteCmd("[dbo].[Friends_PaginationV2]",
                (param) =>
                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@pageSize", pageSize);
                }, (reader, recordSetIndex) =>
                {
                    FriendV2 friend = MapperV2(reader);
                    totalCount = reader.GetSafeInt32(13);

                    if (list == null)
                        {
                        list = new List<FriendV2>();
                        }
                    list.Add(friend);
                }
                );
            if (list != null)
                {
                pagedList = new Paged<FriendV2>(list, pageIndex, pageSize, totalCount);
                }
            return pagedList;
            }
        public Paged<FriendV2> SearchPagination(int pageIndex, int pageSize, string query)
            {
            Paged<FriendV2> pagedList = null;
            List<FriendV2> list = null;
            int totalCount = 0;
            //string query = null;

            _data.ExecuteCmd("[dbo].[Friends_Search_PaginationV2]",
                (param) =>
                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                    param.AddWithValue("@Query", query);
                }, (reader, recordSetIndex) =>
                {
                    FriendV2 friend = MapperV2(reader);
                    totalCount = reader.GetSafeInt32(13);
                    //query = reader.GetSafeString()

                    if (list == null)
                        {
                        list = new List<FriendV2>();
                        }
                    list.Add(friend);
                }
                );
            if (list != null)
                {
                pagedList = new Paged<FriendV2>(list, pageIndex, pageSize, totalCount);
                }
            return pagedList;
            }
        public int AddV2(FriendAddRequestV2 model, int userId)
            {
            int id = 0;
            _data.ExecuteNonQuery("[dbo].[Friends_InsertV2]",
                inputParamMapper: delegate (SqlParameterCollection col)
                    {
                        SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                        idOut.Direction = ParameterDirection.Output;
                        col.Add(idOut);

                        AddCommonParamsV2(model, col);

                        col.AddWithValue("@UserId", userId);
                        },
                returnParameters: delegate (SqlParameterCollection returnCol)
                    {
                        object oId = returnCol["@Id"].Value;
                        int.TryParse(oId.ToString(), out id);
                        }
                );
            return id;
            }
        public void UpdateV2(FriendUpdateRequestV2 model, int userId)
            {
            _data.ExecuteNonQuery("[dbo].[Friends_UpdateV2]",
                inputParamMapper: delegate (SqlParameterCollection col)
                    {
                        col.AddWithValue("@Id", model.Id);
                        AddCommonParamsV2(model, col);

                        col.AddWithValue("@UserId", userId);
                        },
                returnParameters: null
                );
            }
        public void DeleteV2(int id)
            {
            _data.ExecuteNonQuery("[dbo].[Friends_DeleteV2]",
                inputParamMapper: delegate (SqlParameterCollection col)
                    {
                        col.AddWithValue("@Id", id);
                        },
                returnParameters: null
                );
            }
        private static void AddCommonParamsV2(FriendAddRequestV2 model, SqlParameterCollection col)
            {
            col.AddWithValue("@Title", model.Title);
            col.AddWithValue("@Bio", model.Bio);
            col.AddWithValue("@Summary", model.Summary);
            col.AddWithValue("@Headline", model.Headline);
            col.AddWithValue("@Slug", model.Slug);
            col.AddWithValue("@StatusId", model.StatusId);
            col.AddWithValue("@ImageTypeId", model.ImageTypeId);
            col.AddWithValue("@ImageUrl", model.ImageUrl);
            }
        private static FriendV2 MapperV2(IDataReader reader)
            {
            FriendV2 friend = new FriendV2();
            int i = 0;
            friend.Id = reader.GetInt32(i++);
            friend.Title = reader.GetSafeString(i++);
            friend.Bio = reader.GetSafeString(i++);
            friend.Summary = reader.GetSafeString(i++);
            friend.Headline = reader.GetSafeString(i++);
            friend.Slug = reader.GetSafeString(i++);
            friend.StatusId = reader.GetInt32(i++);
            friend.UserId = reader.GetInt32(i++);
            friend.PrimaryImage = new Image();
            friend.PrimaryImage.Id = reader.GetInt32(i++);
            friend.PrimaryImage.Url = reader.GetSafeString(i++);
            friend.PrimaryImage.TypeId = reader.GetSafeInt32(i++);
            friend.DateCreated = reader.GetDateTime(i++);
            friend.DateModified = reader.GetDateTime(i++);
            return friend;
            }
        #endregion

        #region --- FriendV3 CRUD ---
        public FriendV3 GetV3(int id)
            {
            FriendV3 friend = null;

            _data.ExecuteCmd("[dbo].[Friends_SelectByIdV3]",
                delegate (SqlParameterCollection col)
                    {
                        col.AddWithValue("@Id", id);
                        },
                delegate (IDataReader reader, short set)
                    {
                        int index = 0;
                        friend = MapperV3(reader, ref index);
                        }
                );
            return friend;
            }
        public List<FriendV3> GetAllV3()
            {
            List<FriendV3> list = null;
            _data.ExecuteCmd("[dbo].[Friends_SelectAllV3]",
                inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    FriendV3 aFriend = MapperV3(reader, ref index);
                    if (list == null)
                        {
                        list = new List<FriendV3>();
                        }
                    list.Add(aFriend);
                    }
                );
            return list;
            }
        public Paged<FriendV3> PaginationV3(int pageIndex, int pageSize)
            {
            Paged<FriendV3> pagedList = null;
            List<FriendV3> list = null;
            int totalCount = 0;

            _data.ExecuteCmd("[dbo].[Friends_PaginationV3]",
                (param) =>
                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@pageSize", pageSize);
                }, (reader, recordSetIndex) =>
                {
                    int index = 0;

                    FriendV3 friend = MapperV3(reader, ref index);
                    totalCount = reader.GetSafeInt32(index++);

                    if (list == null)
                        {
                        list = new List<FriendV3>();
                        }
                    list.Add(friend);
                }
                );
            if (list != null)
                {
                pagedList = new Paged<FriendV3>(list, pageIndex, pageSize, totalCount);
                }
            return pagedList;
            }
        public Paged<FriendV3> SearchPaginationV3(int pageIndex, int pageSize, string query)
            {
            Paged<FriendV3> pagedList = null;
            List<FriendV3> list = null;
            int totalCount = 0;

            _data.ExecuteCmd("[dbo].[Friends_Search_PaginationV3]",
                (param) =>
                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                    param.AddWithValue("@Query", query);
                }, (reader, recordSetIndex) =>
                {
                    int index = 0;
                    FriendV3 friend = MapperV3(reader, ref index);
                    totalCount = reader.GetSafeInt32(index++);

                    if (list == null)
                        {
                        list = new List<FriendV3>();
                        }
                    list.Add(friend);
                }
                );
            if (list != null)
                {
                pagedList = new Paged<FriendV3>(list, pageIndex, pageSize, totalCount);
                }
            return pagedList;
            }
        public DataTable MapSkills(List<string> skill)
            {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));

            foreach (string aSkill in skill)
                {
                DataRow dr = dt.NewRow();
                dr.SetField("Name", aSkill);

                dt.Rows.Add(dr);
                }

            return dt;
            }
        public int AddV3(FriendAddRequestV3 model, int userId)
            {
            DataTable myValue = MapSkills(model.Skills);
            int id = 0;
            _data.ExecuteNonQuery("[dbo].[Friends_InsertV3]",
                inputParamMapper: (param) =>
                {
                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;
                    param.Add(idOut);


                    AddCommonParmsV3(model, param, myValue);
                    param.AddWithValue("@UserId", userId);
                },
                returnParameters: delegate (SqlParameterCollection returnCol)
                    {
                        object oId = returnCol["@Id"].Value;
                        int.TryParse(oId.ToString(), out id);
                        }
                );
            return id;
            }
        public void FriendUpdateV3(FriendUpdateRequestV3 model, int userId)
            {
            DataTable myValue = MapSkills(model.Skills);

            _data.ExecuteNonQuery("[dbo].[Friends_UpdateV3]",
            (param) =>
            {
                param.AddWithValue("@Id", model.Id);
                AddCommonParmsV3(model, param, myValue);

                param.AddWithValue("@UserId", userId);
            }, returnParameters: null
            );
            }
        //public List<IdPair> AddExternalSkills(FriendAddRequestV3 model)
        //    {
        //    List<IdPair> ids = null;
        //    DataTable myParam = MapSkills(model.Skills);

        //    _data.ExecuteCmd("")
        //    }
        private static void AddCommonParmsV3(FriendAddRequestV3 model, SqlParameterCollection param, DataTable myValue)
            {
            param.AddWithValue("@Title", model.Title);
            param.AddWithValue("@Bio", model.Bio);
            param.AddWithValue("@Summary", model.Summary);
            param.AddWithValue("@Headline", model.Headline);
            param.AddWithValue("@Slug", model.Slug);
            param.AddWithValue("@StatusId", model.StatusId);
            param.AddWithValue("@ImageTypeId", model.ImageTypeId);
            param.AddWithValue("@ImageUrl", model.ImageUrl);
            param.AddWithValue("@BatchSkills", myValue);
            }

        private static FriendV3 MapperV3(IDataReader reader, ref int i)
            {
            FriendV3 friend = new FriendV3();

            friend.Id = reader.GetSafeInt32(i++);
            friend.Title = reader.GetSafeString(i++);
            friend.Bio = reader.GetSafeString(i++);
            friend.Summary = reader.GetSafeString(i++);
            friend.Headline = reader.GetSafeString(i++);
            friend.Slug = reader.GetSafeString(i++);
            friend.StatusId = reader.GetSafeInt32(i++);
            friend.UserId = reader.GetSafeInt32(i++);
            friend.PrimaryImage = new Image();
            friend.PrimaryImage.Id = reader.GetSafeInt32(i++);
            friend.PrimaryImage.Url = reader.GetSafeString(i++);
            friend.PrimaryImage.TypeId = reader.GetSafeInt32(i++);
            friend.Skills = reader.DeserializeObject<List<Skill>>(i++);
            friend.DateCreated = reader.GetDateTime(i++);
            friend.DateModified = reader.GetDateTime(i++);

          

            return friend;
            }
        #endregion
        }

    }
