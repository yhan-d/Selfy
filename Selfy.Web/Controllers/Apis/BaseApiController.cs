using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Selfy.Web.Controllers.Apis
{
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {

    }
}
