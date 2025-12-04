    public Point(double x, double y)  
    {  
        X = x;  
        Y = y;  
    }  

    public override string ToString() => $"({X}, {Y})";  
}  

public interface IGeometricFigure  
{  
    void SetVertices(params Point[] newVertices);  
    void DisplayVertices();  
    double GetArea();  
    int VertexCount { get; }  
}  

public abstract class FigureBase : IGeometricFigure  
{  
    protected Point[] Vertices { get; private set; }  
    public int VertexCount { get; private set; }  
    protected string FigureName { get; }  

    protected FigureBase(int count, string name, params Point[] initialVertices)  
    {  
        if (initialVertices.Length < count)  
            throw new ArgumentException($"Потрібно передати {count} вершин.");  

        VertexCount = count;  
        FigureName = name;  
        Vertices = new Point[count];  
        InitializeVertices(initialVertices);  
        Console.WriteLine($"-> Конструктор '{FigureName}' викликано.");  
    }  

    protected void InitializeVertices(Point[] points)  
    {  
        for (int i = 0; i < VertexCount; i++)  
            Vertices[i] = points[i];  
    }  

    public abstract void SetVertices(params Point[] newVertices);  
    public abstract double GetArea();  

    public virtual void DisplayVertices()  
    {  
        if (Vertices == null || Vertices.Length != VertexCount)  
        {  
            Console.WriteLine("Вершини не задано.");  
            return;  
        }  

        Console.WriteLine($"--- Фігура: {FigureName} ({VertexCount} вершин) ---");  
        for (int i = 0; i < VertexCount; i++)  
            Console.WriteLine($"Вершина {i + 1}: {Vertices[i]}");  
    }  
}  

public class Triangle : FigureBase  
{  
    public Triangle(Point p1, Point p2, Point p3) : base(3, "Трикутник", p1, p2, p3) { }  

    public override void SetVertices(params Point[] newVertices)  
    {  
        if (newVertices.Length < VertexCount)  
            throw new ArgumentException($"Трикутник потребує {VertexCount} точок.");  

        InitializeVertices(newVertices);  
    }  

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

public class ConvexQuadrilateral : FigureBase  
{  
    public ConvexQuadrilateral(Point p1, Point p2, Point p3, Point p4) : base(4, "Опуклий чотирикутник", p1, p2, p3, p4) { }  

    public override void SetVertices(params Point[] newVertices)  
    {  
        if (newVertices.Length < VertexCount)  
            throw new ArgumentException($"Чотирикутник потребує {VertexCount} точок.");  

        if (!IsConvex(newVertices))  
            throw new ArgumentException("Чотирикутник не є опуклим.");  

        InitializeVertices(newVertices);  
    }  

    public override double GetArea()  
    {  
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

    private bool IsConvex(Point[] points)  
    {  
        bool? sign = null;  
        int n = points.Length;  
        for (int i = 0; i < n; i++)  
        {  
            double dx1 = points[(i + 1) % n].X - points[i].X;  
            double dy1 = points[(i + 1) % n].Y - points[i].Y;  
            double dx2 = points[(i + 2) % n].X - points[(i + 1) % n].X;  
            double dy2 = points[(i + 2) % n].Y - points[(i + 1) % n].Y;  
            double cross = dx1 * dy2 - dy1 * dx2;  
            if (cross != 0)  
            {  
                if (!sign.HasValue) sign = cross > 0;  
                else if (sign.Value != (cross > 0)) return false;  
            }  
        }  
        return true;  
    }  
}  

public interface ILogger { void LogInfo(string message); }  

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
    private bool _disposed;  

    public FileLogger()  
    {  
        _writer = new StreamWriter(_filePath, true);  
        LogInfo($"--- Сесія логування розпочата ({DateTime.Now}) ---");  
    }  

    public void LogInfo(string message)  
    {  
        if (_disposed) throw new ObjectDisposedException(nameof(FileLogger));  
        string logEntry = $"[LOG: File] {DateTime.Now:HH:mm:ss} | {message}";  
        _writer.WriteLine(logEntry);  
        _writer.Flush();  
    }  

    protected virtual void Dispose(bool disposing)  
    {  
        if (!_disposed)  
        {  
            if (disposing)  
            {  
                LogInfo($"--- Сесія логування завершена ({DateTime.Now}) ---");  
                _writer?.Close();  
                _writer = null;  
            }  
            _disposed = true;  
        }  
    }  

    public void Dispose()  
    {  
        Dispose(true);  
        GC.SuppressFinalize(this);  
    }  
}  

class Program  
{  
    static void Main()  
    {  
        Console.OutputEncoding = System.Text.Encoding.UTF8;  

        IGeometricFigure triangle = new Triangle(new Point(0, 0), new Point(3, 0), new Point(0, 4));  
        IGeometricFigure quad = new ConvexQuadrilateral(new Point(1, 1), new Point(5, 1), new Point(6, 4), new Point(2, 4));  

        IGeometricFigure[] figures = { triangle, quad };  

        foreach (var figure in figures)  
        {  
            figure.DisplayVertices();  
            Console.WriteLine($"✅ Обчислена площа: {figure.GetArea():F2}\n");  
        }  

        ILogger consoleLogger = new ConsoleLogger();  
        consoleLogger.LogInfo("Програма розпочала роботу.");  

        using (var fileLogger = new FileLogger())  
        {  
            fileLogger.LogInfo($"Трикутник має {triangle.VertexCount} вершин.");  
            fileLogger.LogInfo($"Чотирикутник має {quad.VertexCount} вершин.");  
        }  
    }  
}  
