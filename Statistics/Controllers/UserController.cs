using Microsoft.AspNetCore.Mvc;
using Statistics.Data;

namespace Statistics.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly UserDataAccess _userDataAccess;
        public UserController(ILogger<UserController> logger, UserDataAccess userDataAccess)
        {
            _logger = logger;
            _userDataAccess = userDataAccess;
        }

        [HttpGet]
        public int GetUsersCount()
        {
            return _userDataAccess.GetUsersCount();
        }
    }
}