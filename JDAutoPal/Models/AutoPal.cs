using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System.Drawing;
using System.Windows.Automation;
using System.ComponentModel;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Forms = System.Windows.Forms;
using System.Reflection;
using JDAutoPal.Properties;
using System.Net;
using System.Net.Sockets;


namespace JDAutoPal.Models
{
    public struct AccountInfo
    {
        public string Account;
        public string Password;
        public string FullName;
        public string Mobile;
        public string Province;
        public string City;
        public string County;
        public string Town;
        public string DetailAddress;
    }

    public struct JDAccount
    {
        public string UserName;
        public string Password;
    }

    public class AutoPal: INotifyPropertyChanged, IDisposable
    {
        //Properties
        private IWebDriver driver;
        private List<AccountInfo> aAccountInfo;
        private List<JDAccount> aJDAccount;
        private CancellationTokenSource cts;
        private string OrderMoney;
        private string OrderNo;

        private int _singlePalCount;
        public int SinglePalCount
        {
            get
            {
                return _singlePalCount;
            }
            set
            {
                _singlePalCount = value;
                OnPropertyChanged("SinglePalCount");
            }
        }

        private int _browserIndex;
        public int BrowserIndex
        {
            get
            {
                return _browserIndex;
            }
            set
            {
                _browserIndex = value;
                OnPropertyChanged("BrowserIndex");
            }        
        }


        private string _localIpAddress;
        public string LocalIpAddress
        {
            get
            {
                return _localIpAddress;
            }
            set
            {
                _localIpAddress = value;
                OnPropertyChanged("LocalIpAddress");
            }
        }

        private int _successPalCount;
        public int SuccessPalCount
        {
            get
            {
                return _successPalCount;
            }
            set
            {
                _successPalCount = value;
                OnPropertyChanged("SuccessPalCount");
            }
        }    

        //Functions
        public AutoPal()
        {
            m_disposed = false;
            BrowserIndex = 0;
            SinglePalCount = 1;
            OrderMoney = "100";
            OrderNo = "T123456789";
            SuccessPalCount = 0;
            LocalIpAddress = GetIpAddress();
            aAccountInfo = new List<AccountInfo>();
            aJDAccount = new List<JDAccount>();
        }

        public void InitCTS()
        {
            cts = new CancellationTokenSource();
        }

        public void CancelWaitting()
        {
            if(cts != null)
                cts.Cancel();
        }

        public bool OpenBrowser(int BrowserIndex)
        {
            Trace.TraceInformation("Rudy Trace =>OpenBrowser: Set webdriver");
            string DriverTitle = System.Environment.CurrentDirectory;
            if (BrowserIndex == 0)
            {
                string ProfilePath = Environment.GetEnvironmentVariable("LocalAppData") + "\\Google\\Chrome\\User Data";
                var Options = new ChromeOptions();
                Options.AddArguments("--incognito");
                Options.AddArguments("--user-data-dir=" + ProfilePath);
                Options.AddArguments("--disable-extensions");

                driver = new ChromeDriver(Options);
                DriverTitle += "\\chromedriver.exe";
                Trace.TraceInformation("Rudy Trace =>OpenBrowser: driver = [{0}]", DriverTitle);

                App.WindowHide(DriverTitle);
            }
            else if (BrowserIndex == 1)
            {
                driver = new InternetExplorerDriver();
                DriverTitle += "\\IEDriverServer.exe";
                App.WindowHide(DriverTitle);
            }
            else if (BrowserIndex == 2)
            {
                //string firefox_path = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                //FirefoxBinary binary = new FirefoxBinary(firefox_path);
                FirefoxProfile profile = new FirefoxProfile();
                profile.SetPreference("network.proxy.type", 0);
                driver = new FirefoxDriver(profile);
            }
            else
            {
                Trace.TraceInformation("Rudy Trace =>Invalid Browser Type.");
                return false;
            }

            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(60));
            //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
            
            return true;
        }

        public async Task<IWebElement>  WaitForElementAsync(string el_mark, string el_flag,  int timeout = 60)
        {
            IWebElement elementFound = await Task.Run(() =>
            {
                Trace.TraceInformation("Rudy Trace =>Searching element: [{0}]", el_mark);
                IWebElement ele = null;
                DateTime begins = DateTime.Now;
                TimeSpan span = DateTime.Now - begins;
                while ((span.TotalSeconds < timeout) && (ele == null))
                {
                    try
                    {
                        if (el_flag.Equals("Id"))
                            ele = driver.FindElement(By.Id(el_mark));
                        else if (el_flag.Equals("Class"))
                            ele = driver.FindElement(By.ClassName(el_mark));
                        else if (el_flag.Equals("Name"))
                            ele = driver.FindElement(By.Name(el_mark));
                        else if (el_flag.Equals("XPath"))
                            ele = driver.FindElement(By.XPath(el_mark));
                        else
                        {
                            Trace.TraceInformation("Rudy Trace =>Element flag is invalid.");
                            return null;
                        }
                        if (!ele.Displayed)
                        {
                            ele = null;
                        }
                    }
                    catch(Exception)
                    {
                        cts.Token.ThrowIfCancellationRequested();
                    }     
                    span = DateTime.Now - begins;
                }
                if(ele != null)
                    Trace.TraceInformation("Rudy Trace =>Found element: [{0}]", el_mark);
                else
                    Trace.TraceInformation("Rudy Trace =>Time out to find element: [{0}]", el_mark);
                return ele;
            }, cts.Token);

            return elementFound;
        }


