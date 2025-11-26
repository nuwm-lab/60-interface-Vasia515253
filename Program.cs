using System;

namespace LabWork
{
    // ----------------------------------------------------
    // 1. Структура для представлення точки (вершини)
    // ----------------------------------------------------
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    // ----------------------------------------------------
    // 2. Інтерфейс для геометричних фігур
    // ----------------------------------------------------
    // Визначає загальні контракти, які повинні реалізувати всі фігури
    public interface IGeometricFigure
    {
        void SetVertices(params Point[] newVertices);
        void DisplayVertices();
        double CalculateArea();
        int GetVertexCount();
    }

    // ----------------------------------------------------
    // 3. Абстрактний базовий клас: FigureBase
    // ----------------------------------------------------
    // Абстрактний клас реалізує інтерфейс IGeometricFigure та містить 
    // спільну логіку для всіх похідних фігур (Трикутник, Чотирикутник).
    public abstract class FigureBase : IGeometricFigure
    {
        protected Point[] vertices; 
        protected int vertexCount;
        protected string className;

        // Конструктор абстрактного класу
        public FigureBase(int count, string name)
        {
            vertexCount = count;
            vertices = new Point[vertexCount];
            className = name;
            Console.WriteLine($"-> Конструктор {className} викликано.");
        }

        // Реалізація методу інтерфейсу - може бути абстрактною або віртуальною
        // Залишаємо абстрактним, оскільки логіка встановлення точок різна
        public abstract void SetVertices(params Point[] newVertices);

        // Реалізація методу інтерфейсу - може бути абстрактною або віртуальною
        // Залишаємо віртуальною, щоб дати можливість перевизначити, але надати базовий текст
        public virtual void DisplayVertices()
        {
            Console.WriteLine($"--- Фігура: {className} ({vertexCount} вершин) ---");
            for (int i = 0; i < vertexCount; i++)
            {
                Console.WriteLine($"Вершина {i + 1}: ({vertices[i].X}, {vertices[i].Y})");
            }
        }

        // Реалізація методу інтерфейсу - абстрактний, оскільки формула площі різна
        public abstract double CalculateArea();

        // Додатковий метод інтерфейсу
        public int GetVertexCount()
        {
            return vertexCount;
        }

        // Деструктор (Фіналізатор) для демонстрації
        ~FigureBase()
        {
            Console.WriteLine($"<- Деструктор {className} викликано.");
        }
    }

    // ----------------------------------------------------
    // 4. Клас: Triangle (Трикутник) - успадковує FigureBase
    // ----------------------------------------------------
    public class Triangle : FigureBase
    {
        public Triangle(Point p1, Point p2, Point p3)
            : base(3, "Трикутник") // Виклик конструктора базового класу
        {
            SetVertices(p1, p2, p3);
        }

        /// <summary>
        /// Реалізація абстрактного методу для задання 3 координат.
        /// </summary>
        public override void SetVertices(params Point[] newVertices)
        {
            if (newVertices.Length >= vertexCount)
            {
                for (int i = 0; i < vertexCount; i++)
                {
                    vertices[i] = newVertices[i];
                }
            }
        }

        /// <summary>
        /// Реалізація абстрактного методу для обчислення площі.
        /// </summary>
        public override double CalculateArea()
        {
            // Формула площі трикутника за координатами
            double area = 0.5 * Math.Abs(
                vertices[0].X * (vertices[1].Y - vertices[2].Y) +
                vertices[1].X * (vertices[2].Y - vertices[0].Y) +
                vertices[2].X * (vertices[0].Y - vertices[1].Y)
            );
            return area;
        }
    }

    // ----------------------------------------------------
    // 5. Клас: ConvexQuadrilateral (Опуклий чотирикутник) - успадковує FigureBase
    // ----------------------------------------------------
    public class ConvexQuadrilateral : FigureBase
    {
        public ConvexQuadrilateral(Point p1, Point p2, Point p3, Point p4) 
            : base(4, "Опуклий чотирикутник") // Виклик конструктора базового класу
        {
            SetVertices(p1, p2, p3, p4);
        }

        /// <summary>
        /// Реалізація абстрактного методу для задання 4 координат.
        /// </summary>
        public override void SetVertices(params Point[] newVertices)
        {
            if (newVertices.Length >= vertexCount)
            {
                for (int i = 0; i < vertexCount; i++)
                {
                    vertices[i] = newVertices[i];
                }
            }
        }

        /// <summary>
        /// Реалізація абстрактного методу для обчислення площі (сума двох трикутників).
        /// </summary>
        public override double CalculateArea()
        {
            // Площа трикутника 1-2-3
            double area123 = 0.5 * Math.Abs(
                vertices[0].X * (vertices[1].Y - vertices[2].Y) +
                vertices[1].X * (vertices[2].Y - vertices[0].Y) +
                vertices[2].X * (vertices[0].Y - vertices[1].Y)
            );

            // Площа трикутника 1-3-4
            double area134 = 0.5 * Math.Abs(
                vertices[0].X * (vertices[2].Y - vertices[3].Y) +
                vertices[2].X * (vertices[3].Y - vertices[0].Y) +
                vertices[3].X * (vertices[0].Y - vertices[2].Y)
            );

            return area123 + area134;
        }
    }

    // ----------------------------------------------------
    // 6. Головна програма та демонстрація роботи
    // ----------------------------------------------------
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("## ✍️ Демонстрація Абстрактного Класу та Інтерфейсу");
            Console.WriteLine("-------------------------------------------------");

            // 1. Демонстрація роботи класів та конструкторів
            Console.WriteLine("-> Створення об'єктів:");
            Triangle triangle = new Triangle(new Point(1, 1), new Point(4, 5), new Point(1, 5));
            ConvexQuadrilateral quad = new ConvexQuadrilateral(new Point(0, 0), new Point(6, 0), new Point(7, 3), new Point(1, 4));
            Console.WriteLine("-------------------------------------------------");
            
            // 2. Демонстрація роботи Інтерфейсу (Поліморфізм)
            Console.WriteLine("-> Демонстрація роботи через Інтерфейс (IGeometricFigure):");
            
            // Створення масиву посилань на інтерфейс
            IGeometricFigure[] figures = new IGeometricFigure[] { triangle, quad };

            foreach (var figure in figures)
            {
                // Виклик методів через інтерфейс. 
                // Runtime визначає, який саме клас (Triangle чи ConvexQuadrilateral) був створений
                figure.DisplayVertices();
                double area = figure.CalculateArea();
                Console.WriteLine($"✅ Обчислена площа фігури: {area:F2}");
                Console.WriteLine($"Кількість вершин: {figure.GetVertexCount()}");
                Console.WriteLine(new string('-', 45));
            }
            
            Console.WriteLine("-------------------------------------------------");
            // Завершення програми. GC викличе деструктори (фіналізатори) при збиранні сміття.
            Console.WriteLine("Завершення Main(). Деструктори будуть викликані пізніше GC.");

            // Для гарантованого виклику деструкторів (лише для демонстраційних цілей, 
            // в реальному коді не використовується!)
            // GC.Collect(); 
            // GC.WaitForPendingFinalizers(); 
        }
    }
}
