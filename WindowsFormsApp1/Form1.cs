﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Threading;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using static System.Windows.Forms.AxHost;
using System.IO.Ports;
using static WindowsFormsApp1.Form1;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Bitmap map;
        public Bitmap map2; // для маленького поля задания 2
        //string cubeFilePath = "C:/Users/m8951/OneDrive/Рабочий стол/study/computer graphic/lab7/WindowsFormsApp1/input.txt";
        string cubeFilePath = "inputRomb.txt"; //  // inputCube // inputRomb // inputTriangle
        string filePath;
        int divi = 5;
        int task2startCount = 5;

        List<point> task2points = new List<point>(); // точки фигуры вращения
        List<edge> task2edges = new List<edge>();

        public class point
        {
            public double x;
            public double y;
            public double z;
            //public Color color;
            //List<int> 

            public point(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                //this.color = color;
            }

            //void draw()
            //{
            //    for (int x = this.x - 3; x < this.x + 3; x++)
            //    {
            //        for (int y = this.y - 3; y < this.y + 3; y++)
            //        {
            //            map.SetPixel(x, y, this.color);
            //        }
            //    }
            //}
        }

        class edge
        {
            public point a;
            public point b;
            //public Color color;

            public edge(point a, point b)//, Color color)
            {
                this.a = a;
                this.b = b;
                //this.color = color;
            }
        }

        class face
        {
            List<edge> edges;
            face(List<edge> edges)
            {
                this.edges = edges;
            }
        }

        class polyhedron
        {
            List<face> faces;
            polyhedron(List<face> faces)
            {
                this.faces = faces;
            }
        }

        List<point> points = new List<point>(); // точки основной фигуры, считанной из файла
        List<edge> edges = new List<edge>(); 
        List<edge> axes = new List<edge>();
        List<point> axes_points = new List<point>();
        bool isPerspectPr = false;
        double r = 0.001;
        double d1 = 0; // координаты центра фигуры
        double d2 = 0; 
        double d3 = 0;



        public Form1()
        {
            InitializeComponent();
            map = new Bitmap(pictureBox.Width, pictureBox.Height);
            map2 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox.BackColor = Color.White;
            pictureBox1.BackColor = Color.White;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void makeCube()
        {
            
        }

        void drawEdge(edge e, int delta)
        {
            double x1 = e.a.x + delta;
            double y1 = e.a.y + delta;
            double x2 = e.b.x + delta;
            double y2 = e.b.y + delta;
            Color color = Color.Black;// e.color;

            if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 ||
                x1 >= map.Width || y1 >= map.Height ||
                x2 >= map.Width || y2 >= map.Height)
                return;
            double deltaX = Math.Abs(x2 - x1);
            double deltaY = Math.Abs(y2 - y1);
            double signX = x1 < x2 ? 1 : -1;
            double signY = y1 < y2 ? 1 : -1;
            double error = deltaX - deltaY;
            map.SetPixel((int)x2, (int)y2, color);
            while (x1 != x2 || y1 != y2)
            {
                map.SetPixel((int)x1, (int)y1, color);
                double error2 = error * 2;
                if (error2 > -deltaY)
                {
                    error -= deltaY;
                    x1 += signX;
                }
                if (error2 < deltaX)
                {
                    error += deltaX;
                    y1 += signY;
                }
            }
        }

        void DrawLine(double x1, double y1, double x2, double y2, Color color)
        {
            if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 ||
                x1 >= map.Width || y1 >= map.Height ||
                x2 >= map.Width || y2 >= map.Height)
                return;
            double deltaX = Math.Abs(x2 - x1);
            double deltaY = Math.Abs(y2 - y1);
            double signX = x1 < x2 ? 1 : -1;
            double signY = y1 < y2 ? 1 : -1;
            double error = deltaX - deltaY;
            map.SetPixel((int)x2, (int)y2, color);
            while (x1 != x2 || y1 != y2)
            {
                map.SetPixel((int)x1, (int)y1, color);
                double error2 = error * 2;
                if (error2 > -deltaY)
                {
                    error -= deltaY;
                    x1 += signX;
                }
                if (error2 < deltaX)
                {
                    error += deltaX;
                    y1 += signY;
                }
            }
        }

        void clean()
        {
            Graphics graphics = Graphics.FromImage(map);
            graphics.Clear(Color.White);
            graphics = Graphics.FromImage(map);// mapPrim);
            graphics.Clear(Color.White);
            //points = new List<Point>();
            //Graphics graphics = Graphics.FromImage(map);// mapPrim);
            graphics.Clear(Color.White);

            map = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = map;

            //pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);
        }

        /// <summary>
        /// Соединение точек линиями
        /// </summary>
        /// <param name="e"></param>
        public void DrawLinePoint(PaintEventArgs e)
        {
            if (isPerspectPr)
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    
                    double ah = r * edges[i].a.z + 1;
                    double bh = r * edges[i].b.z + 1;

                    double temp_ax = edges[i].a.x / ah + 200;
                    double temp_ay = edges[i].a.y / ah + 200;

                    double temp_bx = edges[i].b.x / bh + 200;
                    double temp_by = edges[i].b.y / bh + 200;

                    Pen myPen = new Pen(Color.Black, 2);
                    e.Graphics.DrawLine(myPen,
                                       new Point((int)temp_ax, (int)temp_ay),
                                       new Point((int)temp_bx, (int)temp_by));
                }
            }
            else
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    Pen myPen = new Pen(Color.Black, 2);
                    e.Graphics.DrawLine(myPen,
                                       new Point((int)edges[i].a.x + 200, (int)edges[i].a.y + 200),
                                       new Point((int)edges[i].b.x + 200, (int)edges[i].b.y + 200));
                }
            }


            //for (int i = 0; i < axes.Count; i++)
            //{
            //    Pen myPen = new Pen(Color.Black, 2);
            //    e.Graphics.DrawLine(myPen,
            //                       new Point((int)axes[i].a.x + 200, (int)axes[i].a.y + 200),
            //                       new Point((int)axes[i].b.x + 200, (int)axes[i].b.y + 200));
            //}

            //axes_points

            //for (int i = 0; i < axes.Count; i++)
            //{
            //    Pen myPen = new Pen(axes[i].color, 2);
            //    e.Graphics.DrawLine(myPen,
            //                       new Point((int)axes[i].a.x + 200, (int)axes[i].a.y + 200),
            //                       new Point((int)axes[i].b.x + 200, (int)axes[i].b.y + 200));
            //}

            //for (int i = 0; i < points.Count; i++)
            //{
            //    RectangleF rect = new RectangleF(points[i].);
            //    Pen myPen = new Pen(edges[i].color, 3);
            //    e.Graphics.DrawLine(myPen,
            //                       new Point((int)edges[i].a.x + 200, (int)edges[i].a.y + 200),
            //                       new Point((int)edges[i].b.x + 200, (int)edges[i].b.y + 200));
            //}

        }

        /// <summary>
        /// Построить фигуру в аксонометрии по списку точек
        /// </summary>
        void showCube()
        {
            List<point> p = points;
            try
            {
                for (int i = 0; i < p.Count; i++)
                {
                double temp_x = p[i].x + 200;
                double temp_y = p[i].y + 200;
            
                for (int xx = (int)(temp_x - 3); xx < (int)(temp_x + 3); xx++)
                {
                    for (int yy = (int)(temp_y - 3); yy < (int)(temp_y + 3); yy++)
                    {
                        map.SetPixel(xx, yy, Color.Red);
                    }
                }
            }

                label1.Text = ((int)p[0].x).ToString() + " " + ((int)p[0].y).ToString() + " " + ((int)p[0].z).ToString();
                label2.Text = ((int)p[1].x).ToString() + " " + ((int)p[1].y).ToString() + " " + ((int)p[1].z).ToString();
                label3.Text = ((int)p[2].x).ToString() + " " + ((int)p[2].y).ToString() + " " + ((int)p[2].z).ToString();
                label4.Text = ((int)p[3].x).ToString() + " " + ((int)p[3].y).ToString() + " " + ((int)p[3].z).ToString();
                label5.Text = ((int)p[4].x).ToString() + " " + ((int)p[4].y).ToString() + " " + ((int)p[4].z).ToString();
                label6.Text = ((int)p[5].x).ToString() + " " + ((int)p[5].y).ToString() + " " + ((int)p[5].z).ToString();
                label7.Text = ((int)p[6].x).ToString() + " " + ((int)p[6].y).ToString() + " " + ((int)p[6].z).ToString();
                label8.Text = ((int)p[7].x).ToString() + " " + ((int)p[7].y).ToString() + " " + ((int)p[7].z).ToString();

                //label1.Text = points.Count.ToString();

            }
            catch (Exception e)
            {
                //label1.Text = ((int)p[0].x).ToString() + " " + ((int)p[0].y).ToString() + " " + ((int)p[0].z).ToString();
                //label2.Text = ((int)p[1].x).ToString() + " " + ((int)p[1].y).ToString() + " " + ((int)p[1].z).ToString();
                //label3.Text = ((int)p[2].x).ToString() + " " + ((int)p[2].y).ToString() + " " + ((int)p[2].z).ToString();
                //label4.Text = ((int)p[3].x).ToString() + " " + ((int)p[3].y).ToString() + " " + ((int)p[3].z).ToString();
                //label5.Text = ((int)p[4].x).ToString() + " " + ((int)p[4].y).ToString() + " " + ((int)p[4].z).ToString();
                //label6.Text = ((int)p[5].x).ToString() + " " + ((int)p[5].y).ToString() + " " + ((int)p[5].z).ToString();
                //label7.Text = ((int)p[6].x).ToString() + " " + ((int)p[6].y).ToString() + " " + ((int)p[6].z).ToString();
                //label8.Text = ((int)p[7].x).ToString() + " " + ((int)p[7].y).ToString() + " " + ((int)p[7].z).ToString();
            }

            pictureBox.Image = map;
        }

        /// <summary>
        /// Построить фигуру в перспективе по списку точек
        /// </summary>
        void showCubePersp()
        {
            List<point> p = points;
            try
            {
                for (int i = 0; i < p.Count; i++)
                {
                    double h = r * p[i].z + 1;

                    double temp_x = p[i].x / h + 200;
                    double temp_y = p[i].y / h + 200;
                    
                    for (int xx = (int)(temp_x - 3); xx < (int)(temp_x + 3); xx++)
                    {
                        for (int yy = (int)(temp_y - 3); yy < (int)(temp_y + 3); yy++)
                        {
                            map.SetPixel(xx, yy, Color.Red);
                        }
                    }
                }

                label1.Text = ((int)p[0].x).ToString() + " " + ((int)p[0].y).ToString() + " " + ((int)p[0].z).ToString();
                label2.Text = ((int)p[1].x).ToString() + " " + ((int)p[1].y).ToString() + " " + ((int)p[1].z).ToString();
                label3.Text = ((int)p[2].x).ToString() + " " + ((int)p[2].y).ToString() + " " + ((int)p[2].z).ToString();
                label4.Text = ((int)p[3].x).ToString() + " " + ((int)p[3].y).ToString() + " " + ((int)p[3].z).ToString();
                label5.Text = ((int)p[4].x).ToString() + " " + ((int)p[4].y).ToString() + " " + ((int)p[4].z).ToString();
                label6.Text = ((int)p[5].x).ToString() + " " + ((int)p[5].y).ToString() + " " + ((int)p[5].z).ToString();
                label7.Text = ((int)p[6].x).ToString() + " " + ((int)p[6].y).ToString() + " " + ((int)p[6].z).ToString();
                label8.Text = ((int)p[7].x).ToString() + " " + ((int)p[7].y).ToString() + " " + ((int)p[7].z).ToString();
            }
            catch (Exception e)
            {
                //label1.Text = ((int)p[0].x).ToString() + " " + ((int)p[0].y).ToString() + " " + ((int)p[0].z).ToString();
                //label2.Text = ((int)p[1].x).ToString() + " " + ((int)p[1].y).ToString() + " " + ((int)p[1].z).ToString();
                //label3.Text = ((int)p[2].x).ToString() + " " + ((int)p[2].y).ToString() + " " + ((int)p[2].z).ToString();
                //label4.Text = ((int)p[3].x).ToString() + " " + ((int)p[3].y).ToString() + " " + ((int)p[3].z).ToString();
                //label5.Text = ((int)p[4].x).ToString() + " " + ((int)p[4].y).ToString() + " " + ((int)p[4].z).ToString();
                //label6.Text = ((int)p[5].x).ToString() + " " + ((int)p[5].y).ToString() + " " + ((int)p[5].z).ToString();
                //label7.Text = ((int)p[6].x).ToString() + " " + ((int)p[6].y).ToString() + " " + ((int)p[6].z).ToString();
                //label8.Text = ((int)p[7].x).ToString() + " " + ((int)p[7].y).ToString() + " " + ((int)p[7].z).ToString();
            }

            pictureBox.Image = map;
        }

        void start()
        {
            //RotateXAxes(45);
            //RotateZAxes(90);
            //RotateYAxes(90);
            //RotateXAxes(90);

            //label26.Text = axes_points[1].x.ToString() + " " + axes_points[1].y.ToString() + " " + axes_points[1].z.ToString();
            //label27.Text = axes_points[2].x.ToString() + " " + axes_points[2].y.ToString() + " " + axes_points[2].z.ToString();
            //label28.Text = axes_points[3].x.ToString() + " " + axes_points[3].y.ToString() + " " + axes_points[3].z.ToString();
        }

        double[,] getOneMatr(int n)
        {
            double[,] matr = new double[4, 4];
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                for (int j = 0; j < matr.GetLength(1); j++)
                {
                    if (i == j)
                        matr[i, j] = 1;
                    else
                        matr[i, j] = 0;
                }
            }
            return matr;
        }

        void RotateX(double Angle) 
        {
            double[,] res = getOneMatr(4);

            double cos = Math.Cos((Angle * (Math.PI)) / 180);
            double sin = Math.Sin((Angle * (Math.PI)) / 180);
            res[1, 1] = cos;
            res[2, 1] = (-1) * sin;
            res[1, 2] = sin;
            res[2, 2] = cos;
            Rotate(res, Angle);
        }

        void RotateXAxes(double Angle)
        {
            double[,] res = getOneMatr(4);

            double cos = Math.Cos((Angle * (Math.PI)) / 180);
            double sin = Math.Sin((Angle * (Math.PI)) / 180);
            res[1, 1] = cos;
            res[2, 1] = (-1) * sin;
            res[1, 2] = sin;
            res[2, 2] = cos;
            RotateAxes(res, Angle);
        }

        void RotateY(double Angle)
        {
            double[,] res = getOneMatr(4);

            double cos = Math.Cos((Angle * (Math.PI)) / 180);
            double sin = Math.Sin((Angle * (Math.PI)) / 180);
            res[0, 0] = cos;
            res[2, 0] = sin;
            res[0, 2] = (-1) * sin;
            res[2, 2] = cos;
            Rotate(res, Angle);
        }

        void RotateYAxes(double Angle)
        {
            double[,] res = getOneMatr(4);

            double cos = Math.Cos((Angle * (Math.PI)) / 180);
            double sin = Math.Sin((Angle * (Math.PI)) / 180);
            res[0, 0] = cos;
            res[2, 0] = sin;
            res[0, 2] = (-1) * sin;
            res[2, 2] = cos;
            RotateAxes(res, Angle);
        }

        void RotateZ(double Angle) 
        {
            double[,] res = getOneMatr(4);

            double cos = Math.Cos((Angle * (Math.PI)) / 180);
            double sin = Math.Sin((Angle * (Math.PI)) / 180);

            res[0, 0] = cos;
            res[1, 0] = (-1) * sin;
            res[0, 1] = sin;
            res[1, 1] = cos;
            Rotate(res, Angle);
        }

        void RotateZAxes(double Angle)
        {
            double[,] res = getOneMatr(4);

            double cos = Math.Cos((Angle * (Math.PI)) / 180);
            double sin = Math.Sin((Angle * (Math.PI)) / 180);

            res[0, 0] = cos;
            res[1, 0] = (-1) * sin;
            res[0, 1] = sin;
            res[1, 1] = cos;
            RotateAxes(res, Angle);
        }

        void RotateAxes(double[,] T, double Angle)
        {
            for (int i = 0; i < axes_points.Count; i++)
            {
                double[] new_p = new double[4];
                new_p[0] = axes_points[i].x;
                new_p[1] = axes_points[i].y;
                new_p[2] = axes_points[i].z;
                new_p[3] = 1;

                double[] res = new double[4];
                for (int j = 0; j < 4; j++)
                    res[j] = 0;

                for (int k = 0; k < 4; k++)
                {
                    for (int l = 0; l < 4; l++)
                    {
                        res[k] += new_p[l] * T[l, k];
                    }
                }

                axes_points[i].x = res[0]; // можно убрать инт и преобразовывать только на выводе
                axes_points[i].y = res[1];
                axes_points[i].z = res[2];
            }
        }

        void Rotate(double[,] T, double Angle)
        {
            for (int i = 0; i < points.Count; i++)
            {
                double[] new_p = new double[4];
                new_p[0] = points[i].x;
                new_p[1] = points[i].y;
                new_p[2] = points[i].z;
                new_p[3] = 1;

                double[] res = new double[4];
                for (int j = 0; j < 4; j++)
                    res[j] = 0;

                for(int k = 0; k < 4; k++)
                {
                    for(int l = 0; l < 4; l++)
                    {
                        res[k] += new_p[l] * T[l, k];
                    }
                }

                points[i].x = res[0]; 
                points[i].y = res[1];
                points[i].z = res[2];
            }
        }

        void Translate()
        {
            double[,] T = getOneMatr(4);

            double dx = 0;
            double dy = 0;
            double dz = 0;

            if (textBox6.Text.Length != 0)
                dx = Convert.ToDouble(textBox6.Text);
            if (textBox5.Text.Length != 0)
                dy = Convert.ToDouble(textBox5.Text);
            if (textBox4.Text.Length != 0)
                dz = Convert.ToDouble(textBox4.Text);


            T[3, 0] = dx;
            T[3, 1] = dy;
            T[3, 2] = dz;

            for (int i = 0; i < points.Count; i++)
            {
                double[] new_p = new double[4];
                new_p[0] = points[i].x;
                new_p[1] = points[i].y;
                new_p[2] = points[i].z;
                new_p[3] = 1;

                double[] res = new double[4];
                for (int j = 0; j < 4; j++)
                    res[j] = 0;

                for (int k = 0; k < 4; k++)
                {
                    for (int l = 0; l < 4; l++)
                    {
                        res[k] += new_p[l] * T[l, k];
                    }
                }

                points[i].x = res[0]; //(int)res[0];
                points[i].y = res[1]; //(int)res[1];
                points[i].z = res[2]; //(int)res[2];
            }
        }

        void TranslateOn(double dx, double dy, double dz)
        {
            double[,] T = getOneMatr(4);

            T[3, 0] = dx;
            T[3, 1] = dy;
            T[3, 2] = dz;

            for (int i = 0; i < points.Count; i++)
            {
                double[] new_p = new double[4];
                new_p[0] = points[i].x;
                new_p[1] = points[i].y;
                new_p[2] = points[i].z;
                new_p[3] = 1;

                double[] res = new double[4];
                for (int j = 0; j < 4; j++)
                    res[j] = 0;

                for (int k = 0; k < 4; k++)
                {
                    for (int l = 0; l < 4; l++)
                    {
                        res[k] += new_p[l] * T[l, k];
                    }
                }

                points[i].x = res[0]; //(int)res[0];
                points[i].y = res[1]; //(int)res[1];
                points[i].z = res[2]; //(int)res[2];
            }
        }

        void Scale()
        {
            double[,] T = getOneMatr(4);

            double dx = 1;
            double dy = 1;
            double dz = 1;

            if (textBox9.Text.Length != 0)
                dx = Convert.ToDouble(textBox9.Text);
            if (textBox8.Text.Length != 0)
                dy = Convert.ToDouble(textBox8.Text);
            if (textBox7.Text.Length != 0)
                dz = Convert.ToDouble(textBox7.Text);

            double d1 = points[0].x;
            double d2 = points[0].y;
            double d3 = points[0].z;
            //TranslateOn(-200, -200, -200);
            TranslateOn(-points[0].x, -points[0].y, -points[0].z);

            T[0, 0] = dx;
            T[1, 1] = dy;
            T[2, 2] = dz;

            for (int i = 0; i < points.Count; i++)
            {
                double[] new_p = new double[4];
                new_p[0] = points[i].x;
                new_p[1] = points[i].y;
                new_p[2] = points[i].z;
                new_p[3] = 1;

                double[] res = new double[4];
                for (int j = 0; j < 4; j++)
                    res[j] = 0;

                for (int k = 0; k < 4; k++)
                {
                    for (int l = 0; l < 4; l++)
                    {
                        res[k] += new_p[l] * T[l, k];
                    }
                }

                points[i].x = res[0];
                points[i].y = res[1];
                points[i].z = res[2];
            }

            TranslateOn(d1, d2, d3);
        }

        void newCentre()
        {
            d1 = 0;
            d2 = 0;
            d3 = 0;

            for (int i = 0; i < points.Count; i++)
            {
                d1 += points[i].x;
                d2 += points[i].y;
                d3 += points[i].z;
            }

            d1 /= points.Count;
            d2 /= points.Count;
            d3 /= points.Count;
        }

        void ScaleOn(double dx, double dy, double dz)
        {
            double[,] T = getOneMatr(4);

            T[0, 0] = dx;
            T[1, 1] = dy;
            T[2, 2] = dz;

            for (int i = 0; i < points.Count; i++)
            {
                double[] new_p = new double[4];
                new_p[0] = points[i].x;
                new_p[1] = points[i].y;
                new_p[2] = points[i].z;
                new_p[3] = 1;

                double[] res = new double[4];
                for (int j = 0; j < 4; j++)
                    res[j] = 0;

                for (int k = 0; k < 4; k++)
                {
                    for (int l = 0; l < 4; l++)
                    {
                        res[k] += new_p[l] * T[l, k];
                    }
                }

                points[i].x = res[0];
                points[i].y = res[1];
                points[i].z = res[2];
            }
        }

        void ScaleCentre()
        {
            double dx = 1;
            double dy = 1;
            double dz = 1;

            if (textBox12.Text.Length != 0)
                dx = Convert.ToDouble(textBox12.Text);
            if (textBox11.Text.Length != 0)
                dy = Convert.ToDouble(textBox11.Text);
            if (textBox10.Text.Length != 0)
                dz = Convert.ToDouble(textBox10.Text);

            newCentre();
            TranslateOn(-d1, -d2, -d3);
            ScaleOn(dx, dy, dz);

            TranslateOn(d1, d2, d3);
        }

        void MirrorX()
        {
            double[,] res = getOneMatr(4);

            res[1, 1] = -1;
            Mirror(res);
        }

        void MirrorY()
        {
            double[,] res = getOneMatr(4);

            
            res[0, 0] = -1;
            Mirror(res);
        }

        void MirrorZ()
        {
            double[,] res = getOneMatr(4);

            res[2, 2] = -1;
            Mirror(res);
        }

        void Mirror(double[,] T)
        {
            for (int i = 0; i < points.Count; i++)
            {
                double[] new_p = new double[4];
                new_p[0] = points[i].x;
                new_p[1] = points[i].y;
                new_p[2] = points[i].z;
                new_p[3] = 1;

                double[] res = new double[4];
                for (int j = 0; j < 4; j++)
                    res[j] = 0;

                for (int k = 0; k < 4; k++)
                {
                    for (int l = 0; l < 4; l++)
                    {
                        res[k] += new_p[l] * T[l, k];
                    }
                }

                points[i].x = res[0];
                points[i].y = res[1];
                points[i].z = res[2];
            }
        }

        /// <summary>
        /// Аксонометрическое отображение выбранной из директории фигуры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            clean();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\Users\\m8951\\OneDrive\\Рабочий стол\\study\\computer graphics\\lab7\\WindowsFormsApp1\\WindowsFormsApp1\\bin\\Debug";//"c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    //using (StreamReader reader = new StreamReader(fileStream))
                    //{
                    //    fileContent = reader.ReadToEnd();
                    //}
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(' ');
                        try
                        {
                            if (parts.Length == 3) // точка
                            {
                                points.Add(new point(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2])));
                            }
                            if (parts.Length == 2) // грань
                            {
                                edges.Add(new edge(points[Convert.ToInt32(parts[0])], points[Convert.ToInt32(parts[1])]));
                            }
                        }
                        catch (Exception ex)
                        {
                            label26.Text = line;
                            label27.Text = parts.Length.ToString();
                        }

                    }
                }
            }

            isPerspectPr = false;
            makeCube(); // заполнение точек, осей и ребер
            //start();
            showCube();  // отрисовка. если другая проекция, то переделать вывод у
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clean();
            if (textBox1.Text.Length == 0)
                RotateX(30);
            else
                RotateX(Convert.ToDouble(textBox1.Text));

            if (isPerspectPr)
                showCubePersp();
            else
                showCube();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            clean();
            if (textBox2.Text.Length == 0)
                RotateY(30);
            else
                RotateY(Convert.ToDouble(textBox2.Text));

            if (isPerspectPr)
                showCubePersp();
            else
                showCube();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            clean();
            if (textBox3.Text.Length == 0)
                RotateZ(30);
            else
                RotateZ(Convert.ToDouble(textBox3.Text));

            if (isPerspectPr)
                showCubePersp();
            else
                showCube();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            clean();
            points = new List<point>();
            edges = new List<edge>();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            clean();
            Translate();

            if (isPerspectPr)
                showCubePersp();
            else
                showCube();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            clean();
            Translate();

            if (isPerspectPr)
                showCubePersp();
            else
                showCube();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            clean();
            Translate();

            if (isPerspectPr)
                showCubePersp();
            else
                showCube();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            clean();
            Scale();

            if (isPerspectPr)
                showCubePersp();
            else
                showCube();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            clean();
            MirrorX();

            if (isPerspectPr)
                showCubePersp();
            else
                showCube();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            clean();
            MirrorY();

            if (isPerspectPr)
                showCubePersp();
            else
                showCube();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            clean();
            MirrorZ();

            if (isPerspectPr)
                showCubePersp();
            else
                showCube();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            DrawLinePoint(e);
        }

        // куб перспектива
        /// <summary>
        /// Перспективное отображение выбранной из директории фигуры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click_1(object sender, EventArgs e)
        {
            clean();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\Users\\m8951\\OneDrive\\Рабочий стол\\study\\computer graphics\\lab7\\WindowsFormsApp1\\WindowsFormsApp1\\bin\\Debug";//"c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(' ');
                        try
                        {
                            if (parts.Length == 3) // точка
                            {
                                points.Add(new point(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2])));
                            }
                            if (parts.Length == 2) // грань
                            {
                                edges.Add(new edge(points[Convert.ToInt32(parts[0])], points[Convert.ToInt32(parts[1])]));
                            }
                        }
                        catch (Exception ex)
                        {
                            label26.Text = line;
                            label27.Text = parts.Length.ToString();
                        }

                    }
                }
            }

            isPerspectPr = true;
            makeCube();
            //start();
            showCubePersp();
        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        // масштабирование относительно центра
        private void button8_Click_1(object sender, EventArgs e)
        {
            clean();
            ScaleCentre();

            if (isPerspectPr)
                showCubePersp();
            else
                showCube();
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }

        // roteteXCentre
        /// <summary>
        /// Поворот вокрут своего центра по оси X
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {
            clean();
            if (textBox15.Text.Length == 0)
            {
                newCentre();
                TranslateOn(-d1, -d2, -d3);
                RotateX(30);
                TranslateOn(d1, d2, d3);
            }
            else
            {
                newCentre();
                TranslateOn(-d1, -d2, -d3);
                RotateX(Convert.ToDouble(textBox15.Text));
                TranslateOn(d1, d2, d3);
            }

            if (isPerspectPr)
                showCubePersp();
            else
                showCube();
        }

        // roteteYCentre
        /// <summary>
        /// Поворот вокруг центра по оси Y
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            clean();
            if (textBox14.Text.Length == 0)
            {
                newCentre();
                TranslateOn(-d1, -d2, -d3);
                RotateY(30);
                TranslateOn(d1, d2, d3);
            }
            else
            {
                newCentre();
                TranslateOn(-d1, -d2, -d3);
                RotateY(Convert.ToDouble(textBox14.Text));
                TranslateOn(d1, d2, d3);
            }

            if (isPerspectPr)
                showCubePersp();
            else
                showCube();
        }

        // roteteZCentre
        /// Поворот вокруг центра по оси Z
        private void button10_Click(object sender, EventArgs e)
        {
            clean();
            if (textBox13.Text.Length == 0)
            {
                newCentre();
                TranslateOn(-d1, -d2, -d3);
                RotateZ(30);
                TranslateOn(d1, d2, d3);
            }
            else
            {
                newCentre();
                TranslateOn(-d1, -d2, -d3);
                RotateZ(Convert.ToDouble(textBox13.Text));
                TranslateOn(d1, d2, d3);
            }

            if (isPerspectPr)
                showCubePersp();
            else
                showCube();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Установить точки на pictureBox1 для создания фигуры вращения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (points.Count == 0) //task2points
                // добавляем набор элементов 
                comboBox1.Items.AddRange(new string[] { "X", "Y", "Z" });

            point new_p = new point(e.X, e.Y, 0);
            points.Add(new_p);

            for (int x = e.X - 3; x < e.X + 3; x++)
            {
                for (int y = e.Y - 3; y < e.Y + 3; y++)
                {
                    map2.SetPixel(x, y, Color.Red);
                }
            }
            pictureBox1.Image = map2;

            edges = new List<edge>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                edges.Add(new edge(points[i], points[i + 1]));
            }
            label26.Text = points.Count.ToString();
        }

        /// <summary>
        /// Рисует грани фигуре вращения
        /// </summary>
        /// <param name="e"></param>
        public void DrawLineTask2(PaintEventArgs e)
        {

            for (int i = 0; i < edges.Count; i++) //task2edges
            {

                Pen myPen = new Pen(Color.Black, 2);
                e.Graphics.DrawLine(myPen,
                                   new Point((int)edges[i].a.x, (int)edges[i].a.y),
                                   new Point((int)edges[i].b.x, (int)edges[i].b.y));
            }
            label27.Text = edges.Count.ToString();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DrawLineTask2(e);
        }

        /// <summary>
        /// Построить фигуру вращения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button16_Click(object sender, EventArgs e)
        {
            
            int divisions = 5;
            string rotateAxis = "Y";
            if (textBox16.Text.Length > 0)
                divisions = Convert.ToInt32(textBox16.Text); //  количествo разбиений
            divi = divisions;
            if (comboBox1.Text.Length > 0)
                rotateAxis = comboBox1.SelectedItem.ToString(); // ось вращения

            task2startCount = points.Count;
            for (int i = 0; i < points.Count; i++)
                task2points.Add(new point(points[i].x, points[i].y, points[i].z)); //дублируем точки из списка точек, которые будем вращать

            for (int j = 0; j < divisions; j++)
            {
                double d1 = points[0].x;
                double d2 = points[0].y;
                double d3 = points[0].z;
                TranslateOn(-d1, -d2, -d3);
                RotateY(360 / divisions);
                TranslateOn(d1, d2, d3);


                for (int i = 0; i < points.Count; i++)
                    task2points.Add(new point(points[i].x, points[i].y, points[i].z));

            }

            //if (rotateAxis == "X")
            //{
            //    for(int i = 0; i < divisions; i++)
            //    {

            //    }
            //}

            //if (rotateAxis == "Y")
            //{
            //    for (int j = 0; j < divisions; j++)
            //    {
            //        RotateY(360 / divisions);
            //        for (int i = 0; i < points.Count; i++)
            //            task2points.Add(points[i]);

            //    }
            //}

            //if (rotateAxis == "Z")
            //{

            //}




            edges = new List<edge>();
            //for (int i = 0; i < points.Count; i++)
            //{
            //    for (int j = 0; j < divisions; j++)
            //    {
            //        //edges.Add(new edge(task2points[i * points.Count + j], task2points[i * points.Count + j + 1]));
            //        //edges.Add(new edge(task2points[i * points.Count + j], task2points[i * points.Count + j + points.Count]));

            //        //edges.Add(new edge(task2points[i * points.Count + j], task2points[i * points.Count + j + points.Count]));

            //        for(int k = 0; k < 3; k++)
            //        {

            //        }
            //    }
                
            //}

            for (int j = 0; j < divisions; j++)
            {
                for (int k = 0; k < points.Count - 1; k++)
                {
                    edges.Add(new edge(task2points[j * points.Count + k], task2points[j * points.Count + k + 1])); // 0 1 ;
                    edges.Add(new edge(task2points[j * points.Count + k + 1], task2points[j * points.Count + k + points.Count + 1])); // 1 
                    edges.Add(new edge(task2points[j * points.Count + k], task2points[j * points.Count + k + 1 + points.Count]));
                }
            }




            //else
            //{
            //    for (int j = 0; j < divisions; j++)
            //    {
            //        RotateY(360 / divisions);
            //        for (int i = 0; i < points.Count; i++)
            //            task2points.Add(points[i]);

            //    }
            //}

            points = task2points;
            //label26.Text = points.Count.ToString();
            label27.Text = task2points.Count.ToString();
            showCube();
        }

        /// <summary>
        /// Очистить pictureBox1 - поле для задания фигуры вращения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button17_Click(object sender, EventArgs e)
        {
            points = new List<point>();
            edges = new List<edge>();
            task2edges = new List<edge>();
            task2points = new List<point>();
            Graphics graphics = Graphics.FromImage(map2);
            graphics.Clear(Color.White);
            graphics = Graphics.FromImage(map2);// mapPrim);
            graphics.Clear(Color.White);
            graphics.Clear(Color.White);

            map2 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = map2;
        }

        /// <summary>
        /// Запись данных в файл в формате .obj
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button18_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "OBJ files|*.obj";
            saveFileDialog1.Title = "Save an OBJ File";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog1.OpenFile()))
                {
                    for (int i = 0; i < task2points.Count; i++)
                    {
                        writer.WriteLine("v " + task2points[i].x.ToString() + " " +  task2points[i].y.ToString() + " " + task2points[i].z.ToString());
                    }

                    //for (int i = 0; i < task2points.Count - 2; i++)
                    //{
                    //    //writer.WriteLine("f " + (i).ToString() + " " + (i + 1).ToString() + " " + (i + task2startCount).ToString());
                    //    writer.WriteLine("f " + (i).ToString() + " " + (i + 1).ToString() + " " + (i + 2).ToString());
                    //}

                    for (int j = 0; j < divi; j++)
                    {
                        for (int k = 0; k < task2startCount; k++)
                        {
                            // 0 1 4
                            //writer.WriteLine("f " + (j * task2startCount + k).ToString() + " " + ((j * task2startCount + k + 1)).ToString() + " " + ((j * task2startCount + task2startCount + 1)).ToString());
                            //writer.WriteLine("f " + (j * task2startCount + k).ToString() + " " + ((j * task2startCount + task2startCount)).ToString() + " " + ((j * task2startCount + task2startCount + 1)).ToString());

                            writer.WriteLine("f " + (j * task2startCount + k + 1).ToString() + " " + ((j * task2startCount + k + 1 + 1)).ToString() + " " + ((j * task2startCount + task2startCount + 1 + 1)).ToString());
                            writer.WriteLine("f " + (j * task2startCount + k + 1).ToString() + " " + ((j * task2startCount + + k + task2startCount + 1)).ToString() + " " + ((j * task2startCount + task2startCount + 1 + 1)).ToString());

                            //writer.WriteLine("f " + (j * task2startCount + k + 1).ToString() + " " + ((j * task2startCount + k + 1 + 1) % (divi * task2startCount)).ToString() + " " + ((j * task2startCount + task2startCount + 1 + 1) % (divi * task2startCount)).ToString());
                            //writer.WriteLine("f " + (j * task2startCount + k + 1).ToString() + " " + ((j * task2startCount + task2startCount + 1) % (divi * task2startCount)).ToString() + " " + ((j * task2startCount + task2startCount + 1 + 1) % (divi * task2startCount)).ToString());

                        }

                        //for (int k = 0; k < task2startCount - 2; k++)
                        //{
                        //    // 5 4 1
                        //    writer.WriteLine("f " + (j * task2startCount + k + task2startCount + 2).ToString() + " " + (j * task2startCount + k + 1).ToString() + " " + (j * task2startCount + k + task2startCount + 1).ToString());
                        //    //edges.Add(new edge(task2points[j * points.Count + k], task2points[j * points.Count + k + 1])); // 0 1 ;
                        //    //edges.Add(new edge(task2points[j * points.Count + k + 1], task2points[j * points.Count + k + points.Count + 1])); // 1 4
                        //    //edges.Add(new edge(task2points[j * points.Count + k], task2points[j * points.Count + k + 1 + points.Count])); // 0 4
                        //}
                    }
                }
            }


        }

        /// <summary>
        /// Загрузить из obj файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button19_Click(object sender, EventArgs e)
        {
            points = new List<point>();
            edges = new List<edge>();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\Users\\m8951\\OneDrive\\Рабочий стол\\study\\computer graphics\\lab7\\WindowsFormsApp1\\WindowsFormsApp1\\bin\\Debug";//"c:\\";
                openFileDialog.Filter = "OBJ files|*.obj";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(' ');
                        try
                        {
                            if (parts[0] == "v") // точка
                            {
                                points.Add(new point(Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]), Convert.ToInt32(parts[3])));
                            }
                            if (parts[0] == "f") // грань
                            {
                                edges.Add(new edge(points[Convert.ToInt32(parts[1])], points[Convert.ToInt32(parts[2])]));
                                edges.Add(new edge(points[Convert.ToInt32(parts[1])], points[Convert.ToInt32(parts[3])]));
                                edges.Add(new edge(points[Convert.ToInt32(parts[3])], points[Convert.ToInt32(parts[2])]));
                            }
                            label26.Text = line;
                        }
                        catch (Exception ex)
                        {
                            label26.Text = line;
                            label27.Text = parts.Length.ToString();
                        }

                    }
                }
                
            }

            isPerspectPr = false;
            makeCube(); // заполнение точек, осей и ребер
            showCube();
        }
    }
}