        public void WaitForPageLoad(int maxWaitTimeInSeconds = 60) 
        {
            string state = string.Empty;
            try 
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(maxWaitTimeInSeconds));

                //Checks every 500 ms whether predicate returns true if returns exit otherwise keep trying till it returns ture
                wait.Until(d => 
                {
                    try 
                    {
                        state = ((IJavaScriptExecutor) driver).ExecuteScript(@"return document.readyState").ToString();
                    } 
                    catch (InvalidOperationException) 
                    {
                        //Ignore
                    } 
                    catch (NoSuchWindowException) 
                    {
                        //when popup is closed, switch to last windows
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                    }
                    //In IE7 there are chances we may get state as loaded instead of complete
                    return (state.Equals("complete", StringComparison.InvariantCultureIgnoreCase) || state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase));
                });
            } 
            catch (TimeoutException) 
            {
                //sometimes Page remains in Interactive mode and never becomes Complete, then we can still try to access the controls
                if (!state.Equals("interactive", StringComparison.InvariantCultureIgnoreCase))
                    throw;
            } 
            catch (NullReferenceException) 
            {
                //sometimes Page remains in Interactive mode and never becomes Complete, then we can still try to access the controls
                if (!state.Equals("interactive", StringComparison.InvariantCultureIgnoreCase))
                    throw;
            } 
            catch (WebDriverException) 
            {
                if (driver.WindowHandles.Count == 1) 
                {
                    driver.SwitchTo().Window(driver.WindowHandles[0]);
                }
                state = ((IJavaScriptExecutor) driver).ExecuteScript(@"return document.readyState").ToString();
                if (!(state.Equals("complete", StringComparison.InvariantCultureIgnoreCase) || state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase)))
                    throw;
            }
        }


        public async Task<bool> WaitForPageAsync(string PageTitle, int timeout = 120)
        {
            bool bRet = await Task.Run(() =>
            {
                Trace.TraceInformation("Rudy Trace =>WaitForPageAsync: Waitting for page [{0}]...", PageTitle);
                string defaultWindow = driver.CurrentWindowHandle;
                DateTime begins = DateTime.Now;
                TimeSpan span = DateTime.Now - begins;
                while (span.TotalSeconds < timeout)
                {
                    foreach (string strWindow in driver.WindowHandles)
                    {
                        cts.Token.ThrowIfCancellationRequested();// Throw the Cancellation Request.
                        try
                        {

                            driver.SwitchTo().Window(strWindow);
                            //Trace.TraceInformation("Rudy Trace =>WaitForPageAsync: Switch to page [{0}]", driver.Title);
                            if (driver.Title.Contains(PageTitle))
                            {
                                Trace.TraceInformation("Rudy Trace =>WaitForPageAsync: Page [{0}] Load Succeed!", PageTitle);
                                return true;
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.TraceInformation("Rudy Exception=> WaitForPageAsync: " + e.Message);
                        }
                    }
                    span = DateTime.Now - begins;
                }
                //Trace.TraceInformation("Rudy Trace =>Switch to default window.");
                //driver.SwitchTo().Window(defaultWindow);
                Trace.TraceInformation("Rudy Trace =>WaitForPageAsync: Page [{0}] Load Time Out!", PageTitle);
                return false;
            }, cts.Token);
            return bRet;
        }


        public async Task<bool> CreateDDPalReportAsync(string FilePath)
        {
            Trace.TraceInformation("Rudy Trace =>CreateDDPalReportAsync: Report Path = " + FilePath);
            bool bRet = await Task.Run(() =>
            {
                try
                {
                    Application excel = new Application();
                    excel.Visible = false;
                    Workbook wb = excel.Workbooks.Add();
                    Worksheet ws = wb.Sheets[1] as Worksheet;

                    ws.Cells[1, 1] = "账户";
                    ws.Cells[1, 2] = "密码";
                    ws.Cells[1, 3] = "订单编号";
                    ws.Cells[1, 4] = "数量";
                    ws.Cells[1, 5] = "金额(元)";
                    ws.Cells[1, 6] = "备注";
                    ws.Cells[1, 7] = "已评论";
                    for (int i = 1; i < 8; i++)
                    {
                        ((Range)(ws.Cells[1, i])).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                        ((Range)(ws.Cells[1, i])).ColumnWidth = 12;
                    }

                    wb.SaveAs(FilePath);

                    if (wb != null)
                        wb.Close();
                    if (excel != null)
                    {
                        excel.Quit();
                        App.KillExcel(excel);
                        excel = null;
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.Message, Globals.JD_CAPTION);
                    Trace.TraceInformation("Rudy Exception=> CreateDDPalReportAsync： " + e.Source + ";" + e.Message);
                    return false;
                }

                return true;
            }).ConfigureAwait(false);
            return bRet;
        }

        public async Task<bool> UpdateDDPalReportAsync(string FilePath, int AccountNo, bool bSuccess)
        {
            bool bRet = await Task.Run(() =>
            {
                try
                {
                    Application excel = new Application();
                    excel.Visible = false;
                    Workbook wb = excel.Workbooks.Open(FilePath);
                    Worksheet ws = wb.ActiveSheet as Worksheet;

                    int nRow = AccountNo + 1;
                    int nAccountIndex = AccountNo - 1;

                    for (int i = 1; i < 8; i++)
                    {
                        ((Range)(ws.Cells[nRow, i])).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                    }

                    ws.Cells[nRow, 1] = aJDAccount[nAccountIndex].UserName;
                    ws.Cells[nRow, 2] = aJDAccount[nAccountIndex].Password;
                    if (bSuccess)
                    {
                        ws.Cells[nRow, 3] = OrderNo;
                        ws.Cells[nRow, 4] = SinglePalCount;
                        ws.Cells[nRow, 5] = OrderMoney;
                        ws.Cells[nRow, 6] = Settings.Default.Remark;
                        ws.Cells[nRow, 7] = "否";
                    }
                    else
                    {
                        ((Range)(ws.Cells[nRow, 1])).Interior.ColorIndex = 3;
                        ((Range)(ws.Cells[nRow, 2])).Interior.ColorIndex = 3;
                    }
                    
                    wb.Save();

                    if (wb != null)
                        wb.Close();
                    if (excel != null)
                    {
                        excel.Quit();
                        App.KillExcel(excel);
                        excel = null;
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.Message, Globals.JD_CAPTION);
                    Trace.TraceInformation("Rudy Exception=> UpdateDDPalReportAsync： " + e.Source + ";" + e.Message);
                    return false;
                }

                return true;
            }).ConfigureAwait(false);
            return bRet;
        }

        public async Task<bool> SetJDAccoutInfoAsync(string FilePath)
        {
            bool bRet = await Task.Run(() =>
            {
                if (aJDAccount != null)
                    aJDAccount.Clear();
                try
                {
                    Application excel = new Application();
                    excel.Visible = false;
                    Workbook wb = excel.Workbooks.Open(FilePath);
                    Worksheet ws = wb.ActiveSheet as Worksheet;
                    int nRowCount = ws.UsedRange.Cells.Rows.Count;//get the used rows count.

                    JDAccount infoTemp;

                    for (int i = 2; i <= nRowCount; i++)
                    {
                        infoTemp.UserName = ((Range)ws.Cells[i, 1]).Text;
                        infoTemp.Password = ((Range)ws.Cells[i, 2]).Text;
                        aJDAccount.Add(infoTemp);
                    }
                    if (wb != null)
                        wb.Close();
                    if (excel != null)
                    {
                        excel.Quit();
                        App.KillExcel(excel);
                        excel = null;
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.Message, Globals.JD_CAPTION);
                    Trace.TraceInformation("Rudy Exception=> SetJDAccoutInfoAsync： " + e.Source + ";" + e.Message);
                    return false;
                }

                return true;
            }).ConfigureAwait(false);
            return bRet;
        }

        public async Task<bool> SetAddressAccoutInfoAsync(string FilePath)
        {
            bool bRet = await Task.Run(() =>
            {
                if (aAccountInfo != null)
                    aAccountInfo.Clear();
                try
                {
                    Application excel = new Application();
                    excel.Visible = false;
                    Workbook wb = excel.Workbooks.Open(FilePath);
                    Worksheet ws = wb.ActiveSheet as Worksheet;
                    int nRowCount = ws.UsedRange.Cells.Rows.Count;//get the used rows count.

                    AccountInfo infoTemp;
                    for (int i = 2; i <= nRowCount; i++)
                    {
                        infoTemp.Account = ((Range)ws.Cells[i, 1]).Text;
                        infoTemp.Password = ((Range)ws.Cells[i, 2]).Text;
                        infoTemp.FullName = ((Range)ws.Cells[i, 3]).Text;
                        infoTemp.Mobile = ((Range)ws.Cells[i, 4]).Text;
                        infoTemp.Province = ((Range)ws.Cells[i, 5]).Text;
                        infoTemp.City = ((Range)ws.Cells[i, 6]).Text;
                        infoTemp.County = ((Range)ws.Cells[i, 7]).Text;
                        infoTemp.Town = ((Range)ws.Cells[i, 8]).Text;
                        infoTemp.DetailAddress = ((Range)ws.Cells[i, 9]).Text;
                        aAccountInfo.Add(infoTemp);
                    }
                    if (wb != null)
                        wb.Close();
                    if (excel != null)
                    {
                        excel.Quit();
                        App.KillExcel(excel);
                        excel = null;
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.Message, Globals.JD_CAPTION);
                    Trace.TraceInformation("Rudy Exception=> SetAddressAccoutInfoAsync： " + e.Source + ";" + e.Message);
                    return false;
                }

                return true;
            }).ConfigureAwait(false);
            return bRet;
        }

        public async Task<bool> LoginAsync(string Account, string Password)
        {
            try
            {
                Trace.TraceInformation("Rudy Trace =>LoginAsync: Login page loading...");
                driver.Navigate().GoToUrl(Globals.LOGIN_URL);
                Trace.TraceInformation("Rudy Trace =>LoginAsync: Log in page load complete.");
                

                driver.SwitchTo().Window(driver.WindowHandles.Last());

                var inputUserName = await WaitForElementAsync(Globals.LOGIN_NAME_ID, "Id").ConfigureAwait(false);
                Trace.TraceInformation("Rudy Trace =>LoginAsync: input user name.");
                inputUserName.Clear();
                inputUserName.SendKeys(Account);

                var inputPassword = await WaitForElementAsync(Globals.LOGIN_PASSWORD_ID, "Id").ConfigureAwait(false);
                Trace.TraceInformation("Rudy Trace =>LoginAsync: input password.");
                inputPassword.Clear();
                inputPassword.SendKeys(Password);

                await Task.Delay(1000).ConfigureAwait(false);

                var btnLogin = await WaitForElementAsync(Globals.LOGIN_SUBMIT_ID, "Id").ConfigureAwait(false);
                Trace.TraceInformation("Rudy Trace =>LoginAsync: click login button");
                btnLogin.Click();

                bool bRet = await WaitForPageAsync(Globals.LOGGEDIN_TITLE).ConfigureAwait(false);
                if (!bRet)
                {
                    Trace.TraceInformation("Rudy Trace =>Log in time out.");
                    return false;
                }

            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                {
                    throw e;
                }
                Trace.TraceInformation("Rudy Exception =>LoginAsync: {0};{1}", e.Source, e.Message);
                return false;
            }
            
            Trace.TraceInformation("Rudy Trace =>Log in succeed.");
            return true;
        }

        public async Task<bool> BindingDeliveryAddressAsync(AccountInfo info)
        {            
            try
            {
                await PrepareEnvironmentAsync(BrowserIndex).ConfigureAwait(false);

                bool bRet = OpenBrowser(BrowserIndex);
                if (!bRet)
                {
                    Trace.TraceInformation("Rudy Trace =>BindingDeliveryAddressAsync: Open browser failed.");
                    return false;
                } 

                bRet = await LoginAsync(info.Account, info.Password).ConfigureAwait(false);
                if (!bRet)
                {
                    Trace.TraceInformation("Rudy Trace =>BindingDeliveryAddressAsync: Log in Failed!");
                    return false;
                }

            
                driver.Navigate().GoToUrl(Globals.EASYBUY_URL);

                var btnAddAddress = await WaitForElementAsync(Globals.BTN_ADD_ADDRESS_XPATH, "XPath").ConfigureAwait(false);
                btnAddAddress.Click();

                var dlgAddressPop = await WaitForElementAsync(Globals.DLG_ADDRESS_POP_ID, "Id");
                if(dlgAddressPop == null)
                {
                    Trace.TraceInformation("Rudy Trace =>BindingDeliveryAddressAsync: Open address input dialog failed!");
                    return false;
                }

                var inputShipMan = await WaitForElementAsync(Globals.SHIP_MAN_ID, "Id").ConfigureAwait(false);
                inputShipMan.SendKeys(info.FullName);


                bRet = await SelectElementByTextAsync(Globals.SELECT_PROVINCE_ID, "Id", info.Province).ConfigureAwait(false);
                if(!bRet)
                {
                    Trace.TraceInformation("Rudy Trace =>BindingDeliveryAddressAsync: Select Province failed!");
                    return false;
                }

                bRet = await SelectElementByTextAsync(Globals.SELECT_CITY_ID, "Id", info.City).ConfigureAwait(false);
                if (!bRet)
                {
                    Trace.TraceInformation("Rudy Trace =>BindingDeliveryAddressAsync: Select City failed!");
                    return false;
                }

                bRet = await SelectElementByTextAsync(Globals.SELECT_COUNTY_ID, "Id", info.County).ConfigureAwait(false);
                if (!bRet)
                {
                    Trace.TraceInformation("Rudy Trace =>BindingDeliveryAddressAsync: Select County failed!");
                    return false;
                }

                if(info.Town.Length > 0)
                {
                    bRet = await SelectElementByTextAsync(Globals.SELECT_CITY_ID, "Id", info.Town).ConfigureAwait(false);
                    if (!bRet)
                    {
                        Trace.TraceInformation("Rudy Trace =>BindingDeliveryAddressAsync: Select Town failed!");
                        return false;
                    }
                }
                

                var inputAddressDetail = await WaitForElementAsync(Globals.ADDRESS_DETAIL_ID, "Id").ConfigureAwait(false);
                inputAddressDetail.SendKeys(info.DetailAddress);

                var inputMobile = await WaitForElementAsync(Globals.MOBILE_ID, "Id").ConfigureAwait(false);
                inputMobile.SendKeys(info.Mobile);

                var inputAddressAlias = await WaitForElementAsync(Globals.ADDRESS_ALIAS_ID, "Id").ConfigureAwait(false);
                inputAddressAlias.SendKeys("家庭地址");


                var btnSaveAddress = await WaitForElementAsync(Globals.BTN_SAVE_ADDRESS_XPATH, "XPath").ConfigureAwait(false);
                btnSaveAddress.Click();

                await Task.Delay(3000).ConfigureAwait(false);

                var btnUpgradeEasyBuy = await WaitForElementAsync(Globals.BTN_UPGRADE_EASYBUY_XPATH, "XPath").ConfigureAwait(false);
                btnUpgradeEasyBuy.Click();
                Trace.TraceInformation("Rudy Trace =>BindingDeliveryAddressAsync: Button upgrade easy buy clicked!");

                var dlgSetEasyBuyPop = await WaitForElementAsync(Globals.DLG_UPGRADE_EASYBUY_ID, "Id");
                if (dlgSetEasyBuyPop == null)
                {
                    Trace.TraceInformation("Rudy Trace =>BindingDeliveryAddressAsync: Open easy buy setting dialog failed!");
                    return false;
                }

                var radioPayMethod = await WaitForElementAsync(Globals.RADIO_PAY_ONLINE_ID, "Id").ConfigureAwait(false);
                radioPayMethod.Click();
                Trace.TraceInformation("Rudy Trace =>BindingDeliveryAddressAsync: Radio pay online clicked!");

                await Task.Delay(1000).ConfigureAwait(false);
                var btnConfirmUpgrade = await WaitForElementAsync(Globals.BTN_CONFIRM_SET_CLASS, "Class").ConfigureAwait(false);
                btnConfirmUpgrade.Click();
                Trace.TraceInformation("Rudy Trace =>BindingDeliveryAddressAsync: Button confirm clicked!");

                await Task.Delay(3000).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                {
                    throw e;
                }
                Trace.TraceInformation("Rudy Exception=> BindingDeliverAddress: {0};{1}", e.Source, e.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> SelectElementByTextAsync(string ele_mark, string ele_flag, string ele_text, int timeout = 30)
        {
            var selectCounty = await WaitForElementAsync(ele_mark, ele_flag).ConfigureAwait(false);
            bool bRet = false;  

            DateTime begins = DateTime.Now;
            TimeSpan span = DateTime.Now - begins;
            while (span.TotalSeconds < timeout)
            {
                try
                {
                    bRet = true;
                    SelectElement seCounty = new SelectElement(selectCounty);
                    seCounty.SelectByText(ele_text);  
                }
                catch (Exception)
                {
                    bRet = false;
                    span = DateTime.Now - begins;
                    continue;
                }
                break;
            }

            return bRet;
        }



        public async Task BindAllAccountAddressAsync()
        {
            bool bRet = await SetAddressAccoutInfoAsync(Settings.Default.BindAccountFile).ConfigureAwait(false);
            if (bRet)
            {
                Trace.TraceInformation("Rudy Trace =>Set Address Account Info Success!");
                foreach (AccountInfo info in aAccountInfo)
                {
                    bRet = await BindingDeliveryAddressAsync(info).ConfigureAwait(false);
                    if (bRet)
                        Trace.TraceInformation("Rudy Trace =>Accout[{0}]Binding Success!", info.Account);
                    else
                    {
                        Trace.TraceInformation("Rudy Trace =>Accout[{0}]Binding Failed!", info.Account);
                        return;
                    }
                        
                }
            }
            else
                Trace.TraceInformation("Rudy Trace =>Set Address Account Info Failed!");
        }

        public async Task<bool> PalProductAsync()
        {
            try
            {
                Trace.TraceInformation("Rudy Trace =>PalProductAsync: Product page loading...");
                driver.Navigate().GoToUrl(Settings.Default.ProductLink);
                Trace.TraceInformation("Rudy Trace =>PalProductAsync: Product page load complete.");

                var inputBuyNum = await WaitForElementAsync(Globals.INPUT_BUYNUM_ID, "Id").ConfigureAwait(false);
                inputBuyNum.Clear();
                inputBuyNum.SendKeys(SinglePalCount.ToString());

                var btnEasyBuy = await WaitForElementAsync(Globals.BTN_EASYBUY_ID, "Id").ConfigureAwait(false);
                btnEasyBuy.Click();

                Trace.TraceInformation("Rudy Trace =>PalProductAsync: Order settle page loading...");
                bool bRet = await WaitForPageAsync(Globals.ORDER_SETTLE_TITLE).ConfigureAwait(false);
                if (!bRet)
                {
                    Trace.TraceInformation("Rudy Trace =>PalProductAsync: Order settle page load time out.");
                    return false;
                }

                IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                js.ExecuteScript("window.scrollTo(0,document.body.scrollHeight)");

                await Task.Delay(3000);

                var btnAddRemark = await WaitForElementAsync(Globals.BTN_ADDREMARK_XPATH, "XPath").ConfigureAwait(false);
                btnAddRemark.Click();

                var inputRemark = await WaitForElementAsync(Globals.INPUT_REMARK_ID, "Id").ConfigureAwait(false);
                inputRemark.Clear();
                inputRemark.SendKeys(Settings.Default.Remark);

                var btnSubmitOrder = await WaitForElementAsync(Globals.BTN_SUBMITORDER_ID, "Id").ConfigureAwait(false);
                btnSubmitOrder.Click();

                bRet = await WaitForPageAsync(Globals.PAYMENT_PLATFORM_TITLE).ConfigureAwait(false);
                if (!bRet)
                    return false;
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                {
                    throw e;
                }
                Trace.TraceInformation("Rudy Exception=> PalProductAsync: " + e.Source + ";" + e.Message);
                return false;
            }
            Trace.TraceInformation("Rudy Trace =>Switch to payment platform select page succeed.");
            return true;
        }

        public async Task<bool> SelectPayPlatformAsync(string Platform)
        {
            try
            {
                var txtOrderMoney = await WaitForElementAsync(Globals.TXT_PAYMENT_MONEY_ID, "Id").ConfigureAwait(false);
                OrderMoney = txtOrderMoney.Text;

                var txtOrderNo = await WaitForElementAsync(Globals.TXT_ORDER_NO_ID, "Id").ConfigureAwait(false);
                OrderNo = txtOrderNo.Text;

                var tabPayPlatform = await WaitForElementAsync(Globals.TAB_PAYMENT_PLATFORM_ID, "Id").ConfigureAwait(false);
                tabPayPlatform.Click();

                var radioTenpay = await WaitForElementAsync(Platform, "XPath").ConfigureAwait(false);
                if(!radioTenpay.Selected)
                    radioTenpay.Click();

                var btnNext = await WaitForElementAsync(Globals.BTN_NEXT_ID, "Id").ConfigureAwait(false);
                btnNext.Click();

                bool bRet = await WaitForPageAsync("").ConfigureAwait(false);
                if (!bRet)
                    return false;
            }
            catch(Exception e)
            {
                if (e is OperationCanceledException)
                {
                    throw e;
                }
                Trace.TraceInformation("Rudy Exception =>SelectPayPlatformAsync: " + e.Source + ";" + e.Message);
                return false;
            }
            Trace.TraceInformation("Rudy Trace =>Switch to Tenpay page succeed.");
            return true;
        }

        public async Task<bool> TenpayAsync(string TenpayUser, string TenpayPass)
        {
            try
            { 
                var inputTenpayUser = await WaitForElementAsync(Globals.TENPAY_USERNAME_ID, "Id").ConfigureAwait(false);
                inputTenpayUser.Clear();
                inputTenpayUser.SendKeys(TenpayUser);
                var inputTenpayPass = await WaitForElementAsync(Globals.TENPAY_PASSWORD_ID, "Id").ConfigureAwait(false);
                inputTenpayPass.Clear();
                inputTenpayPass.SendKeys(TenpayPass);

                await Task.Delay(3000).ConfigureAwait(false);

                var btnLogin = await WaitForElementAsync(Globals.TENPAY_LOGIN_ID, "Id").ConfigureAwait(false);
                btnLogin.Click();

                Trace.TraceInformation("Rudy Trace =>LoginAsync: Waitting for Ten payment page to reload...");

                var radioBalance = await WaitForElementAsync(Globals.RADIO_BALANCEPAY_CLASS, "Class").ConfigureAwait(false);
                if (radioBalance != null)
                    driver.SwitchTo().Window(driver.WindowHandles.Last());//Switch to the reload page
                else
                    return false;

                radioBalance = await WaitForElementAsync(Globals.RADIO_BALANCEPAY_CLASS, "Class").ConfigureAwait(false);
                if (!radioBalance.Selected)
                    radioBalance.Click();
                
                #region==========Comment for Tenpay Password Input========================
                /*
                 *  
                AutomationElement ctrlPassword = FindChildElementByClass("Edit", "Chrome_WidgetWin_1");
                if (ctrlPassword == null)
                {
                    Trace.TraceInformation("Rudy Trace =>ctrlPassword is null.");
                    return false;
                }

                System.Windows.Point ctrlPassPosition = ctrlPassword.GetClickablePoint();
                int password_X = (int)ctrlPassPosition.X;
                int password_Y = (int)ctrlPassPosition.Y;
                Trace.TraceInformation("Rudy Trace =>ctrlPassPosition.X = {0}", password_X);
                Trace.TraceInformation("Rudy Trace =>ctrlPassPosition.Y = {0}", password_Y);

                var patternValue = (ValuePattern)ctrlPassword.GetCurrentPattern(ValuePattern.Pattern);
                if (patternValue != null)
                    patternValue.SetValue("");
                else
                {
                    Trace.TraceInformation("Rudy Trace =>patternValue is null.");
                    return false;
                }

                App.ClickLeft(password_X, password_Y);
                Trace.TraceInformation("Rudy Trace =>Foucs Set!");

                await Task.Delay(3000).ConfigureAwait(false);
                Forms.SendKeys.SendWait("P");
                Trace.TraceInformation("Rudy Trace =>Send 'P'");

                await Task.Delay(1000).ConfigureAwait(false);
                Forms.SendKeys.SendWait("A");
                Trace.TraceInformation("Rudy Trace =>Send 'A'");

                await Task.Delay(1000).ConfigureAwait(false);
                Forms.SendKeys.SendWait("S");
                Trace.TraceInformation("Rudy Trace =>Send 'S'");

                await Task.Delay(1000).ConfigureAwait(false);
                Forms.SendKeys.SendWait("S");
                Trace.TraceInformation("Rudy Trace =>Send 'S'");
                
                var btnConfirmToPay = await WaitForElementAsync(Globals.CONFIRM_TO_PAY_XPATH, "XPath");
                btnConfirmToPay.Click();
                */
                #endregion

                //TODO: waitting for the pay successed page.
 
            }
            catch(Exception e)
            {
                if (e is OperationCanceledException)
                {
                    throw e;
                }
                Trace.TraceInformation("Rudy Exception =>TenpayAsync: " + e.Source + ";" + e.Message);
                return false;
            }

            Trace.TraceInformation("Rudy Trace =>Tenpay Succeed.");
            return true;
        }

        public async Task AutoPalAllAccount()
        {
            string ReportPath = System.Environment.CurrentDirectory + "\\京东拍货报表";
            int nCount = 1;
            bool bSuccess = false;

            bool bRet = await SetJDAccoutInfoAsync(Settings.Default.AccountFile).ConfigureAwait(false);
            if (bRet)
            {
                bRet = true;//await CreateDDPalReportAsync(ReportPath).ConfigureAwait(false);
                if (bRet)
                {
                    foreach (JDAccount account in aJDAccount)
                    {
                        bSuccess = await AutoPalProcessAsync(account.UserName, account.Password).ConfigureAwait(false);
                        bRet = await UpdateDDPalReportAsync(ReportPath, nCount, bSuccess).ConfigureAwait(false);
                        if (!bRet)
                            Trace.TraceInformation("Rudy Trace =>AutoPalAllAccount: Update Report Failed[Line: {0}]!", nCount + 1);
                        nCount++;
                    }
                }
                else
                {
                    Trace.TraceInformation("Rudy Trace =>AutoPalAllAccount: Create Report Failed!");
                }
            }
            else
            {
                Trace.TraceInformation("Rudy Trace =>AutoPalAllAccount: SetJDAccoutInfoAsync Failed!");
            }
        }

        public async Task<bool> AutoPalProcessAsync(string Account, string Password)
        {
            Trace.TraceInformation("Rudy Trace =>AutoPalProcessAsync: : Prepare the environment...");
            await PrepareEnvironmentAsync(BrowserIndex);
            Trace.TraceInformation("Rudy Trace =>AutoPalProcessAsync: : Environment ready！");

            bool bRet = OpenBrowser(BrowserIndex);
            if (!bRet)
            {
                Trace.TraceInformation("Rudy Trace =>AutoPalProcessAsync: Open browser failed.");
                return false;
            }

            //Trace.TraceInformation("Rudy Trace =>AutoPalProcessAsync: Go to link[{0}]", Settings.Default.ProductLink);
            //await Task.Run(() =>
            //{
            //    Trace.TraceInformation("Rudy Trace =>AutoPalProcessAsync: Page is loading...");
            //    driver.Navigate().GoToUrl(Settings.Default.ProductLink);
            //}).ConfigureAwait(false);

            //string PageTitle = driver.Title;

            //var linkLogin = await WaitForElementAsync(Globals.LOGIN_LINK_CLASS, "Class").ConfigureAwait(false);
            //Trace.TraceInformation("Rudy Trace =>AutoPalProcessAsync: click Login Link.");
            //linkLogin.Click();

           // bRet = await WaitForPageAsync(Globals.LOGIN_PAGE_TITLE, 30).ConfigureAwait(false);
           // if (!bRet)
           //     return false;

            bRet = await LoginAsync(Account, Password).ConfigureAwait(false);
            if (!bRet)
            {
                Trace.TraceInformation("Rudy Trace =>AutoPalProcessAsync: Log in Failed!");
                return false;
            }

            bRet = await PalProductAsync().ConfigureAwait(false);
            if (!bRet)
            {
                Trace.TraceInformation("Rudy Trace =>AutoPalProcessAsync: Pal Product Failed!");
                return false;
            }
            /*
            bRet = await SelectPayPlatformAsync(Globals.RADIO_TENPAY_XPATH).ConfigureAwait(false);
            if (!bRet)
            {
                Trace.TraceInformation("Rudy Trace =>AutoPalProcessAsync: Select Pay Platform Failed!");
                return false;
            }

            bRet = await TenpayAsync(Settings.Default.TenpayAccount, Settings.Default.TenpayPassword);
            if (!bRet)
            {
                Trace.TraceInformation("Rudy Trace =>AutoPalProcessAsync: TenPay Failed!");
                return false;
            }
*/
            SuccessPalCount++;
            await RenewIpAddressAsync();
            Trace.TraceInformation("Rudy Trace =>AutoPalProcessAsync: Renew IP Finished!");
            LocalIpAddress = GetIpAddress();

            return true;
        }

        public AutomationElement FindChildElementByClass(string ClassName, string BrowserWndClassName)
        {
            try
            {
                Condition propCondition = new PropertyCondition(AutomationElement.ClassNameProperty, BrowserWndClassName);
                AutomationElement rootElement = AutomationElement.RootElement.FindFirst(TreeScope.Children, propCondition);
                if (rootElement == null)
                {
                    Trace.TraceInformation("Rudy Trace =>rootElement is null.");
                    return null;
                }

                propCondition = new PropertyCondition(AutomationElement.ClassNameProperty, "WrapperNativeWindowClass");
                AutomationElement wrapperElement = rootElement.FindFirst(TreeScope.Children, propCondition);
                if (wrapperElement == null)
                {
                    Trace.TraceInformation("Rudy Trace =>wrapperElement is null.");
                    return null;
                }
                

                propCondition = new PropertyCondition(AutomationElement.ClassNameProperty, ClassName);
                AutomationElement childElement = wrapperElement.FindFirst(TreeScope.Descendants, propCondition);
                if(childElement == null)
                {
                    Trace.TraceInformation("Rudy Trace =>childElement is null.");
                    return null;
                }
                return childElement;
            }
            catch(Exception e)
            {
                Trace.TraceInformation("Rudy Exception =>FindChildElementByClass: " + e.Source + ";" + e.Message);
                return null;
            }
            
        }

        public Task PrepareEnvironmentAsync(int nBrowserIndex)
        {
            return Task.Run(() =>
            {
                if (nBrowserIndex == 0)
                {
                    Process[] pBrowsers = Process.GetProcessesByName("chrome");
                    foreach (Process pBrowser in pBrowsers)
                    {
                        if(!pBrowser.CloseMainWindow())
                            pBrowser.Kill();
                    }
                    Process[] pDrivers = Process.GetProcessesByName("chromedriver");
                    foreach (Process pDriver in pDrivers)
                    {
                        if (!pDriver.CloseMainWindow())
                            pDriver.Kill();
                    }
                }
                else if (nBrowserIndex == 1)
                {
                    Process[] pBrowsers = Process.GetProcessesByName("iexplore");
                    foreach (Process pBrowser in pBrowsers)
                    {
                        if (!pBrowser.CloseMainWindow())
                            pBrowser.Kill();
                    }
                    Process[] pDrivers = Process.GetProcessesByName("IEDriverServer");
                    foreach (Process pDriver in pDrivers)
                    {
                        if (!pDriver.CloseMainWindow())
                            pDriver.Kill();
                    }
                }
                else if (nBrowserIndex == 2)
                {
                    Process[] pBrowsers = Process.GetProcessesByName("firefox");
                    foreach (Process pBrowser in pBrowsers)
                    {
                        if (!pBrowser.CloseMainWindow())
                            pBrowser.Kill();
                    }
                }
                else
                {
                    Trace.TraceInformation("Rudy Trace =>Invalid Browser Type.");
                }
            });
        }


        public string GetIpAddress()
        {
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in IpEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return null;
        }

        public async Task RenewIpAddressAsync()
        {
            Trace.TraceInformation("Rudy Trace =>Renewing the ip...");
            try
            {
                string DisconnectCMDLine = "rasdial /DISCONNECT";
                string ConnectCMDLine = "rasdial 宽带连接 " + Settings.Default.ADSLAccount + " " + Settings.Default.ADSLPassword;

                await RunCmdAsync(DisconnectCMDLine).ConfigureAwait(false);
                await Task.Delay(5000).ConfigureAwait(false);
                await RunCmdAsync(ConnectCMDLine).ConfigureAwait(false);
                await Task.Delay(10000).ConfigureAwait(false);//wait for the new ip configuration
            }
            catch(Exception e)
            {
                Trace.TraceInformation("Rudy Trace =>RenewIpAddressAsync: {0};{1}", e.Source, e.Message);
            }
        }

        public async Task RunCmdAsync(string CmdLine)
        {
            await Task.Run(() =>
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                p.Start();//启动程序

                //向cmd窗口发送输入信息
                p.StandardInput.WriteLine(CmdLine + "&exit");
                p.StandardInput.AutoFlush = true;
                //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
                //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
            }); 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //===========The implementation of IDispose interface.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
 
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    //release managed resources.
                }

                if (driver != null)
                {
                    Debug.WriteLine("Rudy Debug =>Dispose driver");
                    driver.Quit();
                }

                if (cts != null)
                {
                    Debug.WriteLine("Rudy Debug =>Dispose cts");
                    cts.Dispose();
                }
                m_disposed = true;
            }
        }
  
        ~AutoPal()
        {
            Dispose(false);
        }
  
        private bool m_disposed;
        //================================================
    }
}
