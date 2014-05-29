using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDAutoPal.Models
{
    public class Globals
    {
        public const string CHROME_DRIVER_TITLE = "C:\\Windows\\System32\\chromedriver.exe";
        public const string IE_DRIVER_TITLE = "C:\\Windows\\system32\\IEDriverServer.exe";
        public const string JD_CAPTION = "京东自动拍货";

        #region======================Page Title======================
        public const string LOGGEDIN_TITLE = "京东网上商城";
        public const string PAYMENT_PLATFORM_TITLE = "京东支付-请选择支付方式";
        public const string ORDER_SETTLE_TITLE = "订单结算页 -京东商城";
        #endregion

        #region======================Page Url========================
        public const string LOGIN_URL = "http://passport.jd.com/uc/login";
        public const string ONEKEY_BUY_URL = "http://customer.JD.com/onekey_buy/info.php";
        public const string PRODUCT_URL = "http://item.jd.com/1137731608.html";
        #endregion

        #region======================Page Elements===================
        //Log in information
        public const string LOGIN_NAME_ID = "loginname";
        public const string LOGIN_PASSWORD_ID = "nloginpwd";
        public const string LOGIN_SUBMIT_ID = "loginsubmit";

        public const string BTN_ADD_ADDRESS_CLASS = "add";
        public const string LOGIN_LINK_CLASS = "add";
        
        //Delivery Address Infomation
        public const string SHIP_MAN_ID = "ship_man";
        public const string SELECT_COUNTRY_ID = "country_id";
        public const string SELECT_PROVINCE_ID = "province_id";
        public const string SELECT_CITY_ID = "city_id";
        public const string SELECT_TOWN_ID = "town_id";
        public const string ADDRESS_DETAIL_ID = "addr_detail";
        public const string ZIP_CODE_ID = "ship_zip";
        public const string MOBILE_ID = "ship_mb";
        public const string CONFIRM_ADDRESS_XPATH = "//a[@href='javascript:show_shipment();']";
        public const string RADIO_NORMALSHIP_XPATH = "//input[@name='ship_type' and @value='1']";
        public const string CONFIRM_PAYMENT_XPATH = "//a[@href='javascript:show_payment();']";

        public const string RADIO_NETPAY_XPATH = "//input[@name='pay_id' and @value='-1']";
        public const string CONFIRM_INVOICE_XPATH = "//a[@href='javascript:show_invoice();']";

        public const string CHB_INVOICE_ID = "no_need_invoice";
        public const string CHECK_SUBMIT_XPATH = "//a[@onclick='javascript:check_submit();return false;']";

        //Product Page
        public const string BTN_EASYBUY_ID = "btn-easybuy-submit";

        public const string BTN_SUBMITORDER_ID = "order-submit";
        public const string BTN_ADDREMARK_CLASS = "toggler";
        public const string INPUT_REMARK_ID = "remarkText";
        /// <summary>
        /// /////////////////////////////////////
        /// </summary>
        public const string BUY_NUM_ID = "buy_num";
        public const string BUY_NOW_ID = "buy_now_button";
        public const string ADD_TO_CART_ID = "part_buy_button";
        public const string BUY_NOW_XPATH = "//div[@class='btn_p']/a[@id='buy_now_button']";
        public const string BUY_NOW_POPUP_ID = "div_onekey_select_pop";
        public const string BTN_CONFIRM_BUY_ID = "onekey_select_pop_confirm";

        //Payment Platform page
        public const string TXT_PAYMENT_MONEY_ID = "lblAmount";
        public const string TXT_ORDER_NO_ID = "lblNo";
        public const string TAB_PAYMENT_PLATFORM_ID = "go_tab3";
        public const string RADIO_TENPAY_XPATH = "//p[@bid='44']/input";
        public const string BTN_NEXT_ID = "A4";

        //Tenpay Login Page
        public const string TENPAY_LOGIN_ERR_MSG_ID = "login_err_msg";
        public const string TENPAY_USERNAME_ID = "login_uin";
        public const string TENPAY_PASSWORD_ID = "login_pwd";
        public const string TENPAY_LOGIN_ID = "btn_login";

        //Tenpay Payment Page
        public const string RADIO_BALANCEPAY_CLASS = "ctrl-radio";
        public const string CTRL_TENPWD_LABEL_XPATH = "//div[@id='paypwd_line']/label";
        public const string CONFIRM_TO_PAY_XPATH = "//span[@id='btn_pay_submit']/button";
        #endregion
    }
}
