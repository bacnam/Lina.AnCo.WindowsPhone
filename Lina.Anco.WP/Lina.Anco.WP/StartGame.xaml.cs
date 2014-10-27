using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Threading;
using Lina.AnCo.Core;

namespace Lina.Anco.WP
{
    public partial class StartGame : PhoneApplicationPage
    {
        private static DispatcherTimer dtm;
        public StartGame()
        {
            InitializeComponent();
            txtLvUp.Text = UserModel.CurrentUser.Level.ToString();
            txtqusUp.Text = UserModel.CurrentUser.NumberOfQuestionAnswered.ToString();

        }
        int lv, qs;

        void dtm_Tick(object sender, EventArgs e)
        {
            if (lv < UserModel.CurrentUser.Level)
            {
                lv++;
                txtLvUp.Text = lv.ToString();
            }
            if (qs < UserModel.CurrentUser.NumberOfQuestionAnswered)
            {
                qs++;
                txtqusUp.Text = qs.ToString();
            }
            if (lv >= UserModel.CurrentUser.Level && qs >= UserModel.CurrentUser.NumberOfQuestionAnswered)
            {
                dtm.Stop();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Mainpage.xaml", UriKind.Relative));
        }

        private void btnStore_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/APIStoreKit.xaml", UriKind.Relative));
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                //GameModel currenrQs =(GameModel) PhoneApplicationService.Current.State["currentQS"];
                ///btnlvUp.Content = currenrQs.User.Level.ToString();
                //btnQsUp.Content = currenrQs.User.NumberOfQuestionAnswered.ToString();
                lv = int.Parse(txtLvUp.Text) - 1;
                qs = int.Parse(txtqusUp.Text) - 1;
                dtm = new DispatcherTimer();
                dtm.Interval = TimeSpan.FromSeconds(0.05);
                dtm.Tick += dtm_Tick;
                dtm.Start();
            }
            catch (Exception)
            { }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}