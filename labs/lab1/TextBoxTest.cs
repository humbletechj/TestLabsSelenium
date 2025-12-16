using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace labs.lab1
{
    [TestFixture]
    public class TextBoxTest
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
        public void TextBoxFormTest()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            
            var elementsCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Elements']")));
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elementsCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", elementsCard);
            
            var textBoxItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-0']//span[text()='Text Box']")));
            textBoxItem.Click();
            
            var fullNameField = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("userName")));
            fullNameField.Clear();
            fullNameField.SendKeys("John Doe");
            
            var emailField = driver.FindElement(By.Id("userEmail"));
            emailField.Clear();
            emailField.SendKeys("john.doe@example.com");
            
            var currentAddressField = driver.FindElement(By.Id("currentAddress"));
            currentAddressField.Clear();
            currentAddressField.SendKeys("123 Main Street, Apt 4B, New York, NY 10001");
            
            var permanentAddressField = driver.FindElement(By.Id("permanentAddress"));
            permanentAddressField.Clear();
            permanentAddressField.SendKeys("456 Oak Avenue, Suite 200, Los Angeles, CA 90210");
            
            var submitButton = driver.FindElement(By.Id("submit"));
            submitButton.Click();
            
            var outputSection = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("output")));
            
            Assert.IsTrue(outputSection.Displayed, "Output section should be visible after form submission");
            
            var outputText = outputSection.Text;
            Assert.IsTrue(outputText.Contains("John Doe"), "Full name should be displayed in output");
            Assert.IsTrue(outputText.Contains("john.doe@example.com"), "Email should be displayed in output");
            Assert.IsTrue(outputText.Contains("123 Main Street, Apt 4B, New York, NY 10001"), "Current address should be displayed in output");
            Assert.IsTrue(outputText.Contains("456 Oak Avenue, Suite 200, Los Angeles, CA 90210"), "Permanent address should be displayed in output");
        }
    }
}

