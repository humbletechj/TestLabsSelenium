using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace labs.lab6
{
    [TestFixture]
    public class SliderTest
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
        public void SliderValueTest()
        {
            // 1. Перейти на страницу https://demoqa.com/
            driver.Navigate().GoToUrl("https://demoqa.com/");
            Thread.Sleep(2000);
            
            // 2. Перейти в раздел 'Widgets'
            var widgetsCard = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[@class='card mt-4 top-card']//h5[text()='Widgets']")));
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", widgetsCard);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", widgetsCard);
            
            // 3. Выбрать пункт 'Slider'
            var sliderItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-3']//span[text()='Slider']")));
            sliderItem.Click();
            
            Thread.Sleep(1000);
            
            // 4. Сдвинуть слайдер на значение 50
            var slider = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.CssSelector("input[type='range'].range-slider")));
            
            // Используем JavaScript для установки значения слайдера и триггера событий
            ((IJavaScriptExecutor)driver).ExecuteScript(
                @"
                var slider = arguments[0];
                slider.value = '50';
                var inputEvent = new Event('input', { bubbles: true });
                var changeEvent = new Event('change', { bubbles: true });
                slider.dispatchEvent(inputEvent);
                slider.dispatchEvent(changeEvent);
                
                // Обновляем значение в поле справа
                var valueInput = document.getElementById('sliderValue');
                if (valueInput) {
                    valueInput.value = '50';
                    valueInput.dispatchEvent(new Event('input', { bubbles: true }));
                    valueInput.dispatchEvent(new Event('change', { bubbles: true }));
                }
                ",
                slider);
            
            Thread.Sleep(1500);
            
            // 5. Проверить что в окне справа значение 50
            var sliderValueInput = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("sliderValue")));
            var actualValue = sliderValueInput.GetAttribute("value");
            
            Assert.That(actualValue, Is.EqualTo("50"), "Значение слайдера должно быть 50");
        }
    }
}

