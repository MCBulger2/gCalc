using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpftest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point Origin;
        Point DragStart;
        Point Offset;
        bool IsDragging = false;

        Line xAxis = new Line();
        Line yAxis = new Line();

        public MainWindow()
        {
            InitializeComponent();

            Origin = new Point(50, 50);

            xAxis.Stroke = SystemColors.WindowFrameBrush;
            Canvas.Children.Add(xAxis);
            yAxis.Stroke = SystemColors.WindowFrameBrush;
            Canvas.Children.Add(yAxis);
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            DrawAxes();
        }

        private void Update()
        {
            DrawAxes();
        }

        private void DrawAxes()
        {
            double y = Math.Min(Math.Max(Origin.Y, 1), Canvas.ActualHeight);
            xAxis.X1 = 0;
            xAxis.X2 = Canvas.ActualWidth;
            xAxis.Y1 = y;
            xAxis.Y2 = y;
            xAxis.InvalidateVisual();
            
            double x = Math.Min(Math.Max(Origin.X, 1), Canvas.ActualWidth - 2);
            yAxis.X1 = x;
            yAxis.X2 = x;
            yAxis.Y1 = 0;
            yAxis.Y2 = Canvas.ActualHeight;
            yAxis.InvalidateVisual();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Offset = e.GetPosition(this);
            Point dragEnd = Offset;

            double deltaX = dragEnd.X - DragStart.X;
            double deltaY = dragEnd.Y - DragStart.Y;

            //if (IsDragging /*&& DistancesFromCursor.Min() > MIN_DISTANCE_FROM_CURSOR*/)
            if (IsDragging)
            {
                Console.WriteLine("here");
                double[] scale = {50,50};
                //lock (Buffer.Scale)
                //{
                //    scale = Buffer.Scale;
                //}

                //Origin.X += deltaX;
                //Origin.Y += deltaY;

                Origin = TranslatePoint(Origin, deltaX, deltaY, 1, 1);

                //for (int i = 0; i < CurrentPoints.Count; i++)
                //{
                //    CurrentPoints[i] = TranslatePoints(CurrentPoints[i], deltaX, deltaY, 1, 1);
                //}

                //UpdateWindow();

                DragStart = dragEnd;

                //InvalidateCanvas(false);
                //InvalidateCanvas();
                Update();
            }
            else
            {
                //InvalidateCanvas(false);
            }

            // TODO - we shouldn't have to recalculate all the points for a simple transformation
            // do some linear algebra on the existing points and redraw them for efficiency
            //InvalidateCanvas(false);
            //Canvas.Invalidate();

        }

        private Point TranslatePoint(Point p, double xOffset, double yOffset, double scaleX, double scaleY)
        {
            System.Windows.Media.Matrix transformationMatrix = new System.Windows.Media.Matrix(scaleX, 0, 0, scaleX, xOffset, yOffset);
            System.Windows.Point point = new System.Windows.Point(p.X, p.Y);

            var mappedPoint = System.Windows.Point.Multiply(point, transformationMatrix);
            return new Point((int)mappedPoint.X, (int)mappedPoint.Y);
            //return new Point((int)(p.X + xOffset), (int)(p.Y + yOffset));
        }

        private Point[] TranslatePoints(Point[] points, double xOffset, double yOffset, double scaleX, double scaleY)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = TranslatePoint(points[i], xOffset, yOffset, scaleX, scaleY);
                }

                //Canvas.Invalidate();
                return points;
            }
            return null;
        }

        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("start");
            DragStart = e.GetPosition(Canvas);
            IsDragging = true;
        }

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("hereen d");
            IsDragging = false;
        }
    }
}
