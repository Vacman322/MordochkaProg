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
using MordochkaProg.EF;

namespace MordochkaProg
{
    /// <summary>
    /// Interaction logic for VisitsWindow.xaml
    /// </summary>
    public partial class VisitsWindow : Window
    {
        Client client;
        public VisitsWindow(Client clnt)
        {
            client = clnt;
            InitializeComponent();
            VisitsListView.ItemsSource = client.ClientService;
        }

        private void VisitClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var selected = (ClientService)VisitsListView.SelectedItem;
                new ListOfFilesWindow(selected.DocumentByService.ToList()).ShowDialog();
            }
        }
    }
}
