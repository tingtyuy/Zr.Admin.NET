using ZR.Model.Business.Dto;
using ZR.Model.Business;

namespace ZR.Service.Business.IBusinessService
{
    /// <summary>
    /// service接口
    /// </summary>
    public interface IUserWxacountService : IBaseService<UserWxacount>
    {
        PagedInfo<UserWxacountDto> GetList(UserWxacountQueryDto parm);

        UserWxacount GetInfo(int Id);


        UserWxacount AddUserWxacount(UserWxacount parm);
        int UpdateUserWxacount(UserWxacount parm);


    }
}
