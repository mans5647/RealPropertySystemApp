using RealPropertySystemApp.bodies;
using RealPropertySystemApp.events;
using RealPropertySystemApp.models;
using RealPropertySystemApp.pages;
using RealPropertySystemApp.ui;
using RealPropertySystemApp.utils;
using System;
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
using System.Windows.Shapes;

namespace RealPropertySystemApp.windows
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        
        private Dictionary<string, Button> buttons;

        private Session currentSession;
        
        private UserProfileEditPage editPage;
        private MainEvents.onSessionFinishedCb cb;
        
        public Home()
        {
            InitializeComponent();

            currentSession = Session.GetCurrent();


            buttons = BuildAllButtons();
            editPage = UserProfileEditPage.withUserModel(currentSession.CurrentUser, currentSession.Jwt.AccessToken);
            editPage.SetOnUpdateCallback(onUserInformationUpdated);
            AddButtons();
            AddCallbacks();
            setWelcomeMessage();
        }


        public async void StartTimeoutCalculation()
        {
            await Task.Run(UpdateLeftTimeout);
            leftTimeout.Content = "Выход из приложения ...";
            

            actionsBox.IsEnabled = false;
            mainFrame.IsEnabled = false;
            //Hide();

            if (cb != null)
            {
                cb(null);
            }
        }


        public void SetSessionFinishedCallback(MainEvents.onSessionFinishedCb cb)
        {
            this.cb = cb;
        }

        private void closeEvent(object sender, EventArgs args)
        {
        }


        private void UpdateLeftTimeout()
        {
            while (true)
            {
                if (currentSession.isSessionExpired()) break;

                leftTimeout.Dispatcher.Invoke(() =>
                {
                    currentSession.TimeLeft = currentSession.SessionExpireTime - DateTime.Now;
                    leftTimeout.Content = $"Отключение через: {currentSession.TimeLeft.Minutes} минут и {currentSession.TimeLeft.Seconds} секунд";

                });
                Thread.Sleep(1000);
            }

            
        }

        private void AddButtons()
        {
            Button manageProfileButton = buttons["all_manage_profile"];

            actionsBox.Children.Add(manageProfileButton);

            switch (currentSession.GetRoleSuffix())
            {
                case "ADMIN":
                    actionsBox.Children.Add(buttons["admin_1"]);
                    actionsBox.Children.Add(buttons["admin_2"]);
                    actionsBox.Children.Add(buttons["btn_open_logs_page"]);
                    break;
                
                case "CLIENT":

                    actionsBox.Children.Add(buttons["show_props"]);
                    actionsBox.Children.Add(buttons["add_property"]);
                    actionsBox.Children.Add(buttons["show_deals"]);
                    break;
            }

        }

        private void AddCallbacks()
        {
            Button showEditPage = buttons["all_manage_profile"];
            showEditPage.Click += onClickProfileEdit;

            ExitButton.Click += ExitButtonClicked;

            buttons["btn_open_logs_page"].Click += OpenLogsBtnClicked;

            buttons["admin_1"].Click += OnClickViewUsers;

        }

        private Dictionary<string, Button> BuildAllButtons()
        {
            Button adminViewAllUsersButton = new Button();
            Button adminViewAllActionsButton = new Button();
            Button manageProfileButton = new Button();

            ClassicButton showDialsHistoryButton = new ClassicButton("Показать историю сделок", Resources);
            ClassicButton showAvailablePropertiesButton = new ClassicButton("Недвижимости", Resources);
            ClassicButton addProperty = new ClassicButton("Добавить свою недвижимость", Resources);
            
            adminViewAllUsersButton.Content = "Просмотреть пользователей";
            adminViewAllUsersButton.Style = (Style)Resources["EnhancedSwitchButtonStyle"];

            adminViewAllActionsButton.Content = "История действия";
            adminViewAllActionsButton.Style = (Style)Resources["EnhancedSwitchButtonStyle"];
            
            manageProfileButton.Content = "Подробнее о профиле";
            manageProfileButton.Style = (Style)Resources["EnhancedSwitchButtonStyle"];

            ClassicButton buttonOpenLogsView = new ClassicButton("Посмотреть логи", Resources);

            return new Dictionary<string, Button>
            {
                {"admin_1", adminViewAllUsersButton},
                {"admin_2", adminViewAllActionsButton },
                {"all_manage_profile", manageProfileButton },
                {"show_deals", showDialsHistoryButton },
                {"add_property", addProperty},
                {"show_props", showAvailablePropertiesButton },
                {"btn_open_logs_page", buttonOpenLogsView}
            };


        }

        private async void OnClickViewUsers(object sender, EventArgs e)
        {
            var viewer = await UsersViewer.Create();
            mainFrame.Content = viewer;
        }

        private void onClickProfileEdit(object from, RoutedEventArgs args)
        {
            mainFrame.Content = editPage;
        }

        private void onUserInformationUpdated(UserModel updated)
        {
            currentSession.CurrentUser = updated;
            Session.SaveAll();
            setWelcomeMessage();
        }

        private void setWelcomeMessage()
        {
            welcomeLabel.Content = $"Добро пожаловать, {currentSession.CurrentUser.firstName}!";
        }


        private void ExitButtonClicked(object sender, RoutedEventArgs args)
        {
            Session.GetCurrent().Last = false;
            Session.SaveAll();
            Close();
        }

        private void OpenLogsBtnClicked(object sender, RoutedEventArgs args)
        {
            mainFrame.Content = new LogViewPage();
        }

    }
}
