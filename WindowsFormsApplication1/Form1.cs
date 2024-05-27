using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public int wCount = 0; //количество открытых дочерних окон
        public int lineWidth = 1;   //текущая толщина линии
        public int pictHeight = 600, pictWidth = 800; //параметры нового окна
        public bool solidFill = false;
        public int figureID = 0;
        public bool textMode = false;// режим надпись
        public Font textFont = new Font("Times New Roman", 12);//параметры надписи
        public bool selection = false; //флажок для "Выделить"
        public int gridStep = 10;//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
        public bool showGrid = false;//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
        public bool gridAttach = false;//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12

        public void newWindow()
        {
            Form2 f2 = new Form2(); //create new window
            f2.MdiParent = this;
            //применять исходные параметры рисования
            f2.lineWidth = lineWidth;
            f2.primaryColor = primColorDialog.Color;
            f2.secondaryColor = secondColorDialog.Color;
            f2.backColor = backColorDialog.Color;
            f2.solidFill = solidFill;
            f2.figureID = figureID;
            f2.textFont = textFont;
            f2.showGrid = showGrid;//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
            f2.gridAttach = gridAttach;//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
            f2.gridStep = gridStep;//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
            //параметры формы
            f2.pictHeight = pictHeight;
            f2.pictWidth = pictWidth;
            f2.AutoScrollMinSize = new Size(pictWidth, pictHeight);
            f2.AutoScroll = true;
            f2.Text = "Изображение № " + (++wCount);
            f2.Show();
        }

        //INTERFACE
        //======================================================================================

        public void fileOperationsMenu(bool act)
        { //enable/disable menu items
            saveToolStripMenuItem.Enabled = act;
            saveAsToolStripMenuItem.Enabled = act;
            toolStripButton3.Enabled = act;
            selectAllToolStripMenuItem.Enabled = act;
            reattachToolStripMenuItem.Enabled = act;//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
        }

        public void setWindowSizeCaption(int w, int h)
        {
            if (w >= 0 && h >= 0)
                statusBarPanel4.Text = "Размер рисунка:(" + w + "," + h + ")";
            else
                statusBarPanel4.Text = "";
        }
        //размер окна //строка состояния
        public void setMousePositionCaption(int x, int y)
        {
            if (x >= 0 && y >= 0)
                statusBarPanel5.Text = x + "," + y;
            else
                statusBarPanel5.Text = "";
        }//мышь

        public void statusMessage() //сообщение о выбранных настройках текста в строке состояния
        {
            if (textMode)
            {
                statusBar1.Panels[5].Text = textFont.Name + " " + textFont.Size + "pt";
            }
            else
                statusBar1.Panels[5].Text = "";
        }

        public void clipboardToolsMenu(bool act)
        {
            copyMetafileToolStripMenuItem.Enabled = act;
            copySelectedToolStripMenuItem.Enabled = act;
            cutSelectedToolStripMenuItem.Enabled = act;
        }

        public void pasteMenu(bool act)
        {
            pasteToolStripMenuItem.Enabled = act;
        }

        public void setSelection()
        {
            if (selectToolStripMenuItem.Checked)
                setSelectionMode(false);
            else
                setSelectionMode(true);
        }
        //FILE I/O
        //======================================================================================

        public void openFile()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                newWindow(); //create new window
                ((Form2)this.ActiveMdiChild).LoadFile(openFileDialog1.FileName);
                ((Form2)this.ActiveMdiChild).fromFile = true;
                ((Form2)this.ActiveMdiChild).fileName = openFileDialog1.FileName;
                this.ActiveMdiChild.Text = openFileDialog1.FileName; //задать текст заголовка окна
            }
            fileOperationsMenu(true);
        }

        public void saveAsFile()
        {
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ((Form2)this.ActiveMdiChild).SaveFile(saveFileDialog1.FileName);
                ((Form2)this.ActiveMdiChild).fileName = saveFileDialog1.FileName;
                ((Form2)this.ActiveMdiChild).fromFile = true;
                this.ActiveMdiChild.Text = saveFileDialog1.FileName; //установить текст
            }
        }

        public void saveFile()
        {
            if (((Form2)this.ActiveMdiChild).fromFile)
                ((Form2)this.ActiveMdiChild).SaveFile(((Form2)this.ActiveMdiChild).fileName); //сохранить существующий файл
            else
                saveAsFile(); //сохранить новый файл
        }

        //Настройки изображения
        //======================================================================================

        public void setPenWidth()
        {
            penWidthDialog pwd = new penWidthDialog();
            pwd.Val = lineWidth; //установить старую ширину пера
            if (pwd.ShowDialog() == DialogResult.OK)
            {
                foreach (Form f2 in this.MdiChildren)
                    ((Form2)f2).lineWidth = pwd.Val; //распространяются на всех детей окна
                lineWidth = pwd.Val;
                statusBarPanel1.Text = "Линия:" + lineWidth + " размер" ;
            }
        }

        public void setPictureSize()
        {
            pictureSizeDialog psd = new pictureSizeDialog();
            if (psd.ShowDialog() == DialogResult.OK)
            {
                pictHeight = psd.newHeight;
                pictWidth = psd.newWidth;
            }
        }

        public void setPrimaryColor()
        {
            if (primColorDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (Form f2 in this.MdiChildren)
                    ((Form2)f2).primaryColor = primColorDialog.Color;
                statusBar1.Refresh();
            }

        }

        public void setSecondaryColor()
        {
            if (secondColorDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (Form f2 in this.MdiChildren)
                    ((Form2)f2).secondaryColor = secondColorDialog.Color;
                statusBar1.Refresh();
            }
        }

        public void setFont() // вызов диалогового окна для изменения параметров надписи
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                textFont = fontDialog1.Font;
                statusMessage();
                foreach (Form f2 in this.MdiChildren)
                    ((Form2)f2).textFont = textFont;

            }
        }

        public void setGridStep()
        {
            gridStepDialog gsd = new gridStepDialog();
            gsd.gridStep = gridStep;
            if (gsd.ShowDialog() == DialogResult.OK)
            {
                gridStep = gsd.gridStep;
                foreach (Form f2 in MdiChildren)
                {
                    ((Form2)f2).gridStep = gridStep; //set new grid step
                    if (gridAttach)
                        ((Form2)f2).attachFigures();
                    else
                        ((Form2)f2).redrawAll();
                }
            }
        }

        public void setShowGrid()
        {
            showGrid = gridToolStripMenuItem.Checked;
            foreach (Form f2 in MdiChildren)
            {
                ((Form2)f2).showGrid = showGrid; //set grid state
                ((Form2)f2).redrawAll(); //refresh
            }
        }
        public void setAttach()
        {
            gridAttach = attachToGridToolStripMenuItem.Checked;
            foreach (Form f2 in MdiChildren)
            {
                ((Form2)f2).gridAttach = gridAttach;
                if (gridAttach)
                    ((Form2)f2).attachFigures();
            }
        }

        public void reattach()
        {
            foreach (Form f2 in MdiChildren)
                ((Form2)f2).attachFigures();
        }
        //FUGURE SELECTION/FILL Пересчет данных ВЫБОР / заливка
        //======================================================================================

        public void allDown()
        { //установить все фигуры типа управления "проверили" на ложный
            lieToolStripMenuItem.Checked = false;
            rectangleToolStripMenuItem.Checked = false;
            ellipseToolStripMenuItem.Checked = false;
            curveToolStripMenuItem.Checked = false;
            textToolStripMenuItem.Checked = false;// выключить текст
            toolStripButton8.Checked = false;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = false;
            toolStripButton13.Checked = false;//кнопка на инструменте о тексте
            textMode = false;
            statusMessage(); //refresh status message
        }

        public void setFigureType()
        {
            foreach (Form f2 in this.MdiChildren) //применять фигуры типа все активные окна
                ((Form2)f2).figureID = figureID;
        }

        public void setFill() //заливка
        {
            if (!fillToolStripMenuItem.Checked)
            {
                solidFill = true;
                fillToolStripMenuItem.Checked = true;
                toolStripButton12.Checked = true;
            }
            else
            {
                solidFill = false;
                fillToolStripMenuItem.Checked = false;
                toolStripButton12.Checked = false;
            }
            foreach (Form f2 in this.MdiChildren)
                ((Form2)f2).solidFill = solidFill;

        }

        public void setLine()
        {
            allDown();
            lieToolStripMenuItem.Checked = true;
            toolStripButton8.Checked = true;
            figureID = 0;
            setFigureType();
        }

        public void setCurve()
        {
            allDown();
            curveToolStripMenuItem.Checked = true;
            toolStripButton9.Checked = true;
            figureID = 1;
            setFigureType();
        }

        public void setRectangle()
        {
            allDown();
            rectangleToolStripMenuItem.Checked = true;
            toolStripButton10.Checked = true;
            figureID = 2;
            setFigureType();
        }

        public void setEllipse()
        {
            allDown();
            ellipseToolStripMenuItem.Checked = true;
            toolStripButton11.Checked = true;
            figureID = 3;
            setFigureType();
        }

        public void setTextLabel()// текстовая надпись
        {
            allDown();
            textToolStripMenuItem.Checked = true;
            toolStripButton14.Checked = true;
            textMode = true;
            statusMessage();
            figureID = 4;
            setFigureType();
        }

        //FIGURE MANAGEMENT
        //======================================================================================

        public void deleteFigure()
        {
            if (this.ActiveMdiChild != null)
                ((Form2)this.ActiveMdiChild).deleteSelected();
        }
        public void setSelectionMode(bool act)
        {
            if (act)
            {
                selectToolStripMenuItem.Checked = true;
                toolStripButton15.Checked = true;
                selection = true;
            }
            else
            {
                selectToolStripMenuItem.Checked = false;
                toolStripButton15.Checked = false;
                selection = false;
            }
            foreach (Form f2 in this.MdiChildren)
            {
                ((Form2)f2).selection = selection;
                if (!selection)
                    ((Form2)f2).dropSelection();
            }
        }
        public void selectAll()
        {
            setSelectionMode(true);
            if (this.ActiveMdiChild != null)
                ((Form2)this.ActiveMdiChild).selectAll();
        }

        public void copySelected()
        {
            if (this.ActiveMdiChild != null)
                ((Form2)this.ActiveMdiChild).copySelected();
        }

        public void copyMetafile()
        {
            if (this.ActiveMdiChild != null)
                ((Form2)this.ActiveMdiChild).copyMetafile();
        }

        public void cutSelected()
        {
            if (this.ActiveMdiChild != null)
                ((Form2)this.ActiveMdiChild).cutSelected();
        }

        public void pasteData()
        {
            if (this.ActiveMdiChild != null)
                ((Form2)this.ActiveMdiChild).pasteData();
        }
        //======================================================================================

        public Form1()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            newWindow();
            fileOperationsMenu(true);
        }

        private void oPenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFile();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAsFile();
        }

        private void lineWidthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setPenWidth();
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setSecondaryColor();
        }

        private void lineColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setPrimaryColor();
        }

        private void newPicturesSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setPictureSize();
        }

        private void fillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setFill();
        }

        private void lieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLine();
        }

        private void curveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setCurve();
        }

        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setRectangle();
        }

        private void ellipseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setEllipse();
        }

        private void statusBar1_DrawItem(object sender, StatusBarDrawItemEventArgs sbdevent)
        {
            Graphics g = statusBar1.CreateGraphics();
            g.FillRectangle(new SolidBrush(primColorDialog.Color), 100, 2, sbdevent.Panel.Width, statusBar1.Height);
            g.FillRectangle(new SolidBrush(secondColorDialog.Color), 150, 2, sbdevent.Panel.Width, statusBar1.Height);
            g.Dispose();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            newWindow();
            fileOperationsMenu(true);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            openFile();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            saveFile();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            setPenWidth();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            setPrimaryColor();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            setSecondaryColor();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            setPictureSize();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            setLine();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            setCurve();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            setRectangle();
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            setEllipse();
        }
      
        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            setFill();
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setTextLabel();
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            setTextLabel();
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            setFont();
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setFont();
        }

        private void fontDialog1_Apply(object sender, EventArgs e)
        {

        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setSelection();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            setSelection();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteFigure();
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            deleteFigure();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectAll();
        }

        private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copySelected();
        }

        private void copyMetafileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyMetafile();
        }

        private void cutSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cutSelected();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pasteData();
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setShowGrid();
        }

        private void gridStepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setGridStep();
        }


        private void attachToGridToolStripMenuItem_Click(object sender, EventArgs e)//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
        {
            setAttach();
        }

        private void привязатьКривыеКСеткеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = (Form2)ActiveMdiChild;
            form2.MdiParent = this;
            foreach (AbstractFigure figure in form2.fstorage)
            {
                if (figure is GCurve curve)
                {
                    curve.AlignToGrid(gridStep);
                }
            }

            form2.drawCanvas();
            Refresh();
        }



        private void reattachToolStripMenuItem_Click(object sender, EventArgs e)//***НОВЫЕ ИЗМЕНЕНИЯ ЛАБА 12
        {
            reattach();
        }


    }
}
