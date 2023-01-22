using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;

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

    public static void CheckPendingRequests(this IWebDriver driver, int timeoutInSeconds)
    {

        try
        {
            if (driver is IJavaScriptExecutor)
            {
                IJavaScriptExecutor jsDriver = (IJavaScriptExecutor)driver;

                for (int i = 0; i < timeoutInSeconds; i++)
                {
                    Object numberOfAjaxConnections = jsDriver.ExecuteScript("return window.openHTTPs");
                    // return should be a number
                    if (numberOfAjaxConnections is long)
                    {
                        long n = (long)numberOfAjaxConnections;
                        Console.WriteLine("Number of active calls: " + n);

                        if (n == 0L) break;

                        else
                        {
                            // If it's not a number, the page might have been freshly loaded indicating the monkey
                            // patch is replaced or we haven't yet done the patch.
                            MonkeyPatchXMLHttpRequest(driver);
                        }
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        MonkeyPatchXMLHttpRequest(driver);

                    }

                }
            }
            else
            {
                Console.WriteLine("Web driver: " + driver + " cannot execute javascript");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }



    public static void MonkeyPatchXMLHttpRequest(this IWebDriver driver)
    {
        try
        {
            if (driver is IJavaScriptExecutor)
            {
                IJavaScriptExecutor jsDriver = (IJavaScriptExecutor)driver;
                Object numberOfAjaxConnections = jsDriver.ExecuteScript("return window.openHTTPs");
                if (numberOfAjaxConnections is long)
                {
                    return;
                }
                String script = "  (function() {" +
                    "var oldOpen = XMLHttpRequest.prototype.open;" +
                    "window.openHTTPs = 0;" +
                    "XMLHttpRequest.prototype.open = function(method, url, async, user, pass) {" +
                    "window.openHTTPs++;" +
                    "this.addEventListener('readystatechange', function() {" +
                    "if(this.readyState == 4) {" +
                    "window.openHTTPs--;" +
                    "}" +
                    "}, false);" +
                    "oldOpen.call(this, method, url, async, user, pass);" +
                    "}" +
                    "})();";
                jsDriver.ExecuteScript(script);
            }
            else
            {
                Console.WriteLine("Web driver: " + driver + " cannot execute javascript");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}