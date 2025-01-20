using RealPropertySystemApp.bodies;
using RealPropertySystemApp.models;
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
    
    public partial class UsersViewer : Page
    {
        private long CurrentPage;
        private long TotalPages;
        private const long CountPerSize = 5;
        private int selectedIndex = -1;
        private bool FilterEnabled = false;
        private UserFilterBody CurrentFilter;
        private RPClient client = RPClient.GetClient();

        private UsersViewer()
        {
            InitializeComponent();
            NextPageButton.Click += NextPageButton_Click;
            PreviousPageButton.Click += PreviousPageButton_Click;

            UsersDataGrid.IsReadOnly = true;
            UsersDataGrid.SelectionMode = DataGridSelectionMode.Single;
            UsersDataGrid.MouseDoubleClick += OnUserDoubleClicked;
            UsersDataGrid.SelectionChanged += OnUserSelected;

            UserFilterBar.ApplyFilterBtn.Click += OnApplyFilterButtonClicked;
            UserFilterBar.ResetFilterBtn.Click += OnResetFilterButtonClicked;
            AddUserBtn.Click += OnAddBtnClicked;
            DeleteUserBtn.Click += OnDeleteBtnPressed;
        }

        private async void OnDeleteBtnPressed(object sender, RoutedEventArgs e)
        {
            if (selectedIndex != -1)
            {
                if (UsersDataGrid.ItemsSource is List<UserModel>  users)
                {
                    var SelectedUser = users[selectedIndex];
                    bool result = await client.DeleteUser(SelectedUser.id);

                    if (result)
                    {
                        UpdateTable();
                        await FloatNotification.Create("Успешно", "Пользователь удален", Window.GetWindow(this)).ShowAnimated();
                    }

                }
            }
        }

        private void OnUserSelected(object sender, SelectionChangedEventArgs e)
        {
            selectedIndex = UsersDataGrid.SelectedIndex;
        }

        private void OnAddBtnClicked(object sender, RoutedEventArgs e)
        {
            Window pwindow = new Window();
            pwindow.Title = "Добавление нового пользователя";
            pwindow.Content = UserAddPage.Create();
            pwindow.Show();
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            PrevPage();
            UpdateTable();
            CounterTb.Text = CurrentPage.ToString();


        }


        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            NextPage();
            UpdateTable();
            CounterTb.Text = CurrentPage.ToString();
        }

        private void OnUserDoubleClicked(object sender, RoutedEventArgs e)
        {
            DataGrid d = (DataGrid)sender;

            if (d.SelectedItem is UserModel m )
            {
                selectedIndex = d.SelectedIndex;
                Window parent = new Window();

                var page = UserProfileEditPage.Create(m, false);
                page.SetOnUpdateCallback(OnUserUpdatedCallback);
                
                parent.Content = page;
                parent.Show();
            }
        }

        private void OnResetFilterButtonClicked(object sender, RoutedEventArgs e)
        {
            FilterEnabled = false;
            UpdateTable();
        }

        private void OnApplyFilterButtonClicked(object sender, RoutedEventArgs e)
        {
            UserFilterBody userFilterBody = new UserFilterBody();
            userFilterBody.Firstname = UserFilterBar.FilterFirstname.Text;
            userFilterBody.Lastname = UserFilterBar.FilterLastname.Text;
            userFilterBody.DateMin = UserFilterBar.FilterDateMin.SelectedDate != null ? UserFilterBar.FilterDateMin.SelectedDate.Value : DateTime.MinValue;
            userFilterBody.DateMax = UserFilterBar.FilterDateMax.SelectedDate != null ? UserFilterBar.FilterDateMax.SelectedDate.Value : DateTime.MaxValue;
            userFilterBody.Rolename = UserFilterBar.FilterRolename.Text;

            FilterEnabled = true;
            CurrentFilter = userFilterBody;


            UpdateTableFiltered(userFilterBody);
        }

        private void OnUserUpdatedCallback(UserModel model)
        {
            var internalArr = UsersDataGrid.ItemsSource as List<UserModel>;

            if (internalArr != null)
            {
                internalArr[selectedIndex] = model;
                UsersDataGrid.Items.Refresh();
            }

            
        }

        public static async Task<UsersViewer> Create()
        {
            var viewer = new UsersViewer();
            var count = await viewer.client.GetTotalCountOfUsers();

            viewer.CurrentPage = 0;
            viewer.TotalPages = CalculateTotal(count);
            
            viewer.UpdateTable();
            viewer.CounterTb.Text = viewer.CurrentPage.ToString();
            return viewer;
        }

        private void ClearTable()
        {
            UsersDataGrid.ItemsSource = null;
        }

        private void PrevPage()
        {
            CurrentPage--;
            if (CurrentPage < 0) CurrentPage = 0;
            
        }
        private void NextPage()
        {
            CurrentPage++;
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
        }

        private async void UpdateTable()
        {
            
            if (FilterEnabled)
            {
                UpdateTableFiltered(CurrentFilter);
            }
            else
            {
                ClearTable();
                List<long> ids = new List<long>
                {
                    Session.GetCurrent().CurrentUser.id
                };
                var data = await client.GetUsersChunked(CurrentPage, CountPerSize, ids);

                UsersDataGrid.ItemsSource = data;
            }
            
        }

        private async void UpdateTableFiltered(UserFilterBody filter)
        {
            ClearTable();
            List<long> ids = new List<long>
            {
                Session.GetCurrent().CurrentUser.id
            };

            var data = await client.GetUsersChunkedAndFiltered(CurrentPage, CountPerSize, ids, filter);

            UsersDataGrid.ItemsSource = data;
        }

        private static long CalculateTotal(long count)
        {
            return (count / CountPerSize);
        }
        
    }
}
