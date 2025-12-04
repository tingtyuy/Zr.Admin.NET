using Infrastructure.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZR.Model.Business;
using ZR.Service.Business.IBusinessService;
using ZR.Repository;

namespace ZR.Service.Business
{
    [AppService(ServiceType = typeof(ITbDailyMatchService), ServiceLifetime = LifeTime.Transient)]
    public class TbDailyMatchService: BaseService<TbDailyMatch>, ITbDailyMatchService
    {
        public TbDailyMatch GetInfo(int Id)
        {
            var theObject = Queryable().Where(x=> x.id==Id).First();
            return theObject;
        }

        public TbDailyMatch AddRecord(TbDailyMatch _parm)
        {            
            return Insertable(_parm).ExecuteReturnEntity();
        }
        public int UpdateRecord(TbDailyMatch _parm)
        {
            return Update(_parm, true);
        }

    }
}
