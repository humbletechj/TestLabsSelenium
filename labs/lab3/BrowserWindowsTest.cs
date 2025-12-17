using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace labs.lab3
{
    [TestFixture]
    public class BrowserWindowsTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string? mainWindowHandle;

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
        public void TestBrowserWindowsFunctionality()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            Thread.Sleep(2000);

            var alertsFrameWindowsCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Alerts, Frame & Windows']")));
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", alertsFrameWindowsCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", alertsFrameWindowsCard);

            var browserWindowsItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//span[text()='Browser Windows']")));
            browserWindowsItem.Click();

            mainWindowHandle = driver.CurrentWindowHandle;

            var newTabButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("tabButton")));
            newTabButton.Click();

            wait.Until(driver => driver.WindowHandles.Count == 2);
            SwitchToNewWindow();

            string samplePageUrl = "https://demoqa.com/sample";
            Assert.That(driver.Url, Is.EqualTo(samplePageUrl), 
                $"Expected URL: {samplePageUrl}, but got {driver.Url}");

            driver.Close();
            driver.SwitchTo().Window(mainWindowHandle);

            var newWindowButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("windowButton")));
            newWindowButton.Click();

            wait.Until(driver => driver.WindowHandles.Count == 2);
            SwitchToNewWindow();

            Assert.That(driver.Url, Is.EqualTo(samplePageUrl),
                $"Expected URL: {samplePageUrl}, but got {driver.Url}");

            driver.Close();
            driver.SwitchTo().Window(mainWindowHandle);
        }

        private void SwitchToNewWindow()
        {
            foreach (string handle in driver.WindowHandles)
            {
                if (handle != mainWindowHandle)
                {
                    driver.SwitchTo().Window(handle);
                    break;
                }
            }
        }
    }
}
