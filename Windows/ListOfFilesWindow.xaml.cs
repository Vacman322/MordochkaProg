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
    /// Interaction logic for ListOfFilesWindow.xaml
    /// </summary>
    public partial class ListOfFilesWindow : Window
    {
        public ListOfFilesWindow(List<DocumentByService> docs)
        {
            InitializeComponent();
            FilesListView.ItemsSource = docs;
        }
    }
}
