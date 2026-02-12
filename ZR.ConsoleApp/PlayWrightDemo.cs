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
            await using var browser = await playwright.Chromium.LaunchAsync(new()
            {
                Channel = "msedge",
                Headless = false,
            });
            // 加载已保存的登录状态
            var context = await browser.NewContextAsync();

            //检测时候登录
            IPlayWrightService playWrightService = new PlayWrightService();
            bool isLoggedIn = await playWrightService.IsLoggedInAsync("auth.json");

            if (!isLoggedIn)
            {

                // 如果未登录，提示用户登录并保存状态
                Console.WriteLine("请先登录到 https://www.doubao.com/chat，然后按任意键继续...");
                Console.ReadKey();

            }
            // 保存当前登录状态到文件
            await context.StorageStateAsync(new()
            {
                Path = "auth.json"
            });
            // 重新创建上下文以加载新的登录状态
            context = await browser.NewContextAsync(new()
            {
                StorageStatePath = "auth.json"
            });

            var page = await context.NewPageAsync();
            await page.GotoAsync("https://www.doubao.com/chat");
            await page.GetByText("新对话").ClickAsync();
            await page.GetByTestId("chat_input_input").ClickAsync();
            await page.GetByTestId("chat_input_input").FillAsync("你好");
            await page.GetByTestId("chat_input_send_button").ClickAsync();

            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();









        }



    }
}
