using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Lina.Anco.WP.Resources;

using Lina.Store.WP;
using Lina.AnCo.Core;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using Lina.Customs.Core;


namespace Lina.Anco.WP
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private GameModel lnCurrentGameMD = null;
        private static DispatcherTimer dtm;
        private bool lnPlayPauseGame = true;
        void OnTimerTick(Object s, EventArgs e)
        {

            if (prgTime.Value < 1.0)
            {

                ShowGameOver(this.lnCurrentGameMD);
                initGame();
                dtm.Stop();
                prgTime.Value = prgTime.Value;
                return;
            }
            //ACGlobalService.Game.PlayAudio("Assets/Sound/music.m4a");

            prgTime.Value = prgTime.Value - 1;
        }
        private void SetImageForButton(Button btn, string path)
        {
            var imgBrush = new ImageBrush();
            imgBrush.ImageSource = new BitmapImage(new Uri(path, UriKind.Relative));
            btn.Background = imgBrush;
        }
        private void HideMarkGame()
        {
            markPauseGame.Visibility = System.Windows.Visibility.Collapsed;
            markPauseGame.Opacity = 0.0;
            markLavelUp.Visibility = System.Windows.Visibility.Collapsed;
            markPauseGame.Opacity = 0.0;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (lnPlayPauseGame == false)
            {
                lnPlayPauseGame = true;
                SetImageForButton(btnStart, "Assets/Images/pause@2x.png");
                HideMarkGame();

                ACGlobalService.Game.StartGame((isOK, GameMD) =>
                {
                    if (isOK)
                    {
                        RestartGame(GameMD);
                        prgTime.Maximum = GameMD.Timer;
                        dtm = new DispatcherTimer();
                        dtm.Interval = TimeSpan.FromSeconds(1);
                        dtm.Tick += OnTimerTick;
                        dtm.Start();
                    }
                    else
                    {

                    }
                });
            }
            else
            {
                markPauseGame.Visibility = System.Windows.Visibility.Visible;
                dtm.Stop();
                SetImageForButton(btnStart, "Assets/Images/start@2x.png");
                linaStorybord.Stop();
                Storyboard.SetTargetName(animationOpacity, "markPauseGame");
                linaStorybord.Begin();
                ACGlobalService.Game.PauseGame(this.lnCurrentGameMD, (isOK) =>
                {
                    if (isOK)
                    {
                        lnPlayPauseGame = false;
                        this.lnCurrentGameMD = null;

                    }
                });


            }

        }

        private void RestartGame(GameModel GameMD)
        {
            btnleftQuestion.Content = GameMD.Question.LeftQuestion.ToString();
            switch (GameMD.Question.Calculation)
            {
                case CalculationType.Addition:
                    btncalculation.Source = new BitmapImage(new Uri("/Assets/Images/dau cong@2x.png", UriKind.RelativeOrAbsolute));
                    break;
                case CalculationType.Division:
                    btncalculation.Source = new BitmapImage(new Uri("/Assets/Images/dau chia@2x.png", UriKind.RelativeOrAbsolute));
                    break;
                case CalculationType.Multiplication:
                    btncalculation.Source = new BitmapImage(new Uri("/Assets/Images/dau nhan@2x.png", UriKind.RelativeOrAbsolute));
                    break;
                case CalculationType.Subtraction:
                    btncalculation.Source = new BitmapImage(new Uri("/Assets/Images/dau tru@2x.png", UriKind.RelativeOrAbsolute));
                    break;
            }
            btnrightQuestion.Content = GameMD.Question.RightQuestion.ToString();
            result1.Content = GameMD.Question.LeftAnswer;
            result2.Content = GameMD.Question.RightAnswer;
            currentquestion.Text = GameMD.NumberOfQuestionAnswered.ToString();
            maxQuestion.Text = GameMD.MaxNumberOfQuestion.ToString();
            this.lnCurrentGameMD = GameMD;
        }
        private void initGame()
        {
            prgTime.Value = prgTime.Maximum;
            btnleftQuestion.Content = "...";
            btncalculation.Source = new BitmapImage(new Uri("/Assets/Images/dau cong@2x.png", UriKind.RelativeOrAbsolute));
            btnrightQuestion.Content = "...";
            result1.Content = "..";
            result2.Content = "..";
            currentquestion.Text = "0";
            maxQuestion.Text = "0";

            dtm.Stop();
            this.lnCurrentGameMD = null;
        }

        private void result1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.lnCurrentGameMD.Question.UserAnswer = int.Parse(((Button)sender).Content.ToString());
                ACGlobalService.Game.Return(this.lnCurrentGameMD, (isOK, notifiMD, newGameMD) =>
                {
                    this.lnCurrentGameMD = newGameMD;
                    if (isOK)
                    {

                        ACGlobalService.Game.PlayAudio("Assets/Sound/dinh.wav");
                        if (notifiMD == NotificationType.LevelUp)
                            ShowLevelUp();
                        else
                            RestartGame(newGameMD);
                    }
                    else
                    {
                        ACGlobalService.Game.PlayAudio("Assets/Sound/comedy-bounce.mp3");
                        if (notifiMD == NotificationType.QuestionUp)
                        {
                            dtm.Stop();
                            NavigationService.GoBack();
                        }
                        else
                        {
                            dtm.Stop();
                            ShowGameOver(newGameMD);
                        }

                    }
                    ACGlobalService.Game.Vibrate();

                });
            }
            catch (Exception)
            {
            }
        }
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            try
            {
                dtm.Stop();
            }
            catch (Exception)
            {
            }

        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void HideGameOver()
        {
            markGameover.Visibility = System.Windows.Visibility.Collapsed;
            my_popup_xaml1.IsOpen = false;
        }

        private void ShowGameOver(GameModel GameMD)
        {
            dtm.Stop();
            lnUserLevel.Text = UserModel.CurrentUser.Level.ToString();
            lnNumberOfQS.Text = UserModel.CurrentUser.NumberOfQuestionAnswered.ToString();

            markGameover.Visibility = System.Windows.Visibility.Visible;
            my_popup_xaml1.IsOpen = true;
            FoldingPanel.RenderTransform = new CompositeTransform() { CenterX = FoldingPanel.Width / 2, CenterY = FoldingPanel.Height / 2 };
            var s = new Storyboard();
            s.AddScalingAnimation(FoldingPanel, 0, 1.1f, 0, 1.1f, TimeSpan.FromMilliseconds(300));
            s.AddOpacityAnimation(FoldingPanel, 0, 1, TimeSpan.FromMilliseconds(300));
            s.Begin();
            s.Completed += (object sender0, EventArgs e0) =>
            {
                FoldingPanel.RenderTransform = new CompositeTransform() { CenterX = FoldingPanel.Width / 2, CenterY = FoldingPanel.Height / 2 };

                s = new Storyboard();
                s.AddScalingAnimation(FoldingPanel, 1.1f, 0.9f, 1.1f, 0.9f, TimeSpan.FromMilliseconds(100));
                s.AddOpacityAnimation(FoldingPanel, 1, 0.9f, TimeSpan.FromMilliseconds(100));
                s.Begin();
                s.Completed += (object sender1, EventArgs e1) =>
                {
                    FoldingPanel.RenderTransform = new CompositeTransform() { CenterX = FoldingPanel.Width / 2, CenterY = FoldingPanel.Height / 2 };

                    s = new Storyboard();
                    s.AddScalingAnimation(FoldingPanel, 0.9f, 1, 0.9f, 1, TimeSpan.FromMilliseconds(100));
                    s.AddOpacityAnimation(FoldingPanel, 0.9f, 1, TimeSpan.FromMilliseconds(100));

                    s.Begin();
                };
                myStoryboard.Begin();
                myStoryboard.Completed += (object sender1, EventArgs e1) =>
                {
                    myStoryboard1.Begin();
                    myStoryboard1.Completed += (object sender2, EventArgs e2) =>
                    {
                        myStoryboard2.Begin();
                        myStoryboard2.Completed += (object sender3, EventArgs e3) =>
                        {
                            myStoryboard3.Begin();
                            myStoryboard3.Completed += (object sender4, EventArgs e4) =>
                            {
                                myStoryboard4.Begin();
                            };
                        };
                    };
                };
            };
        }

        private void ShowLevelUp()
        {
            prgTime.Value = prgTime.Maximum;
            txtlvUp.Text = UserModel.CurrentUser.Level.ToString();
            markLavelUp.Visibility = System.Windows.Visibility.Visible;
            linaStorybord.Stop();
            Storyboard.SetTargetName(animationOpacity, "markLavelUp");
            dtm.Stop();
            SetImageForButton(btnStart, "Assets/Images/start@2x.png");
            linaStorybord.Begin();
            ACGlobalService.Game.PlayAudio("Assets/Sound/Votay.mp3");
            linaStorybord.Completed += (object sender, EventArgs e) =>
            {

                ACGlobalService.Game.PauseGame(this.lnCurrentGameMD, (isOK) =>
                {
                    if (isOK)
                    {
                        lnPlayPauseGame = false;
                        this.lnCurrentGameMD = null;

                    }
                });
                showStart.Begin();
                showStart.Completed += (object sender1, EventArgs e1) =>
                {
                    hideStart.Begin();
                    hideStart.Completed += (object sender2, EventArgs e2) =>
                    {
                        showStart.Begin();
                    };

                };
            };
        }


        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            bool stateVibra = ACGlobalService.Game.GetVibrateState();
            bool stateSound = ACGlobalService.Game.GetSoundState();
            if (stateVibra == false)
            {
                vibration.Source = new BitmapImage(new Uri("/Assets/Images/rung@2x.png", UriKind.RelativeOrAbsolute));
                vibration.Tag = (object)"on";
            }
            else
            {
                vibration.Source = new BitmapImage(new Uri("/Assets/Images/Not_Rung@2x.png", UriKind.RelativeOrAbsolute));
                vibration.Tag = (object)"off";
            }
            if (stateSound == false)
            {
                sound.Source = new BitmapImage(new Uri("/Assets/Images/amthanh@2x.png", UriKind.RelativeOrAbsolute));
                sound.Tag = (object)"on";
            }
            else
            {
                sound.Source = new BitmapImage(new Uri("/Assets/Images/Not_amthanh@2x.png", UriKind.RelativeOrAbsolute));
                sound.Tag = (object)"off";
            }
            ACGlobalService.Game.StartGame((isOK, GameMD) =>
            {
                if (isOK)
                {
                    RestartGame(GameMD);
                    prgTime.Maximum = GameMD.Timer;
                    dtm = new DispatcherTimer();
                    dtm.Interval = TimeSpan.FromSeconds(1);
                    dtm.Tick += OnTimerTick;
                    dtm.Start();
                }
                else
                {
                    MessageBoxResult result =
                MessageBox.Show("Time is running out.",
                "Message stop game1", MessageBoxButton.OK);

                }
            });
        }

        private void sound_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ACGlobalService.Game.ChangeSoundState((isok) => { });
            flagSound = 1;
            if (sound.Tag.ToString() == "on")
            {
                sound.Source = new BitmapImage(new Uri("/Assets/Images/Not_amthanh@2x.png", UriKind.RelativeOrAbsolute));
                sound.Tag = (object)"off";
            }
            else
            {
                sound.Source = new BitmapImage(new Uri("/Assets/Images/amthanh@2x.png", UriKind.RelativeOrAbsolute));
                sound.Tag = (object)"on";
                ACGlobalService.Game.PlayAudio("Assets/Sound/sound_right.wav");
            }


        }

        private void vibration_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            flagSound = 1;

            if (vibration.Tag.ToString() == "on")
            {
                vibration.Source = new BitmapImage(new Uri("/Assets/Images/Not_Rung@2x.png", UriKind.RelativeOrAbsolute));
                vibration.Tag = (object)"off";
            }
            else
            {
                vibration.Source = new BitmapImage(new Uri("/Assets/Images/rung@2x.png", UriKind.RelativeOrAbsolute));
                vibration.Tag = (object)"on";
            }
            ACGlobalService.Game.ChangeVibrateState((isok) => { });
        }

        public static int flagSound = 0;

        private void btnTryAgain_Click(object sender, RoutedEventArgs e)
        {
            HideGameOver();
            if (flagSound == 0)
            {
                lnPlayPauseGame = true;
                SetImageForButton(btnStart, "Assets/Images/pause@2x.png");
                HideMarkGame();

                ACGlobalService.Game.StartGame((isOK, GameMD) =>
                {
                    if (isOK)
                    {
                        RestartGame(GameMD);
                        prgTime.Maximum = GameMD.Timer;
                        dtm = new DispatcherTimer();
                        dtm.Interval = TimeSpan.FromSeconds(1);
                        dtm.Tick += OnTimerTick;
                        dtm.Start();
                    }
                    else
                    {

                    }
                });
            }
            else
            {
                flagSound = 0;
            }
        }


        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}