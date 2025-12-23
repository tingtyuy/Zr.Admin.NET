using Aliyun.OSS;
using Azure;
using Infrastructure;
using Infrastructure.Attribute;
using Infrastructure.Extensions;
using Infrastructure.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using SqlSugar.IOC;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZR.Common;
using ZR.Service.Business.IBusinessService;
using ZR.ServiceCore.Services;

namespace ZR.Tasks.TaskScheduler
{
    [AppService(ServiceType = typeof(Job_SqlExecute_EmailToCustomer), ServiceLifetime = LifeTime.Scoped)]
    public class Job_SqlExecute_EmailToCustomer : JobBase, IJob
    {
        private readonly ISysTasksQzService tasksQzService;
        private OptionsSetting OptionsSetting;
        private readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly ICompanyService _CompanyService;

        public Job_SqlExecute_EmailToCustomer(ISysTasksQzService tasksQzService, IOptions<OptionsSetting> options, ICompanyService companyService)
        {
            this.tasksQzService = tasksQzService;
            OptionsSetting = options.Value;
            _CompanyService = companyService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await ExecuteJob(context, async () => await Run(context));
        }
        public async Task Run(IJobExecutionContext context)
        {
            MailHelper mailHelper = new(OptionsSetting.MailOptions.FirstOrDefault(w => w.FromName == "客服自动化管理系统管理员"));
            var allCompany = _CompanyService.GetAll();
            var toUsers = allCompany.Where(w => w.OperationEmailTo.IsNotEmpty()).Select(s => s.OperationEmailTo).ToArray();
            string result = mailHelper.SendMail(toUsers, "程序停止", "请人工处理");
            logger.Info($"任务执行结果=" + result);

        }
    }
}
