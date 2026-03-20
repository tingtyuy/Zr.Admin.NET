using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZR.ServiceCore.Services;


namespace ZR.ConsoleApp
{
    public class OllamaDemo
    {
        public async static Task Run()
        {
            // 1. 读取配置
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // 2. 设置依赖注入
            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(config);
            services.AddLogging(builder => builder.AddConsole());
            services.AddScoped<IOllamaService, OllamaService>();

            var serviceProvider = services.BuildServiceProvider();

            // 3. 获取服务
            var ollamaService = serviceProvider.GetRequiredService<IOllamaService>();

            // 4. 开始对话
            Console.WriteLine("开始对话 (输入 quit 退出)\n");

            while (true)
            {
                Console.Write("你: ");
                var input = Console.ReadLine();

                if (input?.ToLower() == "quit") break;
                if (string.IsNullOrWhiteSpace(input)) continue;

                Console.Write("AI: ");
               var replyMsg= await ollamaService.SendMessageAsync(input);
                Console.WriteLine($"{replyMsg }\n");
            }
            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }
    }
}
