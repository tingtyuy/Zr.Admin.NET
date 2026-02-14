using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using ZR.Service;
using ZR.Service.IService;

namespace ZR.ConsoleApp
{
    public class PlayWrightDemo
    {
        public async static Task Run()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
            {
                Channel = "msedge",
                Headless = false,
            });
            var storagePath = "auth.json";
            var context = await browser.NewContextAsync(new BrowserNewContextOptions() { StorageStatePath = storagePath });

            await context.StorageStateAsync(new BrowserContextStorageStateOptions
            {
                Path = storagePath
            });
            var page = await context.NewPageAsync();
            await page.GotoAsync("https://www.doubao.com/chat");

            await IsLoginIn(page);



            //// 关闭当前上下文并使用保存的 storage 重新创建上下文以确保后续页面是已登录状态
            //await context.CloseAsync();
            //context = await browser.NewContextAsync(new BrowserNewContextOptions
            //{
            //    StorageStatePath = storagePath
            //});

            //page = await context.NewPageAsync();
            //await page.GotoAsync("https://www.doubao.com/chat");

            //Console.WriteLine("已检测到登录状态，继续执行...");


            //// 已登录，继续后续操作
            //await page.GetByText("新对话").ClickAsync();
            //await page.GetByTestId("chat_input_input").ClickAsync();
            //await page.GetByTestId("chat_input_input").FillAsync("你好");
            //await page.GetByTestId("chat_input_send_button").ClickAsync();

            //Console.WriteLine("\n按任意键退出...");
            //Console.ReadKey();

            //// 清理
            //await context.CloseAsync();
        }

        private static async Task IsLoginIn(IPage page)
        {
            var loginStatusElement = await page.WaitForSelectorAsync("data-testid=to_login_button", new PageWaitForSelectorOptions { Timeout = 3000 });
            if (loginStatusElement != null)
            {
                Console.WriteLine("请先登录!然后按y继续");
                if (Console.ReadKey().KeyChar != 'y')
                {
                    Console.WriteLine("操作错误,程序退出!!");

                }
                else
                {
                    await IsLoginIn(page);

                }

            }
            Console.WriteLine("已经成功进入主页面!");
        }
    }
}
