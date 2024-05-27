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
using System.Drawing.Imaging;

namespace WindowsFormsApplication1
{
	public partial class Form2 : Form
	{
		//ПЕРЕМЕННЫЕ
		//======================================================================================

		public string fileName; //имя файла, когда картинка сохраняется
		public bool fromFile = false; //флаг: существовал ли данный файл
		bool paintAction = false; // удержание кнопки мыши (флаг)
        bool selectAction = false;    //выбор какой-либо фигуры                 **НОВОЕ
        bool dragAction = false;      //переместить фигуру в выбранное место    **НОВОЕ
		bool changedCanvas = false; //изменение рисунка (флаг)
		Point start,finish; //точки (для рисования) начала фигуры и конца
		public List<AbstractFigure> fstorage = new List<AbstractFigure>(); //список базовых фигур
		AbstractFigure toPaint; //базовая фигура
        //параметры рисования
        //класс Bitmap инкапсулирует точечный рисунок, состоящий из данных точек графического изображения и атрибутов рисунка.
        Bitmap canvas = new Bitmap(10,10);
		public Color backColor = Color.White;	//цвет фона
		public Color frameColor = Color.Black;		//цвет фигуры непосредственно при её рисовании
		public Color primaryColor = Color.Black;    //цвет линий
		public Color secondaryColor = Color.Black;    //цвет заливки
		public int lineWidth = 1; //толщина пера
		public bool solidFill = false; //по умолчанию заливки нет
        public int figureID = 0; //фигура по умолчанию - линия
		public int pictWidth=1,pictHeight=1;
        public Font textFont;//текстовое окошко
        public bool selection = false; //выбор флага  **НОВОЕ
        //Сетка
        public bool showGrid = false;//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
        public bool gridAttach = false; //attach all figures (all and existing) to grid
        public int gridStep = 10;

		//РИСОВАНИЕ
		//======================================================================================
	
		public void drawCanvas() //рисование точечного рисунка
		{
			canvas.Dispose();                          // Dispose - явное освобождение ресурсов, т.е. "удаляем" старый точечный рисунок...
            canvas = new Bitmap(pictWidth,pictHeight); //...и создаём новый
			Graphics g = Graphics.FromImage(canvas);   //Graphics.FromImage cоздает новый объект Graphics из указанного объекта canvas
            g.Clear(backColor);//Очищаем всю поверхность рисования и выполняем заливку поверхности указанным цветом фона
            if (showGrid)//*** НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
            {
                int gx = pictWidth / gridStep;
                int gy = pictHeight / gridStep;
                Pen gpen = new Pen(Color.LightGray, 1);
                gpen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                for (int i = 0; i < gx; i++) //vertical
                    g.DrawLine(gpen, i * gridStep, 0, i * gridStep, pictHeight);
                for (int i = 0; i < gy; i++)
                    g.DrawLine(gpen, 0, i * gridStep, pictWidth, i * gridStep);
            }
            foreach (AbstractFigure go in fstorage) //**НОВОЕ
            {
                go.draw(ref g);
                if (go.selected)
                {
                    go.drawSelection(ref g);
                    if (dragAction)
                        go.drawDragged(ref g, start, finish);
                }
            }
            if (paintAction || selectAction)//**НОВОЕ
                toPaint.drawFrame(ref g); //если зажата кнопка мыши, то рисуем временный рисунок
            g.Dispose();
		}

		private void initPainter() //определяется фигура по её id
		{   
			switch(figureID)
			{
				case 0:	toPaint = new GLine(); break;
				case 1: toPaint = new GCurve(); break;
				case 2: toPaint = new GRectangle(); break;
				case 3: toPaint = new GEllipse(); break;
                case 4:
                    toPaint = new GTextLabel();
                    ((GTextLabel)toPaint).tFont = textFont;
                    ((GTextLabel)toPaint).tbParent = this;
                    break;
				default: toPaint = new GLine(); break; //фигура по умолчанию - линия
			}
			//устанавливаются параметры фигуры
			toPaint.loadColors(primaryColor,secondaryColor,frameColor); 
			toPaint.firstPoint = start; //точка (для рисования) начала фигуры
            toPaint.secondPoint = start; //точка (для рисования) конца фигуры
            toPaint.lineWidth = lineWidth; //толщина пера
			toPaint.fill = solidFill; //наличие заливки
		}
       
        public void deleteSelected() // ** НОВОЕ
        {
            for (int i = 0; i < fstorage.Count; i++)
                if (fstorage[i].selected)
                    fstorage.RemoveAt(i--); //убрать выбранную фигуру
            ((Form1)this.MdiParent).clipboardToolsMenu(false);
            redrawAll();
        }

