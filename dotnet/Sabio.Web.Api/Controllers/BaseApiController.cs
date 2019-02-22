using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Web.Models;
using Sabio.Web.Models.Responses;

namespace Sabio.Web.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected ILogger Logger { get; set; }
        public BaseApiController(ILogger logger)
        {
            logger.LogInformation($"Controller Firing {this.GetType().Name} ");
            Logger = logger;
        }

        public OkObjectResult Ok200(BaseResponse respone)
        {
            return base.Ok(respone);
        }

        public CreatedResult Created201(IItemResponse respone)
        {
            string url = Request.Path + "/" + respone.Item.ToString();

            return base.Created(url, respone);
        }

        public NotFoundObjectResult NotFound404(BaseResponse respone)
        {
            return base.NotFound(respone);
        }
    }
}