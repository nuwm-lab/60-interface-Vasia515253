using System;
using System.IO; 

namespace LabWork
{
    // ====================================================================
    // 1. Point (public readonly struct)
    // ====================================================================
    /// <summary>
    /// Представляє незмінну координату вершини фігури.
    /// </summary>
    public readonly struct Point
    {
        public double X { get; } 
        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X:F2}, {Y:F2})"; 
        }
    }

    // ====================================================================
    // 2. Інтерфейс для Геометричних Фігур (додано VertexCount)
    // ====================================================================
    /// <summary>
    /// Визначає контракт для всіх геометричних фігур.
    /// </summary>
    public interface IGeometricFigure
    {
        void SetVertices(params Point[] newVertices);
        void DisplayVertices();
        double GetArea();

        /// <summary>
        /// Повертає необхідну кількість вершин для фігури.
        /// </summary>
        int VertexCount { get; }
    }

    // ====================================================================
    // 3. Абстрактний Базовий Клас (FigureBase)
    // =================================0====================
    /// <summary>
    /// Абстрактний клас, що надає спільну основу для всіх фігур, реалізуючи IGeometricFigure.
    /// </summary>
    public abstract class FigureBase : IGeometricFigure
    {
        private Point[] _vertices; 
        private readonly int _vertexCount;
        private readonly string _figureName;

        // Реалізація властивості інтерфейсу
        public int VertexCount => _vertexCount;

        // Захищені властивості для доступу з похідних класів
        protected Point[] Vertices => _vertices;
        protected string FigureName => _figureName;

        /// <summary>
        /// Ініціалізує новий екземпляр класу FigureBase.
        /// </summary>
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

            // Перевірка ініціалізації: якщо перша точка має координати (0,0),
            // і це не було задано явно, може бути default-позиція.
            // Краще перевіряти, чи Vertices ініціалізовано.
            if (Vertices == null || Vertices.Length != VertexCount)
            {
                Console.WriteLine("Координати вершин не задані або задані некоректно.");
                return;
            }

            for (int i = 0; i < VertexCount; i++)
            {
                Console.WriteLine($"Вершина {i + 1}: {Vertices[i]}");
            }
        }
        // Фіналізатор (~FigureBase()) ВИДАЛЕНО згідно з рекомендацією
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
        /// <exception cref="ArgumentException">Викидається, якщо передано не 3 вершини.</exception>
        public override void SetVertices(params Point[] newVertices)
        {
            // Посилена валідація: точна відповідність кількості
            if (newVertices == null || newVertices.Length != VertexCount)
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
        public override double GetArea()
        {
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
        /// <exception cref="ArgumentException">Викидається, якщо передано не 4 вершини.</exception>
        public override void SetVertices(params Point[] newVertices)
        {
            // Посилена валідація: точна відповідність кількості
            if (newVertices == null || newVertices.Length != VertexCount)
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
        /// </summary>
        public override double GetArea()
        {
            // Площа 4-кутника = Площа(T1-2-3) + Площа(T1-3-4)
            double area123 = 0.5 * Math.Abs(
                Vertices[0].X * (Vertices[1].Y - Vertices[2].Y) +
                Vertices[1].X * (Vertices[2].Y - Vertices[0].Y) +
                Vertices[2].X * (Vertices[0].Y - Vertices[1].Y)
            );

            double area134 = 0.5 * Math.Abs(
                Vertices[0].X * (Vertices[2].Y - Vertices[3].Y) +
                Vertices[2].X * (Vertices[3].Y - Vertices[0].Y) +
                Vertices[3].X * (Vertices[0].Y - Vertices[2].Y)
            );

            return area123 + area134;
        }
    }
    
    // ====================================================================
    // 6. Реалізація ILogger та IDisposable (FileLogger покращено)
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
        private bool _disposed = false; // Захист від повторного виклику Dispose

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
            if (!_disposed && _writer != null)
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
            Dispose(true);
            GC.SuppressFinalize(this); // Фіналізатор відсутній, але це гарна практика
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Звільнення керованих ресурсів
                if (_writer != null)
                {
                    LogInfo($"--- Сесія логування завершена ({DateTime.Now}) ---");
                    _writer.Dispose(); 
                    _writer = null;
                    Console.WriteLine("[LOG: File] Файл логу закрито.");
                }
            }
            // Звільнення некерованих ресурсів (тут відсутні)

            _disposed = true;
        }
        
        // Фіналізатор (~FileLogger()) ВИДАЛЕНО згідно з рекомендацією
    }

    // ====================================================================
    // 7. Головна програма (Program)
    // ====================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("## 📐 Лабораторна робота: Абстракція та Інтерфейси (Фінал)\n");

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
                Console.WriteLine($"Кількість вершин (через інтерфейс): {figure.VertexCount}");
                double area = figure.GetArea();
                Console.WriteLine($"✅ Обчислена площа: {area:F2}\n");
            }
            
            // --- 2. Демонстрація валідації SetVertices ---
            Console.WriteLine("--- Демонстрація Валідації SetVertices ---\n");
            try
            {
                // Спроба встановити невірну кількість вершин (2 замість 3)
                triangle.SetVertices(new Point(1, 1), new Point(2, 2)); 
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"❌ Успішна помилка валідації: {ex.Message}");
            }
            
            // Спроба встановити коректну кількість
            try
            {
                triangle.SetVertices(new Point(5, 5), new Point(6, 6), new Point(7, 7));
                Console.WriteLine("✅ Коректне оновлення координат виконано.");
                triangle.DisplayVertices();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"❌ Помилка при коректному оновленні: {ex.Message}");
            }
            Console.WriteLine(new string('-', 45));


            // --- 3. Демонстрація ILogger та IDisposable ---
            Console.WriteLine("--- Демонстрація ILogger та IDisposable ---\n");

            ILogger consoleLogger = new ConsoleLogger();
            consoleLogger.LogInfo("Програма розпочала логування.");

            // Використання using блоку для FileLogger
            using (var fileLogger = new FileLogger())
            {
                fileLogger.LogInfo("Логування фігур у файл...");
                fileLogger.LogInfo($"Площа чотирикутника: {quad.GetArea():F2}.");
            } // Тут автоматично викликається Dispose()

            Console.WriteLine("\n✅ Виконано. Перевірте файл 'log.txt' для логів.");
        }
    }
}
