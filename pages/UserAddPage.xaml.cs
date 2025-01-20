using RealPropertySystemApp.codes;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RealPropertySystemApp.pages
{

    public partial class UserAddPage : Page
    {


        private UserModel user;
        private UserAddPage()
        {
            InitializeComponent();

            user = new UserModel();
            DataContext = user;
            EditPassportBtn.Click += OnPassportEditClick;
            SaveUserBtn.Click += OnSaveButtonClicked;
        }

        public static UserAddPage Create()
        {
            return new UserAddPage();
        }

        private async void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool result = await RPClient.GetClient().CreateUser(user);
                if (result)
                {
                    await FloatNotification.Create("Успешное создание", "Пользователь добавлен!", Window.GetWindow(this)).ShowAnimated();
                }
                

                Window.GetWindow(this).Close();
            }



            catch (InvalidOperationException ex)
            {
                await FloatNotification.Create("Ошибка создания", ex.Message, Window.GetWindow(this)).ShowAnimated();
            }
            catch (InvalidUserBodyException ex)
            {
                string desc = InvalidUserBodyException.GetAllFormatted(ex);
                await FloatNotification.Create(ex.Message, desc, Window.GetWindow(this)).ShowAnimated();
            }

            catch (Exception ex)
            {
                await FloatNotification.Create("Ошибка создания", "Пользователь не добавлен!", Window.GetWindow(this)).ShowAnimated();
            }
        }

        private void OnPassportEditClick(object sender, RoutedEventArgs e)
        {
            var peditr = UserPassportEditor.NewWithPassport(null, PassportSaveCb);
            peditr.Show();
        }

        private void PassportSaveCb(Passport passport)
        {
            user.passport = passport;
        }
        private void OnPasswordChanged(object sender, EventArgs e)
        {
            user.password = xpassword.Password;
        }
    }
}
