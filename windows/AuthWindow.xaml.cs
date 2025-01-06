using RealPropertySystemApp.bodies;
using RealPropertySystemApp.codes;
using RealPropertySystemApp.pages;
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
using JWT;
using JWT.Serializers;
using JWT.Builder;
using Newtonsoft.Json.Linq;
using JWT.Algorithms;
using System.Runtime.InteropServices.JavaScript;
using RealPropertySystemApp.models;
using RealPropertySystemApp.utils;
using RealPropertySystemApp.ui;
using System.Windows.Controls.Primitives;

namespace RealPropertySystemApp.windows
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private static AuthPage authPage = new AuthPage();
        private static RegisterPage registerPage = new RegisterPage();
        private static Home homeWindow;
        private static SessionListPage sessionListPage = new SessionListPage();
        
        public AuthWindow()
        {
            InitializeComponent();
            
            AuthSwitch.Click += onSwitchAuthPressed;
            RegSwitch.Click += onSwitchRegPressed;
            ShowSavedSessionsButton.Click += OnShowSavedSessionsButtonClick;
            authPage.SetLoginEvent(loginPerformedEvent);

            
            if (Session.isSessionFileExists())
            {
                if (!Session.isFileEmpty())
                {
                    ContentStatus stat = ContentStatus.OK;
                    Session.LoadAllFromFile(ref stat);

                    Session s = Session.findByMaxLastLogin();

                    if (s.Last)
                    {
                        s.LastLoginTime = DateTime.Now;
                        s.TimeLeft = TimeSpan.MinValue;
                        Session.SetCurrent(s);

                        homeWindow = new Home();
                        homeWindow.StartTimeoutCalculation();
                        homeWindow.Show();
                    }

                }
            }
        }


        public void onSwitchAuthPressed(object sender, EventArgs e)
        {
            MainFrame.Content = authPage;
        }

        public void onSwitchRegPressed(object sender, EventArgs e)
        {
            MainFrame.Content = registerPage;
        }

        private async void loginPerformedEvent(object data, int code)
        {
            string titleError = "Ошибка авторизации", errorDesc = string.Empty;
            switch (code)
            {
                case AuthCode.AuthLoginNotFound:
                    errorDesc = "Неправильный логин";
                    break;
                case AuthCode.AuthPasswordIncorrect:
                    errorDesc = "Неправильный пароль";

                    break;
                case GenericCode.NoError:

                    JwtResponse jwt = (JwtResponse)data;

                    var validationParams = new ValidationParameters
                    {
                        ValidateSignature = false,
                    };

                    IJsonSerializer serializer = new JsonNetSerializer();
                    IDateTimeProvider provider = new UtcDateTimeProvider();
                    IJwtValidator validator = new JwtValidator(serializer, provider);
                    IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                    IJwtAlgorithm algorithm = new HMACSHA256Algorithm();

                    IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

                    var json = decoder.Decode(jwt.AccessToken, false);

                    JObject credits = JObject.Parse(json);

                    string login = credits["login"].ToString();
                    long expiresAt = credits["exp"].ToObject<long>();
                    RPClient client = RPClient.GetClient();

                    UserModel model = await client.GetUserByLogin(login, jwt.AccessToken);

                    var expireTime = DateTimeOffset.FromUnixTimeSeconds(expiresAt);

                    Session nSession = Session.FromData(model, jwt);

                    nSession.SessionExpireTime = expireTime.DateTime.ToLocalTime();

                    ContentStatus s = ContentStatus.Unknown;
                    Session.LoadAllFromFile(ref s);

                    int foundIndex = Session.indexOf(nSession);

                    if (foundIndex == -1)
                    {
                        Session.Add(nSession);
                    }
                    else
                    {
                        Session.RewriteAt(nSession, foundIndex);
                    }

                    Session.SetCurrent(nSession);

                    Session.GetCurrent().LastLoginTime = DateTime.Now;
                    Session.GetCurrent().Last = true;

                    Session.SaveAll();
                    
                    homeWindow = new Home();
                    homeWindow.StartTimeoutCalculation();
                    homeWindow.Show();

                    break;
                case GenericCode.NetError:

                    errorDesc = "Ошибка подключения!";
                    break;
            }

            if (!string.IsNullOrEmpty(errorDesc))
            {
                var fastNotification = FloatNotification.withClassicChildIncluded(titleError, errorDesc, this);
                await fastNotification.ShowAnimated();
            }


        }

        private void OnShowSavedSessionsButtonClick(object sender, EventArgs e)
        {
            MainFrame.Content = sessionListPage;
            sessionListPage.CancelUpdater();
            sessionListPage.ReloadAll();
            sessionListPage.RunSessionListUpdater();
        }
    
        
        private void sessionFinishedCallback(object data)
        {
            Show();
            AuthSwitch.IsEnabled = true;
            RegSwitch.IsEnabled = true;
        }

        private async void showTokenExpiredWin()
        {
            var uiDisp = this.Dispatcher;
            await Task.Run(() =>
            {
                Thread.Sleep((int)(1.5 * 1000));
                uiDisp.Invoke(() =>
                {
                    MessageBox.Show("Сессия истекла, перезайдите в систему");
                });
            });
        }
    }
}
