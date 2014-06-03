using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDAutoPal.Models
{
    public class Globals
    {
        public const string JD_CAPTION = "京东自动拍货";

        #region======================Page Title======================
        public const string LOGGEDIN_TITLE = "京东网上商城";
        public const string PAYMENT_PLATFORM_TITLE = "京东支付-请选择支付方式";
        public const string ORDER_SETTLE_TITLE = "订单结算页 -京东商城";
        #endregion

        #region======================Page Url========================
        public const string LOGIN_URL = "http://passport.jd.com/uc/login";
        public const string PRODUCT_URL = "http://item.jd.com/1137731608.html";
        public const string EASYBUY_URL = "http://easybuy.jd.com/address/getEasyBuyList.action";
        #endregion

        #region======================Page Elements===================
        //Log in information
        public const string LOGIN_NAME_ID = "loginname";
        public const string LOGIN_PASSWORD_ID = "nloginpwd";
        public const string LOGIN_SUBMIT_ID = "loginsubmit";
        
        //Delivery Address Infomation
        public const string BTN_ADD_ADDRESS_XPATH = "//a[@onclick='alertAddAddressDiag()']";
        public const string DLG_ADDRESS_POP_ID = "addressDiagDiv";
        public const string SHIP_MAN_ID = "consigneeName";
        public const string SELECT_PROVINCE_ID = "provinceDiv";
        public const string SELECT_CITY_ID = "cityDiv";
        public const string SELECT_COUNTY_ID = "countyDiv";
        public const string SELECT_TOWN_ID = "townDiv";
        public const string ADDRESS_DETAIL_ID = "consigneeAddress";
        public const string MOBILE_ID = "consigneeMobile";
        public const string ADDRESS_ALIAS_ID = "consigneeAlias";
        public const string BTN_SAVE_ADDRESS_XPATH = "//a[@onclick='addAddress();']";
        public const string BTN_UPGRADE_EASYBUY_XPATH = "//div[@class='ac']/a";

        public const string DLG_UPGRADE_EASYBUY_ID = "paymentDiagDiv";
        public const string RADIO_PAY_ONLINE_ID = "pay-method-1";
        public const string RADIO_PAY_ONDELIVERY_ID = "pay-method-2";
        public const string RADIO_PAY_ONYOURSELF_ID = "pay-method-3";
        public const string BTN_CONFIRM_SET_CLASS = "gray-btn";
        
        //Product Page
        public const string INPUT_BUYNUM_ID = "buy-num";
        public const string BTN_EASYBUY_ID = "btn-easybuy-submit";
        public const string BTN_ADDREMARK_XPATH = "//a[@class='toggler' and @onclick='selectRemark(this)']";
        public const string INPUT_REMARK_ID = "remarkText";
        public const string BTN_SUBMITORDER_ID = "order-submit";

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
