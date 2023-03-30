using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NuGet.Repositories;
using Sabio.Models.Domain.Friends;
using Sabio.Models.Requests.Friends;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Sabio.Web.Api.Controllers
    {
    [Route("api/friends")]
    [ApiController]
    public class FriendApiController : BaseApiController
        {
        private IFriendService _friendService = null;
        private IAuthenticationService<int> _authService = null;
        public FriendApiController(IFriendService service,
                ILogger<PingApiController> logger,
                IAuthenticationService<int> authService) : base(logger)
            {
            _friendService = service;
            _authService = authService;
            }
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Friend>> Get(int id)
            {
            int code = 200;
            BaseResponse response = null;

            try
                {
                Friend friend = _friendService.Get(id);
                if (friend == null)
                    {
                    code = 404;
                    response = new ErrorResponse("Friend not found");
                    }
                else
                    {
                    response = new ItemResponse<Friend> { Item = friend };
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
        public ActionResult<ItemsResponse<Friend>> GetAll()
            {
            int code = 200;
            BaseResponse response = null;

            try
                {
                List<Friend> list = _friendService.GetAll();
                if (list == null)
                    {
                    code = 404;
                    response = new ErrorResponse("App Resource not found");
                    }
                else
                    {
                    response = new ItemsResponse<Friend> { Items = list };
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
        public ActionResult<ItemResponse<int>> Add(FriendAddRequest model)
            {
            ObjectResult result = null;
            try
                {
                int userId = _authService.GetCurrentUserId();
                int id = _friendService.Add(model, userId);
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
        public ActionResult<ItemResponse<int>> Update(FriendUpdateRequest model)
            {
            int userId = _authService.GetCurrentUserId();
            _friendService.Update(model, userId);
            SuccessResponse response = new SuccessResponse();
            return Ok(response);
            }
        [HttpDelete("{id:int}")]
       public ActionResult Delete(int id)
            {
            int code = 200;
            BaseResponse response = null;

            try
                {
                _friendService.Delete(id);
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
