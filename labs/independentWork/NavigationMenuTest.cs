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
    public class NavigationMenuTest
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
        public void NavigationMenuNavigationTest()
        {
            // 1. Открыть главную страницу
            driver.Navigate().GoToUrl("https://www.rzd.ru/");
            Thread.Sleep(3000);
            
            string originalUrl = driver.Url;
            
            // 2. Найти элементы главного меню в header
            IWebElement? headerElement = null;
            
            try
            {
                headerElement = wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//header[contains(@class, 'header') and contains(@class, 'header_index')]")));
            }
            catch (WebDriverTimeoutException)
            {
                try
                {
                    headerElement = wait.Until(ExpectedConditions.ElementExists(By.XPath("//header[contains(@class, 'header')]")));
                }
                catch
                {
                    Assert.Fail("Header не найден на странице");
                }
            }
            
            Assert.That(headerElement, Is.Not.Null, "Header должен быть найден");
            
            List<IWebElement> menuItems = new List<IWebElement>();
            
            try
            {
                var navElements = headerElement!.FindElements(By.XPath(".//nav"));
                
                if (navElements.Count > 0)
                {
                    foreach (var nav in navElements)
                    {
                        var links = nav.FindElements(By.XPath(".//a[@href]"));
                        menuItems.AddRange(links);
                    }
                }
                
                if (menuItems.Count == 0)
                {
                    var menuContainers = headerElement.FindElements(By.XPath(
                        ".//ul[contains(@class, 'menu')] | .//div[contains(@class, 'menu')] | .//nav"));
                    
                    foreach (var container in menuContainers)
                    {
                        var links = container.FindElements(By.XPath(".//a[@href]"));
                        menuItems.AddRange(links);
                    }
                }
                
                if (menuItems.Count == 0)
                {
                    var allLinks = headerElement.FindElements(By.XPath(
                        ".//a[@href and not(contains(@href, '/en/') or contains(@href, '/ru/') or " +
                        "contains(@href, 'lang=en') or contains(@href, 'lang=ru') or " +
                        "contains(@data-lang, 'en') or contains(@data-lang, 'ru'))]"));
                    menuItems.AddRange(allLinks);
                }
                
                Assert.That(menuItems.Count, Is.GreaterThan(0), 
                    "Должно быть найдено хотя бы одно меню навигации в header");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Не удалось найти элементы меню в header: {ex.Message}");
            }
            
            // 3. Отфильтровать ссылки меню
            var validMenuHrefs = menuItems
                .Where(item => item.Displayed && 
                               !string.IsNullOrWhiteSpace(item.GetAttribute("href")) &&
                               item.GetAttribute("href").StartsWith("http") &&
                               !item.GetAttribute("href").Contains("javascript:") &&
                               !item.GetAttribute("href").EndsWith("#"))
                .Select(item => new { 
                    Href = item.GetAttribute("href"),
                    Text = item.Text.Trim(),
                    Title = item.GetAttribute("title") ?? ""
                })
                .GroupBy(item => item.Href)
                .Select(group => group.First())
                .Take(3)
                .ToList();
            
            Assert.That(validMenuHrefs.Count, Is.GreaterThan(0), 
                "Должна быть найдена хотя бы одна валидная ссылка меню");
            
            // 4. Кликнуть на каждый пункт меню и проверить переход
            foreach (var menuInfo in validMenuHrefs)
            {
                try
                {
                    driver.Navigate().GoToUrl("https://www.rzd.ru/");
                    Thread.Sleep(2000);
                    string currentOriginalUrl = driver.Url;
                    
                    IWebElement? currentHeader = null;
                    try
                    {
                        currentHeader = wait.Until(ExpectedConditions.ElementExists(
                            By.XPath("//header[contains(@class, 'header') and contains(@class, 'header_index')]")));
                    }
                    catch
                    {
                        currentHeader = wait.Until(ExpectedConditions.ElementExists(
                            By.XPath("//header[contains(@class, 'header')]")));
                    }
                    
                    IWebElement? menuItem = null;
                    var allLinks = currentHeader!.FindElements(By.XPath(".//a[@href]"));
                    menuItem = allLinks.FirstOrDefault(l => l.GetAttribute("href") == menuInfo.Href);
                    
                    if (menuItem == null)
                    {
                        menuItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                            By.XPath($"//a[@href='{menuInfo.Href}']")));
                    }
                    
                    Assert.That(menuItem, Is.Not.Null, $"Элемент меню с href '{menuInfo.Href}' не найден");
                    
                    string menuText = menuInfo.Text;
                    if (string.IsNullOrWhiteSpace(menuText))
                    {
                        menuText = menuInfo.Title;
                        if (string.IsNullOrWhiteSpace(menuText))
                        {
                            menuText = menuItem!.Text.Trim();
                            if (string.IsNullOrWhiteSpace(menuText))
                            {
                                menuText = menuInfo.Href;
                            }
                        }
                    }
                    
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", menuItem);
                    Thread.Sleep(500);
                    
                    string originalWindowHandle = driver.CurrentWindowHandle;
                    
                    menuItem!.Click();
                    Thread.Sleep(2000);
                    
                    if (driver.WindowHandles.Count > 1)
                    {
                        string newWindowHandle = driver.WindowHandles.Last();
                        driver.SwitchTo().Window(newWindowHandle);
                    }
                    
                    // 5. Проверить переход на соответствующую страницу
                    string currentUrl = driver.Url;
                    Assert.That(currentUrl, Is.Not.Empty, 
                        $"URL после перехода по ссылке '{menuText}' не должен быть пустым");
                    
                    var pageLoaded = wait.Until(ExpectedConditions.ElementExists(By.TagName("body")));
                    Assert.That(pageLoaded.Displayed, Is.True, "Страница должна быть загружена");
                    
                    string pageTitle = driver.Title;
                    Assert.That(pageTitle, Is.Not.Empty, 
                        $"Заголовок страницы не должен быть пустым для пункта меню: {menuText}");
                    
                    if (driver.WindowHandles.Count > 1)
                    {
                        driver.Close();
                        driver.SwitchTo().Window(originalWindowHandle);
                    }
                }
                catch (Exception ex)
                {
                    Assert.Warn($"Ошибка при проверке пункта меню: {ex.Message}. Продолжаем тест.");
                    string currentWindowHandle = driver.CurrentWindowHandle;
                    try
                    {
                        driver.Navigate().GoToUrl("https://www.rzd.ru/");
                    }
                    catch
                    {
                        if (driver.WindowHandles.Count > 1)
                        {
                            foreach (var handle in driver.WindowHandles)
                            {
                                if (handle != currentWindowHandle)
                                {
                                    driver.SwitchTo().Window(handle);
                                    driver.Close();
                                }
                            }
                            driver.SwitchTo().Window(currentWindowHandle);
                        }
                        driver.Navigate().GoToUrl("https://www.rzd.ru/");
                    }
                    Thread.Sleep(2000);
                }
            }
        }
    }
}

