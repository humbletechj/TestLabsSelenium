using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO;
using System.Threading;

namespace labs.lab2
{
    [TestFixture]
    public class UploadDownloadTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string testFilePath;

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
            
            testFilePath = Path.Combine(Path.GetTempPath(), "test_file.txt");
            File.WriteAllText(testFilePath, "Test file content for upload");
        }

        [TearDown]
        public void TearDown()
        {
            driver?.Dispose();
            
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }

        [Test]
        public void UploadDownloadFormTest()
        {
            driver.Navigate().GoToUrl("https://demoqa.com/");
            Thread.Sleep(2000);
            
            var elementsCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Elements']")));
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elementsCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", elementsCard);
            
            var uploadDownloadItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-7']//span[text()='Upload and Download']")));
            uploadDownloadItem.Click();
            
            var uploadInput = wait.Until(ExpectedConditions.ElementExists(By.Id("uploadFile")));
            uploadInput.SendKeys(testFilePath);
            
            Thread.Sleep(2000);
            
            var fileNameField = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//p[@id='uploadedFilePath']")));
            
            string displayedText = fileNameField.Text;
            string fileName = Path.GetFileName(testFilePath);
            string fullPath = Path.GetFullPath(testFilePath);
            
            Assert.IsTrue(displayedText.Contains(fileName), 
                $"Displayed text should contain file name '{fileName}', but was: {displayedText}");
            Assert.IsTrue(displayedText.Contains(fullPath) || displayedText.Contains(fileName), 
                $"Displayed text should contain full path or file name, but was: {displayedText}");
        }
    }
}

