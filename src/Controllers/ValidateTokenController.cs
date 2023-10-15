using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroservice.Controllers
{
    [Route("api/rest/v1/validate")]
    [ApiController]
    public class ValidateTokenController : ControllerBase
    {


        [HttpGet("user")]
        [Authorize(Policy = "MustBeCustomer")]
        public ActionResult isValidCustomer()
        {

            return Ok();

        }

        [HttpGet("admin")]
        [Authorize(Policy = "MustBeAdmin")]
        public ActionResult isValidAdmin()
        {

            return Ok();

        }

    }
    }
