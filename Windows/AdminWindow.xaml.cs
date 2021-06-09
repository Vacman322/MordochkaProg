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
using MordochkaProg.EF;

namespace MordochkaProg
{
    using Sortings = Dictionary<string, Func<List<Client>, IEnumerable<Client>>>;
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        int pageNumber = 0;
        int maxPageNumber;
        List<Client> clients;
        List<string> recordCount = new List<string>()
        {
            "10",
            "50",
            "200",
            "все"
        };

        Sortings sortings = new Sortings()
        {
            {"по ID", l => l.OrderBy(r=> r.ID)},
            {"фамилии (в алфавитном порядке)", l => l.OrderBy(r=> r.LastName)},
            {"дате последнего посещения (от новых к старым)", l => l.OrderByDescending(r=> r.LastVisit)},
            {"количеству посещений (от большего к меньшему)", l => l.OrderByDescending(r=> r.VisitsCount)}
        };

        public AdminWindow()
        {
            InitializeComponent();
            RecordCountComboBox.ItemsSource = recordCount;
            RecordCountComboBox.SelectedIndex = 0;

            var genderList = DB.Context.Gender
                .Select(r => r.Name)
                .ToList();
            genderList.Insert(0, "все");
            GenderComboBox.ItemsSource = genderList;
            GenderComboBox.SelectedIndex = 0;

            SortingComboBox.ItemsSource = sortings.Keys;
            SortingComboBox.SelectedIndex = 0;
            UpdateClients();
        }

        private void UpdateClients(bool updatePageNumber = false)
        {
            DB.Context.Dispose();
            DB.Context = new Context();
            if (updatePageNumber)
                pageNumber = 0;

            clients = DB.Context.Client.ToList();

            var selectedGender = (string)GenderComboBox.SelectedItem;
            if (selectedGender != "все")
            {
                clients = clients
                    .Where(r => r.Gender.Name == selectedGender)
                    .ToList();
            }

            if(!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                var input = SearchTextBox.Text;
                clients = clients
                    .Where(r => r.FirstName.Contains(input) ||
                    r.LastName.Contains(input) ||
                    r.Patronymic.Contains(input) ||
                    r.Email.Contains(input) ||
                    r.Phone.Contains(input))
                    .ToList();
            }

            if(BirthCheckBox.IsChecked.HasValue && BirthCheckBox.IsChecked.Value)
            {
                clients = clients
                    .Where(r => r.Birthday.HasValue && r.Birthday.Value.Month == DateTime.Now.Month)
                    .ToList();
            }

            int filteredRecordCount = clients.Count;

            if(SortingComboBox.SelectedItem != null)
                clients = sortings[(string)SortingComboBox.SelectedItem](clients).ToList();

            var countRecordsOnPageStr = (string)RecordCountComboBox.SelectedItem;
            if (!countRecordsOnPageStr.Equals("все"))
            {
                var countRecordsOnPageInt = int.Parse(countRecordsOnPageStr);
                maxPageNumber = (int)Math.Ceiling((double)clients.Count / (double)countRecordsOnPageInt);
                clients = clients
                    .Skip(countRecordsOnPageInt * pageNumber)
                    .Take(countRecordsOnPageInt)
                    .ToList();
            }
            CountRecordsLabel.Content = $"{clients.Count} из {filteredRecordCount}";

            DataListView.ItemsSource = clients;
        }

        private void RecordCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients(true);
        }

        private void PrevButtonClick(object sender, RoutedEventArgs e)
        {
            if (pageNumber <= 0)
                return;
            pageNumber--;
            UpdateClients();
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            if (pageNumber + 1 < maxPageNumber)
                pageNumber++;
            UpdateClients();
        }

        private void GenderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients(true);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClients(true);
        }

        private void SortingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void CheckChanged(object sender, RoutedEventArgs e)
        {
            UpdateClients(true);
        }

        private void DelButtonClick(object sender, RoutedEventArgs e)
        {
            if(DataListView.SelectedItem is null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            var result = MessageBox.Show("Вы точно хотите удалить эту запись?","Удаление",MessageBoxButton.YesNo);

            if(result == MessageBoxResult.No)
            {
                return;
            }

            var selectedClient = (Client)DataListView.SelectedItem;

            if(selectedClient.ClientService.Count > 0)
            {
                MessageBox.Show("У клиента есть информация о посещениях, удаление запрещено");
                return;
            }

            foreach (var tag in selectedClient.Tag)
            {
                selectedClient.Tag.Remove(tag);
            }

            DB.Context.Client.Remove(selectedClient);
            DB.Context.SaveChanges();
            UpdateClients();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            new AddEditWindow().ShowDialog();
            UpdateClients();
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            if (DataListView.SelectedItem is null)
            {
                MessageBox.Show("Выберите запись для изменения");
                return;
            }

            var selectedClient = (Client)DataListView.SelectedItem;

            new AddEditWindow(selectedClient).ShowDialog();
            UpdateClients();
        }

        private void VisitsButtonClick(object sender, RoutedEventArgs e)
        {
            if (DataListView.SelectedItem is null)
            {
                MessageBox.Show("Выберите запись для просмотра посещений");
                return;
            }

            var selectedClient = (Client)DataListView.SelectedItem;

            if(selectedClient.ClientService.Count <= 0)
            {
                MessageBox.Show("У пользователя 0 посещений");
                return;
            }

            new VisitsWindow(selectedClient).ShowDialog();
        }
    }
}
