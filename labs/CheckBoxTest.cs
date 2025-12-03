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
            
            var availableCheckboxes = driver.FindElements(By.XPath("//input[@type='checkbox']"));
            
            for (int i = 0; i < Math.Min(3, availableCheckboxes.Count); i++)
            {
                var checkbox = availableCheckboxes[i];
                if (checkbox.Displayed && checkbox.Enabled)
                {
                    var label = driver.FindElement(By.XPath($"//label[@for='{checkbox.GetAttribute("id")}']"));
                    label.Click();
                    Thread.Sleep(500);
                }
            }
            
            var collapseAllButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[@class='rct-option rct-option-collapse-all']")));
            collapseAllButton.Click();
            
            var pageTitle = driver.FindElement(By.XPath("//h1[text()='Check Box']"));
            Assert.IsTrue(pageTitle.Displayed, "Check Box page should be displayed");
            
            Assert.IsTrue(expandAllButton.Displayed, "Expand All button should be visible");
            Assert.IsTrue(collapseAllButton.Displayed, "Collapse All button should be visible");
        }
    }
}
