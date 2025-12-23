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
    public class SelectableTest
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
        public void SelectableGridTest()
        {
            // 1. Перейти на страницу https://demoqa.com/
            driver.Navigate().GoToUrl("https://demoqa.com/");
            Thread.Sleep(2000);
            
            // 2. Перейти в раздел 'Interaction'
            var interactionsCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Interactions']")));
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", interactionsCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", interactionsCard);
            
            // 3. Выбрать пункт 'Selectable'
            var selectableItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-1']//span[text()='Selectable']")));
            selectableItem.Click();
            
            Thread.Sleep(1000);
            
            // 4. Перейти на вкладку Grid
            var gridTab = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("demo-tab-grid")));
            gridTab.Click();
            Thread.Sleep(500);
            
            // 5. Выбрать все значения
            var gridContainer = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("demo-tabpane-grid")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", gridContainer);
            Thread.Sleep(500);
            
            var gridItems = gridContainer.FindElements(By.CssSelector(".list-group-item"));
            
            // Кликаем по всем элементам для выбора
            foreach (var item in gridItems)
            {
                item.Click();
                Thread.Sleep(200);
            }
            
            Thread.Sleep(500);
            
            // 6. Снять все значения
            // Кликаем по всем элементам еще раз, чтобы снять выбор
            gridContainer = driver.FindElement(By.Id("demo-tabpane-grid"));
            gridItems = gridContainer.FindElements(By.CssSelector(".list-group-item"));
            foreach (var item in gridItems)
            {
                item.Click();
                Thread.Sleep(200);
            }
            
            Thread.Sleep(500);
            
            // 7. Выбрать только Five
            gridContainer = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("demo-tabpane-grid")));
            var allItemsList = gridContainer.FindElements(By.CssSelector(".list-group-item"));
            
            IWebElement? fiveItem = null;
            foreach (var item in allItemsList)
            {
                if (item.Text == "Five")
                {
                    fiveItem = item;
                    break;
                }
            }
            
            Assert.That(fiveItem, Is.Not.Null, "Элемент Five должен быть найден");
            fiveItem!.Click();
            Thread.Sleep(500);
            
            // Проверяем, что только Five выбран (имеет активный класс)
            gridContainer = driver.FindElement(By.Id("demo-tabpane-grid"));
            var allItemsAfter = gridContainer.FindElements(By.CssSelector(".list-group-item"));
            foreach (var item in allItemsAfter)
            {
                if (item.Text == "Five")
                {
                    Assert.That(item.GetAttribute("class"), Does.Contain("active"), "Элемент Five должен быть выбран (иметь класс active)");
                }
                else
                {
                    Assert.That(item.GetAttribute("class"), Does.Not.Contain("active"), $"Элемент '{item.Text}' не должен быть выбран");
                }
            }
        }
    }
}

