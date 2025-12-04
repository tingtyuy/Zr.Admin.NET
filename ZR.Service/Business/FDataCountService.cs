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
    [AppService(ServiceType = typeof(IFDataCountService), ServiceLifetime = LifeTime.Transient)]
    public class FDataCountService: BaseService<FDataCount>, IFDataCountService
    {
    }
}
