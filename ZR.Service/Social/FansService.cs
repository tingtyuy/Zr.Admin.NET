using Infrastructure;
using Infrastructure.Attribute;
using Infrastructure.Extensions;
using Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using ZR.Model.social;
using ZR.Service.Social.IService;

namespace ZR.Service.Social
{
    /// <summary>
    /// 粉丝
    /// </summary>
    [AppService(ServiceType = typeof(IFansService), ServiceLifetime = LifeTime.Transient)]
    public class FansService : BaseService<Fans>, IFansService, IDynamicApi
    {
        /// <summary>
        /// 关注
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult Follow([FromBody] FansDto dto)
        {
            if (dto.Userid == dto.ToUserid)
            {
                throw new CustomException("不能关注自己");
            };
            //TODO 判断用户是否存在

            var isFollow = GetFirst(x => x.Userid == dto.Userid && x.ToUserid == dto.ToUserid);
            if (isFollow != null)
            {
                throw new CustomException("已关注");
            }
            var fans = new Fans
            {
                Userid = dto.Userid,
                ToUserid = dto.ToUserid,
                FollowTime = DateTime.Now,
                UserIP = HttpContextExtension.GetClientUserIp(App.HttpContext)
            };
            var result = UseTran2(() =>
            {
                //添加粉丝
                InsertReturnSnowflakeId(fans);

                UpdateFollowInfo(dto.Userid, isFollowNum: true);
                UpdateFollowInfo(dto.ToUserid, isFollowNum: false);
            });

            if (!result)
            {
                return ApiResult.Error("关注失败");
            }
            var data = Context.Queryable<FansInfo>()
                    .Where(x => x.Userid == dto.Userid)
                    .First();
            return ApiResult.Success("关注成功", data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult CancelFollow([FromBody] FansDto dto)
        {
            return ApiResult.Success("取消关注成功", null);
        }

        private void UpdateFollowInfo(int userId, bool isFollowNum)
        {
            var count = Context.Updateable<FansInfo>()
                .SetColumns(x => isFollowNum
                    ? new FansInfo { FollowNum = x.FollowNum + 1 }
                    : new FansInfo { FansNum = x.FansNum + 1 })
                .Where(x => x.Userid == userId)
                .ExecuteCommand();

            if (count > 0) return;

            Context.Insertable(new FansInfo
            {
                Userid = userId,
                FollowNum = isFollowNum ? 1 : 0,
                FansNum = isFollowNum ? 0 : 1,
                UpdateTime = DateTime.Now
            }).ExecuteCommand();
        }
    }
}
