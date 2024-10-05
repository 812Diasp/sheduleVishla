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

namespace SheduleApp
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }
        private void RegisterButtonClick(object sender, RoutedEventArgs e)
        {
            // Получаем значения из TextBox и PasswordBox
            string name = txtName.Text;
            string login = txtLogin.Text;
            string password = txtPassword.Password;
            int age = int.Parse(txtAge.Text);
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password)|| age>130)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }
            // Передаем данные в основное окно
            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.ReceiveRegistrationData(name, login, password, age);
            }

            // Закрываем окно регистрации
            this.Close();
        }
    }
}
