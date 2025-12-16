using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace labs.lab1
{
    [TestFixture]
    public class CheckBoxTest
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
        public void CheckBoxFormTest()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            
            var elementsCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Elements']")));
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elementsCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", elementsCard);
            
            var checkBoxItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-1']//span[text()='Check Box']")));
            checkBoxItem.Click();
            
            var expandAllButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[@class='rct-option rct-option-expand-all']")));
            expandAllButton.Click();
            
            Thread.Sleep(2000);
            
            var notesCheckbox = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//label[contains(@for, 'tree-node-notes') or .//span[text()='Notes']]")));
            notesCheckbox.Click();
            Thread.Sleep(500);
            
            var veuCheckbox = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//label[contains(@for, 'tree-node-veu') or .//span[text()='Veu']]")));
            veuCheckbox.Click();
            Thread.Sleep(500);
            
            var privateCheckbox = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//label[contains(@for, 'tree-node-private') or .//span[text()='Private']]")));
            privateCheckbox.Click();
            Thread.Sleep(500);
            
            var collapseAllButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[@class='rct-option rct-option-collapse-all']")));
            collapseAllButton.Click();
        }
    }
}

