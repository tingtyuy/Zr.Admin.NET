using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.ServiceCore.WxBot
{





    public class TextReponseModel
    {
        public int status { get; set; }
        public string message { get; set; }
    }

    public class ResponseModel<T>
    {
        public int status { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }

    public class loginModel
    {
        public bool login { get; set; }
    }

    public class wxidModel
    {
        public string wxid { get; set; }
    }
    public class contactsModel
    {
        public List<contact> contacts { get; set; }
    }
    public class contact
    {

        /// <summary>
        /// 微信ID
        /// </summary>
        [Key]
        [StringLength(50)]
        public string Wxid { get; set; }

        /// <summary>
        /// 验证码（如果适用）
        /// </summary>
        [StringLength(100)]
        public string Code { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [StringLength(255)]
        public string Remark { get; set; }

        /// <summary>
        /// 消息名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        [StringLength(100)]
        public string Country { get; set; }

        /// <summary>
        /// 省/州
        /// </summary>
        [StringLength(100)]
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [StringLength(100)]
        public string City { get; set; }

        /// <summary>
        /// 性别（可选）
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 创建时间（可选）
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新时间（可选）
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}


