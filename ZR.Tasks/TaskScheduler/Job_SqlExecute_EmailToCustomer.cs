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
    [AppService(ServiceType = typeof(Job_SqlExecute_EmailToCustomer), ServiceLifetime = LifeTime.Singleton)]
    public class Job_SqlExecute_EmailToCustomer : JobBase, IJob
    {
        private readonly ISysTasksQzService tasksQzService;
        private OptionsSetting OptionsSetting;
        private readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly ICompanyService _CompanyService;
        private readonly ITbOrderService _TbOrderService;
        public Job_SqlExecute_EmailToCustomer(ISysTasksQzService tasksQzService, IOptions<OptionsSetting> options, ICompanyService companyService, ITbOrderService tbOrderService)
        {
            this.tasksQzService = tasksQzService;
            OptionsSetting = options.Value;
            _CompanyService = companyService;
            _TbOrderService = tbOrderService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await ExecuteJob(context, async () => await Run(context));
        }
        public async Task Run(IJobExecutionContext context)
        {
            MailHelper mailHelper = new(OptionsSetting.MailOptions.FirstOrDefault(w => w.FromName == "客服自动化管理系统管理员"));
            var allCompany = _CompanyService.GetList(a => !string.IsNullOrEmpty(a.OperationEmailCC));
            foreach (var company in allCompany)
            {
                var order = _TbOrderService.AsQueryable().Where(w => w.CompanyId == company.CompanyId).OrderByDescending(o => o.useTime).First();
                if (order is not null)
                {
                    TimeSpan ts = DateTime.Now - order.useTime;
                    if (ts.TotalMinutes > 60)
                    {
                        string result = mailHelper.SendMail(company.OperationEmailTo, $"问题件机器人未启动_{DateTime.Now.ToString("yyyy-MM-dd")}", $"{company.CompanyName}_{order.读取机器人}_问题件机器人未启动,检测时间为{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", cc: company.OperationEmailCC);
                        logger.Info($"任务执行结果=" + result);
                    }
                }
            }
            var toUsers = allCompany.Where(w => w.OperationEmailTo.IsNotEmpty()).Select(s => s.OperationEmailTo).ToArray();
        }
    }
}
