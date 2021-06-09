using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using MordochkaProg.EF;

namespace MordochkaProg
{
    /// <summary>
    /// Interaction logic for AddEditWindow.xaml
    /// </summary>
    public partial class AddEditWindow : Window
    {
        Client client;
        bool isAdd = false;
        string imgPath;
        ObservableCollection<Tag> notUseTags = new ObservableCollection<Tag>();
        ObservableCollection<Tag> useTags = new ObservableCollection<Tag>();
        public AddEditWindow(Client clnt = null)
        {
            InitializeComponent();

            client = clnt;
            if(clnt is null)
            {
                client = new Client();
                isAdd = true;
                IDLabel.Visibility = Visibility.Collapsed;
                IDTextBox.Visibility = Visibility.Collapsed;
            }

            ClientInfoGrid.DataContext = client;

            var clientTagsID = client.Tag.Select(r => r.ID).ToList();
            var tmp = DB.Context.Tag.Where(r => !clientTagsID.Contains(r.ID)).ToArray();
            foreach (var tag in tmp)
            {
                notUseTags.Add(tag);
            }
            NotUseTagsListView.ItemsSource = notUseTags;

            foreach (var tag in client.Tag.ToList())
            {
                useTags.Add(tag);
            }

            UseTagsListView.ItemsSource = useTags;

            GenderComboBox.ItemsSource = DB.Context.Gender
                .ToList();
            GenderComboBox.DisplayMemberPath = "Name";
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            var textBoxes = this.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(f => f.FieldType == typeof(TextBox))
                .ToList();

            foreach (var tbInfo in textBoxes)
            {
                var tb = (TextBox)tbInfo.GetValue(this);
                if(string.IsNullOrWhiteSpace(tb.Text))
                {
                    MessageBox.Show("Заполните все поля!");
                    return;
                }
            }

            if(BirthdayDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Заполните дату рождения");
                return;
            }

            if(GenderComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите пол");
                return;
            }

            var FIO = new[]
            {
                LastNameTextBox,
                FirstNameTextBox,
                PatronymicTextBox
            };


            foreach (var tb in FIO)
            {
                if(!tb.Text
                    .Select(s => char.IsLetter(s) || s == ' ' || s == '-')
                    .Aggregate((b1,b2) => b1 && b2))
                {
                    MessageBox.Show("Поля ФИО могут содержать в себе только буквы и следующие символы: пробел и дефис.");
                    return;
                }
            }

            foreach (var tb in FIO)
            {
                if(tb.Text.Length > 50)
                {
                    MessageBox.Show("Поля фамилии, имени и отчества не могут быть длиннее 50 символов.");
                    return;
                }
            }

            try
            {
                MailAddress mailAddress = new MailAddress(EmailTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Неверный Email");
                return;
            }
            
            if(!PhoneTextBox.Text
                .Select(s => char.IsDigit(s) ||
                s == '+' ||
                s == '-' ||
                s == '(' ||
                s == ')' ||
                s == ' ')
                .Aggregate((b1,b2) => b1 && b2))
            {
                MessageBox.Show("Поле телефона может содержать только цифры и следующие символы: плюс, минус, открывающая и закрывающая круглые скобки, знак пробела.");
                return;
            }

            var result = MessageBox.Show("Вы точно хотите сохранить?", "Сохранение", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            client.PhotoPath = imgPath;
            DB.Context.SaveChanges();
            this.Close();
        }

        private void EditImgButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Изображения|*.jpg; *.jpeg; *.png";
            var result = dialog.ShowDialog();

            if(result.HasValue && result.Value)
            {
                var file = new FileInfo(dialog.FileName);
                if(file.Length > 2000000)
                {
                    MessageBox.Show("Размер фотографии не должен превышать 2 мегабайта");
                    return;
                }

                var prjPath = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName;
                imgPath = "Clients\\" + file.Name;
                file.CopyTo(prjPath + "\\" + imgPath,true);
                ClientImage.Source = new BitmapImage( new Uri(dialog.FileName));
            }
        }

        private void AddTagButtonClick(object sender, RoutedEventArgs e)
        {
            if(NotUseTagsListView.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Выберите тег для добавления");
                return;
            }

            foreach (Tag tag in NotUseTagsListView.SelectedItems)
            {
                client.Tag.Add(tag);
                useTags.Add(tag);
            }

            foreach (var tag in useTags)
            {
                notUseTags.Remove(tag);
            }
        }

        private void DelTagButtonClick(object sender, RoutedEventArgs e)
        {
            if (UseTagsListView.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Выберите тег для удаления");
                return;
            }

            foreach (Tag tag in UseTagsListView.SelectedItems)
            {
                client.Tag.Remove(tag);
                notUseTags.Add(tag);
            }

            foreach (var tag in notUseTags)
            {
                useTags.Remove(tag);
            }
        }
    }
}
