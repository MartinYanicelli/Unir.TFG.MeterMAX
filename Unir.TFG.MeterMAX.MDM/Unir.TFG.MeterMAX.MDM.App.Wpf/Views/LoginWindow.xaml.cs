using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using PubSub.Extension;
using Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels;


namespace Unir.TFG.MeterMAX.MDM.App.Wpf.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            DataContext = Noanet.XamArch.Mvvm.Services.ServiceLocator.Current.GetInstance<LogInViewModel>();

            UserNameTextBox.Focus();

            this.Subscribe<Logic.Validations.AuthenticationResult>(authenticationResult => {
                if (authenticationResult.ValidationResult == Noanet.XamArch.Logic.ValidationResult.Success)
                {
                    Close();
                }
            });
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {

            }
            else
            {

            }
            base.OnStateChanged(e);
        }
    }
}
