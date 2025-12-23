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
    public class DatePickerTest
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
        public void SelectDateAndTimeTest()
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

            // 3. Выбрать пункт 'Date Picker'
            var datePickerItem = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[@id='item-2']//span[text()='Date Picker']")));
            datePickerItem.Click();

            Thread.Sleep(1000);

            // 4. В поле 'Select Date' выбрать 1 декабря 2023 года
            var selectDateInput = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("datePickerMonthYearInput")));
            selectDateInput.Click();
            Thread.Sleep(500);

            var monthSelect = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".react-datepicker__month-select")));
            new SelectElement(monthSelect).SelectByText("December");

            var yearSelect = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".react-datepicker__year-select")));
            new SelectElement(yearSelect).SelectByValue("2023");
            Thread.Sleep(500);

            var firstDecember = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[contains(@class,'react-datepicker__day--001') and not(contains(@class,'outside-month'))]")));
            firstDecember.Click();

            Thread.Sleep(1000);

            // 5. В поле 'Date And Time' выбрать 2 ноября 2022 года 20:00
            var dateTimeInput = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("dateAndTimePickerInput")));
            dateTimeInput.Click();
            Thread.Sleep(1000);

            // Навигация к ноябрю 2022 года
            // Сначала выбираем год
            var yearReadView = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.CssSelector(".react-datepicker__year-read-view--selected-year")));
            yearReadView.Click();
            Thread.Sleep(1000);

            // Прокручиваем до 2022 года - пробуем разные варианты селекторов
            IWebElement? year2022Option = null;
            try
            {
                year2022Option = driver.FindElement(By.XPath("//div[contains(@class,'react-datepicker__year-option')]//span[text()='2022']"));
            }
            catch
            {
                try
                {
                    year2022Option = driver.FindElement(By.XPath("//div[contains(@class,'react-datepicker__year-option') and text()='2022']"));
                }
                catch
                {
                    // Прокручиваем список годов до нужного
                    var allYearOptions = driver.FindElements(By.CssSelector(".react-datepicker__year-option"));
                    foreach (var yearOption in allYearOptions)
                    {
                        var yearText = yearOption.Text;
                        if (yearText.Contains("2022"))
                        {
                            year2022Option = yearOption;
                            break;
                        }
                    }
                }
            }

            if (year2022Option != null)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", year2022Option);
                Thread.Sleep(500);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", year2022Option);
                Thread.Sleep(1000);
            }
            else
            {
                throw new Exception("Could not find year 2022 option");
            }

            // Выбираем месяц November
            var monthReadView = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.CssSelector(".react-datepicker__month-read-view--selected-month")));
            monthReadView.Click();
            Thread.Sleep(1000);

            IWebElement? novemberOption = null;
            try
            {
                novemberOption = driver.FindElement(By.XPath("//div[contains(@class,'react-datepicker__month-option')]//span[text()='November']"));
            }
            catch
            {
                try
                {
                    novemberOption = driver.FindElement(By.XPath("//div[contains(@class,'react-datepicker__month-option') and contains(text(),'November')]"));
                }
                catch
                {
                    var allMonthOptions = driver.FindElements(By.CssSelector(".react-datepicker__month-option"));
                    foreach (var monthOption in allMonthOptions)
                    {
                        if (monthOption.Text.Contains("November"))
                        {
                            novemberOption = monthOption;
                            break;
                        }
                    }
                }
            }

            if (novemberOption != null)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", novemberOption);
                Thread.Sleep(1000);
            }
            else
            {
                throw new Exception("Could not find November option");
            }

            // Выбираем день 2
            var secondDay = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[contains(@class,'react-datepicker__day') and text()='2' and not(contains(@class,'outside-month'))]")));
            secondDay.Click();
            Thread.Sleep(500);

            // Выбираем время 20:00
            var time2000 = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//li[contains(@class,'react-datepicker__time-list-item') and contains(text(),'20:00')]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", time2000);
            Thread.Sleep(300);
            time2000.Click();
            Thread.Sleep(1000);
        }
    }
}

