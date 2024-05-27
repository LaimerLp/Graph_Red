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
	public partial class pictureSizeDialog : Form
	{
		public int newHeight=10,newWidth=10;

		public pictureSizeDialog() //конструктор
        {
			InitializeComponent();
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)//изменение состояния флажка выбора вручную
        {
			if(checkBox1.Checked)//если установлен флажок выбора ввода вручную 
            {
				textBox1.Enabled = true;
				textBox2.Enabled = true;
				label1.Enabled = true;
				label2.Enabled = true;
			}else
            {//если не установлен флажок выбора ввода вручную 
                textBox1.Enabled = false;
				textBox2.Enabled = false;
				label1.Enabled = false;
				label2.Enabled = false;
			}
		}

		private void button1_Click(object sender, EventArgs e)//кнопка "ОК"
        {
			
			if(checkBox1.Checked)//если установлен флажок выбора ввода вручную  
            {
				try{
					newWidth = Int32.Parse(textBox1.Text);//устанавливаем ширину, равной введённому значению
                }
                catch(Exception){
					newWidth = 200; //в случае возникновения исключения
				}
				try{
					newHeight = Int32.Parse(textBox2.Text);//устанавливаем высоту, равной введённому значению
                }
                catch(Exception){
					newHeight = 200; //в случае возникновения исключения
                }
			}else
            {//если не установлен флажок выбора ввода вручную 
                if (radioButton1.Checked)
				{
					newWidth = 320;
					newHeight = 240;
				}else if(radioButton2.Checked)
				{
					newWidth = 640;
					newHeight = 480;
				}else{
					newWidth = 800;
					newHeight = 600;
				}
			}
		}

		private void textBox1_TextChanged(object sender, EventArgs e)//ввод в поле "ширина"
        {
			try{
				Int32.Parse(textBox1.Text);
			}catch(Exception){
				MessageBox.Show("Введите число.");
			}
		}

		private void textBox2_TextChanged(object sender, EventArgs e)//ввод в поле "высота"
        {
			try{
				Int32.Parse(textBox2.Text);
			}catch(Exception){
				MessageBox.Show("Введите число.");
			}
		}

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pictureSizeDialog_Load(object sender, EventArgs e)
		{

		}
	}
}
