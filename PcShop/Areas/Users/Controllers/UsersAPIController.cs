using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcShop.Areas.Interface;

namespace PcShop.Areas.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersAPIController : ControllerBase
    {
        //api call邏輯層
        private readonly UsersInterfaceBus _usersInterfaceBus;
        public UsersAPIController(UsersProfileBus usersInterfaceBus)
        {
            _usersInterfaceBus = usersInterfaceBus;
        }
    }
}
