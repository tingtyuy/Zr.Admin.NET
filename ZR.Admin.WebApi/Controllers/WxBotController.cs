using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Web;
using ZR.Model;
using ZR.ServiceCore.Ollama;
using ZR.ServiceCore.WxBot;

namespace ZR.Admin.WebApi.Controllers
{
    /// <summary>
    /// 微信AI客服
    /// </summary>
    [Route("[controller]/[action]")]
    [AllowAnonymous]
    public class WxBotController : BaseController
    {
        private readonly IWxBotMessageLogService wxBotMessageLogService;
        private readonly IWxServerHubAPI wxServerHubAPI;
        private readonly IOllamaHubAPI ollamaHubAPI;
        public WxBotController(IWxBotMessageLogService wxBotMessageLogService, IWxServerHubAPI wxServerHubAPI, IOllamaHubAPI ollamaHubAPI)
        {
            this.wxBotMessageLogService = wxBotMessageLogService;
            this.wxServerHubAPI = wxServerHubAPI;
            this.ollamaHubAPI = ollamaHubAPI;
        }
        [HttpPost]
        public async Task<JsonResult> Post([FromBody] WxBotMessageLog msgObj)
        {
           var number= wxBotMessageLogService.Insert(msgObj);
            //var loginStatus = await wxServerHubAPI.GetLoginAsync();
            //var wxId = await wxServerHubAPI.GetWxIdAsync();
            //var contacts = await wxServerHubAPI.GetContactsAsync();
            //var saveImagesPath =await wxServerHubAPI.PostSaveImage(new SaveImageRequestModel() { id=msgObj.id, extra= msgObj.extra, dir=msgObj.extra.Substring(0,2), timeout=30} );
            //var loginStatus = await wxServerHubAPI.GetLoginAsync();
            //var genReturnContentModel = await ollamaHubAPI.PostGenerate(new GenerateRequestModel() { prompt = msgObj.content });
            //var textModel=await wxServerHubAPI.PostText(new TextRequestModel() {  msg=genReturnContentModel.response, receiver= msgObj.sender});
            return new JsonResult(number);

        }

    }

}
