using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Fortress.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {

        [HttpPost]
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        public ActionResult<CreateUserResponse> Post([FromBody] CreateUserRequest request)
        {
            //for frontend testing
            var response = new CreateUserResponse()
            {
                Id = SampleData.UserId
            };

            return Created(string.Empty, response);
        }
    }
}
