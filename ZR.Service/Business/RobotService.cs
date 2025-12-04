using Aliyun.OSS;
using Infrastructure.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ZR.Model.Business;
using ZR.Model.Business.Dto;
using ZR.Service.Business.IBusinessService;

namespace ZR.Service.Business
{
    [AppService(ServiceType = typeof(IRobotService), ServiceLifetime = LifeTime.Transient)]
    public class RobotService: BaseService<Robot>, IRobotService
    {
       
    }
}
