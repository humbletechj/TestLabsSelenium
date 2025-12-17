using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace labs.lab4
{
    [TestFixture]
    public class NestedFramesTest
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
        public void NestedFramesFormTest()
        {
            // 1. Перейти на страницу https://demoqa.com/
            driver.Navigate().GoToUrl("https://demoqa.com/");
            Thread.Sleep(2000);
            
            // 2. Перейти в раздел 'Alerts, Frame & Windows'
            var alertsFrameWindowsCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Alerts, Frame & Windows']")));
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", alertsFrameWindowsCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", alertsFrameWindowsCard);
            
            // 3. Выбрать пункт 'Nested Frames'
            var nestedFramesItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-3']//span[text()='Nested Frames']")));
            nestedFramesItem.Click();
            
            // 4. Проверить наличие текста 'Child Iframe' в 1 frame
            var frame1 = wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.Id("frame1")));
            
            var childFrame = wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.TagName("iframe")));
            
            var childFrameBody = wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("body")));
            string childFrameText = childFrameBody.Text;
            Assert.That(childFrameText, Does.Contain("Child Iframe"), 
                "Child frame should contain 'Child Iframe'");
            
            // 5. Проверить наличие текста 'Parent frame' в 2 frame
            driver.SwitchTo().ParentFrame();
            
            var parentFrameBody = wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("body")));
            string parentFrameText = parentFrameBody.Text;
            Assert.That(parentFrameText, Does.Contain("Parent frame"), 
                "Parent frame should contain 'Parent frame'");
            
            driver.SwitchTo().DefaultContent();
        }
    }
}

