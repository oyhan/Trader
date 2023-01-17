using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belem.Core
{
    public static class ShellHelper
    {
        public async static Task<string> Bash(this string cmd)
        {
            var process = GetProcess(cmd);
            process.Start();
            string result = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();
            return result;
        }

        private static Process GetProcess(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            return new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
        }
    }

}
public static class WebDriverExtensions
{
    public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
    {
        if (timeoutInSeconds > 0)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            return wait.Until(drv => drv.FindElement(by));
        }
        return driver.FindElement(by);
    }
}