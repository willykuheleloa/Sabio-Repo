using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sabio.Services.Interfaces;
using Sabio.Services;
using Microsoft.Build.Framework;
using Sabio.Web.Controllers;
using Microsoft.Extensions.Logging;
using Sabio.Web.Models.Responses;
using Sabio.Models.Domain.Users;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using Sabio.Models.Requests.Users;
using System.ComponentModel;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserApiControllerV1 : BaseApiController
        {
        private IUserServiceV1 _userService = null;
        private IAuthenticationService<int> _authService = null;
        public UserApiControllerV1(IUserServiceV1 service,
            ILogger<PingApiController> logger,
            IAuthenticationService<int> authService) : base(logger)
            {
            _userService = service;
            _authService = authService;
            }
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<User>> Get(int id)
            {
            int code = 200;
            BaseResponse response = null;

            try
                {
                User user = _userService.Get(id);
                if (user == null)
                    {
                    code = 404;
                    response = new ErrorResponse("No User Found");
                    }
                else
                    {
                    response = new ItemResponse<User> { Item = user };
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
                response = new ErrorResponse($"ArgumentException Error: {argEx.Message}");
                }
            catch (Exception ex)
                {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
                }
            return StatusCode(code, response);
            }
        [HttpGet]
        public ActionResult<ItemsResponse<User>> GetAll()
            {
            int code = 200;
            BaseResponse response = null;
            try
                {
                List<User> list = _userService.GetAll();
                if (list == null)
                    {
                    code = 404;
                    response = new ErrorResponse("User List not found");
                    }
                else
                    {
                    code = 200;
                    response = new ItemsResponse<User> { Items = list };
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
        public ActionResult<ItemResponse<int>> Add(UserAddRequest model)
            {
            ObjectResult result = null;

            try
                {
                //int userId = _authService.GetCurrentUserId();
                int id = _userService.Add(model);
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
        public ActionResult<ItemResponse<int>> Update(UserUpdateRequest model)
            {
            _userService.Update(model);
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
                _userService.Delete(id);
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
