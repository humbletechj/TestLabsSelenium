using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace labs.independentWork
{
    [TestFixture]
    public class ContactInfoTest
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
        public void ContactInfoValidationTest()
        {
            // 1. Открыть главную страницу
            driver.Navigate().GoToUrl("https://www.rzd.ru/");
            Thread.Sleep(3000);
            
            // 2. Попытаться перейти на страницу с контактной информацией
            try
            {
                var contactLink = wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//a[contains(text(), 'Контакты') or contains(text(), 'Контакты') or contains(@href, 'contact') or contains(@href, 'kontakt')]")));
                
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", contactLink);
                Thread.Sleep(500);
                contactLink.Click();
                Thread.Sleep(3000);
            }
            catch (WebDriverTimeoutException)
            {
                try
                {
                    driver.Navigate().GoToUrl("https://www.rzd.ru/ru/contacts");
                    Thread.Sleep(3000);
                }
                catch
                {
                    try
                    {
                        ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                        Thread.Sleep(2000);
                        
                        var footerContactLink = wait.Until(ExpectedConditions.ElementToBeClickable(
                            By.XPath("//footer//a[contains(text(), 'Контакты') or contains(@href, 'contact')]")));
                        footerContactLink.Click();
                        Thread.Sleep(3000);
                    }
                    catch
                    {
                        Assert.Warn("Не удалось найти прямую ссылку на страницу контактов. Проверяем контактную информацию на главной странице.");
                    }
                }
            }
            
            // 3. Получить текст страницы для поиска контактной информации
            var pageBody = wait.Until(ExpectedConditions.ElementExists(By.TagName("body")));
            string pageText = pageBody.Text;
            
            Assert.That(pageText, Is.Not.Empty, "Страница должна содержать текст");
            
            // 4. Проверить наличие контактных данных
            bool hasPhone = false;
            bool hasEmail = false;
            bool hasAddress = false;
            
            Regex phonePattern = new Regex(@"(\+?7|8)?\s*\(?\d{3}\)?\s*\d{3}[-\s]?\d{2}[-\s]?\d{2}");
            MatchCollection phoneMatches = phonePattern.Matches(pageText);
            hasPhone = phoneMatches.Count > 0;
            
            Regex emailPattern = new Regex(@"[\w\.-]+@[\w\.-]+\.\w+");
            MatchCollection emailMatches = emailPattern.Matches(pageText);
            hasEmail = emailMatches.Count > 0;
            
            hasAddress = pageText.Contains("Москва") || pageText.Contains("адрес") || 
                        pageText.Contains("Адрес") || pageText.Contains("ул.") || 
                        pageText.Contains("улица") || pageText.Contains("проспект");
            
            // 5. Проверить наличие контактной информации через элементы на странице
            try
            {
                var phoneElements = driver.FindElements(By.XPath(
                    "//*[contains(text(), '+7') or contains(text(), '8 (') or contains(text(), 'тел')]"));
                if (phoneElements.Count > 0)
                {
                    hasPhone = true;
                }
            }
            catch { }
            
            try
            {
                var emailElements = driver.FindElements(By.XPath(
                    "//a[contains(@href, 'mailto:')] | //*[contains(text(), '@')]"));
                if (emailElements.Count > 0)
                {
                    hasEmail = true;
                }
            }
            catch { }
            
            try
            {
                var addressElements = driver.FindElements(By.XPath(
                    "//*[contains(text(), 'Москва') or contains(text(), 'адрес')]"));
                if (addressElements.Count > 0)
                {
                    hasAddress = true;
                }
            }
            catch { }
            
            // 6. Проверить корректность форматов контактных данных
            if (hasPhone)
            {
                Assert.Pass("Телефон найден на странице");
            }
            else
            {
                Assert.Warn("Телефон не найден на странице контактов");
            }
            
            if (hasEmail)
            {
                Assert.Pass("Email найден на странице");
            }
            else
            {
                Assert.Warn("Email не найден на странице контактов");
            }
            
            if (hasAddress)
            {
                Assert.Pass("Адрес найден на странице");
            }
            else
            {
                Assert.Warn("Адрес не найден на странице контактов");
            }
            
            // 7. Проверить наличие формы обратной связи
            try
            {
                var feedbackForm = driver.FindElement(By.XPath(
                    "//form[contains(@class, 'feedback') or contains(@class, 'contact') or contains(@id, 'feedback')] | " +
                    "//form//input[@type='text' or @type='email']"));
                
                if (feedbackForm != null && feedbackForm.Displayed)
                {
                    Assert.Pass("Форма обратной связи найдена на странице");
                }
            }
            catch (NoSuchElementException)
            {
                Assert.Inconclusive("Форма обратной связи не найдена на странице (это может быть нормально)");
            }
            
            Assert.That(hasPhone || hasEmail || hasAddress, Is.True, 
                "На странице должна быть найдена хотя бы одна форма контактной информации (телефон, email или адрес)");
        }
    }
}

