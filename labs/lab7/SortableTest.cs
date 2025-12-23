using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace labs.lab7
{
    [TestFixture]
    public class SortableTest
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
        public void SortableListTest()
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
            
            // 3. Выбрать пункт 'Sortable'
            var sortableItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-0']//span[text()='Sortable']")));
            sortableItem.Click();
            
            Thread.Sleep(1000);
            
            // 4. Перейти на вкладку List
            var listTab = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("demo-tab-list")));
            listTab.Click();
            Thread.Sleep(500);
            
            // 5. Отсортировать значения в убывающем порядке
            var listContainer = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("demo-tabpane-list")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", listContainer);
            Thread.Sleep(500);
            
            // Целевой порядок в убывающем порядке: Six, Five, Four, Three, Two, One
            string[] targetOrder = { "Six", "Five", "Four", "Three", "Two", "One" };
            
            // Используем более точный подход: перемещаем каждый элемент на нужную позицию
            // Начинаем с последнего (Six) и перемещаем на позицию 0, затем Five на позицию 1 и т.д.
            var listContainerCurrent = driver.FindElement(By.Id("demo-tabpane-list"));
            var allItems = listContainerCurrent.FindElements(By.CssSelector(".list-group-item"));
            
            // Перемещаем элементы один за другим
            for (int targetPos = 0; targetPos < targetOrder.Length; targetPos++)
            {
                // Находим элемент, который должен быть на целевой позиции
                listContainerCurrent = driver.FindElement(By.Id("demo-tabpane-list"));
                var currentItems = listContainerCurrent.FindElements(By.CssSelector(".list-group-item"));
                
                IWebElement? itemToMove = null;
                foreach (var item in currentItems)
                {
                    if (item.Text == targetOrder[targetPos])
                    {
                        itemToMove = item;
                        break;
                    }
                }
                
                if (itemToMove == null) continue;
                
                // Проверяем, не находится ли элемент уже на нужной позиции
                int currentIndex = -1;
                for (int idx = 0; idx < currentItems.Count; idx++)
                {
                    if (currentItems[idx] == itemToMove)
                    {
                        currentIndex = idx;
                        break;
                    }
                }
                if (currentIndex == targetPos)
                    continue;
                
                // Находим целевой элемент (тот, который должен быть после перемещаемого элемента)
                IWebElement targetElement;
                if (targetPos < currentItems.Count)
                {
                    // Если целевая позиция существует, перемещаем перед этим элементом
                    targetElement = currentItems[targetPos];
                    // Если это тот же элемент, пропускаем
                    if (targetElement == itemToMove)
                        continue;
                }
                else
                {
                    // Если целевая позиция в конце, используем последний элемент
                    targetElement = currentItems[currentItems.Count - 1];
                }
                
                // Перемещаем элемент
                actions.ClickAndHold(itemToMove)
                       .MoveToElement(targetElement)
                       .MoveByOffset(0, -5)
                       .Release()
                       .Perform();
                
                Thread.Sleep(1000);
            }
            
            Thread.Sleep(1000);
            
            // Проверяем результат - элементы должны быть в обратном порядке
            var finalListContainer = driver.FindElement(By.Id("demo-tabpane-list"));
            var finalItems = finalListContainer.FindElements(By.CssSelector(".list-group-item"));
            
            for (int i = 0; i < targetOrder.Length && i < finalItems.Count; i++)
            {
                Assert.That(finalItems[i].Text, Is.EqualTo(targetOrder[i]), 
                    $"Элемент на позиции {i} должен быть '{targetOrder[i]}', но был '{finalItems[i].Text}'");
            }
        }
    }
}

