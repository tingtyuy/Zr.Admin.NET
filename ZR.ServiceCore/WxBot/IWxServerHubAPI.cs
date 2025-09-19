using Microsoft.AspNetCore.Mvc;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.ServiceCore.WxBot
{
    public interface IWxServerHubAPI
    {


        [Post("/callback")]
        Task<string> PostCallBack([FromBody] CallBackRequestModel  requestModel);


        [Get("/login")]
        Task<ResponseModel<loginModel>> GetLoginAsync();

        [Get("/wxid")]
        Task<ResponseModel<wxidModel>> GetWxIdAsync();

        [Get("/contacts")]
        Task<ResponseModel<contactsModel>> GetContactsAsync();



        [Post("/save-image")]
        Task<SaveImageRequestModel> PostSaveImage([FromBody] SaveImageRequestModel requestModel);



        [Post("/text")]
        Task<TextReponseModel> PostText([FromBody] TextRequestModel requestModel);
    }
}