        private void selectFigures()  // ** НОВОЕ
        {
            dropSelection();
            Rectangle trect = toPaint.getRectangle();
            for (int i = 0; i < fstorage.Count; i++)
                if (trect.IntersectsWith(fstorage[i].getRectangle()))
                    fstorage[i].selected = true;
            parentClipboardInterface();
            redrawAll();
        }

        public void dropSelection() // ** НОВОЕ
        {
            for (int i = 0; i < fstorage.Count; i++)
                fstorage[i].selected = false;
            ((Form1)this.MdiParent).clipboardToolsMenu(false);
            redrawAll();
        }

        public bool isInsideOfRectangle(Rectangle rect, Point p) // ** НОВОЕ
        {
            return ((p.X >= rect.Left) && (p.X <= rect.Left + rect.Width) && (p.Y >= rect.Top) && (p.Y <= rect.Top + rect.Height));
        }

        public bool isInside(Rectangle sm, Rectangle lg) // ** НОВОЕ
        {
            return (isInsideOfRectangle(lg, new Point(sm.Left, sm.Top)) && isInsideOfRectangle(lg, new Point(sm.Left + sm.Width, sm.Top + sm.Height)));
        }

        public void selectSingleFigure(Point p) // ** НОВОЕ
        {
            bool inside = false;
            for (int i = fstorage.Count - 1; i >= 0; i--)
                if (isInsideOfRectangle(fstorage[i].getRectangle(), p))
                {
                    fstorage[i].selected = true;
                    inside = true;
                    break;
                }
            if (!inside)
                dropSelection();
            parentClipboardInterface();
        }

        public void moveSelectedFigures(Point f, Point s) // ** НОВОЕ
        {
            for (int i = 0; i < fstorage.Count; i++)
                if (fstorage[i].selected)
                    fstorage[i].move(f, s);
            if (gridAttach)//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
                attachFigures();
            redrawAll();
        }
        public void redrawAll()
        {
            drawCanvas();
            Refresh();
        }
        public void attachFigures()//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
        {
            foreach (AbstractFigure af in fstorage)
                af.attachToGrid(gridStep);
            redrawAll();
        }
		//I/O
		//======================================================================================

		public void SaveFile(string name)
		{
            //класс BinaryFormatter применяется для бинарной сериализации
            BinaryFormatter formatter = new BinaryFormatter();
            //Класс Stream представляет байтовый поток и является базовым для всех остальных классов потоков
            //Класс FileStream предоставляет реализацию абстрактного члена Stream в манере, подходящей для потоковой работы с файлами.
            //Класс FileStream представляет возможности по считыванию из файла и записи в файл как текстовый, так и бинарный
            Stream stream = new FileStream(name,FileMode.Create,FileAccess.Write,FileShare.None);//создаёт новый файл, позволяет др потокам получать доступ для записи до тех пор, пока он остаётся открытым
			formatter.Serialize(stream,pictWidth); //сериализация одним методом formatter.Serialize добавляет данные о ширине в файл stream
            formatter.Serialize(stream,pictHeight);//..о высоте..
			formatter.Serialize(stream,backColor); //..о цвете фона..
			formatter.Serialize(stream,fstorage);  //..о массиве базовых фигур..
			stream.Close();//закрываем поток
			changedCanvas = false; //флаг означает, что изменений нет
		}

		public void LoadFile(string name)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read);// открывает файл, позволяет др потокам получать доступ для чтения

