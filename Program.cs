using System;
using System.IO; 

namespace LabWork
{
    // ====================================================================
    // 1. Point (readonly struct)
    // ====================================================================
    /// <summary>
    /// Представляє незмінну координату вершини фігури.
    /// </summary>
    public readonly struct Point
    {
        // Властивості тільки для читання, що робить структуру незмінною
        public double X { get; } 
        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            // Форматування для фіксованої точності
            return $"({X:F2}, {Y:F2})"; 
        }
    }

    // ====================================================================
    // 2. Інтерфейс для Геометричних Фігур
    // ====================================================================
    /// <summary>
    /// Визначає контракт для всіх геометричних фігур.
    /// </summary>
    public interface IGeometricFigure
    {
        void SetVertices(params Point[] newVertices);
        void DisplayVertices();
        double GetArea();
    }

    // ====================================================================
    // 3. Абстрактний Базовий Клас (FigureBase)
    // ====================================================================
    /// <summary>
    /// Абстрактний клас, що надає спільну основу для всіх фігур.
    /// </summary>
    public abstract class FigureBase : IGeometricFigure
    {
        // Поля з посиленою інкапсуляцією
        private Point[] _vertices; 
        private readonly int _vertexCount;
        private readonly string _figureName;

        // Захищені властивості для доступу з похідних класів
        protected Point[] Vertices => _vertices;
        protected int VertexCount => _vertexCount;
        protected string FigureName => _figureName;

        /// <summary>
        /// Ініціалізує новий екземпляр класу FigureBase.
        /// </summary>
        /// <param name="count">Необхідна кількість вершин.</param>
        /// <param name="name">Назва фігури.</param>
        /// <param name="initialVertices">Початкові координати вершин.</param>
        public FigureBase(int count, string name, params Point[] initialVertices)
        {
            _vertexCount = count;
            _figureName = name;
            _vertices = new Point[_vertexCount];
            SetVertices(initialVertices);
            Console.WriteLine($"-> Конструктор '{FigureName}' викликано.");
        }

        // Абстрактні методи
        public abstract void SetVertices(params Point[] newVertices);
        public abstract double GetArea();

        /// <summary>
        /// Виводить координати всіх вершин фігури на екран.
        /// </summary>
        public virtual void DisplayVertices()
        {
            Console.WriteLine($"--- Фігура: {FigureName} ({VertexCount} вершин) ---");
            if (Vertices == null || Vertices.Length == 0)
            {
                Console.WriteLine("Координати вершин не задані.");
                return;
            }

            for (int i = 0; i < VertexCount; i++)
            {
                Console.WriteLine($"Вершина {i + 1}: {Vertices[i]}");
            }
        }
        
        // Фіналізатор залишено для демонстрації, як вимагалося в завданні
        ~FigureBase()
        {
            // У реальному коді слід уникати, якщо немає unmanaged-ресурсів
            Console.WriteLine($"<- Деструктор {FigureName} викликано.");
        }
    }

    // ====================================================================
    // 4. Клас Triangle (Трикутник)
    // ====================================================================
    public class Triangle : FigureBase
    {
        /// <summary>
        /// Ініціалізує новий екземпляр класу Triangle.
        /// </summary>
        public Triangle(Point p1, Point p2, Point p3)
            : base(3, "Трикутник", p1, p2, p3) { }

        /// <summary>
        /// Встановлює координати трьох вершин трикутника.
        /// </summary>
        /// <param name="newVertices">Масив Point, що містить 3 вершини.</param>
        /// <exception cref="ArgumentException">Викидається, якщо передано недостатню кількість вершин.</exception>
        public override void SetVertices(params Point[] newVertices)
        {
            if (newVertices == null || newVertices.Length < VertexCount)
            {
                throw new ArgumentException($"Трикутник вимагає рівно {VertexCount} вершин. Передано {newVertices?.Length ?? 0}.", nameof(newVertices));
            }

            for (int i = 0; i < VertexCount; i++)
            {
                Vertices[i] = newVertices[i];
            }
        }

        /// <summary>
        /// Обчислює площу трикутника за координатами вершин (Формула Гаусса).
        /// </summary>
        /// <returns>Площа трикутника.</returns>
        public override double GetArea()
        {
            // Формула площі трикутника за координатами
            double area = 0.5 * Math.Abs(
                Vertices[0].X * (Vertices[1].Y - Vertices[2].Y) +
                Vertices[1].X * (Vertices[2].Y - Vertices[0].Y) +
                Vertices[2].X * (Vertices[0].Y - Vertices[1].Y)
            );
            return area;
        }
    }

    // ====================================================================
    // 5. Клас ConvexQuadrilateral (Опуклий чотирикутник)
    // ====================================================================
    public class ConvexQuadrilateral : FigureBase
    {
        /// <summary>
        /// Ініціалізує новий екземпляр класу ConvexQuadrilateral.
        /// </summary>
        public ConvexQuadrilateral(Point p1, Point p2, Point p3, Point p4) 
            : base(4, "Опуклий чотирикутник", p1, p2, p3, p4) { }

        /// <summary>
        /// Встановлює координати чотирьох вершин чотирикутника.
        /// </summary>
        /// <param name="newVertices">Масив Point, що містить 4 вершини.</param>
        /// <exception cref="ArgumentException">Викидається, якщо передано недостатню кількість вершин.</exception>
        public override void SetVertices(params Point[] newVertices)
        {
            if (newVertices == null || newVertices.Length < VertexCount)
            {
                throw new ArgumentException($"Чотирикутник вимагає рівно {VertexCount} вершин. Передано {newVertices?.Length ?? 0}.", nameof(newVertices));
            }

            for (int i = 0; i < VertexCount; i++)
            {
                Vertices[i] = newVertices[i];
            }
        }

        /// <summary>
        /// Обчислює площу чотирикутника (сума площ двох трикутників).
        /// Примітка: метод припускає, що фігура є опуклою, а вершини задані послідовно.
        /// </summary>
        /// <returns>Площа чотирикутника.</returns>
        public override double GetArea()
        {
            // Площа 4-кутника = Площа(T1-2-3) + Площа(T1-3-4)

            // Площа T1-2-3
            double area123 = 0.5 * Math.Abs(
                Vertices[0].X * (Vertices[1].Y - Vertices[2].Y) +
                Vertices[1].X * (Vertices[2].Y - Vertices[0].Y) +
                Vertices[2].X * (Vertices[0].Y - Vertices[1].Y)
            );

            // Площа T1-3-4
            double area134 = 0.5 * Math.Abs(
                Vertices[0].X * (Vertices[2].Y - Vertices[3].Y) +
                Vertices[2].X * (Vertices[3].Y - Vertices[0].Y) +
                Vertices[3].X * (Vertices[0].Y - Vertices[2].Y)
            );

            return area123 + area134;
        }
    }
    
    // ====================================================================
    // 6. Реалізація ILogger та IDisposable
    // ====================================================================

    /// <summary>
    /// Інтерфейс для логування повідомлень.
    /// </summary>
    public interface ILogger
    {
        void LogInfo(string message);
    }

    /// <summary>
    /// Логер, що виводить повідомлення в консоль.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[LOG: Console] {message}");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Логер, що записує повідомлення у файл (реалізує IDisposable).
    /// </summary>
    public class FileLogger : ILogger, IDisposable
    {
        private readonly string _filePath = "log.txt";
        private StreamWriter _writer;
        
        /// <summary>
        /// Ініціалізує FileLogger та відкриває файл для запису.
        /// </summary>
        public FileLogger()
        {
            try
            {
                _writer = new StreamWriter(_filePath, true); 
                LogInfo($"--- Сесія логування розпочата ({DateTime.Now}) ---");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка ініціалізації FileLogger: {ex.Message}");
            }
        }

        /// <summary>
        /// Записує інформаційне повідомлення у файл.
        /// </summary>
        public void LogInfo(string message)
        {
            if (_writer != null)
            {
                string logEntry = $"[LOG: File] {DateTime.Now:HH:mm:ss} | {message}";
                _writer.WriteLine(logEntry);
                _writer.Flush(); 
            }
        }

        /// <summary>
        /// Звільняє некеровані ресурси (StreamWriter).
        /// </summary>
        public void Dispose()
        {
            if (_writer != null)
            {
                LogInfo($"--- Сесія логування завершена ({DateTime.Now}) ---");
                _writer.Dispose(); // Використовуємо Dispose() замість Close()
                _writer = null;
                Console.WriteLine("[LOG: File] Файл логу закрито.");
            }
            GC.SuppressFinalize(this); // Запобігає виклику фіналізатора (якщо він був би)
        }
    }

    // ====================================================================
    // 7. Головна програма (Program)
    // ====================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("## 📐 Лабораторна робота: Абстракція та Інтерфейси (v2.0)\n");

            // --- 1. Демонстрація геометричних фігур та поліморфізму ---
            Console.WriteLine("--- Демонстрація Геометричних Фігур ---\n");
            
            // Створення об'єктів
            IGeometricFigure triangle = new Triangle(new Point(0, 0), new Point(3, 0), new Point(0, 4)); 
            IGeometricFigure quad = new ConvexQuadrilateral(new Point(1, 1), new Point(5, 1), new Point(6, 4), new Point(2, 4)); 

            // Масив посилань на інтерфейс
            IGeometricFigure[] figures = new IGeometricFigure[] { triangle, quad };

            foreach (var figure in figures)
            {
                figure.DisplayVertices();
                double area = figure.GetArea();
                Console.WriteLine($"✅ Обчислена площа: {area:F2}\n");
            }
            
            // --- 2. Демонстрація валідації SetVertices ---
            Console.WriteLine("--- Демонстрація Валідації ---");
            try
            {
                triangle.SetVertices(new Point(1, 1), new Point(2, 2)); // Спроба передати лише 2 вершини
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"❌ Помилка валідації: {ex.Message}");
            }
            Console.WriteLine(new string('-', 45));


            // --- 3. Демонстрація ILogger та IDisposable ---
            Console.WriteLine("--- Демонстрація ILogger та IDisposable ---");

            ILogger consoleLogger = new ConsoleLogger();
            consoleLogger.LogInfo("Програма розпочала логування.");

            // Використання using блоку гарантує виклик Dispose()
            using (var fileLogger = new FileLogger())
            {
                fileLogger.LogInfo("Логування фігур у файл...");
                fileLogger.LogInfo($"Площа чотирикутника: {quad.GetArea():F2}.");
            } // Тут автоматично викликається Dispose()

            Console.WriteLine("\n✅ Виконано. Перевірте файл 'log.txt' для логів.");
        }
    }
}
