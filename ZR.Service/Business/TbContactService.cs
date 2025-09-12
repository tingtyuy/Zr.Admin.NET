using Aliyun.OSS;
using Infrastructure.Attribute;
using Infrastructure.Extensions;
using System.Text.RegularExpressions;
using ZR.Model.Business;
using ZR.Model.Business.Dto;
using ZR.Model.System;
using ZR.Repository;
using ZR.Service.Business.IBusinessService;
using ZR.ServiceCore.Services;

namespace ZR.Service.Business
{
    /// <summary>
    /// Service业务层处理
    /// </summary>
    [AppService(ServiceType = typeof(ITbContactService), ServiceLifetime = LifeTime.Transient)]
    public class TbContactService : BaseService<TbContact>, ITbContactService
    {
        private readonly ISysDictDataService _sysDictDataService;
        public TbContactService(ISysDictDataService sysDictDataService)
        {
            _sysDictDataService = sysDictDataService;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<TbContactDto> GetList(TbContactQueryDto parm)
        {
            var predicate = QueryExp(parm);

            // 1. 先查询每个 CompanyId 的最小 Id
            var minIds = Queryable()
                .Where(predicate.ToExpression())
                .GroupBy(g => new { g.CompanyId, g.群名称 })
                .Select(g => new
                {
                    CompanyId = g.CompanyId,
                    群名称 = g.群名称,
                    MinId = SqlFunc.AggregateMin(g.Id)
                })
                .ToList();

            // 2. 根据最小 Id 查询完整记录（包括导航属性）
            var list = Queryable()
                .Includes(a => a.TbWxGroupMembers) // 正确位置：在 Select 之前
                .Where(t => minIds.Select(x => x.MinId).Contains(t.Id))
                ;

            var response = list.ToPage<TbContact, TbContactDto>(parm);
            foreach (var result in response.Result)
            {
                if (!string.IsNullOrEmpty(result.MatchParam))
                {
                    var arr = result.MatchParam.Split(',');
                    var num = 1;
                    foreach (var item in arr)
                    {

                        result.MatchParamDes += num+ ". "+ _sysDictDataService.GetSingle(w => w.DictType == "wx_group_match_param" && w.DictValue == item).DictLabel+" ";
                        num++;
                    }
                    //result.MatchParamDes= result.MatchParamDes.Remove(result.MatchParamDes.Length - 1, 1);
                }
            }


            return response;
        }


        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TbContact GetInfo(int Id)
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
        public TbContact AddTbContact(TbContact model)
        {
            return Insertable(model).ExecuteReturnEntity();
        }



        //            /// <summary>
        ///// 修改
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public int MatchTbContact(TbContact model)
        //{
        //    if (model.TbWxGroupMembers is not null && model.TbWxGroupMembers.Any())
        //    {
        //        Update(w=>)
        //        Queryable<TbWxGroupMember>().Where(m => m.ContactId == model.Id).ToDelete();
        //    }
        //    return Update(model, true);
        //}

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateTbContact(TbContact model)
        {
            return Update(model, true);
        }

        /// <summary>
        /// 查询导出表达式
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        private static Expressionable<TbContact> QueryExp(TbContactQueryDto parm)
        {
            var predicate = Expressionable.Create<TbContact>();
            predicate.AndIF(!string.IsNullOrEmpty(parm.群名称), m => m.群名称.Contains(parm.群名称));
            predicate.AndIF(parm.IsMatch.HasValue, m => m.IsMatch == parm.IsMatch);
            return predicate;
        }
    }
}