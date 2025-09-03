using ZR.Model.Business.Dto;
using ZR.Model.Business;

namespace ZR.Service.Business.IBusinessService
{
    /// <summary>
    /// service接口
    /// </summary>
    public interface ITbContactService : IBaseService<TbContact>
    {
        PagedInfo<TbContactDto> GetList(TbContactQueryDto parm);

        TbContact GetInfo(int Id);


        TbContact AddTbContact(TbContact parm);
        int UpdateTbContact(TbContact parm);


    }
}
