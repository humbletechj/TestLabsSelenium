using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace labs.lab4
{
    [TestFixture]
    public class AlertsTest
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
        public void AlertsFormTest()
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

            // 3. Выбрать пункт 'Alerts'
            var alertsItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-1']//span[text()='Alerts']")));
            alertsItem.Click();

            // 4. Нажать 1 кнопку Click Me
            var alertButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("alertButton")));
            alertButton.Click();
            // 5. Проверить текст в модальном окне
            var alert1 = wait.Until(ExpectedConditions.AlertIsPresent());
            string alert1Text = alert1.Text;
            Assert.That(alert1Text, Is.Not.Empty, "Текст первого alert не должен быть пустым");
            // 6. Нажать ОК
            alert1.Accept();

            // 7. Нажать 2 кнопку Click Me
            var timerAlertButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("timerAlertButton")));
            timerAlertButton.Click();
            // ожидание дольше для таймера
            var alert2 = new WebDriverWait(driver, TimeSpan.FromSeconds(7)).Until(ExpectedConditions.AlertIsPresent());
            string alert2Text = alert2.Text;
            Assert.That(alert2Text, Is.Not.Empty, "Текст таймер alert не должен быть пустым");
            // 9. Нажать ОК
            alert2.Accept();

            // 10. Нажать 3 кнопку Click Me
            var confirmButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("confirmButton")));
            confirmButton.Click();
            // 11. Проверить текст в модальном окне
            var alert3 = wait.Until(ExpectedConditions.AlertIsPresent());
            string alert3Text = alert3.Text;
            Assert.That(alert3Text, Is.Not.Empty, "Текст confirm alert не должен быть пустым");
            // 12. Нажать Отмена
            alert3.Dismiss();

            // 13. Нажать 4 кнопку Click Me
            var promptButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("promtButton")));
            promptButton.Click();
            // 14. Ввести текст в модальном окне
            var alert4 = wait.Until(ExpectedConditions.AlertIsPresent());
            string promptText = "TestUser";
            alert4.SendKeys(promptText);
            // 15. Нажать ОК
            alert4.Accept();
        }
    }
}

