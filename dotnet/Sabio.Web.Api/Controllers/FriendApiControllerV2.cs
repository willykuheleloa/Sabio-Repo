using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models.Domain.Friends;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System.Data.SqlClient;
using System;
using SendGrid;
using System.Collections.Generic;
using Sabio.Models.Requests.Friends;
using Sabio.Models;

namespace Sabio.Web.Api.Controllers
    {
    [Route("api/v2/friends")]
    [ApiController]
    public class FriendApiControllerV2 : BaseApiController
        {
        private IFriendService _friendService = null;
        private IAuthenticationService<int> _authService = null;
        public FriendApiControllerV2(IFriendService service,
            ILogger<PingApiController> logger,
            IAuthenticationService<int> authService) : base(logger)
            {
            _friendService = service;
            _authService = authService;
            }
        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<FriendV2>>> Pagination(int pageIndex, int pageSize)
            {
            ActionResult result = null;

            try
                {
                Paged<FriendV2> paged = _friendService.Pagination(pageIndex, pageSize);
                if (paged == null)
                    {
                    result = NotFound404(new ErrorResponse("Records not found"));
                    }
                else
                    {
                    ItemResponse<Paged<FriendV2>> response = new ItemResponse<Paged<FriendV2>>();
                    response.Item = paged;
                    result = Ok200(response);
                    }
                }
            catch (Exception ex)
                {
                base.Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message));
                }
            return result;
            }
        [HttpGet("search")]
        public ActionResult<ItemResponse<Paged<FriendV2>>> SearchPagination(int pageIndex, int pageSize, string query)
            {
            ActionResult result = null;

            try
                {
                Paged<FriendV2> paged = _friendService.SearchPagination(pageIndex, pageSize, query);
                if (paged == null)
                    {
                    result = NotFound404(new ErrorResponse("Records not found"));
                    }
                else
                    {
                    ItemResponse<Paged<FriendV2>> response = new ItemResponse<Paged<FriendV2>>();
                    response.Item = paged;
                    result = Ok200(response);
                    }
                }
            catch (Exception ex)
                {
                base.Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message));
                }
            return result;
            }
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<FriendV2>> GetV2(int id)
            {
            int code = 200;
            BaseResponse response = null;

            try
                {
                FriendV2 friend = _friendService.GetV2(id);
                if (friend == null)
                    {
                    code = 404;
                    response = new ErrorResponse("Friend not found");
                    }
                else
                    {
                    response = new ItemResponse<FriendV2> { Item = friend };
                    }
                }
            catch (SqlException sqlEx)
                {
                code = 500;
                response = new ErrorResponse($"SqlException Error: {sqlEx.Message}");
                base.Logger.LogError(sqlEx.ToString());
                }
            catch (ArgumentException argEx)
                {
                code = 500;
                response = new ErrorResponse($"Argument Exception Error: {argEx.Message}");
                }
            catch (Exception ex)
                {
                code = 500;
                response = new ErrorResponse($"Error: {ex.Message}");
                base.Logger.LogError(ex.ToString());
                }
            return StatusCode(code, response);
            }
        [HttpGet]
        public ActionResult<ItemsResponse<FriendV2>> GetAllV2()
            {
            int code = 200;
            BaseResponse response = null;
            try
                {
                List<FriendV2> list = _friendService.GetAllV2();
                if (list == null)
                    {
                    code = 404;
                    response = new ErrorResponse("App Resource not found");
                    }
                else
                    {
                    response = new ItemsResponse<FriendV2> { Items = list };
                    }
                }
            catch (Exception ex)
                {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
                }
            return StatusCode(code, response);
            }
        [HttpPost]
        public ActionResult<ItemResponse<int>> AddV2(FriendAddRequestV2 model)
            {
            ObjectResult result = null;

            try
                {
                int userId = _authService.GetCurrentUserId();
                int id = _friendService.AddV2(model, userId);
                ItemResponse<int> response = new ItemResponse<int> { Item = id};
                result = Created201(response);
                }
            catch (Exception ex)
                {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
                }
            return result;
            }
        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> UpdateV2(FriendUpdateRequestV2 model)
            {
            int userId = _authService.GetCurrentUserId();
            _friendService.UpdateV2(model, userId);
            SuccessResponse response = new SuccessResponse();
            return Ok(response);
            }
        [HttpDelete("{id:int}")]
        public ActionResult DeleteV2(int id)
            {
            int code = 200;
            BaseResponse response = null;

            try
                {
                _friendService.DeleteV2(id);
                response = new SuccessResponse();
                }
            catch (Exception ex)
                {
                code = 500;
                response = new ErrorResponse(ex.Message);
                }
            return StatusCode(code, response);
            }
        }
    }