            pictWidth = (int)formatter.Deserialize(stream); //десериализации  объекта stream -> преобразовать к типу int
            pictHeight = (int)formatter.Deserialize(stream);
			backColor = (Color)formatter.Deserialize(stream);
			fstorage = (List<AbstractFigure>)formatter.Deserialize(stream);
			stream.Close(); //закрываем поток
            drawCanvas(); //перерисовка
            Refresh(); //делает недоступной свою клиентскую область и немедленно перерисовывает себя и все дочерние элементы
        }
        //ВИД ФОРМЫ
        //======================================================================================

        public void selectAll()
        {
            selection = true;
            for (int i = 0; i < fstorage.Count; i++)
                fstorage[i].selected = true;
            parentClipboardInterface();
            redrawAll();
        }

        public void parentClipboardInterface()
        {
            bool anySelected = false;
            foreach (AbstractFigure af in fstorage)
                if (af.selected)
                {
                    anySelected = true;
                    break;
                }
            ((Form1)this.MdiParent).clipboardToolsMenu(anySelected);
        }

        public void copySelected()
        {
            List<AbstractFigure> toc = new List<AbstractFigure>();
            foreach (AbstractFigure af in fstorage)
                if (af.selected)
                    toc.Add(af);

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, toc);
            Clipboard.SetDataObject(ms, true);
            ms.Close();
            checkClipboard();
        }

        public void copyMetafile()
        {
            List<AbstractFigure> toc = new List<AbstractFigure>();
            int minx = Int32.MaxValue, miny = Int32.MaxValue;
            int maxx = 0, maxy = 0;
            foreach (AbstractFigure af in fstorage)
                if (af.selected)
                {
                    toc.Add(af);
                    minx = Math.Min(minx, af.getRectangle().Left);
                    miny = Math.Min(miny, af.getRectangle().Top);
                    maxx = Math.Max(maxx, af.getRectangle().Right);
                    maxy = Math.Max(maxy, af.getRectangle().Bottom);
                }

            Graphics g = Graphics.FromImage(new Bitmap(maxx, maxy));
            IntPtr dc = g.GetHdc();
            Metafile mf = new Metafile(dc, EmfType.EmfOnly);
            g.ReleaseHdc(dc);
            g.Dispose();

            Graphics gr = Graphics.FromImage(mf);
            foreach (AbstractFigure af in toc)
            {
                Rectangle rect = af.getRectangle();
                Point from = new Point(rect.Left, rect.Top);
                Point to = new Point(rect.Left - minx, rect.Top - miny);
                af.move(from, to);
                af.draw(ref gr);
                af.move(to, from);
            }
            gr.Dispose();
            ClipboardMetafileHelper.PutEnhMetafileOnClipboard(this.Handle, mf);
        }

        public void cutSelected()
        {
            copySelected();
            deleteSelected();
        }

        public void pasteData()
        {
            dropSelection();

            MemoryStream ms = (MemoryStream)Clipboard.GetDataObject().GetData(typeof(MemoryStream));
            BinaryFormatter formatter = new BinaryFormatter();
            List<AbstractFigure> toc = (List<AbstractFigure>)formatter.Deserialize(ms);
            ms.Close();

            int minx = Int32.MaxValue, miny = Int32.MaxValue;
            foreach (AbstractFigure af in toc)
            {
                Rectangle rect = af.getRectangle();
                if (rect.Width > pictWidth || rect.Height > pictHeight)
                {
                    MessageBox.Show("Изображение слишком большое!");
                    return;
                }
                minx = Math.Min(minx, rect.Left);
                miny = Math.Min(miny, rect.Top);
            }

            foreach (AbstractFigure af in toc)
            {
                Rectangle rect = af.getRectangle();
                af.move(new Point(rect.Left, rect.Top), new Point(rect.Left - minx, rect.Top - miny));
                af.selected = true;
                fstorage.Add(af);
            }

            if (gridAttach)//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
                attachFigures();

            ((Form1)MdiParent).setSelectionMode(true);
            parentClipboardInterface();
            redrawAll();
        }

        public void checkClipboard()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(typeof(MemoryStream)))
                ((Form1)MdiParent).pasteMenu(true);
            else
                ((Form1)MdiParent).pasteMenu(false);
        }

		//ВИД ФОРМЫ
		//======================================================================================

		public Form2()
		{
			InitializeComponent();
		}

		private void Form2_Shown(object sender, EventArgs e)
		{
			canvas = new Bitmap(pictWidth,pictHeight);
			Graphics.FromImage(canvas).Clear(backColor);//Graphics.FromImage cоздает новый объект Graphics из указанного объекта canvas
            redrawAll();
        }

		private void Form2_Activated(object sender, EventArgs e)
		{
			((Form1)this.ParentForm).setWindowSizeCaption(pictWidth,pictHeight);//Строка состояния_размер окна 
            parentClipboardInterface();
            redrawAll();
        }

		//ДИВЖЕНИЕ МЫШИ
		//======================================================================================

        private void Form2_MouseDown(object sender, MouseEventArgs e)//нажатие кнопки мыши
        {
            int eX = e.X - AutoScrollPosition.X;//определяются текущие координаты 
            int eY = e.Y - AutoScrollPosition.Y;
            if (eX <= pictWidth && eY <= pictHeight)//** РАСШИРЕННО
            {
                if (e.Button == MouseButtons.Left && !selection)
                {
                    dropSelection();
                    start.X = eX;//устанавливаются координаты нажатия в качестве точки начала для рисования
                    start.Y = eY;
                    finish = start;
                    initPainter(); //определяется фигура по её id
                    paintAction = true; //удержание кнопки мыши (флаг)
                }
                else
                    if (e.Button == MouseButtons.Left && selection)
                    {
                        start.X = eX;
                        start.Y = eY;
                        foreach (AbstractFigure af in fstorage)
                            if (af.selected && isInsideOfRectangle(af.getRectangle(), start))
                            {
                                dragAction = true; //prepare for dragging
                                break;
                            }
                        if (!dragAction)
                        {
                            toPaint = new GRectangle();
                            toPaint.loadColors(primaryColor, secondaryColor, frameColor);
                            toPaint.firstPoint = new Point(eX, eY);
                            toPaint.secondPoint = new Point(eX, eY);
                            selectAction = true;
                        }
                    }
            }
        }
		private void Form2_MouseMove(object sender, MouseEventArgs e) //передвижение мыши
		{ 
			int eX = e.X - AutoScrollPosition.X; //определяются текущие координаты
			int eY = e.Y - AutoScrollPosition.Y;
            if (paintAction || selectAction) //если кнопка мыши нажата
			{
				finish.X = eX; //текущие координаты устанавливаются в качестве точки конца для рисования
				finish.Y = eY;
                toPaint.secondPoint = finish; //сохраняется текущая точка как точка конца
                redrawAll();
            }
            if (selectAction)//**НОВОЕ
                selectFigures();
            if (dragAction)//**НОВОЕ
            {
                finish.X = eX;
                finish.Y = eY;
                redrawAll();
            }
			((Form1)this.ParentForm).setMousePositionCaption(eX,eY); //Строка состояния_координаты курсора мыши
        }

		private void Form2_MouseUp(object sender, MouseEventArgs e)//отпускание мыши
		{ 
			int eX = e.X - AutoScrollPosition.X; //определяются текущие координаты
            int eY = e.Y - AutoScrollPosition.Y;
            finish.X = eX;	//определяются конечные координаты//** НОВОЕ
            finish.Y = eY;
			if(paintAction) //если кнопка мыши была нажата
            {
				paintAction = false; //устанавливается флаг - кнопка мыши не нажата
				toPaint.secondPoint = finish; //сохраняется текущая точка как точка конца
                changedCanvas = true; //изменения есть
                if (isInside(toPaint.getRectangle(), new Rectangle(0, 0, pictWidth, pictHeight))) //если фигура внутри формы (окна), то в массив базовых фигур добавляется новая фигура
                {
                    if (gridAttach)
                        toPaint.attachToGrid(gridStep);
                    fstorage.Add(toPaint); //add new figure to storage
                }
            }
            if (selectAction)//**НОВОЕ
            {
                selectAction = false;
                start = toPaint.firstPoint;
                if (Math.Abs(start.X - finish.X) < 3 && Math.Abs(start.Y - finish.Y) < 3)
                    selectSingleFigure(toPaint.firstPoint);
            }
            if (dragAction)//**НОВОЕ
            {
                dragAction = false;//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12

                bool OK = true;
                int dx = finish.X - start.X;
                int dy = finish.Y - start.Y;
                foreach (AbstractFigure af in fstorage)
                {
                    Rectangle rect = af.getRectangle();
                    if (af.selected)
                        if (!isInside(new Rectangle(rect.Left + dx, rect.Top + dy, rect.Width, rect.Height), new Rectangle(0, 0, pictWidth, pictHeight)))
                        {
                            OK = false;
                            break;
                        }
                }
                if (OK)
                    moveSelectedFigures(start, finish);
                changedCanvas = true;
            }
            redrawAll();
		}

		private void Form2_MouseLeave(object sender, EventArgs e)
		{
			((Form1)this.ParentForm).setMousePositionCaption(-1,-1);//Строка состояния_координаты курсора мыши - за границей
        }

		//======================================================================================

		private void Form2_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(Color.LightGray); //заполняем фон рабочей области рисунка
            e.Graphics.DrawImage(canvas,AutoScrollPosition.X,AutoScrollPosition.Y); //рисуем  объект canvas в заданном месте, используя исходный размер; AutoScrollPosition представляет местоположение видимой части элемента управления с возможностью прокрутки
        }

		private void Form2_Resize(object sender, EventArgs e)
		{
            redrawAll();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        //закрытие активной формы
        //======================================================================================

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
		{
			if(this.ParentForm.MdiChildren.Length==1)
			{
                ((Form1)this.ParentForm).fileOperationsMenu(false); //последнее дочернее окно закрыто 
                ((Form1)this.ParentForm).setMousePositionCaption(-1, -1);//Строка состояния_координаты курсора мыши
                ((Form1)this.ParentForm).setWindowSizeCaption(-1, -1);//Строка состояния_размер окна - пусто
                ((Form1)this.ParentForm).clipboardToolsMenu(false); //ничего нет для "копирования"
                ((Form1)this.ParentForm).pasteMenu(false); //ничего нет для "вставки"
            }
		}

		private void Form2_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(changedCanvas) //если рисунок изменялся
				switch(MessageBox.Show("Сохранить изменения в \""+this.Text+"\"?","",MessageBoxButtons.YesNoCancel))
				{
					case DialogResult.Yes:
						((Form1)this.ParentForm).saveFile();
					break;
					case DialogResult.Cancel:
						e.Cancel = true;
					break;
				}
		}
        private void timer1_Tick(object sender, EventArgs e)
        {
            checkClipboard();
        }

        private void Form2_Scroll(object sender, ScrollEventArgs e)//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
        {
            redrawAll();
        }	
		//======================================================================================
	}

}
