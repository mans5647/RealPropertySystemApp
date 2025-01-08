using JWT.Algorithms;
using JWT.Serializers;
using JWT;
using Newtonsoft.Json.Linq;
using RealPropertySystemApp.bodies;
using RealPropertySystemApp.codes;
using RealPropertySystemApp.events;
using RealPropertySystemApp.models;
using RealPropertySystemApp.ui;
using RealPropertySystemApp.utils;
using RealPropertySystemApp.windows;
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

            var wnd = Window.GetWindow(this);
            
            string titleError = "Ошибка авторизации", errorDesc = string.Empty;

            int code = (int)data["code"];

            switch (code)
            {
                case AuthCode.AuthLoginNotFound:
                    errorDesc = "Неправильный логин";
                    break;
                case AuthCode.AuthPasswordIncorrect:
                    errorDesc = "Неправильный пароль";

                    break;
                case GenericCode.NoError:

                    JwtResponse jwt = (JwtResponse)data["jwt"];

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

                    var homeWindow = new Home();
                    homeWindow.StartTimeoutCalculation();
                    homeWindow.Show();

                    break;
                case GenericCode.NetError:

                    errorDesc = "Ошибка подключения!";
                    break;
            }

            if (!string.IsNullOrEmpty(errorDesc))
            {
                var fastNotification = FloatNotification.withClassicChildIncluded(titleError, errorDesc, wnd);
                await fastNotification.ShowAnimated();
            }

            doLoginButton.IsEnabled = true;
        }

    }
}
