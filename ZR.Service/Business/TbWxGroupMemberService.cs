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
    [AppService(ServiceType = typeof(ITbWxGroupMemberService), ServiceLifetime = LifeTime.Transient)]
    public class TbWxGroupMemberService : BaseService<TbWxGroupMember>, ITbWxGroupMemberService
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<TbWxGroupMemberDto> GetList(TbWxGroupMemberQueryDto parm)
        {
            var predicate = QueryExp(parm);

            var response = Queryable()
                .Where(predicate.ToExpression())
                .ToPage<TbWxGroupMember, TbWxGroupMemberDto>(parm);

            return response;
        }


        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TbWxGroupMember GetInfo(int Id)
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
        public TbWxGroupMember AddTbWxGroupMember(TbWxGroupMember model)
        {
            return Insertable(model).ExecuteReturnEntity();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateTbWxGroupMember(TbWxGroupMember model)
        {
            return Update(model, true);
        }

        /// <summary>
        /// 查询导出表达式
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        private static Expressionable<TbWxGroupMember> QueryExp(TbWxGroupMemberQueryDto parm)
        {
            var predicate = Expressionable.Create<TbWxGroupMember>();

            return predicate;
        }
    }
}