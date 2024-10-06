using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
using static SheduleApp.MainWindow;

namespace SheduleApp
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        User user;
        MainWindow mainWindow;
        public Page1()
        {
            InitializeComponent();


            TextBoxNewTask.MouseLeftButtonDown += TextBoxNewTask_MouseLeftButtonDown;
        }
        //Выбор поля
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           

        }
      

        private void TextBoxNewTask_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            mainWindow = (MainWindow)Window.GetWindow(this);
            
            user = mainWindow.currentUser;
            
            if (e.Key == Key.Enter)
            {
                //создать новую таску
                var taskDesc = TextBoxNewTask.Text;
                if(taskDesc != null)
                {
                  

                    var task = new UserTasks
                    {
                        Description = $"{taskDesc}",
                        prioritet = 4,
                        userId = user.Id,
                        Start = DateTime.Now,
                        End = DateTime.Now,
                    };
                    
                    mainWindow._USER_TASKS.InsertOne(task);
                    mainWindow.PageInit();
                }
              //  MessageBox.Show($"Задача создана {}");
            }

        }

        private void TextBoxNewTask_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBoxNewTask.Text = "";
        }

        private void TextBoxNewTask_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Стираем текст в TextBox
            TextBoxNewTask.Text = string.Empty;

            // Устанавливаем фокус на TextBox, чтобы можно было начать ввод текста
            TextBoxNewTask.Focus();
        }
    }
}
