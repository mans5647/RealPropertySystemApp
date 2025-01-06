using RealPropertySystemApp.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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

namespace RealPropertySystemApp.ui
{
    /// <summary>
    /// Interaction logic for ClassicNotificationContent.xaml
    /// </summary>
    public partial class ClassicNotificationContent : UserControl, ISecondable
    {
        string fullMessage;
        public ClassicNotificationContent(string title, string description)
        {
            InitializeComponent();
            TitleTb.Text = title;

            fullMessage = description;

            if (description.Length > 40)
            {
                DescriptionTb.Text = description.Substring(0, 40) + "...";
            }

            else DescriptionTb.Text = description;

        }

        public void SetTimeoutLeftSeconds(int seconds)
        {
            SecondsRemeiningTb.Text = seconds.ToString();
        }

    }
}
