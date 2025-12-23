using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace labs.lab5
{
    [TestFixture]
    public class AccordianTest
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
        public void AccordianFormTest()
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
            
            // 3. Выбрать пункт 'Accordian'
            var accordianItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-0']//span[text()='Accordian']")));
            accordianItem.Click();
            
            // 4. Раскрыть аккордион 'What is Lorem Ipsum?'
            var section1Heading = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("section1Heading")));
            section1Heading.Click();
            
            Thread.Sleep(2000);
            
            // 5. Проверить наличие текста
            var section1Content = driver.FindElement(By.Id("section1Content"));
            string section1Text = ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].textContent || arguments[0].innerText;", section1Content).ToString() ?? "";
            Assert.That(section1Text, Does.Contain("Lorem Ipsum"), 
                $"Section 1 should contain 'Lorem Ipsum' text, but got: {section1Text}");
            
            // 6. Раскрыть аккордион 'Where does it come from?'
            var section2Heading = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("section2Heading")));
            section2Heading.Click();
            
            Thread.Sleep(2000);
            
            // 7. Проверить наличие текста
            var section2Content = driver.FindElement(By.Id("section2Content"));
            string section2Text = ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].textContent || arguments[0].innerText;", section2Content).ToString() ?? "";
            Assert.That(section2Text, Does.Contain("Contrary to popular belief"), 
                $"Section 2 should contain 'Contrary to popular belief' text, but got: {section2Text}");
            
            // 8. Раскрыть аккордион 'Why do we use it?'
            var section3Heading = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("section3Heading")));
            section3Heading.Click();
            
            Thread.Sleep(2000);
            
            // 9. Проверить наличие текста
            var section3Content = driver.FindElement(By.Id("section3Content"));
            string section3Text = ((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].textContent || arguments[0].innerText;", section3Content).ToString() ?? "";
            Assert.That(section3Text, Does.Contain("It is a long established fact"), 
                $"Section 3 should contain 'It is a long established fact' text, but got: {section3Text}");
        }
    }
}

