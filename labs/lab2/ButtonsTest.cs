using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace labs.lab2
{
    [TestFixture]
    public class ButtonsTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private Actions actions;

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
            actions = new Actions(driver);
        }

        [TearDown]
        public void TearDown()
        {
            driver?.Dispose();
        }

        [Test]
        public void ButtonsFormTest()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            Thread.Sleep(2000);
            
            var elementsCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Elements']")));
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elementsCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", elementsCard);
            
            var buttonsItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-4']//span[text()='Buttons']")));
            buttonsItem.Click();
            
            var clickMeButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[text()='Click Me']")));
            clickMeButton.Click();
            
            var dynamicClickMessage = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//p[contains(text(), 'You have done a dynamic click')]")));
            Assert.IsTrue(dynamicClickMessage.Displayed, "Dynamic click message should be displayed");
            Assert.IsTrue(dynamicClickMessage.Text.Contains("You have done a dynamic click"), 
                "Should display 'You have done a dynamic click'");
            
            var doubleClickButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("doubleClickBtn")));
            actions.DoubleClick(doubleClickButton).Perform();
            
            var doubleClickMessage = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//p[contains(text(), 'You have done a double click')]")));
            Assert.IsTrue(doubleClickMessage.Displayed, "Double click message should be displayed");
            Assert.IsTrue(doubleClickMessage.Text.Contains("You have done a double click"), 
                "Should display 'You have done a double click'");
            
            var rightClickButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("rightClickBtn")));
            actions.ContextClick(rightClickButton).Perform();
            
            var rightClickMessage = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//p[contains(text(), 'You have done a right click')]")));
            Assert.IsTrue(rightClickMessage.Displayed, "Right click message should be displayed");
            Assert.IsTrue(rightClickMessage.Text.Contains("You have done a right click"), 
                "Should display 'You have done a right click'");
        }
    }
}

