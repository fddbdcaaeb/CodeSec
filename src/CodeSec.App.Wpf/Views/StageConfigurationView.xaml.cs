using CodeSec.App.Wpf.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
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

namespace CodeSec.App.Wpf.Views
{
    public partial class StageConfigurationView : Window
    {
        public StageConfigurationView()
        {
            ViewModel = new StageConfigurationViewModel();
            DataContext = ViewModel;

            InitializeComponent();


            var c = new CommandBinding();
            var c1 = new RoutedCommand();

            var dlg = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            //var result = dlg.ShowDialog();

            return;
        }

        public StageConfigurationViewModel ViewModel { get; }


        private void OnClickLoading(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
