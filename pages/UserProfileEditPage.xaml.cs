using Newtonsoft.Json;
using RealPropertySystemApp.bodies;
using RealPropertySystemApp.codes;
using RealPropertySystemApp.events;
using RealPropertySystemApp.models;
using RealPropertySystemApp.ui;
using RealPropertySystemApp.utils;
using RealPropertySystemApp.windows;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
    /// Interaction logic for UserProfileEditPage.xaml
    /// </summary>
    public partial class UserProfileEditPage : Page
    {
        private UserModel editable;
        private readonly UserModel originalModel;
        private MainEvents.UserChanged callback;
        private UserPassportEditor EditorWnd;
        private bool IsEmbedded = true;
        private string token;
        public UserProfileEditPage(UserModel prevInfo, string accessToken)
        {
            InitializeComponent();
            editable = new UserModel();
            
            this.DataContext = editable;

            editable.login = prevInfo.login;
            editable.id = prevInfo.id;
            editable.userRole = prevInfo.userRole;
            editable.passport = prevInfo.passport;
            editable.lastName = prevInfo.lastName;
            editable.firstName = prevInfo.firstName;
            editable.birthDate = prevInfo.birthDate;

            
            originalModel = prevInfo;
            token = accessToken;

            
            SaveUserBtn.Click += OnSaveButtonClick;
            EditPassportBtn.Click += EditPassportClicked;
            fillInProps();
            
        }

        private void fillInProps()
        {
            if (editable.login != null)
            {
                xlogin.Text = editable.login;
                filledStatus_login.Text = null;
            }
            if (editable.firstName != null)
            {
                xfname.Text = editable.firstName;
                filledStatus_firstname.Text = null;
            }
            if (editable.lastName != null)
            {
                xlname.Text = editable.lastName;
                filledStatus_lastname.Text = null;
            }
            if (editable.birthDate != null)
            {
                xbirth.Text = editable.birthDate.ToString();
                filledStatus_birth.Text = null;
            }
            if (editable.password != null)
            {
                xpassword.Password = editable.password;
                filledStatus_password.Text = null;
            }

            if (editable.passport != null)
            {
                filledStatus_passport.Text = "Есть";
            }
        }

        public static UserProfileEditPage withUserModel(UserModel data, string accessToken)
        {
            return new UserProfileEditPage(data, accessToken);
        }

        public static UserProfileEditPage Create(UserModel model, bool Embedded)
        {
            var editPage = new UserProfileEditPage(model, Session.GetCurrent().getAT());
            editPage.IsEmbedded = Embedded;
            return editPage;
        }

        public void SetOnUpdateCallback(MainEvents.UserChanged callback)
        {
            this.callback = callback;
        }

        private void OnPassportSaved(Passport passport)
        {
            editable.passport = passport;
        }

        private async void OnSaveButtonClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(editable.password))
            {
                MessageBox.Show("Заполните пароль!");
                return;
            }


            RPClient client = RPClient.GetClient();

            try
            {
                var fetched = await client.UpdateUserByLogin(editable, token);
                callback(fetched);

                if (!IsEmbedded)
                {
                    Window.GetWindow(this).Close();
                }

            }
            catch (InvalidUserBodyException ex)
            {
                ValidationResponse response = ex.Get();

                string desc = string.Empty;

                var errs = response.Errors;

                foreach (FieldError i in errs)
                {
                    desc += (i.Message + '\n');
                }

                await FloatNotification.Create(ex.Message, desc, Window.GetWindow(this)).ShowAnimated();
            }
        }

        private void EditPassportClicked(object sender, RoutedEventArgs e)
        {
            EditorWnd = UserPassportEditor.NewWithPassport(editable.passport, OnPassportSaved);
            EditorWnd.Show();
        }

        private void onPasswordChanged(object sender, EventArgs e)
        {
            editable.password = ((PasswordBox)sender).Password;
        }
    }
}
