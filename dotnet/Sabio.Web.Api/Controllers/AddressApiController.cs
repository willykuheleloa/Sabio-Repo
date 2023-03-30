using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Sabio.Models.Domain.Addresses;
using Sabio.Models.Requests.Addresses;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Sabio.Web.Api.Controllers
    {
    [Route("api/addresses")]
    [ApiController]
    public class AddressApiController : BaseApiController

        {
        private IAddressService _addressService = null;
        private IAuthenticationService<int> _authService = null;
        public AddressApiController(IAddressService service,
                ILogger<PingApiController> logger,
                IAuthenticationService<int> authService) : base(logger)
            {
            _addressService = service;
            _authService = authService;
            }
        [HttpGet]
        public ActionResult<ItemsResponse<Address>> GetAll()
            {
            int code = 200;
            BaseResponse response = null;
            try
                {
                List<Address> list = _addressService.GetRandomAddresses();

                if (list == null)
                    {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                    }
                else
                    {
                    response = new ItemsResponse<Address> { Items = list };
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
        
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Address>> Get(int id)
            {
            int iCode = 200;
            BaseResponse response = null;
            try
                {

                Address address = _addressService.Get(id);

                if (address == null)
                    {
                    iCode = 404;
                    response = new ErrorResponse("Application Resoure not Found");
                    }
                else
                    {
                    response = new ItemResponse<Address> { Item = address };
                    }
                }
            catch (SqlException sqlEx)
                {
                iCode = 500;
                response = new ErrorResponse($"SqlException Error: {sqlEx.Message}");
                base.Logger.LogError(sqlEx.ToString());
                }
            catch (ArgumentException argEx)
                {
                iCode = 500;
                response = new ErrorResponse($"ArgumentException Error: {argEx.Message}");
                }
            catch (Exception ex)
                {
                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
                }
            return StatusCode(iCode, response);
            }
        
        [HttpPost]
        public ActionResult<ItemResponse<int>> Add(AddressAddRequest model)
            {
            ObjectResult result = null;
            try
                {
                int userId = _authService.GetCurrentUserId();
                int id = _addressService.Add(model, userId);

                ItemResponse<int> response = new ItemResponse<int>() { Item = id};
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
        public ActionResult<ItemResponse<int>> Update(AddressUpdateRequest model)
            {
            _addressService.Update(model);
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
                _addressService.Delete(id);
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
