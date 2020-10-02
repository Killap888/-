using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        OpenFileDialog ofd = new OpenFileDialog();
        Bitmap image;

        struct Dot//структура хранения значений точки
        {
            public int[,] matrix_of_points;
            public int numberZone;

            /*public Dot(int[,] matrixOfPoints, int numberOfZone)
            {
                matrix_of_points = matrixOfPoints;
                numberZone = numberOfZone;

            }*/

        }

        private void button_load_map_Click(object sender, EventArgs e)//загружаем изображение
        {
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG"; 
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    image = new Bitmap(ofd.FileName);
                    pictureBox1.Image = image;
                }
                catch
                {
                    DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }      


        public void LookSomePaintDots()//функция зон
        {
            int pictureMaxWidth = image.Width;
            int pictureMaxHeight = image.Height;
            int[,] matrixOfPoints = new int[pictureMaxWidth, pictureMaxHeight];
            int[,] matrixOfZone = new int[pictureMaxWidth, pictureMaxHeight];
            int[] matrixOfTopologicalFeatureDescription = new int[256];
            int numberOfZone = 0;
            bool firstZone = false;

            DateTime time = DateTime.Now;

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    matrixOfZone[x, y] = 0;
                }
            }

            

            for (int stepOfAverage = 0; stepOfAverage < 255; stepOfAverage++)
            {
                for (int x = 0; x < image.Width-1; x++)
                {
                    for (int y = 0; y < image.Height-1; y++)
                    {
                        int red, blue, green, averange;//переменные
                        red = (image.GetPixel(x, y).R);
                        green = (image.GetPixel(x, y).G);
                        blue = (image.GetPixel(x, y).B);
                        averange = (int)(0.299 * red + 0.587 * green + 0.114 * blue);//средн знач
                        Color p = Color.FromArgb(255, averange, averange, averange);

                        if (averange == stepOfAverage)
                        {
                            matrixOfPoints[x, y] = averange;
                            matrixOfTopologicalFeatureDescription[stepOfAverage] += averange;                            

                            if (firstZone == true)
                            {
                                if (x != 0 && y != 0)
                                {
                                    if (matrixOfZone[x, y] < matrixOfZone[x+1, y])
                                    {
                                        matrixOfZone[x, y] = matrixOfZone[x+1, y];                                        
                                    }
                                    else if (matrixOfZone[x, y] < matrixOfZone[x, y+1])
                                    {
                                        matrixOfZone[x, y] = matrixOfZone[x, y+1];
                                    }
                                    else if (matrixOfZone[x, y] < matrixOfZone[x - 1, y])
                                    {
                                        matrixOfZone[x, y] = matrixOfZone[x - 1, y];
                                    }
                                    else if (matrixOfZone[x, y] < matrixOfZone[x, y - 1])
                                    {
                                        matrixOfZone[x, y] = matrixOfZone[x, y - 1];
                                    }
                                    else
                                    {
                                        matrixOfZone[x, y] = numberOfZone;
                                        numberOfZone++;

                                    }
                                }
                            }
                            else
                            {
                                firstZone = true;
                                numberOfZone++;
                            }

                        }


                    }
                }
            }


            for(int i=0; i<255; i++)
            {
                string s = string.Format("[step: {0} || data: {1}] \n\n\n", i, matrixOfTopologicalFeatureDescription[i]);
                textBox1.Text += s;
            }

            labelNumberOfZone.Text = numberOfZone.ToString();
            TimeSpan sp = DateTime.Now - time;
            labelTimeOfWorking.Text = sp.ToString();

            dataGridView1.ColumnCount = pictureMaxWidth;
            dataGridView1.RowCount = pictureMaxHeight;
            for (int i = 0; i < pictureMaxWidth; i++)
            {
                for (int j = 0; j < pictureMaxHeight; j++)
                {
                    /*int red, blue, green, averange;//переменные
                    red = (image.GetPixel(i, j).R);
                    green = (image.GetPixel(i, j).G);
                    blue = (image.GetPixel(i, j).B);
                    averange = (int)(0.299 * red + 0.587 * green + 0.114 * blue);//средн знач
                    Color p = Color.FromArgb(255, averange, averange, averange);*/

                    dataGridView1.Rows[j].Cells[i].Value = matrixOfZone[i, j];
                    //dataGridView1.Rows[j].Cells[i].Style.BackColor = p;
                }
            }

            pictureBox1.Image = image;

        }

        private void button_Do_Some_Magic_Thing_Click(object sender, EventArgs e)//Маджик!!!!
        {
            LookSomePaintDots();
        }
    }
}
