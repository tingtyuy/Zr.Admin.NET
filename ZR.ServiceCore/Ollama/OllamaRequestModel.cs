using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.ServiceCore.WxBot
{

    public class GenerateRequestModel
    {

        public string model { get; set; } = "llama3.2-vision:latest";
        public string prompt { get; set; }
    }

}


