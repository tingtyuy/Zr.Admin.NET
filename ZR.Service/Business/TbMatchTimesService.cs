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
    [AppService(ServiceType =typeof(ITbMatchTimesService), ServiceLifetime =LifeTime.Transient)]
    public class TbMatchTimesService: BaseService<TbMatchTimes>, ITbMatchTimesService
    {

    }
}
