using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace labs
{
    [TestFixture]
    public class RadioButtonTest
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
        public void RadioButtonFormTest()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            
            var elementsCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Elements']")));
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elementsCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", elementsCard);
            
            var radioButtonItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-2']//span[text()='Radio Button']")));
            radioButtonItem.Click();
            
            var yesRadio = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//label[@for='yesRadio']")));
            yesRadio.Click();
            
            var resultText = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//span[@class='text-success']")));
            Assert.IsTrue(resultText.Text.Contains("Yes"), "Should display 'Yes' selection result");
            
            var impressiveRadio = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//label[@for='impressiveRadio']")));
            impressiveRadio.Click();
            
            var resultText2 = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//span[@class='text-success']")));
            Assert.IsTrue(resultText2.Text.Contains("Impressive"), "Should display 'Impressive' selection result");
            
            var noRadio = driver.FindElement(By.XPath("//input[@id='noRadio']"));
            Assert.IsFalse(noRadio.Enabled, "No radio button should be disabled");
        }
    }
}

