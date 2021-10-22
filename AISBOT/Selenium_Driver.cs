using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace AISBOT
{
    class Selenium_Driver
    {
        public static List<IWebDriver> Driver = new List<IWebDriver>();

        private static IJavaScriptExecutor Scripts(IWebDriver driver)
        {
            return (IJavaScriptExecutor)driver;
        }

        private static bool InputDate(IWebDriver driver, String cardNo, String Month, String Year, String seccode)
        {
            while (true)
            {
                Thread.Sleep(1000);
                if (Form1.Exit == true)
                    break;

                try
                {
                    driver.FindElement(By.Id("cardNo")).Clear();
                    Thread.Sleep(500);
                    driver.FindElement(By.Id("cardNo")).SendKeys(cardNo);

                    driver.FindElement(By.Id("securityCode")).Clear();
                    Thread.Sleep(500);
                    driver.FindElement(By.Id("securityCode")).SendKeys(seccode);
                }
                catch (Exception) { }

                try
                {
                    Scripts(driver).ExecuteScript("document.getElementById('epMonth').style.display = 'block'");
                    Scripts(driver).ExecuteScript("document.getElementById('epYear').style.display = 'block'");

                    Thread.Sleep(500);
                    var MonthElement = driver.FindElement(By.Id("epMonth"));
                    var Select_Month = new SelectElement(MonthElement);
                    Select_Month.SelectByValue(Month);

                    var YearElement = driver.FindElement(By.Id("epYear"));
                    var Select_Year = new SelectElement(YearElement);
                    Select_Year.SelectByValue(Year);

                    Thread.Sleep(1000);
                    driver.FindElement(By.Id("nextBtn")).Click();
                    return true;
                }
                catch (Exception) { }
            }
            return false;
        }

        private static string OTPFind(string Ref)
        {
            int Expire = 61;
            while (true)
            {
                Thread.Sleep(1000);

                if (File.Exists("smsreader.txt"))
                {
                    String[] F = File.ReadAllLines("smsreader.txt");
                    for (int i = 0; i < F.Length; i++)
                    {
                        var Rege = Regex.Split(F[i], "=");
                        if (Rege.Length > 1)
                        {
                            if (Ref.ToUpper() == Rege[0].ToUpper())
                                return Rege[1];
                        }
                    }
                }

                Expire--;
                if (Expire <= 0 || Form1.Exit == true)
                    break;
            }
            return "";
        }

        private static string getBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }

        private static JObject InputOTP(IWebDriver driver, string ref_again)
        {
            dynamic Res = new JObject();

            while (true)
            {
                Thread.Sleep(1000);
                try
                {
                    if (Form1.Exit == true)
                        break;

                    string Ref = driver.FindElement(By.Id("otp-ref")).Text;
                    if (Ref != "" && ref_again != Ref)
                    {
                        Res.Ref = Ref;
                        Scripts(driver).ExecuteScript("document.title = 'REF: " + Ref + "'");
                        // Wait OTP
                        string OTP = OTPFind(Ref);
                        if (OTP != "")
                        {
                            Res.OTP = OTP;
                            driver.FindElement(By.Id("otp-input")).SendKeys(OTP);
                            Thread.Sleep(1000);
                            driver.FindElement(By.Id("btn-submit")).Click();
                            Thread.Sleep(10000);
                            return Res;
                        }
                        else
                        {
                            // Reagain
                            driver.FindElement(By.Id("btn-request")).Click();
                            Thread.Sleep(5000);
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        driver.FindElement(By.Id("btn-request")).Click();
                        Thread.Sleep(2000);
                    }
                }
                catch (Exception) { }
            }
            return null;
        }

        private static int CheckPageFinal(IWebDriver driver, String phone)
        {
            try
            {
                if (driver.Url == "https://become-ais-family.ais.co.th/result")
                {
                    try
                    {
                        //paymentSuccess
                        String resultId = (String)Scripts(driver).ExecuteScript("return document.getElementById('paymentSuccess').style.display");
                        var res = getBetween(resultId, "bl", "ck");
                        if (res != "")
                        {
                            ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile("Buy/" + phone + "_success.png");
                            return 1;
                        }
                        else
                        {
                            ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile("Buy/" + phone + "_payment_error.png");
                            return 99;
                        }
                    }
                    catch (Exception) { }
                }
                else
                {
                    String checkinvalid = (String)Scripts(driver).ExecuteScript("return document.getElementById('err-invalid-otp').style.display");
                    var res = getBetween(checkinvalid, "bl", "ck");
                    if (res != "")
                        return 2;
                }
            }
            catch (Exception) { }
            return 0;
        }

        //public static JObject Payment(String uri, Visacard vis, String phone)
        //{
        //    var driverService = ChromeDriverService.CreateDefaultService();
        //    driverService.HideCommandPromptWindow = true;
        //    IWebDriver driver = new ChromeDriver(driverService);

        //    Driver.Add(driver);
        //    driver.Navigate().GoToUrl(uri);
        //    InputDate(driver, vis.idcard, vis.month_exp, vis.year_exp, vis.cvv);
        //    JObject OTPSet = InputOTP(driver, "");
        //    while (true)
        //    {
        //        int result = CheckPageFinal(driver, phone);
        //        if (result == 2)
        //        {
        //            OTPSet = InputOTP(driver, OTPSet["Ref"].ToString());
        //        }
        //        else if (result == 1 || result == 99)
        //        {
        //            OTPSet.Add("status", result.ToString());
        //            driver.Quit();
        //            break;
        //        }
        //        Thread.Sleep(2000);
        //    }
        //    return OTPSet;
        //}

        public static JObject Payment(String uri, Visacard vis, String phone)
        {
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            IWebDriver driver = new ChromeDriver(driverService);

            Driver.Add(driver);
            driver.Navigate().GoToUrl(uri);
            InputDate(driver, vis.idcard, vis.month_exp, vis.year_exp, vis.cvv);
            dynamic OTPSet = new JObject();
            //JObject OTPSet = InputOTP(driver, "");
            //while (true)
            //{
            //    int result = CheckPageFinal(driver, phone);
            //    if (result == 2)
            //    {
            //        OTPSet = InputOTP(driver, OTPSet["Ref"].ToString());
            //    }
            //    else if (result == 1 || result == 99)
            //    {
            //        OTPSet.Add("status", result.ToString());
            //        driver.Quit();
            //        break;
            //    }
            //    Thread.Sleep(2000);
            //}
            return OTPSet;
        }

        public static JObject Test(String uri)
        {
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            IWebDriver driver = new ChromeDriver(driverService);
            Driver.Add(driver);
            driver.Navigate().GoToUrl(uri);
            return null;
        }
    }
}
