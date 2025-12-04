using Infrastructure.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZR.Model.Business;
using ZR.Service.Business.IBusinessService;

namespace ZR.Service.Business
{
    
    [AppService(ServiceType = typeof(ITbRunningStatusService), ServiceLifetime =LifeTime.Transient)]
    public class TbRunningStatusService :BaseService<TbRobotRunningStatus>, ITbRunningStatusService
    {
        public TbRobotRunningStatus GetInfo(int id)
        {
            var theObject = Queryable().Where(x => x.id == id).First();
            return theObject;
        }
        public TbRobotRunningStatus AddRecord(TbRobotRunningStatus _parm)
        {
            return Insertable(_parm).ExecuteReturnEntity();
        }
        public int UpdateRecord(TbRobotRunningStatus _parm)
        {
            return Update(_parm, true);
        }
    }
}
