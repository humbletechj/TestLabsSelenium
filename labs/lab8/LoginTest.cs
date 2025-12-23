using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace labs.lab8
{
    [TestFixture]
    public class LoginTest
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
        public void LoginAndRegistrationTest()
        {
            // 1. Перейти на страницу https://demoqa.com/
            driver.Navigate().GoToUrl("https://demoqa.com/");
            Thread.Sleep(3000);
            
            // 2. Перейти в раздел 'Book Store Application'
            Thread.Sleep(1000); // Дополнительное ожидание для загрузки страницы
            var bookStoreCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Book Store Application']")));
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", bookStoreCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", bookStoreCard);
            
            // 3. Выбрать пункт 'Login'
            var loginItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-0']//span[text()='Login']")));
            loginItem.Click();
            
            Thread.Sleep(1000);
            
            // 4. Ввести UserName
            var userNameField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("userName")));
            userNameField.Clear();
            userNameField.SendKeys("testuser");
            Thread.Sleep(500);
            
            // 5. Ввести Password
            var passwordField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("password")));
            passwordField.Clear();
            passwordField.SendKeys("Qwerty123@");
            Thread.Sleep(500);
            
            // 6. Нажать Login
            var loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("login")));
            loginButton.Click();
            Thread.Sleep(1000);
            
            // 7. Проверить что появилась надпись Invalid username or password!
            Thread.Sleep(2000); // Даем время для появления сообщения
            var outputDiv = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("output")));
            var errorMessage = outputDiv.Text;
            Assert.That(errorMessage, Does.Contain("Invalid username or password!").Or.Contains("Invalid"), 
                $"Должно появиться сообщение об ошибке. Текущее: {errorMessage}");
            
            // 8. Нажать NewUser
            var newUserButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("newUser")));
            newUserButton.Click();
            Thread.Sleep(1000);
            
            // 9. Заполнить First Name
            var firstNameField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("firstname")));
            firstNameField.Clear();
            firstNameField.SendKeys("John");
            Thread.Sleep(500);
            
            // 10. Заполнить Last Name
            var lastNameField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("lastname")));
            lastNameField.Clear();
            lastNameField.SendKeys("Doe");
            Thread.Sleep(500);
            
            // 11. Заполнить UserName
            var newUserNameField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("userName")));
            newUserNameField.Clear();
            newUserNameField.SendKeys("johndoe123");
            Thread.Sleep(500);
            
            // 12. Заполнить Password 1 символом
            var newPasswordField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("password")));
            newPasswordField.Clear();
            newPasswordField.SendKeys("1");
            Thread.Sleep(500);
            
            // 13. Нажать Register
            var registerButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("register")));
            registerButton.Click();
            Thread.Sleep(1000);
            
            // 14. Проверить что появилась надпись Please verify reCaptcha to register!
            Thread.Sleep(3000); // Даем время для появления сообщения
            var outputAfterRegister = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("output")));
            var recaptchaMessage = outputAfterRegister.Text;
            Assert.That(recaptchaMessage, Does.Contain("Please verify reCaptcha to register!").Or.Contains("reCaptcha").Or.Contains("reCAPTCHA"), 
                $"Должно появиться сообщение о reCaptcha. Текущее: {recaptchaMessage}");
            
            // 15. Поставить чек бокс I'm not a robot
            // Просто кликаем на чекбокс, не проходим капчу (это невозможно в автоматизированных тестах)
            Thread.Sleep(2000);
            try
            {
                // Ищем основной iframe с reCAPTCHA (не challenge)
                var allFrames = driver.FindElements(By.TagName("iframe"));
                IWebElement? recaptchaFrame = null;
                foreach (var frame in allFrames)
                {
                    var title = frame.GetAttribute("title");
                    if (title != null && title.Contains("reCAPTCHA") && !title.Contains("challenge"))
                    {
                        recaptchaFrame = frame;
                        break;
                    }
                }
                
                if (recaptchaFrame != null)
                {
                    driver.SwitchTo().Frame(recaptchaFrame);
                    Thread.Sleep(1000);
                    try
                    {
                        var recaptchaCheckbox = wait.Until(ExpectedConditions.ElementToBeClickable(
                            By.CssSelector(".recaptcha-checkbox-border")));
                        recaptchaCheckbox.Click();
                        Thread.Sleep(2000);
                    }
                    catch { }
                    driver.SwitchTo().DefaultContent();
                }
            }
            catch
            {
                driver.SwitchTo().DefaultContent();
            }
            
            // Останавливаемся здесь - дальше шаги не выполняем, так как капчу нельзя пройти автоматически
        }
    }
}

