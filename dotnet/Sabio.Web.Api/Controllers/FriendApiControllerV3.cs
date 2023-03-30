using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Services.Interfaces;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using Sabio.Models.Domain.Friends;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using Sabio.Models.Requests.Friends;
using Sabio.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Sabio.Web.Api.Controllers
    {
    [Route("api/v3/friends")]
    [ApiController]
    public class FriendApiControllerV3 : BaseApiController
        {
        private IFriendService _friendService = null;
        private IAuthenticationService<int> _authService = null;
        public FriendApiControllerV3(IFriendService service,
            ILogger<PingApiController> logger,
            IAuthenticationService<int> authService) : base(logger)
            {
            _friendService = service;
            _authService = authService;
            }
        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<FriendV3>>> PaginationV3(int pageIndex, int pageSize)
            {
            ActionResult result = null;
            //coming from url for pagin
            try
                {
                Paged<FriendV3> paged = _friendService.PaginationV3(pageIndex, pageSize);
                if (paged == null)
                    {
                    result = NotFound404(new ErrorResponse("Records not found"));
                    }
                else
                    {
                    ItemResponse<Paged<FriendV3>> response = new ItemResponse<Paged<FriendV3>>();
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
        public ActionResult<ItemResponse<Paged<FriendV3>>> SearchPaginationV3(int pageIndex, int pageSize, string query)
            {
            ActionResult result = null;

            try
                {
                Paged<FriendV3> paged = _friendService.SearchPaginationV3(pageIndex, pageSize, query);
                if (paged == null)
                    {
                    result = NotFound404(new ErrorResponse("Records not found"));
                    }
                else
                    {
                    ItemResponse<Paged<FriendV3>> response = new ItemResponse<Paged<FriendV3>>();
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
        public ActionResult<ItemResponse<FriendV3>> GetV3(int id)
            {
            int code = 200;
            BaseResponse response = null;

            try
                {
                FriendV3 friend = _friendService.GetV3(id);
                if (friend == null)
                    {
                    code = 404;
                    response = new ErrorResponse("Friend not Found");
                    }
                else
                    {
                    response = new ItemResponse<FriendV3> { Item = friend };
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
        public ActionResult<ItemsResponse<FriendV3>> GetAllV3()
            {
            int code = 200;
            BaseResponse response = null;

            try
                {
                List<FriendV3> list = _friendService.GetAllV3();
                if (list == null)
                    {
                    code = 404;
                    response = new ErrorResponse("App Resource not found");
                    }
                else
                    {
                    response = new ItemsResponse<FriendV3> { Items = list };
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
        public ActionResult<ItemResponse<int>> AddV3(FriendAddRequestV3 model)
            {
            ObjectResult result = null;

            try
                {
                int userId = _authService.GetCurrentUserId();
                int id = _friendService.AddV3(model, userId);
                ItemResponse<int> response = new ItemResponse<int> { Item = id };
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
        public ActionResult<ItemResponse<int>> FriendUpdateV3(FriendUpdateRequestV3 model)
            {
            int code = 200;
            BaseResponse response = null;

            int userId = _authService.GetCurrentUserId();

            try
                {
                    _friendService.FriendUpdateV3(model, userId);
                 response = new SuccessResponse();
                }
            catch (Exception ex)
                {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
                }
            return StatusCode(code, response);
            }
        }
    }
