using RealPropertySystemApp.bodies;
using RealPropertySystemApp.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for ServerLogViewer.xaml
    /// </summary>
    public partial class ServerLogViewer : UserControl
    {
        private const int DefaultSize = 5;
        private int CurrentPage;
        private int TotalPages;

        private static RPClient client = RPClient.GetClient();
        private static Session current = Session.GetCurrent();
        public ServerLogViewer()
        {
            InitializeComponent();

            NextPage.Click += OnButtonNextClicked;
            PrevPage.Click += OnButtonPrevClicked;
        }

        public bool FillWithData(List<ServerLog> logs)
        {
            foreach (ServerLog log in logs)
            {
                var item = new LogItem(log);
                AddLogItem(item);
            }

            

            return true;
        }

        private bool AddLogItem(LogItem logItem)
        {
            logItem.AdjustWidth(XLogList.Width);
            ListViewItem item = new ListViewItem();
            item.Content = logItem;
            return XLogList.Items.Add(item) != -1;
        }

        private async void OnButtonNextClicked(object sender, RoutedEventArgs e)
        {
            int CurPage = CurrentPage + 1;

            if (CurPage > TotalPages) CurPage = TotalPages;

            SetCurrentPage(CurPage);

            var data = await client.GetLogs(current.Jwt.AccessToken, CurPage);

            XLogList.Items.Clear();

            FillWithData(data);

        }

        private async void OnButtonPrevClicked(object sender, RoutedEventArgs e)
        {
            int CurPage = CurrentPage - 1;

            if (CurPage < 0) CurPage = 0;

            SetCurrentPage(CurPage);


            var data = await client.GetLogs(current.Jwt.AccessToken, CurPage);

            XLogList.Items.Clear();

            FillWithData(data);
        }

        public void SetCurrentPage(int currentPage)
        {
            CurrentPage = currentPage;

            LogPageCurrent.Text = currentPage.ToString();
            
        }

        public void SetTotalPages(int elemCount)
        {
            int pages = GetTotalPages(elemCount);
            
            TotalPages = pages;

            LogPageTotal.Text = pages.ToString();

        }

        private static int GetTotalPages(int count)
        {
            return (count / DefaultSize);
        }
    }
}
