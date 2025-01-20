using RealPropertySystemApp.utils;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace RealPropertySystemApp.ui
{
    /// <summary>
    /// Interaction logic for AppLogViewer.xaml
    /// </summary>
    public partial class AppLogViewer : UserControl
    {
        public AppLogViewer()
        {
            InitializeComponent();
        }

        public void Load()
        {
            string path = FileLogger.GetGenericLogsFilePath();
            try
            {
                XText.Text = File.ReadAllText(path);
            }

            catch (FileNotFoundException)
            {

            }

            catch (DirectoryNotFoundException)
            {

            }
        }
    }
}
