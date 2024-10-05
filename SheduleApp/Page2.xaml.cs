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

namespace SheduleApp
{
    /// <summary>
    /// Логика взаимодействия для Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        public class Lesson()
        {
            public string Subject { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public bool Break { get; set; }

        }
        private List<Lesson> lessons = new List<Lesson>
        {
            new Lesson { Subject = "Математика", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0), Break = false },
            new Lesson { Subject = "Физика", StartTime = new TimeSpan(10, 20, 0), EndTime = new TimeSpan(11, 50, 0), Break = false },
            new Lesson { Subject = "История", StartTime = new TimeSpan(12, 0, 0), EndTime = new TimeSpan(12, 30, 0), Break = false },
            new Lesson { Subject = "Проектная деятельность", StartTime = new TimeSpan(12, 50, 0), EndTime = new TimeSpan(14, 20, 0) },
            // Добавьте больше пар...
        };
        private Grid CreateLessonBlock(Lesson lesson)
        {
            Grid grid = new Grid();
            grid.Width = 150; // Ширина элемента
            grid.Height = 150; // Высота элемента

            // Определяем столбцы и строки:
            grid.ColumnDefinitions.Add(new ColumnDefinition()); // Один столбец
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            // Задаем цвет фона:
            grid.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(127, 255, 170)); // Светло-зеленый цвет

            // Создаем TextBlock для названия:
            TextBlock subjectTextBlock = new TextBlock()
            {
                Text = lesson.Subject,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center
            };
            Grid.SetColumn(subjectTextBlock, 0);
            Grid.SetRow(subjectTextBlock, 0);
            grid.Children.Add(subjectTextBlock);

            // Создаем TextBlock для времени:
            TextBlock timeTextBlock = new TextBlock()
            {
                Text = lesson.StartTime.ToString(@"hh\:mm") + " - " + lesson.EndTime.ToString(@"hh\:mm"),
                FontSize = 12,
                TextAlignment = TextAlignment.Center
            };
            Grid.SetColumn(timeTextBlock, 0);
            Grid.SetRow(timeTextBlock, 1);
            grid.Children.Add(timeTextBlock);

            // Возвращаем созданный Grid:
            return grid;
        }
        public Page2()
        {
            InitializeComponent();
            LoadLessons();


        }
        public void LoadLessons()
        {

            int column = 0; // Начальный номер столбца

            foreach (Lesson l in lessons)
            {
                string formatStart = l.StartTime.ToString(@"hh\:mm");
                string formatEnd = l.EndTime.ToString(@"hh\:mm");
                Grid newLesson = CreateLessonBlock(l);
                // Задаем столбец и строку
                Grid.SetColumn(newLesson, column);
                Grid.SetRow(newLesson, 1);
                Lessons.Children.Add(newLesson);

                // Увеличиваем номер столбца для следующей итерации
                column++;
            }

        }
    }
}
