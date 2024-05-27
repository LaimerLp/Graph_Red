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
    public class GLine : AbstractFigure 
    {
        public override void drawFrame(ref Graphics g) //рисование "предварительной" линии
        {
            Pen p = new Pen(frameColor); //перо определённого цвета (см. форму2) с толщиной по умолчанию
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; //пунктир
            g.DrawLine(p, p1, p2); //от точки нажатия до текущей точки 
            p.Dispose(); //сброс
        }
        public override void draw(ref Graphics g) //рисование окончательной линии (при отпускании)
        {
            Pen p = new Pen(primaryColor, lWidth); //перо выбранного цвета с выбранной толщиной 
            g.DrawLine(p, p1, p2); //от точки нажатия до точки отпускания
            p.Dispose(); //сброс
        }
        public override Rectangle getRectangle()//**НОВОЕ
        {
            return Rectangle.FromLTRB(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
        }
    }
}
