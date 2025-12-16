using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace labs.lab3
{
    [TestFixture]
    public class PracticeFormTest
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
        public void PracticeFormFormTest()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            Thread.Sleep(2000);
            
            var formsCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Forms']")));
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", formsCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", formsCard);
            
            var practiceFormItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-0']//span[text()='Practice Form']")));
            practiceFormItem.Click();
            
            var firstNameField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("firstName")));
            firstNameField.Clear();
            firstNameField.SendKeys("John");
            
            var lastNameField = driver.FindElement(By.Id("lastName"));
            lastNameField.Clear();
            lastNameField.SendKeys("Doe");
            
            var emailField = driver.FindElement(By.Id("userEmail"));
            emailField.Clear();
            emailField.SendKeys("john.doe@example.com");
            
            var genderMale = driver.FindElement(By.Id("gender-radio-1"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", genderMale);
            
            var mobileField = driver.FindElement(By.Id("userNumber"));
            mobileField.Clear();
            mobileField.SendKeys("1234567890");
            
            var dateOfBirthInput = driver.FindElement(By.Id("dateOfBirthInput"));
            dateOfBirthInput.Click();
            Thread.Sleep(500);
            dateOfBirthInput.SendKeys(OpenQA.Selenium.Keys.Control + "a");
            dateOfBirthInput.SendKeys("16 Dec 2025");
            Thread.Sleep(500);
            dateOfBirthInput.SendKeys(OpenQA.Selenium.Keys.Enter);
            Thread.Sleep(500);
            
            var subjectsInput = driver.FindElement(By.Id("subjectsInput"));
            subjectsInput.Click();
            subjectsInput.SendKeys("Math");
            Thread.Sleep(1000);
            subjectsInput.SendKeys(OpenQA.Selenium.Keys.Enter);
            
            subjectsInput.Click();
            subjectsInput.SendKeys("Physics");
            Thread.Sleep(1000);
            subjectsInput.SendKeys(OpenQA.Selenium.Keys.Enter);
            
            subjectsInput.Click();
            subjectsInput.SendKeys("Chemistry");
            Thread.Sleep(1000);
            subjectsInput.SendKeys(OpenQA.Selenium.Keys.Enter);
            
            var hobbiesSports = driver.FindElement(By.Id("hobbies-checkbox-1"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", hobbiesSports);
            
            var hobbiesReading = driver.FindElement(By.Id("hobbies-checkbox-2"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", hobbiesReading);
            
            var currentAddressField = driver.FindElement(By.Id("currentAddress"));
            currentAddressField.Clear();
            currentAddressField.SendKeys("123 Main Street, New York, NY 10001");
            
            var stateContainer = driver.FindElement(By.Id("state"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", stateContainer);
            Thread.Sleep(500);
            stateContainer.Click();
            Thread.Sleep(500);
            
            var stateSelect = driver.FindElement(By.Id("react-select-3-input"));
            stateSelect.SendKeys("NCR");
            Thread.Sleep(1000);
            stateSelect.SendKeys(OpenQA.Selenium.Keys.Enter);
            
            Thread.Sleep(1000);
            
            var cityContainer = driver.FindElement(By.Id("city"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", cityContainer);
            Thread.Sleep(500);
            cityContainer.Click();
            Thread.Sleep(500);
            
            var citySelect = driver.FindElement(By.Id("react-select-4-input"));
            citySelect.SendKeys("Delhi");
            Thread.Sleep(1000);
            citySelect.SendKeys(OpenQA.Selenium.Keys.Enter);
            
            var submitButton = driver.FindElement(By.Id("submit"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitButton);
            Thread.Sleep(500);
            submitButton.Click();
            
            Thread.Sleep(2000);
            
            var modalTitle = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//div[@id='example-modal-sizes-title-lg']")));
            Assert.IsTrue(modalTitle.Displayed, "Modal should be displayed");
            
            var modalBody = driver.FindElement(By.XPath("//div[@class='modal-body']"));
            string modalText = modalBody.Text;
            
            Assert.IsTrue(modalText.Contains("John"), "Modal should contain first name");
            Assert.IsTrue(modalText.Contains("Doe"), "Modal should contain last name");
            Assert.IsTrue(modalText.Contains("john.doe@example.com"), "Modal should contain email");
            Assert.IsTrue(modalText.Contains("Male"), "Modal should contain gender");
            Assert.IsTrue(modalText.Contains("1234567890"), "Modal should contain mobile number");
            Assert.IsTrue(modalText.Contains("16") && modalText.Contains("Dec") && modalText.Contains("2025"), 
                "Modal should contain date of birth");
            Assert.IsTrue(modalText.Contains("Math") || modalText.Contains("Physics") || modalText.Contains("Chemistry"), 
                "Modal should contain at least one subject");
            Assert.IsTrue(modalText.Contains("Sports") || modalText.Contains("Reading"), 
                "Modal should contain at least one hobby");
            Assert.IsTrue(modalText.Contains("123 Main Street"), "Modal should contain current address");
            Assert.IsTrue(modalText.Contains("NCR") && modalText.Contains("Delhi"), 
                "Modal should contain state and city");
            
            var closeButton = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//button[@id='closeLargeModal']")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", closeButton);
        }
    }
}

