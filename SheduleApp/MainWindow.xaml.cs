using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using static SheduleApp.MainWindow;

namespace SheduleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MongoClient _client;

        public bool isLogined;

        public IMongoDatabase _database;
        public IMongoCollection<User> _collectionUser;
        public IMongoCollection<TaskDay> _collectionTasks;
        public IMongoCollection<UserTasks> _userTasks;
        public User currentUser;
        public List<UserTasks> currentUserTasks;
        public class User
        {
            public ObjectId Id { get; set; }
            public string Name { get; set; }
            public string _login { get; set; }
            public string _password { get; set; }
            public int Age { get; set; }
        }
        public class UserTasks
        {
            public ObjectId Id { get; set; }
            public string Description { get; set; }
            /*
             приоритет 1 = 🎯
                2 = 🔥
                3 = 🔥🔥
                4 = ⚡⚡⚡
             */
            public ObjectId userId { get; set; }
            public int? prioritet { get; set; }
            public DateTime? Start { get; set; }
            public DateTime? End { get; set; }
        }
        public class TaskDay
        {
            public ObjectId Id { get; set; }
            public int TaskCount { get; set; }
            public string DayOfWeek { get; set; }
            public List<ObjectId> TasksID { get; set; }
            public DateTime Date { get; set; }
        }
         
        public MainWindow()
        {
            InitializeComponent();

           

            // подключение бд
            ConnectToMongoDB();
            // просто вывод пользователей из коллекции
            // LoadAndDisplayUsers();
           
            KeyDown += MainWindow_KeyDown;


        }

        public void checkLogin()
        {

            // Путь к файлу cfg.txt
            string filePath = Path.Combine(Environment.CurrentDirectory, "data", "cfg.txt");
            
            // Проверяем, существует ли файл
            if (File.Exists(filePath))
            {
                // Читаем первую строку из файла
                ObjectId usrid = ObjectId.Parse(File.ReadLines(filePath).First());
                var filter = Builders<User>.Filter.Eq("_id", usrid);
                currentUser = _collectionUser.Find(filter).FirstOrDefault();
                if (currentUser != null)
                {
                    isLogined = true;
                }
                else
                {
                    isLogined = false;
                    MessageBox.Show("Вас нету в базе данных");
                }

            }
           
        }
        public void ConnectToMongoDB()
        {
            // Подключение к MongoDB и получение всех коллекций 
        
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            // Get MongoDB connection string from configuration
            var connectionString = configuration.GetConnectionString("MongoDB");

            _client = new MongoClient(connectionString);

            _database = _client.GetDatabase("SheduleDB");
            _collectionUser = _database.GetCollection<User>("User"); // Изменено на GetCollection<User>
            _collectionTasks = _database.GetCollection<TaskDay>("TaskDay");
            //получем пользователя currentUser
            checkLogin();
            
            _userTasks = _database.GetCollection<UserTasks>("UserTasks");


            // Создание объекта User

            //User user = new User
            //{
            //    Name = "Jane Doe",
            //    _login = "user_jane_doe",
            //    _password = "passwrd",
            //    Age = 22,
            //};
            //var taskDay = new TaskDay
            //{
            //    TaskCount = 42,
            //    Date = DateTime.Now,
            //    DayOfWeek = DateTime.Now.Day.ToString(),
            //    TasksID = new List<ObjectId>(),

            //};
            //var task = new UserTasks
            //{
            //    Description = "Задача с ID юзера",
            //    prioritet = 4,
            //    userId = currentUser.Id,
            //    Start = DateTime.Now,
            //    End = DateTime.Now,
            //};
            //_userTasks.InsertOne(task);
            //Добавление записей по такому шаблону
            //_userTasks.InsertOne(task);
            //_collectionTasks.InsertOne(taskDay);
             //_collectionUser.InsertOne(user);

            PageInit();
            Console.WriteLine("Документ успешно создан.");
        }

        private void LoadAndDisplayUsers()
        {
            var users = _collectionUser.Find(new BsonDocument()).ToList(); 

            string message = "";
            foreach (var user in users)
            {
                message += $"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}{Environment.NewLine}";
            }

            MessageBox.Show(message, "Данные из коллекции");
        }

        private TextBlock СreateTaskLabel(string taskDesc, int? prioritet)
        {
            string prioSybmol = "";
            if (prioritet.HasValue)
            {
                if (prioritet == 1)
                {
                    prioSybmol = " 🎯";
                }
                else if(prioritet == 2)
                {
                    prioSybmol = " 🔥";
                }else if(prioritet == 3)
                {
                    prioSybmol = " 🔥 🔥";
                }else
                {
                    prioSybmol = " ⚡ ⚡ ⚡";
                }
            }
            Color color;
            
            if (prioritet == 1)
            {
                //белый
                color = Color.FromRgb(255,255,255);
            }else if(prioritet == 2)
            {
                //слегка красный
                color = Color.FromRgb(250, 215, 207);
            }
            else if(prioritet == 3)
            {
                //оранжевый
                color = Color.FromRgb(237, 152, 75);
            }
            else
            {
                //краснющий
                color = Color.FromRgb(219, 59, 30);
            }
            
            TextBlock label = new TextBlock
            {
                Background = new SolidColorBrush(color),
                FontSize = 20,
                MinHeight = 60,
                MaxWidth = 700,
                Opacity = 0.9,
                
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Padding = new Thickness(10),
                TextWrapping = TextWrapping.Wrap,
                Text = taskDesc.ToString() +" " + prioSybmol
            };

            // Создание стиля для Label
            Style labelStyle = new Style(typeof(TextBlock));
            labelStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.Black));


            label.MouseEnter += (sender, e) =>
            {
                label.Opacity = 1;
                label.FontWeight = FontWeights.Bold;
            };

            // Изменяем Opacity при событии MouseLeave
            label.MouseLeave += (sender, e) =>
            {
                label.Opacity = 0.9;
                label.FontWeight = FontWeights.Normal;
            };


            // Добавление триггера для изменения цвета текста при наведении курсора
            //Trigger mouseOverTrigger = new Trigger { Property = TextBlock.IsMouseOverProperty, Value = true };
            //mouseOverTrigger.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.Red));
           
           

           // labelStyle.Triggers.Add(mouseOverTrigger);


            label.Style = labelStyle;

            return label;
        }
        public void PageInit()
        {
            // Создание объекта Page1
            Page1 page = new Page1();
            //обновляем таски
            _userTasks = _database.GetCollection<UserTasks>("UserTasks");

            var filter = Builders<UserTasks>.Filter.Eq(u => u.userId, currentUser.Id);

            // Получаем коллекцию задач, отфильтрованную по userId
            currentUserTasks = _userTasks.Find(filter).ToList();

            // Сортируем список задач по prioritet в обратном порядке
            currentUserTasks = currentUserTasks.OrderByDescending(t => t.prioritet).ToList();
            // var allUserTasks = _userTasks.Find(Builders<UserTasks>.Filter.Empty).ToList();


            // Получение ссылки на существующий StackPanel
            StackPanel stackPanel = page.FindName("page1StackPanel") as StackPanel; // Замените "MyStackPanel" на фактическое имя вашего StackPanel



            // Проход по элементам коллекции
            foreach (UserTasks userTask in currentUserTasks)
            {
                TextBlock label = СreateTaskLabel(userTask.Description, userTask.prioritet);



                // Создание Border с заданными свойствами
                Border border = new Border
                {
                    Margin = new Thickness(0, 10, 0, 10),
                    CornerRadius = new CornerRadius(6),
                    // Padding = new Thickness(2),
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Black,
                    Background = label.Background,
                    
                 };
                border.MouseLeftButtonDown += (sender, e) =>
                {
                    // MessageBox.Show("Задание выполнено");
                    label = null;
                    // Удаление Border из StackPanel
                    stackPanel.Children.Remove(border);
                    SoundPlayer soundPlayer = new SoundPlayer("D:\\ПРОЕКТЫ C# EXAM\\SheduleApp\\SheduleApp\\taskComplete.wav");
                    soundPlayer.Play();
                    var filter = Builders<UserTasks>.Filter.Eq(u => u.userId, currentUser.Id);

                    // Удалить одну задачу, соответствующую фильтру 
                    _userTasks.DeleteOne(filter);

                };
                //удаление задачи у текущего пользователя
                label.MouseLeftButtonDown += (sender, e) =>
                {
                    // MessageBox.Show("Задание выполнено");
                    label = null;
                    // Удаление Border из StackPanel
                    stackPanel.Children.Remove(border);
                    SoundPlayer soundPlayer = new SoundPlayer("D:\\ПРОЕКТЫ C# EXAM\\SheduleApp\\SheduleApp\\taskComplete.wav");
                    soundPlayer.Play();
                    var filter = Builders<UserTasks>.Filter.Eq(u => u.userId, currentUser.Id);

                    // Удалить одну задачу, соответствующую фильтру 
                    _userTasks.DeleteOne(filter);

                };

                // Добавление Label в Border
                border.Child = label;


                // Добавление Border в StackPanel
                stackPanel.Children.Add(border);
            }

            // Переход на страницу Page1
            Frame.Navigate(page);
        }
        private void TextBlock_Clicked(object sender, RoutedEventArgs e)
        {
            PageInit();
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            //// Проверяем, была ли нажата клавиша Enter
            //if (e.Key == Key.Enter)
            //{
            //    // Выполняем код, который нужно выполнить при нажатии Enter
                
            //    MessageBox.Show("Клавиша Enter была нажата!");
            //}
        }

        private void Para_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Page2());
        }
    }
}