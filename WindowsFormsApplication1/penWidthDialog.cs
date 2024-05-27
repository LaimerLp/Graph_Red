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
	public partial class penWidthDialog : Form
	{

		public int Val=1; //начальное значение толщины линии 

		public penWidthDialog()
		{
			InitializeComponent();
		}

        //Int32.Parse - строка -> 32-битовое число со знаком
        //Свойство comboBox1.SelectedItem указывает отсчитываемый от нуля индекс выбранного элемента в списке

        private void button1_Click(object sender, EventArgs e) //кнопка ОК
		{
			if(comboBox1.SelectedItem==null) Val = 1; //если значение толщины не было выбрано (осталось пустое поле)
            else Val = Int32.Parse((string)comboBox1.SelectedItem);
		}

        private void penWidthDialog_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
