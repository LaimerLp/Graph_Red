using System.Collections.Generic;
using System.Drawing;
using System;
using WindowsFormsApplication1;
using System.Linq;

[Serializable()]
public class GCurve : AbstractFigure
{
    public List<Point> curve = new List<Point>(); //список curve класса Point (динамический массив)

    private Point _firstPoint; // новое приватное поле для хранения первой точки

    public override Point firstPoint
    {
        get // код аксессора для чтения из поля
        {
            return _firstPoint;
        }
        set // код аксессора для записи в поле
        {
            _firstPoint = value; //неявный параметр value содержит значение, присваиваемое свойству
            curve.Add(value);    //base используется для доступа к членам базового класса из производного класса
        }
    }
    public override Point secondPoint
    {
        get // код аксессора для чтения из поля
        {
            return base.secondPoint;
        }
        set // код аксессора для записи в поле
        {
            base.secondPoint = value;
            curve.Add(value);
        }
    }
    public override void drawFrame(ref Graphics g) //рисование "предварительной" кривой
    {
        Pen p = new Pen(frameColor); //перо определённого цвета (см. форму2) с толщиной по умолчанию
        p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; //пунктир
        g.DrawCurve(p, curve.ToArray()); // удалены обобщенные аргументы
        p.Dispose(); //сброс
    }
    public override void draw(ref Graphics g) //рисование окончательной кривой (при отпускании)
    {
        g.DrawCurve(new Pen(primaryColor, lWidth), curve.ToArray());
    }
    public override Rectangle getRectangle()//**НОВОЕ
    {
        int l = 1000000000, t = 1000000000, r = -1, b = -1;
        foreach (Point p in curve)
        {
            l = Math.Min(l, p.X);
            t = Math.Min(t, p.Y);
            r = Math.Max(r, p.X);
            b = Math.Max(b, p.Y);
        }
        return Rectangle.FromLTRB(l, t, r, b);
    }
    public override void move(Point from, Point to)//**НОВОЕ
    {
        int dx = to.X - from.X;
        int dy = to.Y - from.Y;
        for (int i = 0; i < curve.Count; i++)
        {
            Point p = curve[i];
            p.X += dx;
            p.Y += dy;
            curve[i] = p;
        }
    }

    public void AlignToGrid(int gridStep)
    {
        if (curve.Count < 2)
        {
            return;
        }

        // Вычисление координат ближайших узлов сетки для первой и последней точек кривой
        int firstPointX = (curve[0].X / gridStep) * gridStep;
        int firstPointY = (curve[0].Y / gridStep) * gridStep;
        int lastPointX = (curve[curve.Count - 1].X / gridStep) * gridStep;
        int lastPointY = (curve[curve.Count - 1].Y / gridStep) * gridStep;

        // Перемещение первой и последней точек кривой в соответствующие узлы сетки
        curve[0] = new Point(firstPointX, firstPointY);
        curve[curve.Count - 1] = new Point(lastPointX, lastPointY);

        // Вычисление коэффициентов для каждого сегмента кривой, соединяющего две соседние точки
        List<float> coefficients = new List<float>();
        for (int i = 1; i < curve.Count - 1; i++)
        {
            Point p1 = curve[i - 1];
            Point p2 = curve[i];
            Point p3 = curve[i + 1];
            float dx1 = p2.X - p1.X;
            float dy1 = p2.Y - p1.Y;
            float dx2 = p3.X - p2.X;
            float dy2 = p3.Y - p2.Y;
            float denominator = dx1 * dy2 - dx2 * dy1;
            if (denominator == 0)
            {
                coefficients.Add(0);
            }
            else
            {
                float t = (dx1 * (p1.Y - p3.Y) + dy1 * (p3.X - p1.X)) / denominator;
                coefficients.Add(t);
            }
        }

        // Пересчет координат всех точек кривой, используя вычисленные коэффициенты и координаты первой и последней точек
        for (int i = 1; i < curve.Count - 1; i++)
        {
            float t = coefficients[i - 1];
            Point p1 = curve[i - 1];
            Point p2 = curve[i];
            Point p3 = curve[i + 1];
            float dx1 = p2.X - p1.X;
            float dy1 = p2.Y - p1.Y;
            float dx2 = p3.X - p2.X;
            float dy2 = p3.Y - p2.Y;
            float numerator = dx1 * (p1.Y - p3.Y) + dy1 * (p3.X - p1.X);
            float denominator = dx1 * dy2 - dx2 * dy1;
            float alpha = numerator / denominator;
            float beta = t - alpha;
            curve[i] = new Point(
                (int)Math.Round(p1.X + alpha * dx1 + beta * dx2),
                (int)Math.Round(p1.Y + alpha * dy1 + beta * dy2));
        }
    }
}
