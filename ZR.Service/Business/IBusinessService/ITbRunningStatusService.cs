using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZR.Model.Business;

namespace ZR.Service.Business.IBusinessService
{
    public interface ITbRunningStatusService: IBaseService<TbRobotRunningStatus>
    {
        TbRobotRunningStatus GetInfo(int Id);
        TbRobotRunningStatus AddRecord(TbRobotRunningStatus _parm);
        int UpdateRecord(TbRobotRunningStatus _parm);
    }
}
