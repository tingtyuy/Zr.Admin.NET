using Infrastructure.Helper;
using ZR.Common;

namespace ZR.WinFormsApp
{
  
    internal static class Program
    {
        
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {

                new LogHelper().Logger.Error("程序运行时发生异常", ex.Message);
            }

        }
    }
}