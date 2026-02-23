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
            await using var browser = await playwright.Chromium.LaunchAsync(new()
            {
                Channel = "msedge",
                Headless = false,
            });

            var storagePath = "auth.json";
            var context = await browser.NewContextAsync(new BrowserNewContextOptions() { StorageStatePath = storagePath });

             context = await OpenPage(browser, storagePath, context, "https://www.doubao.com/chat", "豆包", async page => { try { return await page.GetByTestId("to_login_button").IsVisibleAsync(); } catch { return false; } });




            context = await OpenPage(browser, storagePath, context, "https://chat.deepseek.com", "deepseek", async page => { try { return await page.GetByRole(AriaRole.Button, new() { Name = "登录" }).IsVisibleAsync(); } catch { return false; } });
            Console.ReadKey();








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

        private static async Task<IBrowserContext> OpenPage(IBrowser browser, string storagePath, IBrowserContext context, string websiteUrl, string websiteName, Func<IPage, Task<bool>> isLoginCheck)
        {
            var page = await context.NewPageAsync();
            await page.GotoAsync(websiteUrl);
            Thread.Sleep(3000); // 等待页面加载完成，实际使用中可以根据需要调整等待时间或使用更智能的等待方式
                                //await page.GotoAsync(websiteUrl, new PageGotoOptions
                                //{
                                //    WaitUntil = WaitUntilState.Load, // 等待网络空闲
                                //    //LoadState.DOMContentLoaded：DOM 解析完成（最快）
                                //    //LoadState.Load：页面完全加载（包括样式、图片等资源）
                                //    //LoadState.NetworkIdle：网络空闲（500ms 内没有网络请求）

            //    Timeout = 6000 // 30秒超时
            //});

            //判断登录状态
            var newContext = await IsLoginIn(browser, storagePath, context, page, isLoginCheck, websiteName);
            // 重新创建页面以确保使用新的上下文
            var newPage = await newContext.NewPageAsync();
            await newPage.GotoAsync(websiteUrl);
            //await page.GetByTestId("create_conversation_button").GetByText("新对话").ClickAsync();
            //await page.GetByTestId("chat_input_input").ClickAsync();
            //await page.GetByTestId("chat_input_input").FillAsync("你好");

            //await page.GetByText("开启新对话").ClickAsync();
            //await page.GetByRole(AriaRole.Textbox, new() { Name = "给 DeepSeek 发送消息" }).ClickAsync();
            //await page.GetByRole(AriaRole.Textbox, new() { Name = "给 DeepSeek 发送消息" }).FillAsync("你好");

            return newContext;
        }

        private static async Task<IBrowserContext> IsLoginIn(IBrowser browser, string storagePath, IBrowserContext context, IPage page, Func<IPage, Task<bool>> isLoginCheck, string websiteName)
        {

            //var loginButtion = await page.QuerySelectorAsync("data-testid=to_login_button");

            //var loginStatusElement = await doubaoPage.WaitForSelectorAsync("data-testid=to_login_button", new PageWaitForSelectorOptions { Timeout = 3000 });

            // 使用传入的回调判断是否存在登录按钮
            var hasLoginId = await isLoginCheck(page);
            IBrowserContext? newContext = null;
            if (!hasLoginId)
            {
                Console.WriteLine($"{websiteName},成功打开!");
                await context.StorageStateAsync(new BrowserContextStorageStateOptions
                {
                    Path = storagePath
                });
                await context.CloseAsync();
                newContext = await browser.NewContextAsync(new BrowserNewContextOptions
                {
                    StorageStatePath = storagePath
                });
                Console.WriteLine($"{websiteName},已存储登录状态!");
            }
            else
            {

                newContext= await RequiredLoginAsync(browser, storagePath, context, page, isLoginCheck, websiteName);

            }
            if (newContext is null)
            {

                return context;
            }
            else
            {
                return newContext;
            }
        }

        private static async Task<IBrowserContext> RequiredLoginAsync(IBrowser browser, string storagePath, IBrowserContext context, IPage doubaoPage, Func<IPage, Task<bool>> isLoginCheck, string websiteName)
        {
            Console.WriteLine($"请先登录,{websiteName}!!! 登陆后按y键继续");
            if (Console.ReadKey().KeyChar == 'y')
            {
                Console.WriteLine("用户已确认登录,重新验证");
               return await IsLoginIn(browser, storagePath, context, doubaoPage, isLoginCheck, websiteName);

            }
            else
            {
                return await RequiredLoginAsync(browser, storagePath, context, doubaoPage, isLoginCheck, websiteName);
            }
        }

        private async Task Demo()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new()
            {
                Channel = "msedge",
                Headless = false,
            });
            var context = await browser.NewContextAsync();

            var page = await context.NewPageAsync();
            await page.GotoAsync("https://www.doubao.com/chat");

            Console.ReadKey();
        }


    }
}
