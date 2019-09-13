using Noanet.XamArch.Mvvm.Services;
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
using Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels;
using PubSub.Extension;
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum CloseWindowRequest
        {
            None = 0,
            LogOffRequest,
            ShutdownRequest
        }
        private CloseWindowRequest closeWindowRequest = CloseWindowRequest.None;
        public MainWindow()
        {
            InitializeComponent();

            DataContext = ServiceLocator.Current.GetInstance<MainWindowViewModel>();

            this.Subscribe<string>(args => { if (args == nameof(CloseWindowRequest.ShutdownRequest)) { closeWindowRequest = CloseWindowRequest.ShutdownRequest; Application.Current.Shutdown(); }   });
            this.Subscribe<Logic.Validations.AuthenticationResult>(args => { closeWindowRequest = CloseWindowRequest.LogOffRequest; Close(); });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            switch (closeWindowRequest)
            {
                case CloseWindowRequest.None:
                    e.Cancel = true;
                    base.OnClosing(e);
                    break;
                case CloseWindowRequest.LogOffRequest:
                    e.Cancel = false;
                    break;
                case CloseWindowRequest.ShutdownRequest:
                    e.Cancel = true;
                    break;
                default:
                    break;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ItemsListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }
    }
}
