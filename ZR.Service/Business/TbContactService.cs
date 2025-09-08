using Infrastructure.Attribute;
using Infrastructure.Extensions;
using ZR.Model.Business.Dto;
using ZR.Model.Business;
using ZR.Repository;
using ZR.Service.Business.IBusinessService;

namespace ZR.Service.Business
{
    /// <summary>
    /// Service业务层处理
    /// </summary>
    [AppService(ServiceType = typeof(ITbContactService), ServiceLifetime = LifeTime.Transient)]
    public class TbContactService : BaseService<TbContact>, ITbContactService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<TbContactDto> GetList(TbContactQueryDto parm)
        {
            var predicate = QueryExp(parm);

            var response = Queryable()
                .Where(predicate.ToExpression())
                .ToPage<TbContact, TbContactDto>(parm);

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
            predicate.AndIF(!string.IsNullOrEmpty(parm.客户), m => m.客户.Contains(parm.客户));
            predicate.AndIF(!string.IsNullOrEmpty(parm.客户商家名称), m => m.客户商家名称.Contains(parm.客户商家名称));
            predicate.AndIF(!string.IsNullOrEmpty(parm.群名称), m => m.群名称.Contains(parm.群名称));

            return predicate;
        }
    }
}