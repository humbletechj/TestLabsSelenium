using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace labs.lab7
{
    [TestFixture]
    public class SelectMenuTest
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
        public void SelectMenuFormTest()
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
            
            // 3. Выбрать пункт 'Select Menu'
            var selectMenuItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-8']//span[text()='Select Menu']")));
            selectMenuItem.Click();
            
            Thread.Sleep(1000);
            
            // 4. В поле 'Select Value' выбрать 'A root option'
            var selectValueContainer = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("withOptGroup")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", selectValueContainer);
            Thread.Sleep(500);
            selectValueContainer.Click();
            Thread.Sleep(500);
            var selectValueInput = driver.FindElement(By.Id("react-select-2-input"));
            selectValueInput.SendKeys("A root option");
            Thread.Sleep(1000);
            selectValueInput.SendKeys(OpenQA.Selenium.Keys.Enter);
            Thread.Sleep(500);
            
            // 5. В поле 'Select One' выбрать 'Ms.'
            var selectOneContainer = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("selectOne")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", selectOneContainer);
            Thread.Sleep(500);
            selectOneContainer.Click();
            Thread.Sleep(500);
            var selectOneInput = driver.FindElement(By.Id("react-select-3-input"));
            selectOneInput.SendKeys("Ms.");
            Thread.Sleep(1000);
            selectOneInput.SendKeys(OpenQA.Selenium.Keys.Enter);
            Thread.Sleep(500);
            
            // 6. В поле 'Old Style Select Menu' выбрать 'Black'
            var oldSelectMenu = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("oldSelectMenu")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", oldSelectMenu);
            Thread.Sleep(500);
            var selectElement = new SelectElement(oldSelectMenu);
            selectElement.SelectByValue("5"); // Black has value="5"
            Thread.Sleep(500);
            
            // 7. В поле 'Multiselect drop down' выбрать: Black, Red
            var multiselectInput = wait.Until(ExpectedConditions.ElementExists(By.Id("react-select-4-input")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", multiselectInput);
            Thread.Sleep(500);
            
            // Кликаем на контейнер, который содержит input
            var multiselectContainer = multiselectInput.FindElement(By.XPath("./ancestor::div[contains(@class,'css-2b097c-container')]"));
            multiselectContainer.Click();
            Thread.Sleep(500);
            multiselectInput.SendKeys("Black");
            Thread.Sleep(1000);
            multiselectInput.SendKeys(OpenQA.Selenium.Keys.Enter);
            Thread.Sleep(500);
            
            multiselectContainer.Click();
            Thread.Sleep(500);
            multiselectInput = driver.FindElement(By.Id("react-select-4-input"));
            multiselectInput.SendKeys("Red");
            Thread.Sleep(1000);
            multiselectInput.SendKeys(OpenQA.Selenium.Keys.Enter);
            Thread.Sleep(500);
            
            // 8. В поле 'Standard multi select' выбрать 'Opel'
            var standardMultiSelect = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("cars")));
            var multiSelectElement = new SelectElement(standardMultiSelect);
            multiSelectElement.SelectByValue("opel");
            Thread.Sleep(1000);
        }
    }
}

