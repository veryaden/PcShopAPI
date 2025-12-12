using PcShop.Areas.Interface;

namespace PcShop.Areas.Users.Controllers
{
    public class UsersProfileBus:UsersInterfaceBus
    {
        //邏輯層call資料存取層interface
        private readonly UsersInterfaceData _userinterfacedata;
        public UsersProfileBus(UsersInterfaceData userinterfacedata)
        {
            _userinterfacedata = userinterfacedata;
        }
    }
}
