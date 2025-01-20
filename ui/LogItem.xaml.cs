using RealPropertySystemApp.bodies;
using RealPropertySystemApp.codes;
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

namespace RealPropertySystemApp.ui
{
    /// <summary>
    /// Interaction logic for LogItem.xaml
    /// </summary>
    public partial class LogItem : UserControl
    {

        public LogItem(ServerLog log)
        {
            InitializeComponent();

            XLogColor.Fill = GetLogColor(log.LogLevel);
            XLogText.Text = log.Content;
            XLogTime.Text = log.LogTime.ToString();


        }

        public void AdjustWidth(double contentHolderWidth)
        {
            XLogText.Width += (contentHolderWidth - Width);
        }

        private Brush GetLogColor(int l)
        {
            LogLevel level = (LogLevel)l;
            
            
            switch (level)
            {
                case LogLevel.Debug: return new SolidColorBrush(Colors.AliceBlue);
                case LogLevel.Info: return new SolidColorBrush(Colors.Green);
                case LogLevel.Warn: return new SolidColorBrush(Colors.Yellow);
                case LogLevel.Error: return new SolidColorBrush(Colors.Red);
                case LogLevel.Fatal: return new SolidColorBrush(Colors.Black);
            }

            return null;
        }
    }
}
