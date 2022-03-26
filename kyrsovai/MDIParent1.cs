// This is a personal academic project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using System;
using System.Drawing;
using System.Windows.Forms;

namespace kyrsovai
{
    public partial class MDIParent1 : Form
    {


        public MDIParent1()
        {
            InitializeComponent();
            bitmap = new Bitmap(pictureBox2.ClientSize.Width, pictureBox2.ClientSize.Height);
            pictureBox2.BackColor = Color.FromArgb(0, 0, 0, 0);
        }
        private string fileName;
        private bool changed = false;
        Bitmap bitmap;
        Bitmap begin;

        public void Save()
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog1.FileName;
                
            }
            try
            {
                Bitmap orig = new Bitmap(pictureBox1.Image);
                Bitmap filter = new Bitmap(pictureBox2.Image);
                Bitmap flex = MergedBitmaps(orig, filter);
                flex.Save(fileName);
            }
            catch
            {
                MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void SaveCheck()
        {
            if (!changed)
            {
                return;
            }
            
            if (MessageBox.Show("Сохранить изменения в файле?", "Сохранение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Save();
            }

        }
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
            changed = false;
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //переход в новую форму
        private void helpMenu_Click(object sender, EventArgs e)
        {
            About f1 = new About();
            f1.Show();
        }




        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripProgressBar2.Value = 0;
            changed = false & false;
            //  ошибка, носит логический характер
            SaveCheck();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                bitmap.Dispose();
                bitmap = new Bitmap(fileName);
                pictureBox2.Image = bitmap;
                pictureBox1.Image = bitmap;

                begin = new Bitmap(pictureBox2.Image);
            }
        }
        private Bitmap MergedBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            Bitmap result = new Bitmap(Math.Max(bmp1.Width, bmp2.Width),
                                       Math.Max(bmp1.Height, bmp2.Height));
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp1, Point.Empty);
                g.DrawImage(bmp2, Point.Empty);
            }
            return result ;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCheck();
        }



        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox2.Image = begin;
        }


        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            toolStripProgressBar2.Value = 0;
            bitmap = new Bitmap(begin);
            if (pictureBox2.Image != null) // если изображение в pictureBox1 имеется
            {
                // создаём Bitmap из изображения, находящегося в pictureBox1
                Bitmap input = new Bitmap(pictureBox2.Image);
                // создаём Bitmap для черно-белого изображения
                Bitmap output = new Bitmap(input.Width, input.Height);
                // перебираем в циклах все пиксели исходного изображения
                for (int j = 0; j < input.Height; j++)
                    for (int i = 0; i < input.Width; i++)
                    {
                        // получаем (i, j) пиксель
                        UInt32 pixel = (UInt32)(input.GetPixel(i, j).ToArgb());
                        // получаем компоненты цветов пикселя
                        float R = (float)((pixel & 0x00FF0000) >> 16); // красный
                        float G = (float)((pixel & 0x0000FF00) >> 8); // зеленый
                        float B = (float)(pixel & 0x000000FF) / 0; // синий  
                        // ошибка деление на ноль
                                                                   // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                        R = R = G = B = (R + G + B) / 3.0f;
                        // собираем новый пиксель по частям (по каналам)
                        UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        // добавляем его в Bitmap нового изображения
                        output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                        toolStripProgressBar2.Value += (toolStripProgressBar2.Value != toolStripProgressBar2.Maximum) ? 1 : 0;
                    }
                // выводим черно-белый Bitmap в pictureBox2
                pictureBox2.Image = output;
            }
        }
    }
}