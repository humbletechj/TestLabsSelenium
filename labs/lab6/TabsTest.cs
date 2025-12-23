using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace labs.lab6
{
    [TestFixture]
    public class TabsTest
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
        public void TabsNavigationTest()
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
            
            // 3. Выбрать пункт 'Tabs'
            var tabsItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-5']//span[text()='Tabs']")));
            tabsItem.Click();
            
            Thread.Sleep(1000);
            
            // 4. Открыть вкладку What
            var whatTab = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("demo-tab-what")));
            whatTab.Click();
            Thread.Sleep(500);
            
            // 5. Проверить что на ней есть текст
            var whatTabPane = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("demo-tabpane-what")));
            var whatText = whatTabPane.Text;
            Assert.That(whatText, Is.Not.Empty, "Вкладка What должна содержать текст");
            Assert.That(whatText, Does.Contain("Lorem Ipsum"), "Вкладка What должна содержать текст о Lorem Ipsum");
            
            // 6. Открыть вкладку Origin
            var originTab = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("demo-tab-origin")));
            originTab.Click();
            Thread.Sleep(500);
            
            // 7. Проверить что на ней есть текст
            var originTabPane = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("demo-tabpane-origin")));
            var originText = originTabPane.Text;
            Assert.That(originText, Is.Not.Empty, "Вкладка Origin должна содержать текст");
            Assert.That(originText, Does.Contain("Contrary to popular belief"), "Вкладка Origin должна содержать соответствующий текст");
            
            // 8. Открыть вкладку Use
            var useTab = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("demo-tab-use")));
            useTab.Click();
            Thread.Sleep(500);
            
            // 9. Проверить что на ней есть текст
            var useTabPane = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("demo-tabpane-use")));
            var useText = useTabPane.Text;
            Assert.That(useText, Is.Not.Empty, "Вкладка Use должна содержать текст");
            Assert.That(useText, Does.Contain("It is a long established fact"), "Вкладка Use должна содержать соответствующий текст");
            
            // 10. Проверить что вкладка More недоступна
            var moreTab = driver.FindElement(By.Id("demo-tab-more"));
            Assert.That(moreTab.GetAttribute("class"), Does.Contain("disabled"), "Вкладка More должна быть недоступна (иметь класс disabled)");
            Assert.That(moreTab.GetAttribute("aria-disabled"), Is.EqualTo("true"), "Вкладка More должна иметь aria-disabled='true'");
        }
    }
}

