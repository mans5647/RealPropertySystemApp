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
    
    public partial class LogViewPage : Page
    {
        public LogViewPage()
        {
            InitializeComponent();

            XLoadAppLogsButton.Click += OnLoadAppLogsClicked;
            XLoadServerLogsButton.Click += OnLoadServerLogsClicked;
        }

        public void OnLoadAppLogsClicked(object sender, RoutedEventArgs e)
        {
            var appLogsViewer = new AppLogViewer();
            appLogsViewer.Load();
            
            XPageContent.Content = appLogsViewer;
        }

        public async void OnLoadServerLogsClicked(object sender, RoutedEventArgs e)
        {
            var serverLogsViewer = new ServerLogViewer();

            string token = Session.GetCurrent().Jwt.AccessToken;

            var data = await RPClient.GetClient().GetLogs(token, 0);
            var total = await RPClient.GetClient().GetTotalPagesOfLogs(token);
            serverLogsViewer.FillWithData(data);
            serverLogsViewer.SetCurrentPage(0);
            serverLogsViewer.SetTotalPages(total);

            XPageContent.Content = serverLogsViewer;
        }
    }
}
