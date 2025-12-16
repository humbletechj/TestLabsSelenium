using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace labs.lab3
{
    [TestFixture]
    public class DynamicPropertiesTest
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
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        [TearDown]
        public void TearDown()
        {
            driver?.Dispose();
        }

        [Test]
        public void DynamicPropertiesFormTest()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            Thread.Sleep(2000);

            var elementsCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Elements']")));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elementsCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", elementsCard);

            var dynamicPropsItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-8']//span[text()='Dynamic Properties']")));
            dynamicPropsItem.Click();

            var colorChangeButton = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("colorChange")));
            string initialClass = colorChangeButton.GetAttribute("class");

            wait.Until(_ =>
            {
                var current = colorChangeButton.GetAttribute("class");
                return current != initialClass;
            });

            driver.Navigate().Refresh();
            Thread.Sleep(2000);

            var visibleAfterButton = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("visibleAfter")));
            Assert.IsTrue(visibleAfterButton.Displayed, "Button 'Visible After 5 Seconds' should appear.");
        }
    }
}

