using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models.Domain.Concerts;
using Sabio.Models.Requests.Concerts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sabio.Services
    {
    public class ConcertService
        {
        IDataProvider _data = null;
        public ConcertService(IDataProvider data)
            {
            _data = data;
            }
        public Concert GetById(int id)
            {
            Concert concert = null;

            _data.ExecuteCmd("[dbo].[Concerts_SelectById]",
                delegate (SqlParameterCollection col)
                    {
                        col.AddWithValue("@Id", id);
                        },
                delegate (IDataReader reader, short set)
                    {
                        concert = SingleConcertMapper(reader);
                        }
                    );
            return concert;
            }
        public List<Concert> GetAll()
            {
            List<Concert> list = null;
            _data.ExecuteCmd("[dbo].[Concerts_SelectAll]",
                inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                    {
                        Concert aConcert = SingleConcertMapper(reader);
                        if (list == null)
                            {
                            list = new List<Concert>();
                            }
                        list.Add(aConcert);
                        }
                );
            return list;
            }
        public int Add(ConcertAddRequest model)
            {
            int id = 0;
            _data.ExecuteNonQuery("[dbo].[Concerts_Insert]",
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
        public void Update(ConcertUpdateRequest model)
            {
            _data.ExecuteNonQuery("[dbo].[Concerts_Update]",
                inputParamMapper: delegate (SqlParameterCollection col)
                    {
                        col.AddWithValue("@Id", model.Id);
                        AddCommonParams(model, col);
                        },
                returnParameters: null
                );
            }
        public void Delete(int id)
            {
            _data.ExecuteNonQuery("[dbo].[Concerts_Delete]",
                inputParamMapper: delegate (SqlParameterCollection col)
                    {
                        col.AddWithValue("@Id", id);
                        },
                returnParameters: null);
            }

        private static void AddCommonParams(ConcertAddRequest model, SqlParameterCollection col)
            {
            col.AddWithValue("@Name", model.Name);
            col.AddWithValue("@Description", model.Description);
            col.AddWithValue("@IsFree", model.IsFree);
            col.AddWithValue("@Address", model.Address);
            col.AddWithValue("@Cost", model.Cost);
            col.AddWithValue("@DateOfEvent", model.DateOfEvent);
            }

        private static Concert SingleConcertMapper(IDataReader reader)
            {
            Concert concert = new Concert();
            int ord = 0;
            concert.Id = reader.GetSafeInt32(ord++);
            concert.Name = reader.GetSafeString(ord++);
            concert.Description = reader.GetSafeString(ord++);
            concert.IsFree = reader.GetSafeBool(ord++);
            concert.Address = reader.GetSafeString(ord++);
            concert.Cost = reader.GetSafeInt32(ord++);
            concert.DateOfEvent = reader.GetSafeDateTime(ord++);
            return concert;
            }
        }
    }
