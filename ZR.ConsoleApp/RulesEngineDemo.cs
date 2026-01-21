using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Text;
using RulesEngine;
using System.Threading.Tasks; // 确保引用了正确的命名空间
namespace ZR.ConsoleApp
{
    public class RulesEngineDemo
    {
        public async static Task Run()
        {
            Console.WriteLine("RulesEngineDemo running...");
            // Implementation of the rules engine demo goes here.

            // 修复 CS0103: 定义 workflow 变量
            var workflow = new Workflow[] { 
                new Workflow
                {
                    WorkflowName = "DemoWorkflow",
                    Rules = new List<Rule>
                    {
                        new Rule
                        {
                            RuleName = "AlwaysTrue",
                            Expression = "true"
                        }
                    }
                }
            };
        
            // 修复 CS0118: 明确指定类型为 RulesEngine.RulesEngine
            var rulesEngine = new RulesEngine.RulesEngine(workflow);


           await rulesEngine.ExecuteActionWorkflowAsync("DemoWorkflow", "AlwaysTrue", null);

        }
    }
}
