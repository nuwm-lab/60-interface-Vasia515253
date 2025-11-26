using System;
using System.IO; // Потрібно для FileLogger

namespace LabWork
{
    // ====================================================================
    // 1. Структура Point (Відповідає C# Code Conventions)
    // ====================================================================
    /// <summary>
    /// Представляє координату вершини фігури.
    /// </summary>
    public struct Point
    {
        public double X { get; } // Read-only властивості
        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    // ====================================================================
    // 2. Інтерфейс для Геометричних Фігур (IShape)
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
        protected Point[] Vertices { get; private set; } // Інкапсульовані, приватний setter
        protected int VertexCount { get; }
        protected string FigureName { get; }

        // Конструктор
        public FigureBase(int count, string name, params Point[] initialVertices)
        {
            VertexCount = count;
            FigureName = name;
            Vertices = new Point[VertexCount];
            SetVertices(initialVertices);
            Console.WriteLine($"-> Конструктор '{FigureName}' викликано.");
        }

        // Абстрактні методи (повинні бути реалізовані в похідних класах)
        public abstract void SetVertices(params Point[] newVertices);
        public abstract double GetArea();

        // Віртуальний метод (може бути перевизначений)
        public virtual void DisplayVertices()
        {
            Console.WriteLine($"--- Фігура: {FigureName} ({VertexCount} вершин) ---");
            for (int i = 0; i < VertexCount; i++)
            {
                Console.WriteLine($"Вершина {i + 1}: {Vertices[i]}");
            }
        }
    }

    // ====================================================================
    // 4. Клас Triangle (Трикутник)
    // ====================================================================
    public class Triangle : FigureBase
    {
        public Triangle(Point p1, Point p2, Point p3)
            : base(3, "Трикутник", p1, p2, p3) { }

        // Реалізація абстрактного SetVertices
        public override void SetVertices(params Point[] newVertices)
        {
            if (newVertices.Length >= VertexCount)
            {
                for (int i = 0; i < VertexCount; i++)
                {
                    Vertices[i] = newVertices[i];
                }
            }
        }

        // Реалізація абстрактного GetArea
        public override double GetArea()
        {
            // Формула Гаусса для трикутника
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
        public ConvexQuadrilateral(Point p1, Point p2, Point p3, Point p4) 
            : base(4, "Опуклий чотирикутник", p1, p2, p3, p4) { }

        // Реалізація абстрактного SetVertices
        public override void SetVertices(params Point[] newVertices)
        {
            if (newVertices.Length >= VertexCount)
            {
                for (int i = 0; i < VertexCount; i++)
                {
                    Vertices[i] = newVertices[i];
                }
            }
        }

        // Реалізація абстрактного GetArea (сума площ двох трикутників)
        public override double GetArea()
        {
            // Використовуємо іншу формулу, специфічну для 4-кутника
            // Площа 4-кутника = Площа(T1-2-3) + Площа(T1-3-4)

            // Площа T1-2-3 (Vertices[0], Vertices[1], Vertices[2])
            double area123 = 0.5 * Math.Abs(
                Vertices[0].X * (Vertices[1].Y - Vertices[2].Y) +
                Vertices[1].X * (Vertices[2].Y - Vertices[0].Y) +
                Vertices[2].X * (Vertices[0].Y - Vertices[1].Y)
            );

            // Площа T1-3-4 (Vertices[0], Vertices[2], Vertices[3])
            double area134 = 0.5 * Math.Abs(
                Vertices[0].X * (Vertices[2].Y - Vertices[3].Y) +
                Vertices[2].X * (Vertices[3].Y - Vertices[0].Y) +
                Vertices[3].X * (Vertices[0].Y - Vertices[2].Y)
            );

            return area123 + area134;
        }
    }
    
    // ====================================================================
    // 6. Реалізація Інтерфейсу ILogger (Друге завдання)
    // ====================================================================

    public interface ILogger
    {
        void LogInfo(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[LOG: Console] {message}");
            Console.ResetColor();
        }
    }

    public class FileLogger : ILogger, IDisposable
    {
        private readonly string _filePath = "log.txt";
        private StreamWriter _writer;
        
        // Конструктор, який ініціалізує ресурс (файл)
        public FileLogger()
        {
            try
            {
                // Створюємо або відкриваємо файл для дозапису
                _writer = new StreamWriter(_filePath, true); 
                LogInfo($"--- Сесія логування розпочата ({DateTime.Now}) ---");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка ініціалізації FileLogger: {ex.Message}");
            }
        }

        public void LogInfo(string message)
        {
            if (_writer != null)
            {
                string logEntry = $"[LOG: File] {DateTime.Now:HH:mm:ss} | {message}";
                _writer.WriteLine(logEntry);
                _writer.Flush(); // Обов'язково для негайного запису
            }
        }

        // Реалізація IDisposable для управління unmanaged-ресурсами (StreamWriter)
        public void Dispose()
        {
            if (_writer != null)
            {
                LogInfo($"--- Сесія логування завершена ({DateTime.Now}) ---");
                _writer.Close();
                _writer = null;
                Console.WriteLine("[LOG: File] Файл логу закрито.");
            }
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
            Console.WriteLine("## 📐 Лабораторна робота: Абстракція та Інтерфейси\n");

            // --- 1. Демонстрація геометричних фігур через абстрактний клас/інтерфейс ---
            Console.WriteLine("--- Демонстрація Геометричних Фігур ---\n");
            
            // Створення об'єктів
            IGeometricFigure triangle = new Triangle(new Point(0, 0), new Point(3, 0), new Point(0, 4)); // Площа 6
            IGeometricFigure quad = new ConvexQuadrilateral(new Point(1, 1), new Point(5, 1), new Point(6, 4), new Point(2, 4)); // Площа 15 (Трапеція)

            // Масив посилань на інтерфейс/базовий тип (Поліморфізм)
            IGeometricFigure[] figures = new IGeometricFigure[] { triangle, quad };

            foreach (var figure in figures)
            {
                // Виклик DisplayVertices та GetArea. 
                // Runtime викликає відповідний override-метод.
                figure.DisplayVertices();
                double area = figure.GetArea();
                Console.WriteLine($"✅ Обчислена площа: {area:F2}\n");
            }

            // --- 2. Демонстрація Інтерфейсу ILogger (Використання IDisposable) ---
            Console.WriteLine("--- Демонстрація ILogger та IDisposable ---\n");

            // Створення та використання ConsoleLogger
            ILogger consoleLogger = new ConsoleLogger();
            consoleLogger.LogInfo("Програма розпочала роботу.");
            consoleLogger.LogInfo($"Трикутник має площу {triangle.GetArea()}.");

            // Створення та використання FileLogger в блоці using для гарантованого виклику Dispose()
            using (var fileLogger = new FileLogger())
            {
                fileLogger.LogInfo("Початок логування фігур.");
                fileLogger.LogInfo($"Чотирикутник має {((ConvexQuadrilateral)quad).VertexCount} вершин."); // Звернення до властивості похідного класу (явне приведення)
                fileLogger.LogInfo("Завершення логування фігур.");
            } // Тут автоматично викликається Dispose()

            Console.WriteLine("\n✅ Виконано. Перевірте файл 'log.txt' для логів.");
            // Console.ReadKey();
        }
    }
}
