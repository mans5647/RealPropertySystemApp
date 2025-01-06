using Newtonsoft.Json;
using RealPropertySystemApp.events;
using RealPropertySystemApp.models;
using RealPropertySystemApp.utils;
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
        private string token;
        public UserProfileEditPage(UserModel prevInfo, string accessToken)
        {
            InitializeComponent();
            editable = new UserModel();
            
            this.DataContext = editable;

            editable.login = prevInfo.login;
            editable.id = prevInfo.id;
            editable.userRole = prevInfo.userRole;
            editable.lastName = prevInfo.lastName;
            editable.firstName = prevInfo.firstName;
            editable.birthDate = prevInfo.birthDate;

            
            originalModel = prevInfo;
            token = accessToken;

            editable.password = JwtManager.GetPayloadProperty("password", accessToken);

            SaveUserBtn.Click += OnSaveButtonClick;

            fillInProps();
            
        }

        private void fillInProps()
        {

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
        }

        public static UserProfileEditPage withUserModel(UserModel data, string accessToken)
        {
            return new UserProfileEditPage(data, accessToken);
        }

        public void SetOnUpdateCallback(MainEvents.UserChanged callback)
        {
            this.callback = callback;
        }

        private async void OnSaveButtonClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(editable.password))
            {
                MessageBox.Show("Заполните пароль!");
                return;
            }


            RPClient client = RPClient.GetClient();

            var fetched = await client.UpdateUserByLogin(editable, token);

            callback(fetched);

        }

        private void onPasswordChanged(object sender, EventArgs e)
        {
            editable.password = ((PasswordBox)sender).Password;
        }
    }
}
