using Infrastructure.Attribute;
using ZR.Model;

namespace ZR.ServiceCore.Services
{
    /// <summary>
    /// 微信AI客服消息日志服务
    /// </summary>
    [AppService(ServiceLifetime = LifeTime.Transient, ServiceType = typeof(IWxBotMessageLogService))]
    public class WxBotMessageLogService : BaseService<WxBotMessageLog>, IWxBotMessageLogService
    {
        public WxBotMessageLogService( )
        {
        }


    }
}
