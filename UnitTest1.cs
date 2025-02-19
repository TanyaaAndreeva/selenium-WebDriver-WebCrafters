using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Text;

namespace ExamPrep2
{
    public class Tests
    {
        private WebDriver driver;
        private readonly static string BaseUrl = "https://d3s5nxhwblsjbi.cloudfront.net";
        private Actions actions;
        private static string? lastCreatedRevueTitle;
        private static string? lastCreatedRevueDescription;

        [OneTimeSetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            actions = new Actions(driver);
            driver.Navigate().GoToUrl(BaseUrl);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);



            driver.Navigate().GoToUrl($"{BaseUrl}/Users/Login");

            var loginForm = driver.FindElement(By.XPath("//form[@method='post']"));
            actions.ScrollToElement(loginForm).Perform();

            driver.FindElement(By.XPath("//input[@type='email']")).SendKeys("test_tanya@test.bg");
            driver.FindElement(By.XPath("//input[@type='password']")).SendKeys("123456");

            driver.FindElement(By.XPath("//button[@type='submit']")).Click();
        }
        [OneTimeTearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }
        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVYXYZ";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [Test, Order(1)]
        public void CreateReue_WithInvalidData_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Revue/Create");

            var formCard = driver.FindElement(By.XPath("//div[@class='row justify-content-center']"));
            actions.ScrollToElement(formCard).Perform();

            var titleInput = driver.FindElement(By.XPath("//input[@id='form3Example1c']"));
            titleInput.Clear();
            titleInput.SendKeys("");

            var descriptionInput = driver.FindElement(By.XPath("//textarea[@id='form3Example4cd']"));
            descriptionInput.Clear();
            descriptionInput.SendKeys("");

            driver.FindElement(By.XPath("//button[@type='submit']")).Click();

            var currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo($"{BaseUrl}/Revue/Create"), "user shouldl remain on the same page with same URL");


            var errorMessage = driver.FindElement(By.XPath("//ul/li[text()='Unable to create new Revue!']"));
            Assert.That(errorMessage.Text, Is.EqualTo("Unable to create new Revue!"), "The error message for invalid data when creating Revue is not there");
        }
        [Test, Order(2)]
        public void Create_RandomRevue_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Revue/Create");

            var formCard = driver.FindElement(By.XPath("//div[@class='row justify-content-center']"));
            actions.ScrollToElement(formCard).Perform();

            var titleInput = driver.FindElement(By.Id("form3Example1c"));
            titleInput.Clear();
            lastCreatedRevueTitle = GenerateRandomString(6);
            titleInput.SendKeys(lastCreatedRevueTitle);


            var descriptionInput = driver.FindElement(By.Id("form3Example4cd"));
            descriptionInput.Clear();
            lastCreatedRevueDescription = GenerateRandomString(20);
            descriptionInput.SendKeys(lastCreatedRevueDescription);


            driver.FindElement(By.XPath("//button[@type='submit' and text()='Create']")).Click();

            var currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo($"{BaseUrl}/Revue/MyRevues"), "User was not redirected to My Revues Page.");



            var revues = driver.FindElements(By.CssSelector(".card.mb-4"));
            var lastRevueTitle = revues.Last().FindElement(By.CssSelector(".text-muted")).Text;
            Assert.That(lastRevueTitle, Is.EqualTo(lastCreatedRevueTitle), "The last Revue is not present on the screen.");
        }
        [Test, Order(3)]
        public void TestSearchRevueByTitle()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Revue/MyRevues");

            var searchField = driver.FindElement(By.XPath("//input[@type='search']"));
            actions.ScrollToElement(searchField).Perform();

            searchField.SendKeys(lastCreatedRevueTitle);
            driver.FindElement(By.Id("search-button")).Click();

            var searchResutRevueTitle = driver.FindElement(By.CssSelector(".text-muted")).Text;
            Assert.That(searchResutRevueTitle, Is.EqualTo(lastCreatedRevueTitle), "The search resulting Revue is not present on the screen.");

        }
        [Test, Order(4)]
        public void EditLastCreatedReviueTitle_Test()
        {

            driver.Navigate().GoToUrl($"{BaseUrl}/Revue/MyRevues");

            var revues = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            Assert.That(revues.Count(), Is.AtLeast(1), "There are no Revues");

            var lastRevue = revues.Last();
            actions.ScrollToElement(lastRevue).Perform();

            driver.FindElement(By.XPath($"//div[contains(@class, 'card') and .//div[@class='text-muted text-center' and text() ='{lastCreatedRevueTitle}']]//a[text()='Edit']")).Click();

            var formCard = driver.FindElement(By.XPath("//div[@class='row justify-content-center']"));
            actions.ScrollToElement(formCard).Perform();

            var titleInput = driver.FindElement(By.Id("form3Example1c"));
            titleInput.Clear();
            lastCreatedRevueTitle = GenerateRandomString(6) + " edited";
            titleInput.SendKeys(lastCreatedRevueTitle);

            driver.FindElement(By.XPath("//button[@type='submit']")).Click();

            var currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo($"{BaseUrl}/Revue/MyRevues"), "User was not redirected to My Revues Page.");

            var revuesResult = driver.FindElements(By.CssSelector(".card.mb-4"));
            var lastRevueTitle = revuesResult.Last().FindElement(By.CssSelector(".text-muted")).Text;
            Assert.That(lastRevueTitle, Is.EqualTo(lastCreatedRevueTitle), "The last Revue is not present on the screen.");

        }


        [Test, Order(5)]
        public void DeleteTheLastCreatedRevue_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Revue/MyRevues");

            var revues = driver.FindElements(By.XPath("//div[@class='card mb-4 box-shadow']"));
            Assert.That(revues.Count(), Is.AtLeast(1), "There are no Revues");

            var lastRevue = revues.Last();
            actions.ScrollToElement(lastRevue).Perform();

            driver.FindElement(By.XPath($"//div[contains(@class, 'card') and .//div[@class='text-muted text-center' and text() ='{lastCreatedRevueTitle}']]//a[text()='Delete']")).Click();

            var currentUrl = driver.Url;
            Assert.That(currentUrl, Is.EqualTo($"{BaseUrl}/Revue/MyRevues"), "User was not redirected to My Revues Page.");

            var revuesResult = driver.FindElements(By.CssSelector(".card.mb-4"));
            Assert.That(revuesResult.Count(), Is.LessThan(revues.Count()), "The number of Revues did not decrease");

            var lastRevueTitle = revuesResult.Last().FindElement(By.CssSelector(".text-muted")).Text;
            Assert.That(lastRevueTitle, !Is.EqualTo(lastCreatedRevueTitle), "The last Revue is not present on the screen.");


        }
        [Test, Order(6)]
        public void SearchNonExistinRevue_Test()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Revue/MyRevues");

            var searchField = driver.FindElement(By.Id("keyword"));
            actions.ScrollToElement(searchField).Perform();

            searchField.SendKeys("non-existing-revue");

            driver.FindElement(By.Id("search-button")).Click();

            var noResultsMessage = driver.FindElement(By.CssSelector(".text-muted")).Text;

            Assert.That(noResultsMessage, Is.EqualTo("No Revues yet!"));
        }

    }


}
