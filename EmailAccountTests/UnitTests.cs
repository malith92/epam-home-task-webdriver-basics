using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace EmailAccountTests
{
    [TestFixture]
    public class Tests
    {
        IWebDriver driver;
        DefaultWait<IWebDriver> fluentWait;
        string EMAIL_ADDRESS = "ta3862989@gmail.com";
        string PASSWORD = "!QAZ2wsx!@";
        string RECEIVERS_EMAIL_ADDRESS = "malithwanniarachchi@gmail.com";
        //string CURRENT_DATE_TIME = DateTime.Now.ToString("F");
        string EMAIL_SUBJECT = "Test Email";
        string EMAIL_CONTENT = "Test Email Content";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            driver = new ChromeDriver();

            driver.Manage().Window.Maximize();

            fluentWait = new DefaultWait<IWebDriver>(driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(5);
            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(5000);

            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            fluentWait.Message = "Element to be searched not found";

            driver.Navigate().GoToUrl("https://accounts.google.com/ServiceLogin/signinchooser?service=mail");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            //driver.Close();
        }

        private void LoginSteps()
        {
            // Type user name
            IWebElement emailAddressField = driver.FindElement(By.Id("identifierId"));
            Console.WriteLine("emailAddressField is displayed : " + emailAddressField.Displayed);
            emailAddressField.SendKeys(EMAIL_ADDRESS);

            // Click next button
            IWebElement emailAddressNextButton = driver.FindElement(By.XPath("//div[@id='identifierNext']/div/button"));
            Console.WriteLine("emailAddressNextButton is displayed : " + emailAddressNextButton.Displayed);
            emailAddressNextButton.Click();

            // Type password
            IWebElement passwordField = fluentWait.Until(x => x.FindElement(By.Name("Passwd")));
            Console.WriteLine("passwordField is displayed : " + passwordField.Displayed);
            passwordField.SendKeys(PASSWORD);

            // Click next button
            IWebElement passwordNextButton = driver.FindElement(By.XPath("//div[@id='passwordNext']/div/button"));
            passwordNextButton.Click();

            // Wait till the gmail inbox page loads
            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until((IWebDriver driver) =>
            {
                IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)driver;

                string script = "return document.readyState";

                bool scriptOutput = javaScriptExecutor.ExecuteScript(script).Equals("complete");

                return scriptOutput;
            });
        }

        [Test]
        public void LoginTest()
        {
            LoginSteps();

            // Get Gmail image displayed state
            IWebElement gmailImage = fluentWait.Until(x => x.FindElement(By.XPath("//a[@title='Gmail']/img")));
            bool gmailLinkDisplayed = gmailImage.Displayed;
            Console.WriteLine("Gmail Image is displayed : " + gmailLinkDisplayed);

            // Assert Login is successful
            Assert.IsTrue(gmailLinkDisplayed);
        }

        private void DraftEmailSteps()
        {
            // Click Compose button
            IWebElement composeButton = driver.FindElement(By.XPath("//div[text()='Compose']"));
            Console.WriteLine("composeButton is displayed : " + composeButton.Displayed);
            composeButton.Click();

            // Type receiver's email address
            IWebElement receiverAddressTxtField = fluentWait.Until(x => x.FindElement(By.XPath("//div[@aria-label='Search Field']/div/input")));
            //IWebElement receiverAddressTxtField = driver.FindElement(By.XPath("//div[@aria-label='Search Field']/div/input"));
            receiverAddressTxtField.SendKeys(RECEIVERS_EMAIL_ADDRESS);

            // Type email subject
            IWebElement subjectTxtField = driver.FindElement(By.XPath("//input[@aria-label='Subject']"));
            subjectTxtField.SendKeys(EMAIL_SUBJECT);

            // Type email content
            IWebElement emailContentField = driver.FindElement(By.XPath("//div[@aria-label='Message Body']"));
            emailContentField.SendKeys(EMAIL_CONTENT);

            // Click close button
            IWebElement closeButton = driver.FindElement(By.XPath("//img[@aria-label='Save & close']"));
            closeButton.Click();

        }

        private void ViewDraftEmailsSteps()
        {
            // Click on Drafts link
            IWebElement draftsLink = fluentWait.Until(x => x.FindElement(By.XPath("//a[text()='Drafts']")));
            //IWebElement draftsLink = driver.FindElement(By.XPath("//a[text()='Drafts']"));
            draftsLink.Click();

            //Click on Draft email
            IWebElement draftEmailLink = fluentWait.Until(x => x.FindElement(By.XPath("//span[text()='Test Email']")));
            draftEmailLink.Click();

            //Get receivers email address from draft email
            IWebElement draftEmailReceiversAddress = fluentWait.Until(x => x.FindElement(By.XPath("//div[@id=':7n']/span")));
            string draftEmailReceiversEmailAddress = draftEmailReceiversAddress.GetAttribute("email");

            IWebElement draftEmailSubjectElement = fluentWait.Until(x => x.FindElement(By.XPath("//input[@name='subject']")));
            string draftEmailSubject = draftEmailSubjectElement.GetAttribute("value");

            IWebElement draftEmailBodyElement = fluentWait.Until(x => x.FindElement(By.XPath("//input[@aria-label='Message Body']")));
            string draftEmailBody = draftEmailBodyElement.Text;

            Assert.AreEqual(RECEIVERS_EMAIL_ADDRESS, draftEmailReceiversEmailAddress);
            Assert.AreEqual(EMAIL_SUBJECT, draftEmailSubject);
            Assert.AreEqual(EMAIL_CONTENT, draftEmailBody);
        }

        [Test]
        public void verifyDraftEmail()
        {
            DraftEmailSteps();
            ViewDraftEmailsSteps();


        }
    }
}