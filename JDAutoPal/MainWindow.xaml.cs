﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using System.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;
using JDAutoPal.Models;
using Microsoft.Win32;
using JDAutoPal.Properties;
using logger;


namespace JDAutoPal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {
        private AutoPal JDPal;
        private log DDLog;
        private string LogPath;

        public MainWindow()
        {
            InitializeComponent();
            JDPal = new AutoPal();
            this.DataContext = JDPal;
            pwdBoxADSL.Password = Settings.Default.ADSLPassword;
            pwdBoxTenpay.Password = Settings.Default.TenpayPassword;
            LogPath = System.Environment.CurrentDirectory + "\\DDEvent.log";
            DDLog = new log();
        }
        
        private void OnMainWindow_Closed(object sender, EventArgs e)
        {
            Settings.Default.Save();
            CleanUp();
            Trace.TraceInformation("Rudy Trace =>Application Exited.");
        }

        private void OnBrowserQQ(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel(2007,2010)|*.xlsx";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
                Settings.Default.AccountFile = dlg.FileName;
        }

        private void OnStopPalling(object sender, RoutedEventArgs e)
        {
            if (btnStop.Content.Equals("停止拍货"))
            {
                Trace.TraceInformation("Rudy Trace =>OnStopPalling: Stoppig pal...");
                JDPal.CancelWaitting();
                btnStop.Content = "正在取消...";
                btnStop.IsEnabled = false;
            }
            else if (btnStop.Content.Equals("返回"))
            {
                gdBeginPal.Visibility = Visibility.Visible;
                gdPalling.Visibility = Visibility.Hidden;
            }
        }

        private void OnBrowserBindAccount(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel(2007,2010)|*.xlsx";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
                Settings.Default.BindAccountFile = dlg.FileName;
        }

        private async void OnBeginBind(object sender, RoutedEventArgs e)
        {
            if (tbBindAccount.Text.Length == 0)
            {
                MessageBox.Show("京东账户信息文件不能为空！", Globals.JD_CAPTION, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Trace.TraceInformation(">>>>>>>>>>>>>>>>>>>>Rudy Trace =>OnBeginBind: Address bind start<<<<<<<<<<<<<<<<<<<<");
            JDPal.InitCTS();
            btnBeginBind.Visibility = Visibility.Hidden;
            btnStopBind.Content = "停止绑定";
            btnStopBind.Visibility = Visibility.Visible;
            try
            {
                await JDPal.BindAllAccountAddressAsync();
            }
            catch (OperationCanceledException)
            {
                Trace.TraceInformation("Rudy Trace =>OnBeginBind: Bind Address Stopped.");
            }

            btnStopBind.IsEnabled = true;
            btnStopBind.Visibility = Visibility.Hidden;
            btnBeginBind.Visibility = Visibility.Visible;
        }

        private void OnStopBind(object sender, RoutedEventArgs e)
        {
            Trace.TraceInformation("Rudy Trace =>OnStopPalling: Stoppig bind...");
            btnStopBind.Content = "正在取消...";
            btnStopBind.IsEnabled = false;
            JDPal.CancelWaitting();
        }

        private async void OnBeginPal(object sender, RoutedEventArgs e)
        {
            if (tbAccount.Text.Length == 0)
            {
                MessageBox.Show("京东账户信息文件不能为空！", Globals.JD_CAPTION, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (tbTenpayAccount.Text.Length == 0)
            {
                MessageBox.Show("财付通账户不能为空！", Globals.JD_CAPTION, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (pwdBoxTenpay.Password.Length == 0)
            {
                MessageBox.Show("财付通密码不能为空！", Globals.JD_CAPTION, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (tbProductLink.Text.Length == 0)
            {
                MessageBox.Show("宝贝链接不能为空！", Globals.JD_CAPTION, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (tbADSLAccount.Text.Length == 0)
            {
                MessageBox.Show("宽带账户不能为空！", Globals.JD_CAPTION, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (pwdBoxADSL.Password.Length == 0)
            {
                MessageBox.Show("宽带密码不能为空！", Globals.JD_CAPTION, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
                            
            Trace.TraceInformation(">>>>>>>>>>>>>>>>>>>>Rudy Trace =>OnBeginPal: Pal Start<<<<<<<<<<<<<<<<<<<<");
            JDPal.InitCTS();
            gdBeginPal.Visibility = Visibility.Hidden;
            gdPalling.Visibility = Visibility.Visible;
            btnStop.Content = "停止拍货";

            try
            {
                await JDPal.AutoPalAllAccount();
            }
            catch (OperationCanceledException)
            {
                Trace.TraceInformation("Rudy Trace =>OnBeginPal: Pal Stopped.");
            }
            
            btnStop.IsEnabled = true;
            btnStop.Content = "返回";
        }


        private void CleanUp()
        {
            Trace.TraceInformation("Rudy Trace =>Cleaning up the environment...");
            JDPal.Dispose();
            Trace.TraceInformation("Rudy Trace =>Clean up done.");
        }

        private void OnReduce_Click(object sender, RoutedEventArgs e)
        {
            if (JDPal.SinglePalCount > 1)
                JDPal.SinglePalCount -= 1;
        }

        private void OnAdd_Click(object sender, RoutedEventArgs e)
        {
            JDPal.SinglePalCount += 1;
        }

        private void OnPalCountChanged(object sender, TextChangedEventArgs e)
        {
            //for (int i = 0; i < tbPalCount.Text.Length; i++)
            //{
                //if (!Char.IsNumber(tbPalCount.Text, 0))
                //{
            //    MessageBox.Show("拍货数量必须是整数", Globals.JD_CAPTION);
                    //break;
                //}
            //}
        }

        private void OnADSLPWDChanged(object sender, RoutedEventArgs e)
        {
            Settings.Default.ADSLPassword = pwdBoxADSL.Password;
            
        }

        private void OnTenpayPWDChanged(object sender, RoutedEventArgs e)
        {
            Settings.Default.TenpayPassword = pwdBoxTenpay.Password;
        }
    }
}
