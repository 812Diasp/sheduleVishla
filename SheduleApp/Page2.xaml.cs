using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Threading;
using static SheduleApp.Page2;
using MongoDB.Bson;
using MongoDB.Driver;
using static SheduleApp.MainWindow;

namespace SheduleApp
{
    /// <summary>
    /// Логика взаимодействия для Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
       
        public IMongoCollection<Lesson> _collectionLesson;
        public User currentUserPage { get; set; } = new User();
        List<Lesson> filteredLessons = new List<Lesson>();
        public class Lesson()
        {
            public ObjectId Id { get; set; }
            public ObjectId UserId { get; set; }
            public string Subject { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public DateTime Date{ get; set; }
            public bool Break { get; set; }

        }
        public Page2(User currentUser, IMongoDatabase _DATABASE)
        {
            InitializeComponent();
            //назначаем теущего юзера
            currentUserPage = currentUser;
           
            Lesson l = new Lesson()
            {
                UserId = currentUserPage.Id,
                Subject = "физра",
                StartTime = new TimeSpan(13, 30, 0),
                EndTime = new TimeSpan(14, 50, 0),
                Break = false,
                Date = DateTime.Now,
            };
           
            _collectionLesson = _DATABASE.GetCollection<Lesson>("UserLessons");
            _collectionLesson.InsertOne(l);
            LoadLessons();


        }
        private List<Lesson> lessons = new List<Lesson>
        {};
        private Grid CreateLessonBlock(Lesson lesson)
        {
            Grid grid = new Grid();
            grid.Width = 400; // Ширина элемента
            grid.Height = 80; // Высота элемента

            // Определяем столбцы и строки:
            grid.ColumnDefinitions.Add(new ColumnDefinition()); // Один столбец
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            // Задаем цвет фона:
            grid.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(127, 255, 170)); // Светло-зеленый цвет


            // Получаем число и месяц занятия
            int month = lesson.Date.Month;
            int day = lesson.Date.Day;
            // Создаем TextBlock для названия:
            TextBlock subjectTextBlock = new TextBlock()
            {
                Text = lesson.Subject+$" ({day}.{month})",
                FontSize = 30,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                
                
            };
            Grid.SetColumn(subjectTextBlock, 0);
            Grid.SetRow(subjectTextBlock, 0);
            grid.Children.Add(subjectTextBlock);
           
            // Создаем TextBlock для времени:
            TextBlock timeTextBlock = new TextBlock()
            {
                Text = lesson.StartTime.ToString(@"hh\:mm") + " - " + lesson.EndTime.ToString(@"hh\:mm"),
                FontSize = 25,
                FontStyle = FontStyles.Italic,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TextAlignment = TextAlignment.Center,
                 VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetColumn(timeTextBlock, 0);
            Grid.SetRow(timeTextBlock, 1);
            grid.Margin = new Thickness(5);
            grid.Children.Add(timeTextBlock);
          

            //Возвращаем созданный Grid:
            return grid;
        }

        public void GetLessonsFromDB()
        {
            var filter = Builders<Lesson>.Filter.Eq(u => u.UserId, currentUserPage.Id);
            // Получение документов, соответствующих фильтру
            var userLessons = _collectionLesson.Find(filter).ToList();
            foreach (var userLesson in userLessons)
            {
                filteredLessons.Add(userLesson);
            }
        }
       
        public void LoadLessons()
        {

            GetLessonsFromDB();

            foreach (Lesson l in filteredLessons)
            {
                string formatStart = l.StartTime.ToString(@"hh\:mm");
                string formatEnd = l.EndTime.ToString(@"hh\:mm");
                Grid newLesson = CreateLessonBlock(l);
               
                Lessons.Children.Add(newLesson);

              
            }

        }
    }
}
