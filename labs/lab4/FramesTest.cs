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
    public class FramesTest
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
        public void FramesFormTest()
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
            
            // 3. Выбрать пункт 'Frames'
            var framesItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-2']//span[text()='Frames']")));
            framesItem.Click();
            
            // 4. Проверить наличие текста 'This is a sample page' в 1 frame
            var frame1 = wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.Id("frame1")));
            
            var frame1Body = wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("body")));
            string frame1Text = frame1Body.Text;
            Assert.That(frame1Text, Does.Contain("This is a sample page"), 
                "Frame 1 should contain 'This is a sample page'");
            
            driver.SwitchTo().DefaultContent();
            
            // 5. Проверить наличие текста 'This is a sample page' в 2 frame
            var frame2 = wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.Id("frame2")));
            
            var frame2Body = wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("body")));
            string frame2Text = frame2Body.Text;
            Assert.That(frame2Text, Does.Contain("This is a sample page"), 
                "Frame 2 should contain 'This is a sample page'");
            
            driver.SwitchTo().DefaultContent();
        }
    }
}

