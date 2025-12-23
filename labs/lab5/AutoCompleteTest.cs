using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace labs.lab5
{
    [TestFixture]
    public class AutoCompleteTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            
            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [TearDown]
        public void TearDown()
        {
            driver?.Dispose();
        }

        [Test]
        public void AutoCompleteFormTest()
        {
            // 1. Перейти на страницу https://demoqa.com/
            driver.Navigate().GoToUrl("https://demoqa.com/");
            Thread.Sleep(2000);
            
            // 2. Перейти в раздел 'Widgets'
            var widgetsCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Widgets']")));
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", widgetsCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", widgetsCard);
            
            // 3. Выбрать пункт 'Auto Complete'
            var autoCompleteItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-1']//span[text()='Auto Complete']")));
            autoCompleteItem.Click();
            
            Thread.Sleep(1000);
            
            // 4. В поле 'Type multiple color names' выбрать значения: Black, Red, Magenta
            var multipleInput = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("autoCompleteMultipleInput")));
            multipleInput.Click();
            Thread.Sleep(500);
            multipleInput.SendKeys("Black");
            Thread.Sleep(1000);
            multipleInput.SendKeys(OpenQA.Selenium.Keys.Enter);
            Thread.Sleep(500);
            
            multipleInput.Click();
            Thread.Sleep(500);
            multipleInput.SendKeys("Red");
            Thread.Sleep(1000);
            multipleInput.SendKeys(OpenQA.Selenium.Keys.Enter);
            Thread.Sleep(500);
            
            multipleInput.Click();
            Thread.Sleep(500);
            multipleInput.SendKeys("Magenta");
            Thread.Sleep(1000);
            multipleInput.SendKeys(OpenQA.Selenium.Keys.Enter);
            Thread.Sleep(1000);
            
            // 5. В поле 'Type single color name' выбрать значения: Black
            var singleInput = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("autoCompleteSingleInput")));
            singleInput.Click();
            Thread.Sleep(500);
            singleInput.SendKeys("Black");
            Thread.Sleep(1000);
            singleInput.SendKeys(OpenQA.Selenium.Keys.Enter);
            Thread.Sleep(1000);
            
            // 6. В поле 'Type single color name' заменить значения на Red
            singleInput.Click();
            Thread.Sleep(500);
            singleInput.Clear();
            Thread.Sleep(500);
            singleInput.SendKeys("Red");
            Thread.Sleep(1000);
            singleInput.SendKeys(OpenQA.Selenium.Keys.Enter);
            Thread.Sleep(1000);
        }
    }
}


