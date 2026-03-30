using Microsoft.AspNetCore.Mvc;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZR.ServiceCore.WxBot;

namespace ZR.ServiceCore.Ollama
{
    public interface IOllamaHubAPI
    {
        [Post("/generate")]
        Task<GenerateResponseModel> PostGenerate([FromBody] GenerateRequestModel model);
    }
}
