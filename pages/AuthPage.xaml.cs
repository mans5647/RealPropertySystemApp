using RealPropertySystemApp.bodies;
using RealPropertySystemApp.codes;
using RealPropertySystemApp.events;
using RealPropertySystemApp.ui;
using RealPropertySystemApp.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RealPropertySystemApp.pages
{
    /// <summary>
    /// Interaction logic for AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private MainEvents.LoginPerformed? loginEvent;
        
        public AuthPage()
        {
            InitializeComponent();
            
            doLoginButton.Click += LoginButtonPressed;

        }

        public void SetLoginEvent(MainEvents.LoginPerformed loginPerformed)
        {
            loginEvent = loginPerformed;
        }

        private async void LoginButtonPressed(object sender, RoutedEventArgs e)
        {

            RPClient client = RPClient.GetClient();
            doLoginButton.IsEnabled = false;
            var task = client.Authenticate(login_input.Text, password_input.Password);
            var data = await task;

            loginEvent(data["jwt"], (int)data["code"]);
            doLoginButton.IsEnabled = true;
        }

    }
}
