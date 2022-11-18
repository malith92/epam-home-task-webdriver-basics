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
        DefaultWait<IWebDriver> defaultWait;
        WebDriverWait webDriverWait;

        // Login credentials
        string EMAIL_ADDRESS = "ta3862989@gmail.com";
        string PASSWORD = "!QAZ2wsx!@";

        // Email to be sent
        string EMAIL_ADDRESSED_TO = "malithwanniarachchi@gmail.com";
        string EMAIL_SUBJECT = "Test Email | ";
        string EMAIL_CONTENT = "Test Email Content";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            driver = new ChromeDriver();

            driver.Manage().Window.Maximize();

            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

            // Initialize defaultWait
            defaultWait = new DefaultWait<IWebDriver>(driver);
            defaultWait.Timeout = TimeSpan.FromSeconds(5);
            defaultWait.PollingInterval = TimeSpan.FromMilliseconds(5000);
            defaultWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            defaultWait.IgnoreExceptionTypes(typeof(ElementClickInterceptedException));
            defaultWait.Message = "Element to be searched not found";

            // Initialize webDriverWait
            webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.Navigate().GoToUrl("https://accounts.google.com/ServiceLogin/signinchooser?service=mail");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            driver.Quit();
        }

        [Test, Order(1)]
        public void LoginTest()
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
            IWebElement passwordField = defaultWait.Until(x => x.FindElement(By.Name("Passwd")));
            Console.WriteLine("passwordField is displayed : " + passwordField.Displayed);
            passwordField.SendKeys(PASSWORD);

            // Click next button
            IWebElement passwordNextButton = driver.FindElement(By.XPath("//div[@id='passwordNext']/div/button"));
            passwordNextButton.Click();

            // Wait till the gmail inbox page loads
            webDriverWait.Until((IWebDriver driver) =>
            {
                IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)driver;

                string script = "return document.readyState";

                bool scriptOutput = javaScriptExecutor.ExecuteScript(script).Equals("complete");

                return scriptOutput;
            });

            // Get Gmail image displayed state
            IWebElement gmailImage = defaultWait.Until(x => x.FindElement(By.XPath("//a[@title='Gmail']/img")));
            bool gmailLinkDisplayed = gmailImage.Displayed;
            Console.WriteLine("Gmail Image is displayed : " + gmailLinkDisplayed);

            // Assert Login is successful
            Assert.IsTrue(gmailLinkDisplayed);
        }

        [Test, Order(2)]
        public void VerifyDraftedEmailPresentInDrafts()
        {
            // Click Compose button
            IWebElement composeButton = driver.FindElement(By.XPath("//div[text()='Compose']"));
            Console.WriteLine("composeButton is displayed : " + composeButton.Displayed);
            composeButton.Click();

            // Type receiver's email address
            IWebElement receiverAddressTxtField = defaultWait.Until(x => x.FindElement(By.XPath("//div[@aria-label='Search Field']/div/input")));
            receiverAddressTxtField.SendKeys(EMAIL_ADDRESSED_TO);

            // Type email subject
            IWebElement subjectTxtField = driver.FindElement(By.XPath("//input[@aria-label='Subject']"));
            EMAIL_SUBJECT += DateTime.Now.ToString("F");
            subjectTxtField.SendKeys(EMAIL_SUBJECT);

            // Type email content
            IWebElement emailContentField = driver.FindElement(By.XPath("//div[@aria-label='Message Body']"));
            emailContentField.SendKeys(EMAIL_CONTENT);

            // Click close button
            IWebElement closeButton = driver.FindElement(By.XPath("//img[@aria-label='Save & close']"));
            closeButton.Click();

            // Click on Drafts link
            IWebElement draftsLink = defaultWait.Until(x => x.FindElement(By.LinkText("Drafts")));
            draftsLink.Click();

            // Click on Draft email
            IWebElement previouslyCreatedDraftEmailElement = defaultWait.Until(x => x.FindElement(By.XPath("//span[text()='"+EMAIL_SUBJECT+"']/parent::span/parent::div")));
            previouslyCreatedDraftEmailElement.Click();
            
            bool draftedEmailIsDisplayed = previouslyCreatedDraftEmailElement.Displayed;

            Assert.IsTrue(draftedEmailIsDisplayed);
        }

        [Test, Order(3)]
        public void VerifyDraftEmailReceiver()
        {
            IWebElement draftEmailAddressedToElement = defaultWait.Until(x => x.FindElement(By.XPath("//span[@email='"+ EMAIL_ADDRESSED_TO + "']")));
            bool draftEmailAddressedTo = draftEmailAddressedToElement.Displayed;

            Assert.IsTrue(draftEmailAddressedTo);
        }

        [Test, Order(4)]
        public void VerifyDraftEmailSubject()
        {
            IWebElement draftEmailSubjectElement = defaultWait.Until(x => x.FindElement(By.Name("subject")));
            string draftEmailSubject = draftEmailSubjectElement.GetAttribute("value");

            Assert.AreEqual(EMAIL_SUBJECT, draftEmailSubject);
        }

        [Test, Order(5)]
        public void VerifyDraftEmailBody()
        {
            IWebElement draftEmailBodyElement = defaultWait.Until(x => x.FindElement(By.XPath("//div[@aria-label='Message Body']")));
            string draftEmailBody = draftEmailBodyElement.Text;

            Assert.AreEqual(EMAIL_CONTENT, draftEmailBody);
        }

        [Test, Order(6)]
        public void VerifyDraftEmailDissapearedFromDrafts()
        {
            // Click send button
            IWebElement sendButtonElement = defaultWait.Until(x => x.FindElement(By.XPath("//div[text()='Send']")));
            sendButtonElement.Click();

            bool draftedEmailIsDisplayed;
            try
            {
                IWebElement previouslyCreatedDraftEmailElement = defaultWait.Until(x => x.FindElement(By.XPath("//span[text()='" + EMAIL_SUBJECT + "']/parent::span/parent::div")));
                draftedEmailIsDisplayed = previouslyCreatedDraftEmailElement.Displayed;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                draftedEmailIsDisplayed = false;
            }

            Assert.IsFalse(draftedEmailIsDisplayed);
        }

        [Test, Order(7)]
        public void VerifyMailIsinSentFolder()
        {
            // Click on Sent link
            IWebElement sentLink = defaultWait.Until(x => x.FindElement(By.LinkText("Sent")));
            sentLink.Click();

            // Find the sent email in Sent folder
            IWebElement sentEmailElement = defaultWait.Until(x => x.FindElement(By.XPath("//span[text()='" + EMAIL_SUBJECT + "']/parent::span/parent::div")));
            bool sentEmailIsDisplayed = sentEmailElement.Displayed;

            Assert.IsTrue(sentEmailIsDisplayed);
        }

        [Test, Order(8)]
        public void LogOffTest()
        {
            // Click on account avatar
            IWebElement avatarLinkElement = defaultWait.Until(x => x.FindElement(By.XPath("//a[contains(@aria-label,'Google Account: Test Account')]")));
            avatarLinkElement.Click();

            // Switch to account iframe
            driver.SwitchTo().Frame("account");

            // Click on Sigon out button
            IWebElement signOutElement = defaultWait.Until(x => x.FindElement(By.XPath("//div[text()='Sign out']")));
            signOutElement.Click();

            // Find Account choose element in sigin in page
            IWebElement chooseAccountElement = defaultWait.Until(x => x.FindElement(By.XPath("//div[text()='Test Account']")));

            bool chooseAccountElementIsDisplayed = chooseAccountElement.Displayed;

            Assert.IsTrue(chooseAccountElementIsDisplayed);
        }
    }
}