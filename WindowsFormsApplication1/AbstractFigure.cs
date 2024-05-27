using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace WindowsFormsApplication1
{
    [Serializable()]
    public abstract class AbstractFigure //базовый класс для всех фигур
    {


        /*Свойство являтся одной разновидностью члена класса. Оно сочетает в себе поле с методами доступа к нему.
       Преимущество свойства: его имя может быть использовано при присваивания аналогично имени обычной переменной, 
       но в действительности при обращении к свойству по имени автоматически вызываются его аксессоры get и set. 
       Свойства  лишь управляют доступом к полям, т.е. само свойство не предоставляет поле, и поэтому поле должно быть определено независимо от свойства.*/

        //неявный параметр value содержит значение, присваиваемое свойству

        //для изображения кривой
        public virtual Point firstPoint
        {
            set { p1 = value; } // код аксессора для записи в поле
            get { return p1; } // код аксессора для чтения из поля
        }
        public virtual Point secondPoint
        {
            set { p2 = value; }
            get { return p2; }
        }
        //для ширина линии
        public int lineWidth
        {
            set
            {
                if (value <= 0)
                    lWidth = 1;
                else
                    lWidth = value;
            }
            get { return lWidth; }
        }
        //Поля
        protected Point p1, p2; //позиции мыши
        protected int lWidth; //ширина линии
        protected Color primaryColor, secondaryColor, frameColor;
        public bool fill;
        [NonSerialized()]
        public bool selected;//** НОВОЕ
        //Методы
        public void loadColors(Color pc, Color sc, Color fc)
        {
            primaryColor = pc;
            secondaryColor = sc;
            frameColor = fc;
        }
        public void drawSelection(ref Graphics g)//**НОВОЕ
        {
            Pen p = new Pen(frameColor);
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            g.DrawRectangle(p, getRectangle());
            p.Dispose();
        }
        public void drawDragged(ref Graphics g, Point from, Point to)//**НОВОЕ
        {
            move(from, to);
            drawFrame(ref g);
            move(to, from);
        }
        public virtual void move(Point from, Point to)//**НОВОЕ
        {
            int dx = to.X - from.X;
            int dy = to.Y - from.Y;
            p1.X += dx;
            p1.Y += dy;
            p2.X += dx;
            p2.Y += dy;
        }
        public virtual void attachToGrid(int step)//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
        { //basicaly, attach figure means attach it's base points
            p1 = GraphHelper.attachPoint(p1, step);
            p2 = GraphHelper.attachPoint(p2, step);
        }
        public class GraphHelper// координатная сетка НОВЫЙ КЛАСС
        {
            public static int roundCoordinate(int x, int m)
            {
                int mod = x % m;
                if (mod >= m / 2)
                    return (x / m + 1) * m;
                return x - x % m;
            }
            public static Point attachPoint(Point p, int step)
            {
                return new Point(roundCoordinate(p.X, step), roundCoordinate(p.Y, step));
            }
        }
        public abstract void draw(ref Graphics g);//рисование окончательной фигуры
        public abstract void drawFrame(ref Graphics g);//рисование "предварительной" фигуры
        public abstract Rectangle getRectangle();//**НОВОЕ
    }
}
