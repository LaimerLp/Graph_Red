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
    public class GRectangle : AbstractFigure 
    {
        [NonSerialized()]
        int x, y, w, h;
        void transform()//нормализация координат прямоугольника
        {
            if (p2.X > p1.X) //для X
            {
                x = p1.X;
                w = p2.X - p1.X;
            }
            else {
                x = p2.X;
                w = p1.X - p2.X;
            }
            if (p2.Y > p1.Y) //для Y
            {
                y = p1.Y;
                h = p2.Y - p1.Y;
            }
            else {
                y = p2.Y;
                h = p1.Y - p2.Y;
            }
        }
        public override void drawFrame(ref Graphics g)//рисование "предварительной" фигуры
        {
            Pen p = new Pen(frameColor); //перо определённого цвета (см. форму2) с толщиной по умолчанию
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; //пунктир
            transform(); //нормализация координат прямоугольника
            g.DrawRectangle(p, x, y, w, h);
            p.Dispose(); //сброс
        }
        public override void draw(ref Graphics g)//рисование окончательной фигуры (при отпускании)
        {
            Pen p = new Pen(primaryColor, lWidth); //перо выбранного цвета с выбранной толщиной 
            transform(); //нормализация координат прямоугольника
            if (fill)  g.FillRectangle(new SolidBrush(secondaryColor), x, y, w, h); //если выбрана "Заливка"
            g.DrawRectangle(p, x, y, w, h);
            p.Dispose(); //сброс
        }
        public override Rectangle getRectangle()
        {
            int x, y, w, h;
            if (p2.X > p1.X) //Для X
            {
                x = p1.X;
                w = p2.X - p1.X;
            }
            else
            {
                x = p2.X;
                w = p1.X - p2.X;
            }
            if (p2.Y > p1.Y) //Для Y
            {
                y = p1.Y;
                h = p2.Y - p1.Y;
            }
            else
            {
                y = p2.Y;
                h = p1.Y - p2.Y;
            }
            return new Rectangle(x, y, w, h);
        }
    }
}
