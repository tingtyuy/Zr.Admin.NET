using ZR.Model.Business.Dto;
using ZR.Model.Business;

namespace ZR.Service.Business.IBusinessService
{
    /// <summary>
    /// service接口
    /// </summary>
    public interface ITbOrderService : IBaseService<TbOrder>
    {
        PagedInfo<TbOrderDto> GetList(TbOrderQueryDto parm);

        TbOrder GetInfo(long Id);


        TbOrder AddTbOrder(TbOrder parm);
        int UpdateTbOrder(TbOrder parm);


    }
}
