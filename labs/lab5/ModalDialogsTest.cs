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
    public class ModalDialogsTest
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
        public void ModalDialogsFormTest()
        {
            // 1. Перейти на страницу https://demoqa.com/
            driver.Navigate().GoToUrl("https://demoqa.com/");
            Thread.Sleep(2000);
            
            // 2. Перейти в раздел 'Alerts, Frame & Windows'
            var alertsFrameWindowsCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Alerts, Frame & Windows']")));
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", alertsFrameWindowsCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", alertsFrameWindowsCard);
            
            // 3. Выбрать пункт 'Modal Dialogs'
            var modalDialogsItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-4']//span[text()='Modal Dialogs']")));
            modalDialogsItem.Click();
            
            // 4. Нажать Small Modal
            var smallModalButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("showSmallModal")));
            smallModalButton.Click();
            
            Thread.Sleep(1000);
            
            // 5. В открывшемся окне сверить заголовок
            var modalTitle = wait.Until(ExpectedConditions.ElementIsVisible(
                By.Id("example-modal-sizes-title-sm")));
            Assert.That(modalTitle.Displayed, Is.True, "Modal title should be displayed");
            Assert.That(modalTitle.Text, Is.EqualTo("Small Modal"), 
                $"Expected title 'Small Modal', but got '{modalTitle.Text}'");
            
            // 6. В открывшемся окне сверить основной текст
            var modalBody = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//div[@class='modal-body']")));
            Assert.That(modalBody.Displayed, Is.True, "Modal body should be displayed");
            Assert.That(modalBody.Text, Is.EqualTo("This is a small modal. It has very less content"), 
                $"Expected body text 'This is a small modal. It has very less content', but got '{modalBody.Text}'");
            
            // 7. Нажать Close
            var closeButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("closeSmallModal")));
            closeButton.Click();
            
            Thread.Sleep(1000);
            
            // Verify modal is closed
            try
            {
                var closedModal = driver.FindElement(By.Id("example-modal-sizes-title-sm"));
                Assert.Fail("Modal should be closed");
            }
            catch (NoSuchElementException)
            {
                // Expected - modal is closed
            }
        }
    }
}


