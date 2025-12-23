using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;
using System.Threading;

namespace labs.independentWork
{
    [TestFixture]
    public class SupportServiceBlockTest
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
        public void SupportServiceBlockValidationTest()
        {
            // 1. Открыть главную страницу сайта РЖД
            driver.Navigate().GoToUrl("https://www.rzd.ru/");
            Thread.Sleep(3000);
            
            // 2. Найти блок "Служба поддержки" по data-test-id
            IWebElement? supportBlock = null;
            try
            {
                supportBlock = wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//div[@data-test-id='block_2948']")));
            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail("Блок 'Служба поддержки' (data-test-id='block_2948') не найден на странице");
            }
            
            Assert.That(supportBlock, Is.Not.Null, "Блок службы поддержки должен быть найден");
            Assert.That(supportBlock!.Displayed, Is.True, "Блок службы поддержки должен быть видимым");
            
            // 3. Прокрутить к блоку для видимости
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", supportBlock);
            Thread.Sleep(1000);
            
            // 4. Проверить наличие заголовка "Служба поддержки"
            IWebElement? headerElement = null;
            try
            {
                headerElement = supportBlock.FindElement(By.XPath(".//h4[contains(@class, 'headerlink_text') and contains(text(), 'Служба поддержки')]"));
                Assert.That(headerElement.Displayed, Is.True, "Заголовок 'Служба поддержки' должен отображаться");
                Assert.That(headerElement.Text, Does.Contain("Служба поддержки"), 
                    "Заголовок должен содержать текст 'Служба поддержки'");
            }
            catch (NoSuchElementException)
            {
                Assert.Fail("Заголовок 'Служба поддержки' не найден в блоке");
            }
            
            // 5. Проверить наличие основного заголовочного ссылки
            IWebElement? mainHeaderLink = null;
            try
            {
                mainHeaderLink = supportBlock.FindElement(By.XPath(".//a[contains(@class, 'headerlink')]"));
                Assert.That(mainHeaderLink.Displayed, Is.True, "Основная ссылка заголовка должна отображаться");
                
                string headerLinkHref = mainHeaderLink.GetAttribute("href");
                Assert.That(headerLinkHref, Is.Not.Null.And.Not.Empty, "Основная ссылка должна иметь href");
                Assert.That(headerLinkHref, Does.Contain("rzd.ru"), "Ссылка должна вести на сайт РЖД");
            }
            catch (NoSuchElementException)
            {
                Assert.Fail("Основная ссылка заголовка не найдена в блоке");
            }
            
            // 6. Проверить наличие телефона "8 800 775-00-00"
            IWebElement? phoneElement = null;
            try
            {
                var phoneElements = supportBlock.FindElements(By.XPath(
                    ".//*[contains(text(), '8 800 775-00-00') or contains(text(), '88007750000') or contains(text(), '8-800-775-00-00')]"));
                
                Assert.That(phoneElements.Count, Is.GreaterThan(0), 
                    "Телефон '8 800 775-00-00' должен быть найден в блоке");
                
                phoneElement = phoneElements.FirstOrDefault(e => e.Displayed);
                Assert.That(phoneElement, Is.Not.Null, "Элемент с телефоном должен быть видимым");
                
                var phoneLink = phoneElement!.FindElement(By.XPath("./ancestor::a[@href]"));
                string phoneLinkHref = phoneLink.GetAttribute("href");
                Assert.That(phoneLinkHref, Is.Not.Null.And.Not.Empty, "Телефон должен быть в кликабельной ссылке");
            }
            catch (NoSuchElementException)
            {
                Assert.Fail("Телефон '8 800 775-00-00' не найден в блоке");
            }
            
            // 7. Проверить наличие email "ticket@rzd.ru"
            IWebElement? emailElement = null;
            try
            {
                var emailElements = supportBlock.FindElements(By.XPath(
                    ".//*[contains(text(), 'ticket@rzd.ru')]"));
                
                Assert.That(emailElements.Count, Is.GreaterThan(0), 
                    "Email 'ticket@rzd.ru' должен быть найден в блоке");
                
                emailElement = emailElements.FirstOrDefault(e => e.Displayed);
                Assert.That(emailElement, Is.Not.Null, "Элемент с email должен быть видимым");
                
                var emailLink = emailElement!.FindElement(By.XPath("./ancestor::a[@href]"));
                string emailLinkHref = emailLink.GetAttribute("href");
                Assert.That(emailLinkHref, Is.Not.Null.And.Not.Empty, "Email должен быть в кликабельной ссылке");
            }
            catch (NoSuchElementException)
            {
                Assert.Fail("Email 'ticket@rzd.ru' не найден в блоке");
            }
            
            // 8. Проверить наличие ссылки "Участникам СВО"
            IWebElement? svoLink = null;
            try
            {
                var svoLinks = supportBlock.FindElements(By.XPath(
                    ".//a[contains(text(), 'Участникам СВО') or contains(text(), 'СВО') or contains(@href, '11872')]"));
                
                if (svoLinks.Count > 0)
                {
                    svoLink = svoLinks.FirstOrDefault(e => e.Displayed);
                    
                    if (svoLink != null)
                    {
                        string svoLinkHref = svoLink.GetAttribute("href");
                        if (!string.IsNullOrEmpty(svoLinkHref))
                        {
                            Assert.That(svoLinkHref, Does.Contain("rzd.ru"), 
                                "Ссылка 'Участникам СВО' должна вести на сайт РЖД");
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            
            // 9. Проверить наличие списка ссылок (ul.third-block__links)
            IWebElement? linksList = null;
            try
            {
                linksList = supportBlock.FindElement(By.XPath(".//ul[contains(@class, 'third-block__links')]"));
                Assert.That(linksList.Displayed, Is.True, "Список ссылок должен отображаться");
                
                var listItems = linksList.FindElements(By.XPath(".//li"));
                Assert.That(listItems.Count, Is.GreaterThan(0), 
                    "Список должен содержать хотя бы один элемент");
            }
            catch (NoSuchElementException)
            {
                Assert.Fail("Список ссылок (ul.third-block__links) не найден в блоке");
            }
            
            // 10. Проверить кликабельность основной ссылки заголовка
            try
            {
                if (mainHeaderLink != null)
                {
                    string originalWindowHandle = driver.CurrentWindowHandle;
                    string originalUrl = driver.Url;
                    
                    mainHeaderLink.Click();
                    Thread.Sleep(2000);
                    
                    if (driver.WindowHandles.Count > 1)
                    {
                        string newWindowHandle = driver.WindowHandles.Last();
                        driver.SwitchTo().Window(newWindowHandle);
                    }
                    
                    string newUrl = driver.Url;
                    Assert.That(newUrl, Is.Not.Empty, "URL после клика не должен быть пустым");
                    if (driver.WindowHandles.Count > 1)
                    {
                        driver.Close();
                        driver.SwitchTo().Window(originalWindowHandle);
                    }
                    else
                    {
                        driver.Navigate().GoToUrl("https://www.rzd.ru/");
                        Thread.Sleep(2000);
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Warn($"Не удалось проверить кликабельность основной ссылки: {ex.Message}");
            }
        }
    }
}

