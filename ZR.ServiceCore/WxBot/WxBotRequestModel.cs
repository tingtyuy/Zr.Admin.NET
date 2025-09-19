using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.ServiceCore.WxBot
{

    public class SaveImageRequestModel
    {
        public long id { get; set; }
        public string extra { get; set; }
        public string dir { get; set; }
        public int timeout { get; set; }
    }

    public class TextRequestModel
    {
        public string msg { get; set; }
        public string receiver { get; set; }
        public string aters { get; set; }
    }


    public class CallBackRequestModel   
    {
        public string callback { get; set; }
    }



}


