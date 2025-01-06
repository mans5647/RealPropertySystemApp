using RealPropertySystemApp.bodies;
using RealPropertySystemApp.codes;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RealPropertySystemApp.pages
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        private TextBox codeInputBox;
        public RegisterPage()
        {
            InitializeComponent();

            codeInputBox = new TextBox();

            codeInputBox.Width = 200;
            codeInputBox.Height = 50;
            
            DoRegisterButton.Click += DoRegisterButtonClicked;

            iAmEmployeeCheck.Checked += EmployeeStatusChecked;
            iAmEmployeeCheck.Unchecked += EmployeeStatusChecked;
        }

        private void EmployeeStatusChecked(object sender, RoutedEventArgs e)
        {
            bool value = iAmEmployeeCheck.IsChecked.Value;
            if (value)
            {
                PushCodeInputField();
            }

            else PopCodeInputField();
        }

        private async void DoRegisterButtonClicked(object sender, RoutedEventArgs e)
        {
            string[] fieldContent =
            {
                UserLogin.Text, UserPassword.Password, UserPasswordConfirm.Password
            };

            bool isAnyEmpty = fieldContent.Any(x => string.IsNullOrEmpty(x));
            var wnd = Window.GetWindow(this);

            

            if (isAnyEmpty)
            {
                var not = FloatNotification.withClassicChildIncluded("Ошибка валидации", "Заполните все поля!", wnd);
                not.SetBottomRightPlaceLocation();
                await not.ShowAnimated();
                
                return;
            }

            if (UserPasswordConfirm.Password != UserPassword.Password)
            {
                var not = FloatNotification.withClassicChildIncluded("Ошибка валидации", "Пароли не совпадают", wnd);
                not.SetBottomRightPlaceLocation();
                await not.ShowAnimated();
                return;
            }

            if (iAmEmployeeCheck.IsChecked.Value)
            {
                var code_ = codeInputBox.Text;
                if (string.IsNullOrEmpty(code_))
                {
                    var not = FloatNotification.withClassicChildIncluded("Ошибка", "Введите код сотрудника", wnd);
                    not.SetBottomRightPlaceLocation();
                    await not.ShowAnimated();
                    return;
                }
            }

            RPClient client = RPClient.GetClient();
            string login = fieldContent[0].Trim();
            string password = fieldContent[1].Trim();
            string code = codeInputBox.Text;

            var result = await client.CreateAccount(login, password, code);
            
            if (result.IsValid)
            {
                bool canContinue = false;
                string description = result.OptionalMessage;
                switch (result.Code)
                {
                    case GenericCode.NoError:

                        canContinue = true;
                        

                        break;
                    case AuthCode.ErrInvalidFields:

                        description += ":\r\n";

                        for (int i = 0 ; i < result.Errors.Count; i++)
                        {
                            FieldError error = result.Errors[i];

                            description += $"{i + 1}) {error.Message}\r\n";
                        }

                        break;
                    default:

                        break;
                }


                var notification = FloatNotification.withClassicChildIncluded(StandardTitles.ErrCreateAcc, description, wnd);

                notification.SetBottomRightPlaceLocation();
                
                await notification.ShowAnimated();

                if (canContinue)
                {

                }

            }

            else
            {
                
            }

        }

        private void PushCodeInputField()
        {
            EmployeeOptions.Children.Add(codeInputBox);
        }

        private void PopCodeInputField()
        {
            EmployeeOptions.Children.Remove(codeInputBox);
        }

    }
}
