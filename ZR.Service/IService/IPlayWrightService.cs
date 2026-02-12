using System.Threading.Tasks;

namespace ZR.Service.IService
{
    public interface IPlayWrightService
    {
        /// <summary>
        /// 判断当前是否已登录。
        /// - 优先检查 storageStatePath 文件是否存在且非空；
        /// - 若提供了 testUrl 和 loggedInSelector，会基于 storageStatePath 新建上下文并打开 testUrl，等待 loggedInSelector 出现以确认登录。
        /// </summary>
        /// <param name="storageStatePath">Playwright StorageState 文件路径，默认 "auth.json"</param>
        /// <param name="testUrl">用于检测登录状态的页面 URL（可选）</param>
        /// <param name="loggedInSelector">页面中代表已登录的元素选择器（可选）</param>
        Task<bool> IsLoggedInAsync(string storageStatePath = "auth.json", string testUrl = "", string loggedInSelector = "");
    }
}
