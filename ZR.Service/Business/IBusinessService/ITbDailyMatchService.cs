using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZR.Model.Business;

namespace ZR.Service.Business.IBusinessService
{
    public interface ITbDailyMatchService: IBaseService<TbDailyMatch>
    {
        TbDailyMatch GetInfo(int Id);
        TbDailyMatch AddRecord(TbDailyMatch parm);
        int UpdateRecord(TbDailyMatch parm);
    }
}
