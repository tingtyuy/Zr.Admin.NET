using Infrastructure.Attribute;
using Infrastructure.Extensions;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto;
using SqlSugar;
using SqlSugar.Extensions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ZR.Model.Business;
using ZR.Model.Business.Dto;
using ZR.Repository;
using ZR.Service.Business.IBusinessService;

namespace ZR.Service.Business
{
    /// <summary>
    /// Service业务层处理
    /// </summary>
    [AppService(ServiceType = typeof(ITbResultService), ServiceLifetime = LifeTime.Transient)]
    public class TbResultService : BaseService<TbResult>, ITbResultService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<TbResultDto> GetList(TbResultQueryDto parm)
        {
            var predicate = QueryExp(parm);

            predicate.AndIF(!string.IsNullOrEmpty(parm.单号), m => m.单号.Contains(parm.单号));
            predicate.AndIF(!string.IsNullOrEmpty(parm.商家名称), m => m.商家名称.Contains(parm.商家名称));
            predicate.AndIF(!string.IsNullOrEmpty(parm.收件人信息), m => m.收件人信息.Contains(parm.收件人信息));
            predicate.AndIF(!string.IsNullOrEmpty(parm.处理状态), m => m.处理状态 == parm.处理状态);
            predicate.AndIF(parm.操作开始时间.HasValue, m => DateTime.Parse(m.操作时间) >= parm.操作开始时间);
            predicate.AndIF(parm.操作结束时间.HasValue, m => DateTime.Parse(m.操作时间) <= parm.操作结束时间);
            predicate.AndIF(!string.IsNullOrEmpty(parm.问题件类别), m => m.问题件类别 == parm.问题件类别);
            predicate.AndIF(!string.IsNullOrEmpty(parm.问题件类型), m => m.问题件类型 == parm.问题件类型);
            //predicate.And(w => DateTime.Parse(w.操作时间) >= DateTime.Now.AddDays(-7));

            var response = Queryable()
                .Where(predicate.ToExpression())
                .OrderByDescending(s => s.操作时间)
                .ToPage<TbResult, TbResultDto>(parm);

            return response;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<PagedInfo<TbResultDistinctDto>> GetDistinctList(TbResultQueryDto parm)
        {
            var predicate = QueryExp(parm);

            //predicate.AndIF(!string.IsNullOrEmpty(parm.单号), m => m.单号.Contains(parm.单号));
            //predicate.AndIF(!string.IsNullOrEmpty(parm.商家名称), m => m.商家名称.Contains(parm.商家名称));
            //predicate.AndIF(!string.IsNullOrEmpty(parm.收件人信息), m => m.收件人信息.Contains(parm.收件人信息));
            //predicate.And( m => m.处理状态 == "0");
            //predicate.AndIF(parm.操作开始时间.HasValue, m => DateTime.Parse(m.操作时间) >= parm.操作开始时间);
            //predicate.AndIF(parm.操作结束时间.HasValue, m => DateTime.Parse(m.操作时间) <= parm.操作结束时间);
            //predicate.AndIF(!string.IsNullOrEmpty(parm.问题件类别), m => m.问题件类别 == parm.问题件类别);
            //predicate.AndIF(!string.IsNullOrEmpty(parm.问题件类型), m => m.问题件类型 == parm.问题件类型);
            //predicate.And(w => DateTime.Parse(w.操作时间) >= DateTime.Now.AddDays(-7));

            var list = Queryable()
                .Where(predicate.ToExpression());

            var groupList = list.GroupBy(g => new { g.商家名称, g.收件人信息, g.CompanyId, g.执行机器人, g.处理状态 }).Select(s => new TbResultDistinctDto
            {
                //ids= SqlFunc.Subqueryable<TbResult>().Where(w=>w.商家名称==),
                商家名称 = s.商家名称,
                收件人信息 = s.收件人信息,
                CompanyId = s.CompanyId,
                执行机器人 = s.执行机器人,
                处理状态 = s.处理状态,
                count = SqlFunc.AggregateCount(s.单号)

            }).OrderBy(o=>o.处理状态);

            var response = groupList.ToPage(parm);
            foreach (var item in response.Result)
            {
                item.ids = GetIds(item.商家名称, item.收件人信息).ToList();
                item.ReplyMessage = await GetForwardMessage(item.商家名称, item.收件人信息);
            }
            return response;
        }


        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TbResult GetInfo(int Id)
        {
            var response = Queryable()
                .Where(x => x.Id == Id)
                .First();

            return response;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public TbResult AddTbResult(TbResult model)
        {
            return Insertable(model).ExecuteReturnEntity();
        }

        /// <summary>
        /// 问题件匹配
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public TbResult Match(TbResult model)
        {
            return Insertable(model).ExecuteReturnEntity();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateTbResult(TbResult model)
        {
            return Update(model, true);
        }
        /// <summary>
        /// 复制信息
        /// </summary>
        /// <param name="idArr"></param>
        /// <returns></returns>
        public int UpdateTbResultStatus(long[] idArr)
        {
            return Update(w => idArr.Contains(w.Id), c => new TbResult() { 处理状态 = "已处理" });

        }
        /// <summary>
        /// 获取转发信息
        /// </summary>
        /// <param name="idArr"></param>
        /// <returns></returns>

        public async Task<ReplyMessageDto> GetForwardMessageResult(long[] idArr)
        {
            ReplyMessageDto resultModel = await GetForwardMessageDto(idArr);
            return resultModel;
        }

        private async Task<string> GetForwardMessage(string name, string phone)
        {
            long[] ids = GetIds(name, phone);
            var dto = await GetForwardMessageDto(ids);
            return dto.ReplyMessage;
        }

        private long[] GetIds(string name, string phone)
        {
            return Queryable().Where(w => w.商家名称 == name && w.收件人信息 == phone).Select(s => s.Id).ToArray();
        }

        private async Task<ReplyMessageDto> GetForwardMessageDto(long[] idArr)
        {
            var resultModel = new ReplyMessageDto();
            var list = Queryable().Where(x => idArr.Contains(x.Id));
            var ll = list.ToList();
            var first = await list.FirstAsync();

            resultModel.BussinessName = first.商家名称;
            resultModel.SendUser = first.收件人信息;


            var replyMessageList = ll.GroupBy(g => g.反馈信息).Select(s => new ReplyMessage
            {
                Message = s.Key,
                OrderNo = s.ToList().Select(s => s.单号).ToList()
            }

            ).ToList();
            resultModel.ReplyMessageList.AddRange(replyMessageList);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{first.商家名称} {first.收件人信息}");
            foreach (var item in replyMessageList)
            {
                stringBuilder.AppendLine(item.Message);
                foreach (var id in item.OrderNo)
                {
                    stringBuilder.AppendLine(id);
                }
            }
            resultModel.ReplyMessage = stringBuilder.ToString();
            return resultModel;
        }

        /// <summary>
        /// 查询导出表达式
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        private static Expressionable<TbResult> QueryExp(TbResultQueryDto parm)
        {
            var predicate = Expressionable.Create<TbResult>();

            return predicate;
        }
    }
}