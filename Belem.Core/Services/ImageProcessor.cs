using System.Text;
using System.Text.RegularExpressions;
using Tesseract;

namespace Belem.Core.Services
{
    public class ImageProcessor
    {

        public (TimeSpan buyTime, TimeSpan sellTime, string token) GetTradeInfo(string imagePath)
        {
            var testImagePath = imagePath;
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.LstmOnly))
            {
                using (var img = Pix.LoadFromFile(testImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        return GetBuyAndSellTimes(text);
                    }
                }
            }
        }

        public (TimeSpan buyTime, TimeSpan sellTime, string token) GetBuyAndSellTimes(string text)
        {
            var pattern = "[a-zA-Z]+\\s(0?[0-9]|1[0-9]|2[0-3]):[0-9]+\\s(0?[0-9]|1[0-9]|2[0-3]):[0-9]+";
            var matches = Regex.Matches(text.ToLower(), pattern, RegexOptions.IgnoreCase);
            if (!matches.Any())
            {
                throw new Exception("Can not detect signal man! common!");
            }

            var targetLine = matches.Last().Value;


            var token = targetLine.Substring(0, 3);
            var buyTime = TimeSpan.Parse(targetLine.Substring(4, 5));
            var sellTime = TimeSpan.Parse(targetLine.Substring(10, 5));

            return (buyTime, sellTime, token);
        }

    }
}
