using ZR.Model.Business.Dto;
using ZR.Model.Business;

namespace ZR.Service.Business.IBusinessService
{
    /// <summary>
    /// service接口
    /// </summary>
    public interface ITbWxGroupMemberService : IBaseService<TbWxGroupMember>
    {
        PagedInfo<TbWxGroupMemberDto> GetList(TbWxGroupMemberQueryDto parm);

        TbWxGroupMember GetInfo(int Id);


        TbWxGroupMember AddTbWxGroupMember(TbWxGroupMember parm);
        int UpdateTbWxGroupMember(TbWxGroupMember parm);


    }
}
