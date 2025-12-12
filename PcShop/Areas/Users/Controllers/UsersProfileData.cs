using PcShop.Areas.Interface;
using PcShop.Models;

namespace PcShop.Areas.Users.Controllers
{
    public class UsersProfileData:UsersInterfaceData
    {
        private readonly ExamContext _examContext;
        public UsersProfileData(ExamContext examContext)
        {
            _examContext = examContext;
        }
    }
}
