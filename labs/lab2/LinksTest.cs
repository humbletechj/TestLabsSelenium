using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace labs.lab2
{
    [TestFixture]
    public class LinksTest
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
        public void LinksFormTest()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            Thread.Sleep(2000);
            
            var elementsCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Elements']")));
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elementsCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", elementsCard);
            
            var linksItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-5']//span[text()='Links']")));
            linksItem.Click();
            
            string originalWindow = driver.CurrentWindowHandle;
            
            var homeLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("simpleLink")));
            homeLink.Click();
            
            Thread.Sleep(2000);
            
            var windows = driver.WindowHandles;
            Assert.IsTrue(windows.Count > 1, "New window should be opened");
            
            foreach (string window in windows)
            {
                if (window != originalWindow)
                {
                    driver.SwitchTo().Window(window);
                    break;
                }
            }
            
            string newWindowUrl = driver.Url;
            Assert.IsTrue(newWindowUrl.Contains("demoqa.com"), 
                $"New window URL should contain 'demoqa.com', but was: {newWindowUrl}");
            
            driver.Close();
            
            driver.SwitchTo().Window(originalWindow);
            
            var movedLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("moved")));
            movedLink.Click();
            
            Thread.Sleep(5000);
            
            IWebElement responseMessage;
            try
            {
                responseMessage = driver.FindElement(By.Id("linkResponse"));
            }
            catch
            {
                responseMessage = wait.Until(ExpectedConditions.ElementIsVisible(
                    By.XPath("//*[contains(text(), '301')]")));
            }
            
            Assert.IsTrue(responseMessage.Displayed, "Response message should be displayed");
            string messageText = responseMessage.Text;
            Assert.IsTrue(messageText.Contains("301"), 
                $"Should contain '301' in message, but was: {messageText}");
            Assert.IsTrue(messageText.Contains("Moved Permanently") || messageText.Contains("Moved"), 
                $"Should contain 'Moved Permanently' or 'Moved' in message, but was: {messageText}");
        }
    }
}

