namespace PcShop.Areas.IUsers.Interface
{
    public interface IAuthBus
    {
        Task<Object> GoogleLoginAsync(string idToken);
    }
}