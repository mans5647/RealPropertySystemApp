using RealPropertySystemApp.codes;
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

    class SessionRow
    {
        private readonly Session _value;
        public int Number {  get; set; }
        public string Name { get; set; }
        public string TimeLeftPretty { get; set; }
        public string IsRecoveryAvailable { get; set; }
        public string RecTimeLeft {  get; set; }
        public string LastLoginPretty { get; set; }

        public Session Value
        {
            get { return _value; }
        }

        public SessionRow(Session s)
        {
            _value = s;
        }

    }


    /// <summary>
    /// Interaction logic for SessionListPage.xaml
    /// </summary>
    public partial class SessionListPage : Page
    {
        private CancellationTokenSource tokenSource;
        private CancellationToken updaterCanceler;
        public SessionListPage()
        {
            InitializeComponent();

            SessionsTable.IsReadOnly = true;
            
            tokenSource = new CancellationTokenSource();
            updaterCanceler = tokenSource.Token;
            
            AddColumnsToDG();
            RegisterCallbacks();
        }

        private void AddColumnsToDG()
        {
            DataGridTextColumn SessionNumberHeader, SessionTimeLeftHeader,
                SessionIsRecoveryAvailable, SessionUserNameHeader, SessionUserLastLoginHeader,
                SessionRecoveryTimeLeft;

            SessionNumberHeader = new DataGridTextColumn();
            SessionUserNameHeader = new DataGridTextColumn();
            SessionTimeLeftHeader = new DataGridTextColumn();
            SessionIsRecoveryAvailable = new DataGridTextColumn();
            SessionUserLastLoginHeader = new DataGridTextColumn();
            SessionRecoveryTimeLeft = new DataGridTextColumn();


            SessionNumberHeader.Header = "Номер";
            SessionNumberHeader.Binding = new Binding("Number");


            SessionUserNameHeader.Header = "Имя пользователя";
            SessionUserNameHeader.Binding = new Binding("Name");

            SessionTimeLeftHeader.Header = "Осталось";
            SessionTimeLeftHeader.Binding = new Binding("TimeLeftPretty");

            SessionIsRecoveryAvailable.Header = "Возобновление";
            SessionIsRecoveryAvailable.Binding = new Binding("IsRecoveryAvailable");

            SessionRecoveryTimeLeft.Header = "Время до окончания возобновления";
            SessionRecoveryTimeLeft.Binding = new Binding("RecTimeLeft");

            SessionUserLastLoginHeader.Header = "Последний вход";
            SessionUserLastLoginHeader.Binding = new Binding("LastLoginPretty");
            


            SessionsTable.Columns.Add(SessionNumberHeader);
            SessionsTable.Columns.Add(SessionUserNameHeader);
            SessionsTable.Columns.Add(SessionTimeLeftHeader);
            SessionsTable.Columns.Add(SessionIsRecoveryAvailable);
            SessionsTable.Columns.Add(SessionRecoveryTimeLeft);
            SessionsTable.Columns.Add(SessionUserLastLoginHeader);
        }

        private void RegisterCallbacks()
        {
            SessionsTable.MouseDoubleClick += OnDoubleClickedItem;
        }

        private async void OnDoubleClickedItem(object sender, EventArgs e)
        {
            DataGrid dg = (DataGrid)sender;

            var wnd = Window.GetWindow(this);
            var sessionRow = dg.SelectedItem as SessionRow;
            var index = dg.SelectedIndex;
            bool isRedirectAllowed = false;
            if (sessionRow != null )
            {
                var session = sessionRow.Value;
                
                if (session.isSessionExpired())
                {
                    if (session.isRecoveryAvailable())
                    {
                        var Result =  MessageBox.Show("Вы можете восстановить сессию. Продолжить?", "Восстановление сессии", MessageBoxButton.YesNo);
                        if (Result == MessageBoxResult.Yes)
                        {
                            var nJwt = await RPClient.GetClient().RefreshToken(session.Jwt.RefreshToken);

                            session.Jwt = nJwt;
                            session.Last = true;
                            session.LastLoginTime = DateTime.Now;
                            session.SessionExpireTime = JwtManager.GetExpProperty(session.Jwt.AccessToken);
                            Session.SetCurrent(session);
                            Session.RewriteAt(session, index);

                            Session.SaveAll();

                            isRedirectAllowed = true;
                            
                        }
                    }
                    else
                    {
                        var notify = FloatNotification.withClassicChildIncluded("Ошибка", "Пройдите процедуру авторизации заново", wnd);
                        await notify.ShowAnimated();
                    }
                }

                else
                {
                    session.Last = true;
                    session.LastLoginTime = DateTime.Now;

                    Session.SetCurrent(session);

                    Session.RewriteAt(session, index);
                    Session.SaveAll();
                    isRedirectAllowed = true;

                }

                if (isRedirectAllowed)
                {
                    var homeWnd = new Home();
                    homeWnd.StartTimeoutCalculation();
                    homeWnd.Show();

                    wnd.Close();
                }

            }
        }

        public async void ReloadAll()
        {
            ClearAll();

            ContentStatus stat = ContentStatus.OK;

            var LoadThread = () =>
            {
                Session.LoadAllFromFile(ref stat);
                return Session.Source();
            };

            LoadTb.Text = "Загрузка сессий ...";

            var data = await Task.Run(LoadThread);

            FillWithData(data);
            LoadTb.Text = "Сессии загружены ...";
            SessionCountTb.Text = $"Всего: {SessionsTable.Items.Count}";
        }

        private void FillWithData(List<Session> sessions)
        {
            int counter = 0;
            foreach (Session session in sessions)
            {
                SessionRow row = new SessionRow(session);

                row.Number = FmtGetNext(counter);
                row.Name = session.CurrentUser.login;
                row.TimeLeftPretty = FmtTimeLeft(session);
                row.LastLoginPretty = FmtLastLogin(session);
                bool recAvailable = true;
                row.RecTimeLeft = FmtRefreshTokenGetTimeLeft(session.Jwt.RefreshToken, ref recAvailable);

                if (recAvailable)
                {
                    row.IsRecoveryAvailable = "Да";
                }
                else row.IsRecoveryAvailable = "Нет";

                SessionsTable.Items.Add(row);
                counter++;
            }
        }

        private void UpdateSessionsList()
        {
            while (true)
            {
                if (updaterCanceler.IsCancellationRequested) break;

                int Size = Dispatcher.Invoke(() =>
                {
                    return SessionsTable.Items.Count;
                });

                for (int i = 0; i < Size; i++)
                {
                    Dispatcher.Invoke(() =>
                    {
                        var item = SessionsTable.Items[i] as SessionRow;
                        item.TimeLeftPretty = FmtTimeLeft(item.Value);
                        bool recAvailable = true;
                        item.RecTimeLeft = FmtRefreshTokenGetTimeLeft(item.Value.Jwt.RefreshToken, ref recAvailable);

                        if (recAvailable)
                        {
                            item.IsRecoveryAvailable = "Да";
                        }
                        else item.IsRecoveryAvailable = "Нет";

                        SessionsTable.Items.Refresh();
                    });
                }


                Thread.Sleep(1000);
            }
        }


        public async void RunSessionListUpdater()
        {
            tokenSource = new CancellationTokenSource();
            updaterCanceler = tokenSource.Token;
            await Task.Run(UpdateSessionsList, updaterCanceler);
        }

        public void CancelUpdater()
        {
            tokenSource.Cancel();
        }



        private int FmtGetNext(int c)
        {
            return c + 1;
        }

        private string FmtTimeLeft(Session session)
        {
            var timeLeft = -(DateTime.Now - session.SessionExpireTime);
            if (timeLeft == TimeSpan.Zero || DateTime.Now >= session.SessionExpireTime)
            {
                return "Истекла";
            }
            
            return $"{timeLeft.Minutes} минут, {timeLeft.Seconds} секунд";
        }

        private string FmtLastLogin(Session session)
        {
            return session.LastLoginTime.ToString("H часов mm минут d ddd, yyy");
        }

        private string FmtRefreshTokenGetTimeLeft(string refreshToken, ref bool recoveryAvailable)
        {
            var timestamp = JwtManager.GetPayloadPropertyAs<long>("exp", refreshToken);

            var dt = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime();
            var diff = -(DateTime.Now - dt);

            if (diff == TimeSpan.Zero || DateTime.Now >= dt)
            {
                recoveryAvailable = false;
                return "Истек";
            }

            return ($"{diff.Minutes} минут, {diff.Seconds} секунд");
        }

        private void ClearAll()
        {
            SessionsTable.Items.Clear();
        }

    }
}
