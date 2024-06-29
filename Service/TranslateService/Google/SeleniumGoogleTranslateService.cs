using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TranslationPOBot.Services.Contracts;

namespace TranslationPOBot.Services.TranslateServices.Google
{
    public class SeleniumGoogleTranslateService : ITranslateService
    {
        private readonly IWebDriver driver;
        public SeleniumGoogleTranslateService()
        {
            driver = new ChromeDriver();

            driver.Navigate().GoToUrl("https://translate.google.com/?hl=en&sl=auto&tl=uk&op=translate");
            Thread.Sleep(3000);
        }
        ~SeleniumGoogleTranslateService()
        {

            try
            {

                driver.Quit();
                driver.Dispose();
            }
            catch (Exception ex) { }

        }

        public string Translate(string text)
        {

            try
            {

                var textArea = driver.FindElements(By.TagName("textarea"))[0];
                textArea.Clear();
                textArea.SendKeys(text);
                Thread.Sleep(new Random().Next(10000, 12000));
                // Access deeply nested element's textContent 
                string xpath = "//*[contains(text(), 'Ukrainian')]";

                // Find element using XPath
                IWebElement element = driver.FindElement(By.XPath(xpath));

                // Construct the JavaScript to access deeply nested element's textContent
                string script = @"
                var element = arguments[0];
                return element.parentElement.parentElement.parentElement.parentElement.parentElement
                    .children[1].children[1].children[1].children[1].children[6]
                    .children[0].children[0].children[0].children[0].children[0].textContent;";

                // Execute JavaScript to get textContent of deeply nested element
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                string textContent = (string)jsExecutor.ExecuteScript(script, element);
                //var response = client.TranslateText(
                //    new TranslateTextRequest
                //    {
                //        Contents = { text },
                //        TargetLanguageCode = targetLanguage,
                //        Parent = $"projects/{projectId}"
                //    });

                //return response.Translations[0].TranslatedText;
                return $"msgstr {textContent}";
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
