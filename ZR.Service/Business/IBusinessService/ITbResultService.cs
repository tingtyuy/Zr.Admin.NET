using ZR.Model.Business.Dto;
using ZR.Model.Business;

namespace ZR.Service.Business.IBusinessService
{
    /// <summary>
    /// service接口
    /// </summary>
    public interface ITbResultService : IBaseService<TbResult>
    {
        PagedInfo<TbResultDto> GetList(TbResultQueryDto parm);

        TbResult GetInfo(int Id);


        TbResult AddTbResult(TbResult parm);
        int UpdateTbResult(TbResult parm);
        int UpdateTbResultStatus(long[] idArr);
        ReplyMessageDto GetForwardMessageResult(long[] idArr);
    }
}
