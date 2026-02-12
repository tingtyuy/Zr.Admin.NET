using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;
using ZR.Service.IService;

namespace ZR.Service
{
    public class PlayWrightService : IPlayWrightService
    {
        public async Task<bool> IsLoggedInAsync(string storageStatePath = "auth.json", string testUrl = "", string loggedInSelector = "")
        {
            // 快速判断：storage 文件是否存在且大小合理
            if (string.IsNullOrWhiteSpace(storageStatePath) || !File.Exists(storageStatePath))
                return false;

            var fi = new FileInfo(storageStatePath);
            if (fi.Length < 10) // 简单阈值，避免空文件误判
                return false;

            // 如果未提供 selector 和 testUrl，认为有 storage 即已登录
            if (string.IsNullOrWhiteSpace(testUrl) || string.IsNullOrWhiteSpace(loggedInSelector))
            {
                return true;
            }

            // 使用 Playwright 在带有 storage 的上下文中打开页面，检查 selector 是否存在
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                StorageStatePath = storageStatePath
            });

            var page = await context.NewPageAsync();

            try
            {
                await page.GotoAsync(testUrl);
                // 等待短时间以确认已登录元素出现
                var element = await page.WaitForSelectorAsync(loggedInSelector, new PageWaitForSelectorOptions
                {
                    Timeout = 3000
                });
                return element != null;
            }
            catch (TimeoutException)
            {
                return false;
            }
            catch (PlaywrightException)
            {
                return false;
            }
            finally
            {
                await context.CloseAsync();
            }
        }
    }
}