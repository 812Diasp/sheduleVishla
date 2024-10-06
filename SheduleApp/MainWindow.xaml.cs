using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using SharpCompress.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static SheduleApp.MainWindow;
using Path = System.IO.Path;

namespace SheduleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MongoClient _client;

        public bool isLogined;

        public IMongoDatabase _DATABASE;
        public IMongoCollection<User> _COLLECTION_USER;
        public IMongoCollection<TaskDay> _COLLECTION_TASKS;
        public IMongoCollection<UserTasks> _USER_TASKS;
        



        public User currentUser { get; set; } = new User();
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
            setupUserAvatar();
            //для получения USER по страницам
            

        }
        public void getRandomColorForAvatar()
        {
            Color[] colors = new Color[] {
                Color.FromRgb(255, 0, 0),
               Color.FromRgb(0, 191, 255),
                Color.FromRgb(148, 0, 211),
                Color.FromRgb(165, 82, 42),
                Color.FromRgb(139, 0, 0),
                Color.FromRgb(0, 0, 128),
                Color.FromRgb(128, 0, 128),
            };
            Random random = new Random();
            int randomIndex = random.Next(colors.Length);
            SolidColorBrush solidColorBrush = new SolidColorBrush(colors[randomIndex]);
            UserAvatar.Background = solidColorBrush;
        }
        public void setupUserAvatar()
        {
            //первые две буквы из имени теущего пользователя в аватарке
            TextInUserAvatar.Text = currentUser.Name.ToString().Substring(0, 2); ;
            getRandomColorForAvatar();
        }
        public void registrateUser(string jsonStringPath)
        {
            isLogined = false;
            MessageBox.Show("Вас нету в базе данных");
            RegistrationWindow registration = new RegistrationWindow();
            registration.ShowDialog();
            User regUser = new User()
            {
                Name = lblName,
                _login = lblLogin,
                _password = lblPassword,
                Age = lblAge,
            };
            _COLLECTION_USER.InsertOne(regUser);
            try
            {
                //обновляем записть в json файле
                // Новое значение для currentUserID
                string newCurrentUserID = regUser.Id.ToString();

                // Загружаем JSON файл
                string jsonString = File.ReadAllText(jsonStringPath);

                // Десериализуем JSON в объект
                RootObject rootObject = JsonSerializer.Deserialize<RootObject>(jsonString);

                // Изменяем значение currentUserID
                rootObject.ConnectionStrings.currentUserID = newCurrentUserID;

                // Сериализуем объект обратно в JSON
                string updatedJsonString = JsonSerializer.Serialize(rootObject, new JsonSerializerOptions { WriteIndented = true });

                // Записываем обновленный JSON в файл
                File.WriteAllText(jsonStringPath, updatedJsonString);

                MessageBox.Show("currentUserID успешно обновлен.");


                MessageBox.Show("Вы успешно зарегистрировались");


            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
            checkLogin();
        }
        public void checkLogin()
        {

           

            string jsonStringPath = File.ReadAllText(Directory.GetCurrentDirectory()+ "/appsettings.json");

            // Десериализуем JSON в объект
            RootObject rootObject = JsonSerializer.Deserialize<RootObject>(jsonStringPath);

            // Получаем currentUserID
            string currentUserID = rootObject.ConnectionStrings.currentUserID;

            // Выводим currentUserID в консоль (или используйте его по вашему назначению)
          // MessageBox.Show($"currentUserID: {currentUserID}");



            // Проверяем, существует ли файл
            if (currentUserID.Length >= 4)
            {
                // Читаем первую строку из файла
                try {
                    ObjectId usrid = ObjectId.Parse(currentUserID);
                    var filter = Builders<User>.Filter.Eq("_id", usrid);
                    currentUser = _COLLECTION_USER.Find(filter).FirstOrDefault();
                } catch {
                    registrateUser(jsonStringPath);
                }
               
                if (currentUser != null)
                {
                    isLogined = true;
                }
                else
                {
                    registrateUser(jsonStringPath);

                }

            }
           
        }
       
        // Класс для десериализации JSON
        public class RootObject
        {
            public ConnectionStrings ConnectionStrings { get; set; }
        }

        // Класс для десериализации ConnectionStrings
        public class ConnectionStrings
        {
            public string MongoDB { get; set; }
            public string currentUserID { get; set; }
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

            _DATABASE = _client.GetDatabase("SheduleDB");
            _COLLECTION_USER = _DATABASE.GetCollection<User>("User"); // Изменено на GetCollection<User>
            _COLLECTION_TASKS = _DATABASE.GetCollection<TaskDay>("TaskDay");
            //получем пользователя currentUser
            checkLogin();
            UserNicknameBox.Text = currentUser.Name;
            _USER_TASKS = _DATABASE.GetCollection<UserTasks>("UserTasks");


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

        public string lblName;
        public string lblLogin;
        public string lblPassword;
        public int lblAge;
        //Если пользователь регистируется в окне использем этот метод для получения данных формы
        public void ReceiveRegistrationData(string name, string login, string password, int age)
        {
            // Используйте полученные данные в основном окне
            // Например, выведите их в Label:
            lblName = name;
            lblLogin = login;
            lblPassword = password;
            lblAge = age;
        }


        private void LoadAndDisplayUsers()
        {
            var users = _COLLECTION_USER.Find(new BsonDocument()).ToList(); 

            string message = "";
            foreach (var user in users)
            {
                message += $"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}{Environment.NewLine}";
            }

            MessageBox.Show(message, "Данные из коллекции");
        }

        private TextBlock СreateTaskLabel(string taskDesc, int? prioritet)
        {
           
           
            Color color;
            
            if (prioritet == 1)
            {
                //белый
                color = Color.FromRgb(255,255,255);
            }else if(prioritet == 2)
            {
                //слегка красный
                color = Color.FromRgb(209, 255, 212);
            }
            else if(prioritet == 3)
            {
                //красноватый
                color = Color.FromRgb(255, 205, 205);
            }
            else
            {
                //краснющий
                color = Color.FromRgb(255, 139, 139);
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
                Text = taskDesc.ToString() + " ",
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



            label.Style = labelStyle;

            return label;
        }
        public void PageInit()
        {
            // Создание объекта Page1
            Page1 page = new Page1();
            //обновляем таски
            _USER_TASKS = _DATABASE.GetCollection<UserTasks>("UserTasks");

            var filter = Builders<UserTasks>.Filter.Eq(u => u.userId, currentUser.Id);

            // Получаем коллекцию задач, отфильтрованную по userId
            currentUserTasks = _USER_TASKS.Find(filter).ToList();

            // Сортируем список задач по prioritet в обратном порядке
            currentUserTasks = currentUserTasks.OrderByDescending(t => t.prioritet).ToList();
            // var allUserTasks = _userTasks.Find(Builders<UserTasks>.Filter.Empty).ToList();


            // Получение ссылки на существующий StackPanel
            StackPanel stackPanel = page.FindName("page1StackPanel") as StackPanel; // Замените "MyStackPanel" на фактическое имя вашего StackPanel



            // Проход по элементам коллекции
            foreach (UserTasks userTask in currentUserTasks)
            {
                TextBlock label = СreateTaskLabel(userTask.Description, userTask.prioritet);
                string soundFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasksound.wav");
              
                SoundPlayer soundPlayer = new SoundPlayer(soundFilePath);


                // Создание Border с заданными свойствами
                Border border = new Border
                {
                    Margin = new Thickness(0, 10, 0, 10),
                    CornerRadius = new CornerRadius(6),
                   
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
                    
                    soundPlayer.Play();
                    var filter = Builders<UserTasks>.Filter.Eq(u => u.userId, currentUser.Id);

                    // Удалить одну задачу, соответствующую фильтру 
                    _USER_TASKS.DeleteOne(filter);

                };
                //удаление задачи у текущего пользователя
                label.MouseLeftButtonDown += (sender, e) =>
                {
                    // MessageBox.Show("Задание выполнено");
                    label = null;
                    // Удаление Border из StackPanel
                    stackPanel.Children.Remove(border);
                    
                    var filter = Builders<UserTasks>.Filter.Eq(u => u.userId, currentUser.Id);

                    // Удалить одну задачу, соответствующую фильтру 
                    _USER_TASKS.DeleteOne(filter);

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
           
        }

        private void Para_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Page2(currentUser, _DATABASE));
        }
    }
}