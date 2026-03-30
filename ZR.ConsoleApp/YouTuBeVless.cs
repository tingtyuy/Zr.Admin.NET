using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace ZR.ConsoleApp
{
    /// <summary>
    /// 通过视频分享免费的vless
    /// </summary>
    public class YouTuBeVless
    {
        //流程开始
        public static async Task RunAsync()
        {

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new()
            {
                Channel = "msedge",
                Headless = false,
            });
            var context = await browser.NewContextAsync();

            var page = await context.NewPageAsync();

            //访问视频
            await page.GotoAsync("https://www.youtube.com/@ZYFXS/videos");
            var firstVideo = page.Locator("#thumbnail").Nth(1);
            var videoLink = await firstVideo.EvaluateAsync<string>("el => el.href");
            await firstVideo.ClickAsync();
            //获取加密链接
            await page.GetByRole(AriaRole.Button, new() { Name = "更多" }).ClickAsync();
           var linkLocator=  page.Locator("xpath=//div[@id='expanded']//a[contains(@href, 'paste.to')]");
            var linkInfo = await linkLocator.EvaluateAsync<dynamic>(@"el => ({
                url: el.href,
                text: el.textContent,
                href: el.getAttribute('href')
            })");
            var linkUrl= linkInfo.url;
            Console.WriteLine($"加密链接:{linkUrl}");

            //获取密码
            await page.GotoAsync("https://www.youtube-transcript.io/");
            await page.GetByRole(AriaRole.Textbox, new() { Name = "Paste your Youtube video link" }).First.FillAsync(videoLink);
            await page.GetByRole(AriaRole.Button, new() { Name = "Extract transcript" }).First.ClickAsync();
            var linePassword = "1315";
            Console.WriteLine($"加密链接密码:{linePassword}");

            //打开加密的Vless链接地址,进行解密获取最终的Vless链接
            await page.GotoAsync(linkUrl);
            await page.GetByRole(AriaRole.Textbox, new() { Name = "请输入这份粘贴内容的密码：" }).FillAsync(linePassword);
            await page.GetByRole(AriaRole.Button, new() { Name = "解密" }).ClickAsync();

            var linkLocator2 = page.Locator("xpath=//*[@id='prettyprint']/a[1]");
            var fullUrl = await linkLocator2.EvaluateAsync<string>("el => el.href");
            Console.WriteLine($"vLESS URL: {fullUrl}");

            Console.ReadKey();

        }


    }
}

