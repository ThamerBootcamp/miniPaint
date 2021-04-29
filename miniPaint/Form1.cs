using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace miniPaint
{
    public partial class Form1 : Form
    {

        private bool canMove = false;

        List<Shape> shapes;
        bool isPointerMode = false;

        private Shape currentShape = null;
        private Shape tempShape = null;

        private Shape activeShape = null;

        private float penSize = 4;
        private static Color penColor = System.Drawing.Color.Black;
        private DashStyle penStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        //private Pen p = new Pen(Brushes.Black, 4);

        private string selectedShape = "Circle";

        public Point start;
        public Point end;
        private bool isTempPaint=false;

        public Point delta;
        public Form1()
        {
            InitializeComponent();
            this.shapes = new List<Shape>();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
           

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void drawingBoard_Paint(object sender, PaintEventArgs e)
        {
            //base.OnPaint(e);
            Graphics g = e.Graphics;

            foreach (var shape in this.shapes)
            {
                if (isPointerMode && shape.Equals(activeShape))
                {
                    shape.draw(g, true);
                }
                else
                {
                    shape.draw(g);
                }

            }
            if (isTempPaint)
                tempShape.draw(g);

            this.Invalidate();


        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
    

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            drawingBoard.Paint += new PaintEventHandler(drawingBoard_Paint);

        }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isPointerMode = false;

            selectedShape = "Circle";
        }

        private void drawingBoard_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isPointerMode)
            {
                if (selectedShape == "Rectangle")
                {
                    currentShape = new SRectangle();
                }
                else if (selectedShape == "Circle")
                {
                    currentShape = new Circle();
                }
                else
                {
                    currentShape = new Line();
                }

                this.Invalidate();
            }
            else if (activeShape != null)
            {
                canMove = true;
            }
            start = e.Location;

            drawingBoard.Invalidate();

        }

        private void drawingBoard_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isPointerMode)
            {
                isTempPaint = false;
                end = e.Location;
       
                //befor fix for the lines
                currentShape.Start = start;
                currentShape.End = end;

                Point[] fixedPoints = fixPoints(start, end);

                start = fixedPoints[0];
                end = fixedPoints[1];

                int width = Math.Abs(end.X - start.X);
                int height = Math.Abs(end.Y - start.Y);
                Rectangle rect =new Rectangle(start.X, start.Y, width, height);
                currentShape.rect = rect;
                currentShape.brush = Brushes.Black;
                currentShape.pen = new Pen(currentShape.brush, penSize);
                currentShape.pen.DashStyle = penStyle;

                

                //}

                this.shapes.Add(currentShape);
                drawingBoard.Invalidate();
                //drawingBoard.Refresh();

            }
            //else 
            canMove = false;
            drawingBoard.Invalidate();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
           
         
        }

        private void drawingBoard_MouseMove(object sender, MouseEventArgs e)
        {
             
           if (activeShape != null && canMove ) 
            {
                if (activeShape.Top.Contains(start))
                {
                    activeShape.Start.Y -= start.Y - e.Y;

                    activeShape.rect.Y -= start.Y - e.Y  ;
                    activeShape.rect.Height +=   start.Y- e.Y;
                    start = new Point(e.X, e.Y); 
                }else if (activeShape.Right.Contains(start))
                {
                    activeShape.End.X += e.X - start.X;
                    activeShape.rect.Width += e.X -start.X  ;
                    start = new Point(e.X, e.Y);
                }else if (activeShape.Left.Contains(start))
                {
                    activeShape.Start.X += e.X - start.X;

                    activeShape.rect.X +=  e.X- start.X;
                   activeShape.rect.Width -=  e.X - start.X;
                   start = new Point(e.X, e.Y);
                }
                else if (activeShape.Down.Contains(start))
                {
                    activeShape.End.Y += e.Y - start.Y;

                    activeShape.rect.Height +=  e.Y - start.Y;
                    start = new Point(e.X, e.Y);
                }
                else
                {
                    activeShape.Start.X += e.X - start.X;
                    activeShape.Start.Y += e.Y - start.Y;
                    activeShape.End.X += e.X - start.X;
                    activeShape.End.Y += e.Y - start.Y;

                    activeShape.rect.X += e.X - start.X;
                    activeShape.rect.Y += e.Y - start.Y;
                    start = new Point(e.X, e.Y);
                }
                drawingBoard.Invalidate();
            }
        }

        public Point[] fixPoints(Point start, Point end)
        {
            int w = Math.Abs(end.X - start.X);
            int h = Math.Abs(end.Y - start.Y);
            Point fStart;
            Point fEnd;

            if (end.X > start.X)
            {
                if (end.Y < start.Y)
                {
                    fStart = new Point((end.X - w), end.Y);
                    fEnd = new Point((start.X + w), start.Y);
                }
                else
                {
                    fStart = start;
                    fEnd = end;
                }
            }
            else
            {
                if (end.Y > start.Y)
                {
                    fStart = new Point((start.X - w), start.Y);
                    fEnd = new Point((end.X + w), end.Y);
                }
                else
                {
                    fStart = end;
                    fEnd = start;
                }
            }
            
            return new Point[] {fStart,fEnd }; 

        }

        private void drawingBoard_Click(object sender, EventArgs e)
        {

        }

        private void drawingBoard_MouseClick(object sender, MouseEventArgs e)
        {
            if (isPointerMode )
            {
                foreach (var shape in shapes)
                {
                    if (shape.IsInside(e.Location))
                    {
                        activeShape = shape;
                        break;
                        //shape.pen.Brush= Brushes.Red;
                    }
                    else
                    {
                        activeShape = null;
                    }
                }
            }
            drawingBoard.Invalidate();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            isPointerMode = true;
        }

        private void squareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isPointerMode = false;
            selectedShape = "Rectangle";
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isPointerMode = false;
            selectedShape = "Line";
        }

        private void dottedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.penStyle = System.Drawing.Drawing2D.DashStyle.Dot;

        }

        private void solidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.penStyle = System.Drawing.Drawing2D.DashStyle.Solid;

        }

        private void dashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.penStyle = System.Drawing.Drawing2D.DashStyle.Dash;

        }
    }
    public abstract class Shape
    {
        public System.Drawing.Rectangle rect;
        public System.Drawing.Rectangle Top;
        public System.Drawing.Rectangle Left;
        public System.Drawing.Rectangle Right;
        public System.Drawing.Rectangle Down;
        public Brush brush;
        public Pen pen;
        public int selectOffset = 10;
        public int DotW = 8;
        public int DotH = 8;
        public Point Start;
        public Point End;

        public abstract void draw(Graphics g, bool isActive = false);
        public bool IsInside(Point p)
        {
            return (p.X >= rect.X &&
                p.X <= (rect.X + rect.Width) &&
                p.Y >= rect.Y &&
                p.Y <= (rect.Y + rect.Height));
        }
    }
    public class Circle :Shape
    {
        
        public override void draw(Graphics g, bool isActive)
        {

            g.FillEllipse(this.brush, this.rect);
            if (isActive)
            {
                Pen p = new Pen(Brushes.Gray, 2);
                Pen smallRrectP = new Pen(Brushes.Red, 3);

                Brush fillBrush = Brushes.Red;
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                Rectangle selectRect = new Rectangle(rect.X -selectOffset, rect.Y - selectOffset, rect.Width+ (2*selectOffset), rect.Height+ (2*selectOffset));

                g.DrawRectangle(p, selectRect);


                System.Drawing.Rectangle centerDot = new System.Drawing.Rectangle((rect.X +rect.Width/2)- DotW/ 2, rect.Y + (rect.Height/ 2) - DotH/2, DotW, DotH);

                g.FillEllipse(fillBrush, centerDot);  //(start.X +(width - 10)/2, start.Y+ ( height+ 0)/2, 10, 10)); //center point

                this.Top = new Rectangle((selectRect.X - DotW / 2) + selectRect.Width / 2, selectRect.Y - DotH / 2, DotW, DotH);
                g.FillRectangle(fillBrush, Top); //Top

                this.Left = new Rectangle((selectRect.X - DotW / 2), (selectRect.Y - DotH / 2) + selectRect.Height / 2, DotW, DotH);
                g.FillRectangle(fillBrush, (selectRect.X - DotW / 2), (selectRect.Y - DotH / 2) + selectRect.Height / 2, DotW, DotH); //Left

                this.Right = new Rectangle((selectRect.X - DotW / 2) + selectRect.Width, (selectRect.Y - DotH / 2) + selectRect.Height / 2, DotW, DotH);
                g.FillRectangle(fillBrush, Right); //Ritght

                this.Down = new Rectangle((selectRect.X - DotW / 2) + selectRect.Width / 2, (selectRect.Y - DotH / 2) + selectRect.Height, DotW, DotH);
                g.FillRectangle(fillBrush, Down); //Down                                                                                                                                   

            }
        }

    }
    public class SRectangle : Shape

    {
        public override void draw(Graphics g, bool isActive)
        {

            g.FillRectangle(this.brush, this.rect);
            if (isActive)
            {
                Pen p = new Pen(Brushes.Gray, 2);
                Pen smallRrectP = new Pen(Brushes.Red, 3);

                Brush fillBrush = Brushes.Red;
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                Rectangle selectRect = new Rectangle(rect.X - selectOffset, rect.Y - selectOffset, rect.Width + (2 * selectOffset), rect.Height + (2 * selectOffset));

                g.DrawRectangle(p, selectRect);

        

                System.Drawing.Rectangle centerDot = new System.Drawing.Rectangle((rect.X + rect.Width / 2) - DotW / 2, rect.Y + (rect.Height / 2) - DotH / 2, DotW, DotH);

                g.FillEllipse(fillBrush, centerDot);  //(start.X +(width - 10)/2, start.Y+ ( height+ 0)/2, 10, 10)); //center point

                this.Top = new Rectangle((selectRect.X - DotW / 2) + selectRect.Width / 2, selectRect.Y - DotH / 2, DotW, DotH);
                g.FillRectangle(fillBrush, Top); //Top

                this.Left = new Rectangle((selectRect.X -DotW / 2), (selectRect.Y - DotH / 2) + selectRect.Height / 2, DotW, DotH);
                g.FillRectangle(fillBrush, (selectRect.X - DotW / 2), (selectRect.Y - DotH / 2) + selectRect.Height / 2, DotW, DotH); //Left

                this.Right = new Rectangle((selectRect.X - DotW / 2) + selectRect.Width, (selectRect.Y - DotH / 2) + selectRect.Height / 2, DotW, DotH);
                g.FillRectangle(fillBrush, Right); //Ritght

                this.Down = new Rectangle((selectRect.X - DotW / 2) + selectRect.Width / 2, (selectRect.Y - DotH / 2) + selectRect.Height, DotW, DotH);
                g.FillRectangle(fillBrush, Down); //Down                                                                                                                                   

            }
        }
    }
    public class Line : Shape
    {
    
        public override void draw(Graphics g, bool isActive)
        {
            g.DrawLine(this.pen,this.Start,this.End );
            if (isActive)
            {
                Pen p = new Pen(Brushes.Gray, 2);
                Pen smallRrectP = new Pen(Brushes.Red, 3);

                Brush fillBrush = Brushes.Red;
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                Rectangle selectRect = new Rectangle(rect.X - selectOffset, rect.Y - selectOffset, rect.Width + (2 * selectOffset), rect.Height + (2 * selectOffset));

                g.DrawRectangle(p, selectRect);



                System.Drawing.Rectangle centerDot = new System.Drawing.Rectangle((rect.X + rect.Width / 2) - DotW / 2, rect.Y + (rect.Height / 2) - DotH / 2, DotW, DotH);

                g.FillEllipse(fillBrush, centerDot);  //(start.X +(width - 10)/2, start.Y+ ( height+ 0)/2, 10, 10)); //center point

                this.Top = new Rectangle((selectRect.X - DotW / 2) + selectRect.Width / 2, selectRect.Y - DotH / 2, DotW, DotH);
                g.FillRectangle(fillBrush, Top); //Top

                this.Left = new Rectangle((selectRect.X - DotW / 2), (selectRect.Y - DotH / 2) + selectRect.Height / 2, DotW, DotH);
                g.FillRectangle(fillBrush, (selectRect.X - DotW / 2), (selectRect.Y - DotH / 2) + selectRect.Height / 2, DotW, DotH); //Left

                this.Right = new Rectangle((selectRect.X - DotW / 2) + selectRect.Width, (selectRect.Y - DotH / 2) + selectRect.Height / 2, DotW, DotH);
                g.FillRectangle(fillBrush, Right); //Ritght

                this.Down = new Rectangle((selectRect.X - DotW / 2) + selectRect.Width / 2, (selectRect.Y - DotH / 2) + selectRect.Height, DotW, DotH);
                g.FillRectangle(fillBrush, Down); //Down                                                                                                                                   

            }
        }
    }
    public class DrawingBoard : Panel
    {
        public DrawingBoard()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
        }
    }
}
