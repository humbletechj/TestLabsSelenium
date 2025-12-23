using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace labs.independentWork
{
    [TestFixture]
    public class FooterLinksTest
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
        public void FooterLinksNavigationTest()
        {
            // 1. Открыть главную страницу
            driver.Navigate().GoToUrl("https://www.rzd.ru/");
            Thread.Sleep(3000);
            
            string originalUrl = driver.Url;
            
            // 2. Прокрутить страницу вниз до футера
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            Thread.Sleep(2000);
            
            // 3. Найти футер и ссылки в нем
            IList<IWebElement> footerLinks = new List<IWebElement>();
            
            try
            {
                footerLinks = wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(
                    By.XPath("//a[@data-test-id='footer_contacts_link']")));
                
                if (footerLinks.Count == 0)
                {
                    var footerSection = wait.Until(ExpectedConditions.ElementIsVisible(
                        By.XPath("//section[contains(@class, 'ekmp') and contains(@class, 'footer')] | //ul[@data-test-id='footer_contacts_container']")));
                    
                    footerLinks = footerSection.FindElements(By.XPath(".//a[@href]"));
                }
                
                Assert.That(footerLinks.Count, Is.GreaterThan(0), 
                    "В футере должна быть найдена хотя бы одна ссылка");
            }
            catch (WebDriverTimeoutException ex)
            {
                Assert.Fail($"Не удалось найти ссылки в футере: {ex.Message}");
            }
            
            // 4. Отфильтровать валидные ссылки
            var validFooterLinks = footerLinks
                .Where(link => link.Displayed && 
                               !string.IsNullOrWhiteSpace(link.GetAttribute("href")) &&
                               link.GetAttribute("href").StartsWith("http"))
                .Select(link => new { 
                    Href = link.GetAttribute("href"),
                    Title = link.GetAttribute("title") ?? link.Text.Trim()
                })
                .Distinct()
                .Take(3)
                .ToList();
            
            Assert.That(validFooterLinks.Count, Is.GreaterThan(0), 
                "Должна быть найдена хотя бы одна валидная ссылка в футере");
            
            // 5. Кликнуть на несколько ссылок и проверить корректность переходов
            foreach (var linkInfo in validFooterLinks)
            {
                try
                {
                    driver.Navigate().GoToUrl("https://www.rzd.ru/");
                    Thread.Sleep(2000);
                    ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                    Thread.Sleep(1000);
                    
                    var links = driver.FindElements(By.XPath("//a[@data-test-id='footer_contacts_link']"));
                    var link = links.FirstOrDefault(l => l.GetAttribute("href") == linkInfo.Href);
                    
                    if (link == null)
                    {
                        string escapedHref = linkInfo.Href.Replace("'", "\\'");
                        link = wait.Until(ExpectedConditions.ElementToBeClickable(
                            By.XPath($"//a[@href='{escapedHref}']")));
                    }
                    
                    string linkText = linkInfo.Title;
                    if (string.IsNullOrWhiteSpace(linkText))
                    {
                        linkText = link.Text.Trim();
                    }
                    
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", link);
                    Thread.Sleep(500);
                    
                    int windowsBefore = driver.WindowHandles.Count;
                    string originalWindowHandle = driver.CurrentWindowHandle;
                    
                    link.Click();
                    Thread.Sleep(2000);
                    
                    // 6. Проверить корректность перехода
                    if (driver.WindowHandles.Count > windowsBefore)
                    {
                        string newWindowHandle = driver.WindowHandles.Last();
                        driver.SwitchTo().Window(newWindowHandle);
                    }
                    
                    string currentUrl = driver.Url;
                    Assert.That(currentUrl, Is.Not.Empty, 
                        $"URL после перехода по ссылке '{linkText}' не должен быть пустым");
                    
                    Assert.That(currentUrl, Does.Contain("http"), 
                        $"URL должен быть валидным: {currentUrl}");
                    
                    var pageBody = wait.Until(ExpectedConditions.ElementExists(By.TagName("body")));
                    Assert.That(pageBody.Displayed, Is.True, 
                        $"Страница должна быть загружена после перехода по ссылке: {linkText}");
                    
                    if (driver.WindowHandles.Count > windowsBefore)
                    {
                        driver.Close();
                        driver.SwitchTo().Window(driver.WindowHandles.First());
                    }
                }
                catch (Exception ex)
                {
                    Assert.Warn($"Ошибка при проверке ссылки футера: {ex.Message}. Продолжаем тест.");
                }
            }
        }
    }
}

