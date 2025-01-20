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
    /// Interaction logic for UserPassportEditor.xaml
    /// </summary>
    public partial class UserPassportEditor : Window
    {
        private Passport Editable;
        private event EventHandler SavedHandler;

        public delegate void SaveCb(Passport p);

        SaveCb cb;

        private UserPassportEditor()
        {
            InitializeComponent();
            
        }

        private UserPassportEditor(Passport passport, SaveCb s)
        {
            InitializeComponent();

            DoSaveButton.Click += OnSaveClicked;
            Editable = passport;

            cb = s;
            
            FillIn();
        }

        public static UserPassportEditor Create()
        {
            return new UserPassportEditor();
        }

        public static UserPassportEditor NewWithPassport(Passport passport, SaveCb cb)
        {
            return new UserPassportEditor(passport, cb);
        }

        public Passport GetEditedPassport()
        {
            return Editable;
        }

        private async void OnSaveClicked(object sender, RoutedEventArgs e)
        {

            try
            {
                bool DoAdd = false;
                if (Editable == null)
                {
                    Editable = new Passport();
                    DoAdd = true;
                }

                Editable.DivisionCode = XPassportCodeDiv.Text;
                Editable.GivenBy = XPassportGivenBy.Text;
                Editable.GivenDate = DateTime.Parse(XPassportGivenDate.Text);
                Editable.Number = int.Parse(XPassportNum.Text);
                Editable.Series = int.Parse(XPassportSeries.Text);
                

                if (XPassportGenderCh.SelectedIndex == 0)
                {
                    Editable.Sex = true;
                }
                else Editable.Sex = false;

                Passport updatedValue = null;

                if (DoAdd)
                {
                    updatedValue = await RPClient.GetClient().AddPassport(Editable);
                }
                else
                {
                    updatedValue = await RPClient.GetClient().SavePassport(Editable);
                }

                if (updatedValue != null)
                {
                    cb(updatedValue);
                }
                
                Close();
            }

            catch (Exception ex)
            {
                await FloatNotification.Create("Заполнение", "Ошибка в формате", Window.GetWindow(this)).ShowAnimated();
            }

        }

        private void FillIn()
        {
            if (Editable != null)
            {
                XPassportCodeDiv.Text = Editable.DivisionCode;
                XPassportGivenBy.Text = Editable.GivenBy;
                XPassportGivenDate.Text = Editable.GivenDate.ToString();
                XPassportNum.Text = Editable.Number.ToString();
                XPassportSeries.Text = Editable.Series.ToString();
                XPassportGenderCh.Text = (Editable.Sex) ? "Муж." : "Жен."; 
                XPassportGenderCh.SelectedIndex = (Editable.Sex) ? 0 : 1;
            }
        }

        

    }
}
