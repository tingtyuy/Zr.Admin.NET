using Aliyun.OSS;
using Infrastructure.Attribute;
using Infrastructure.Extensions;
using System.Text;
using System.Text.RegularExpressions;
using ZR.Model.Business;
using ZR.Model.Business.Dto;
using ZR.Model.System;
using ZR.Repository;
using ZR.Service.Business.IBusinessService;
using ZR.ServiceCore.Services;
using ZR.Common;
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
        /// 查询列表2
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<TbContactDto> GetList2(TbContactQueryDto parm)
        {
            var predicate = QueryExp(parm);
            predicate.And(w => !string.IsNullOrEmpty(w.客户商家名称));
            //predicate.And(w => w.IsEnable == false || w.IsEnable == null);
            //parm.Sort = "群名称";

            //var list = Queryable().Includes(a => a.TbWxGroupMembers).Where(predicate.ToExpression()).OrderBy(o=>o.客户).OrderBy(o=>o.客户商家名称);
            var list = Queryable()
                .Where(predicate.ToExpression())
                .LeftJoin<TbContact>((a, b) => a.群名称 == b.群名称 && string.IsNullOrEmpty(b.客户商家名称) && b.CompanyId==parm.CompanyId)
                .Select((a, b) => new TbContact
                {
                    客户 = a.客户,
                    客户商家名称 = a.客户商家名称,
                    群名称 = a.群名称,
                    IsEnable = b.IsEnable,
                    Id=a.Id
                },true)
                .OrderByDescending(a =>a.匹配时间);

            var response = list.ToPage<TbContact, TbContactDto>(parm);
            response.Result = response.Result

           .ToList();

            //foreach (var result in response.Result)
            //{
            //    if (!string.IsNullOrEmpty(result.MatchParam))
            //    {
            //        var arr = result.MatchParam.Split(',');
            //        var num = 1;
            //        foreach (var item in arr)
            //        {
            //            result.MatchParamDes += num + ". " + _sysDictDataService.GetSingle(w => w.DictType == "wx_group_match_param" && w.DictValue == item).DictLabel + " ";
            //            num++;
            //        }
            //    }
            //}

            return response;
        }


        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<TbContactDto> GetList(TbContactQueryDto parm)
        {
            var predicate = QueryExp(parm);
            predicate.And(w => string.IsNullOrEmpty(w.客户商家名称));
            predicate.And(w => w.IsEnable == false || w.IsEnable == null);
            //parm.Sort = "群名称";

            var list = Queryable().Includes(a => a.TbWxGroupMembers).Where(predicate.ToExpression()).OrderBy("CONVERT(`群名称` USING gbk)");

            var response = list.ToPage<TbContact, TbContactDto>(parm);
            response.Result = response.Result

           .ToList();

            foreach (var result in response.Result)
            {
                if (!string.IsNullOrEmpty(result.MatchParam))
                {
                    var arr = result.MatchParam.Split(',');
                    var num = 1;
                    foreach (var item in arr)
                    {
                        result.MatchParamDes += num + ". " + _sysDictDataService.GetSingle(w => w.DictType == "wx_group_match_param" && w.DictValue == item).DictLabel + " ";
                        num++;
                    }
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
            predicate.AndIF(!string.IsNullOrEmpty(parm.客户), m => m.客户.Contains(parm.客户));
            predicate.AndIF(!string.IsNullOrEmpty(parm.客户商家名称), m => m.客户商家名称.Contains(parm.客户商家名称));
            predicate.AndIF(parm.IsEnable.HasValue, m => m.IsEnable == parm.IsEnable);
            predicate.AndIF(parm.IsMatch.HasValue, m => m.IsMatch == parm.IsMatch);
            predicate.And(w => w.CompanyId == parm.CompanyId);
            return predicate;
        }
    }
}