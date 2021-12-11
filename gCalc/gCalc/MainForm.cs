using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NCalc2;

namespace gCalc
{
    public partial class MainForm : Form
    {
        Point Origin;
        Point Offset;
        List<Point[]> CurrentPoints;
        double[] DistancesFromCursor;
        const double MIN_DISTANCE_FROM_CURSOR = 5;
        bool IsDragging;
        bool IsGliding;
        Point DragStart;
        readonly Configuration Config = new Configuration();
        readonly List<Calculator> Calculators = new List<Calculator>();
        readonly List<bool> shouldDraw = new List<bool>();
        readonly List<bool> visible = new List<bool>();
        int FunctionCount = 0;
        List<CheckBox> visibleCheckboxes = new List<CheckBox>();


        public MainForm()
        {
            InitializeComponent();

            Canvas.MouseWheel += Canvas_MouseWheel;

            CurrentPoints = new List<Point[]>();
            AddFunction();
        }

        private void FunctionTextbox_Changed(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            ReplaceFunction(tb.TabIndex, tb.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Origin = new Point(Canvas.Width / 2, Canvas.Height / 2);
            Offset = Origin;

            UpdateWindow();

            InvalidateCanvas();
        }

        private void AddFunction()
        {
            int index = FunctionCount;
            CheckBox newCheckbox = new CheckBox()
            {
                Location = new Point(0, 25 * index),
                Size = new Size(20, 20),
                Name = $"functionCB-{index}",
                Checked = true
            };
            newCheckbox.CheckedChanged += VisibilityCheckbox_Changed;
            visibleCheckboxes.Add(newCheckbox);

            TextBox newTextbox = new TextBox()
            {
                Location = new Point(newCheckbox.Width, 25 * index),
                Size = new Size(AddButton.Width - newCheckbox.Width - 10, 20),
                Name = $"function{index}",
                TabIndex = index
            };
            newTextbox.TextChanged += FunctionTextbox_Changed;
            FunctionsPanel.Controls.Add(newTextbox);
            FunctionsPanel.Controls.Add(newCheckbox);
            AddButton.Location = new Point(AddButton.Location.X, newTextbox.Location.Y + newTextbox.Height + 5);
            FunctionCount++;

            shouldDraw.Add(false);
            visible.Add(newCheckbox.Checked);
            CurrentPoints.Add(new Point[0]);

            Calculator newCalc = new Calculator(null, index);
            newCalc.ComputationCompleted += HandleComputationCompleted;
            newCalc.InvalidFunction += HandleInvalidFunction;
            var t = new Thread(new ThreadStart(newCalc.Run));
            Calculators.Add(newCalc);
            t.Start();
        }

        private void HandleInvalidFunction(int id)
        {
            shouldDraw[id] = false;
        }

        private void VisibilityCheckbox_Changed(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            int index = int.Parse(cb.Name.Split('-')[1]);
            visible[index] = cb.Checked;
            Canvas.Invalidate();
            ToggleAllCheckbox.CheckState = CheckState.Indeterminate;
        }

        private void HandleComputationCompleted(int id)
        {
            shouldDraw[id] = true;
            Canvas.Invalidate();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            // Create a local version of the graphics object for the PictureBox.
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Canvas.Image = null;

            double[] scale = Buffer.GetScale();
            Window window = Buffer.GetWindow();
            List<FunctionPoints> functionsPoints = Buffer.GetFunctions();

            int power = (int)Math.Floor(Math.Log10(window.Domain.Difference));
            DrawGrid(g, scale, window, power, Config.Colors["Grid"]);
            DrawGrid(g, scale, window, power-1, Config.Colors["Subgrid"]);
            DrawAxes(g);
            //DrawBounds(g, scale);
            DrawPoints(g, scale, functionsPoints);
            DrawHoveredValues(g, scale, window);
        }

        private void DrawPoints(Graphics g, double[] scale, List<FunctionPoints> functionsPoints)
        {
            for (int j = 0; j < functionsPoints.Count; j++)
            {
                var newPoints = functionsPoints[j].Points;
                if (newPoints != null && newPoints.Length > 1 && shouldDraw[j] && visible[j])
                {
                    Point[] screenPoints = new Point[newPoints.Length];
                    // Calculators[0].valid.WaitOne();
                    
                    if (functionsPoints[j].IsValid)
                    {
                        for (int i = 0; i < newPoints.Length; i++)
                        {
                            screenPoints[i] = MapFunctionSpaceToScreenSpace(newPoints[i], scale);
                        }
                        CurrentPoints[j] = screenPoints;
                    }
                    else
                    {
                        screenPoints = CurrentPoints[j];
                    }

                    Console.WriteLine(functionsPoints[j].IsValid);

                    if (screenPoints.Length > 2)
                    {
                        if (!Config.Colors.TryGetValue("Function" + j, out Pen pen))
                        {
                            Config.Colors.Add("Function" + j, Config.Colors["Function" + (j % 6)]);
                        }
                        g.DrawLines(Config.Colors["Function" + j], screenPoints);
                    }
                }

            }
        }

        private Tuple<double[], PointRange[][]> GetDistancesFromCursor(double[] scale, Window window)
        {
            double[] distances = new double[Calculators.Count];
            PointRange[][] ranges = new PointRange[Calculators.Count][];

            for (int i = 0; i < Calculators.Count; i++)
            {
                if (!visible[i])
                {
                    distances[i] = double.MaxValue;
                    ranges[i] = new PointRange[2] { new PointRange(double.MinValue, double.MaxValue), new PointRange(double.MinValue, double.MaxValue) };
                    continue;
                }

                FPoint p = MapScreenSpaceToFunctionSpace(DragStart, scale);
                p.Y = Calculators[i].CalculatePoint(p.X);

                Point mappedPoint = MapFunctionSpaceToScreenSpace(p, scale);
                distances[i] = Math.Sqrt(Math.Pow(DragStart.X - mappedPoint.X, 2) + Math.Pow(DragStart.Y - mappedPoint.Y, 2));
                ranges[i] = new PointRange[2]{ new PointRange(mappedPoint.X, DragStart.X), new PointRange(mappedPoint.Y, DragStart.Y) };
            }

            return Tuple.Create(distances, ranges);
        }

        private void DrawHoveredValues(Graphics g, double[] scale, Window window)
        {
            (double[] distances, PointRange[][] ranges) = GetDistancesFromCursor(scale, window);
            DistancesFromCursor = distances;
            int closestFunction = Array.IndexOf(distances, distances.Min());

            FPoint[] points = new FPoint[Calculators.Count];
            if (closestFunction != -1)
            {
                var p0 = new Point((int)(Offset.X - ranges[closestFunction][0].Difference), (int)(ranges[closestFunction][1].Difference));
                FPoint p = MapScreenSpaceToFunctionSpace(Offset, scale);
                p.Y = Calculators[closestFunction].CalculatePoint(p.X);
                if (p.Y < window.Range.End && p.Y > window.Range.Start && distances[closestFunction] < MIN_DISTANCE_FROM_CURSOR && IsDragging)
                {
                    points[closestFunction] = p;
                    Point mappedPoint = MapFunctionSpaceToScreenSpace(p, scale);
                    g.DrawString(p.ToString(), Config.Font, Brushes.Black, mappedPoint);
                    mappedPoint.X -= 2;
                    mappedPoint.Y -= 2;
                    g.DrawRectangle(Config.Colors["Function" + closestFunction],
                        new Rectangle(mappedPoint, new Size(4, 4)));

                    if (p.Y < 0.01 && p.Y > -0.01)
                    {
                        mappedPoint.X -= 2;
                        g.DrawRectangle(Config.Colors["Function" + closestFunction],
                        new Rectangle(new Point(mappedPoint.X, Origin.Y-5), new Size(10, 10)));
                    }
                    
                }
            }

            for (int i = 0; i < points.Length; i++)
            {
                for (int j = i + 1; j < points.Length; j++)
                {
                    if (points[i] != null && points[j] != null && points[i].Y < points[j].Y + 0.05 && points[i].Y > points[j].Y - 0.05)
                    {
                        Point mappedPoint = MapFunctionSpaceToScreenSpace(points[i], scale);
                        g.DrawRectangle(Config.Colors["Function" + i],
                            new Rectangle(mappedPoint, new Size(10, 10)));
                        g.DrawString(points[i].ToString(), Config.Font, Brushes.Black, mappedPoint);
                    }
                }
            }
        }

        private Point MapFunctionSpaceToScreenSpace(FPoint p, double[] scale)
        {
            System.Windows.Media.Matrix transformationMatrix = new System.Windows.Media.Matrix((float)scale[0], 0, 0, (float)scale[1], Origin.X, Origin.Y);
            System.Windows.Point point = new System.Windows.Point(p.X, -p.Y);

            var mappedPoint = System.Windows.Point.Multiply(point, transformationMatrix);
            return new Point((int)mappedPoint.X, (int)mappedPoint.Y);
        }

        private FPoint MapScreenSpaceToFunctionSpace(Point p, double[] scale)
        {
            System.Windows.Media.Matrix transformationMatrix = new System.Windows.Media.Matrix((float)scale[0], 0, 0, (float)scale[1], Origin.X, Origin.Y);
            transformationMatrix.Invert(); // Invert the transformation since we're going backwards
            System.Windows.Point point = new System.Windows.Point(p.X, p.Y);

            var mappedPoint = System.Windows.Point.Multiply(point, transformationMatrix);
            return new FPoint(mappedPoint.X, -mappedPoint.Y);
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

                Canvas.Invalidate();
                return points;
            }
            return null;
        }

        private void DrawAxes(Graphics g)
        {
            // X axis
            Pen greenPen = new Pen(Color.FromArgb(255, 0, 0, 0), 3);
            int y = Math.Min(Math.Max(Origin.Y, 1), Canvas.Height - 2);
            g.DrawLine(greenPen, 0, y,
                Canvas.Width, y);

            // Y axis
            int x = Math.Min(Math.Max(Origin.X, 1), Canvas.Width - 2);
            g.DrawLine(greenPen, x, Canvas.Height,
                x, 0);
        }

        public static int RoundOff(int i, int nearest)
        {
            return i - (i % nearest);
        }

        private void DrawGrid(Graphics g, double[] scale, Window window, int power, Pen pen)
        {
            int gap = (int)(Math.Pow(10, power));
            
            double offset = window.Domain.Start % gap;

            double start = window.Domain.Start - offset;
            double end = window.Domain.End;

            for (double i = start; i < end; i += gap)
            {
                var p = MapFunctionSpaceToScreenSpace(new FPoint(i, i), scale);
                int y = Math.Min(Math.Max(Origin.Y + 2, 2), Canvas.Height - 15);
                g.DrawString(i.ToString(), Config.Font, Brushes.Black, new Point(p.X +2 , y));
                g.DrawLine(pen, p.X, 0,
                        p.X, Canvas.Height);
            }

            offset = window.Range.Start % gap;

            start = window.Range.Start - offset;
            end = window.Range.End;

            for (double i = start; i < end; i += gap)
            {
                var p = MapFunctionSpaceToScreenSpace(new FPoint(i, i), scale);
                int x = Math.Min(Math.Max(Origin.X + 2, 2), Canvas.Width - 15);
                if (i != 0)
                    g.DrawString(i.ToString(), Config.Font, Brushes.Black, new Point(x, p.Y));
                g.DrawLine(pen, 0, p.Y,
                        Canvas.Width, p.Y);
            }
        }

        private void DrawOffset(Graphics g)
        {

            // X axis
            g.DrawLine(Pens.Green, Canvas.Left, Offset.Y,
                Canvas.Right, Offset.Y);

            // Y axis
            g.DrawLine(Pens.Green, Offset.X, Canvas.Bottom,
                Offset.X, Canvas.Top);
        }

        private void DrawBounds(Graphics g, double[] scale)
        {
            Window window = Buffer.GetWindow();

            var p0 = MapFunctionSpaceToScreenSpace(window.BottomRight, scale);
            var p1 = MapFunctionSpaceToScreenSpace(window.TopLeft, scale);

            g.DrawLine(Pens.Blue, p0.X, Canvas.Height,
                p0.X, 0);

            g.DrawLine(Pens.Blue, p1.X, Canvas.Height,
                p1.X, 0);

            g.DrawLine(Pens.Red, 0, p0.Y,
                Canvas.Width, p0.Y);

            g.DrawLine(Pens.Blue, 0, p1.Y,
                Canvas.Width, p1.Y);
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            DragStart = e.Location;
            IsDragging = true;
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            DragStart = Origin;
            IsDragging = false;
            //IsGliding = true;
            //int oldDx = deltaX;
            //int oldDy = deltaY;
            //double friction = 0.5;

            //if (oldDx > 10 || oldDy > 10)
                //Canvas_MouseMove(null, new MouseEventArgs(MouseButtons.None, 0, e.X + (int)(oldDx*friction), e.Y + (int)(oldDx * friction), 0));
        }

        private void UpdateWindow()
        {
            double[] scale;
            lock (Buffer.Scale)
            {
                scale = Buffer.Scale;
            }

            var screenTopLeft = new Point(0, 0);
            var screenBottomRight = new Point(Canvas.Width, Canvas.Height);
            var funcTopLeft = MapScreenSpaceToFunctionSpace(screenTopLeft, scale);
            var funcBottomRight = MapScreenSpaceToFunctionSpace(screenBottomRight, scale);

            lock (Buffer.Window)
            {
                Buffer.Window = new Window(funcTopLeft, funcBottomRight);
            }
        }

        int oldDx = 0;
        int oldDy = 0;
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Offset = e.Location;

            Point dragEnd = e.Location;
            int deltaX = dragEnd.X - DragStart.X;
            int deltaY = dragEnd.Y - DragStart.Y;

            if ((IsDragging || IsGliding) && DistancesFromCursor.Min() > MIN_DISTANCE_FROM_CURSOR)
            {
                double[] scale;
                lock (Buffer.Scale)
                {
                    scale = Buffer.Scale;
                }

                //Origin.X += deltaX;
                //Origin.Y += deltaY;

                Origin = TranslatePoint(Origin, deltaX, deltaY, 1, 1);

                for (int i = 0; i < CurrentPoints.Count; i++)
                {
                    CurrentPoints[i] = TranslatePoints(CurrentPoints[i], deltaX, deltaY, 1, 1);
                }

                UpdateWindow();

                DragStart = dragEnd;

                //InvalidateCanvas(false);
                InvalidateCanvas();
            }
            else
            {
                InvalidateCanvas(false);
            }

            // TODO - we shouldn't have to recalculate all the points for a simple transformation
            // do some linear algebra on the existing points and redraw them for efficiency
            //InvalidateCanvas(false);
            Canvas.Invalidate();

            if (IsGliding)
            {
                int dx = (int)(oldDx * 0.5);
                int dy = (int)(oldDy * 0.5);
                if (dx < 1 && dy < 1) IsGliding = false;
                Canvas_MouseMove(null, new MouseEventArgs(MouseButtons.None, 0, e.X + (int)(oldDx * 0.5), e.Y + (int)(oldDx * 0.5), 0));
            }

            this.oldDx = deltaX;
            this.oldDy = deltaY;
        }

        private void InvalidateCanvas()
        {
            InvalidateCanvas(true);
        }


        private void InvalidateCanvas(bool release)
        {
            lock (Buffer.Functions)
            {
                for (int i = 0; i < Buffer.Functions.Count; i++)
                {
                    Buffer.Functions[i].IsValid = false;
                    //shouldDraw[i] = false;

                    if (!Calculators[i].IsCalculating && release)
                    {
                        Calculators[i].ResultInvalid.Release();
                    }
                }

            }

            //Calculators[0].invalid.Release();
            //Calculators[0].valid.WaitOne();
            
            

            Canvas.Invalidate();
        }

        private void Canvas_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                double factor = 1.1; // zoom in factor
                if (e.Delta < 0) factor = 0.95; // zoom out factor

                // get where mouse is pointing in screen space b
                var mouseLoc = e.Location;

                // convert to function space a
                double[] oldScale = Buffer.GetScale();
                var mouseLocationInFunctionSpace = MapScreenSpaceToFunctionSpace(mouseLoc, oldScale);

                // modify scale
                double[] relativeScale = new double[2];
                lock (Buffer.Scale)
                {
                    Buffer.Scale[0] *= factor;
                    Buffer.Scale[1] *= factor;
                    //relativeScale[0] = oldScale[0] / Buffer.Scale[0];
                    //relativeScale[1] = oldScale[1] / Buffer.Scale[1];
                }
                double[] newScale = Buffer.GetScale();

                // offset origin by how different f'^-1(a)=c is to f^-1(a)=mouseLoc
                UpdateWindow();

                var newMouseLoc = MapFunctionSpaceToScreenSpace(mouseLocationInFunctionSpace, newScale);
                int dx = Math.Abs(mouseLoc.X - Origin.X);
                int dy = Math.Abs(mouseLoc.Y - Origin.Y);
                Point oldOrigin = new Point(Origin.X, Origin.Y);
                if ((dx > 15 || dy > 15) || e.Delta < 0)
                {
                    Origin.X -= newMouseLoc.X - mouseLoc.X;
                    Origin.Y -= newMouseLoc.Y - mouseLoc.Y;
                }

                for (int i = 0; i < CurrentPoints.Count; i++)
                {
                    CurrentPoints[i] = TranslatePoints(CurrentPoints[i], -oldOrigin.X, -oldOrigin.Y, 1, 1);
                    CurrentPoints[i] = TranslatePoints(CurrentPoints[i], 0, 0, factor, factor);
                    CurrentPoints[i] = TranslatePoints(CurrentPoints[i], Origin.X, Origin.Y, 1, 1);
                }
                

                UpdateWindow();

                InvalidateCanvas();
            }
        }

        private void ReplaceFunction(int index, string functionString)
        {
            var expString = functionString.Replace("x", "[x]");

            var multRegex = new Regex(@"(\d+)\[x\]");
            expString = multRegex.Replace(expString, new MatchEvaluator(
                m => m.Groups[1] + "*[x]"));
            var trigRegex = new Regex(@"(\w+)(\()");
            expString = trigRegex.Replace(expString, new MatchEvaluator(
                m => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(m.Groups[1].ToString().ToLower()) + m.Groups[2]));

            Expression exp = null;
            if (expString.Length > 0)
            {
                exp = new Expression(expString);
            }
            
            if (exp == null || !exp.HasErrors())
            {
                Calculators[index].SetExpression(exp);
                InvalidateCanvas();
            }


        }

        private void Canvas_SizeChanged(object sender, EventArgs e)
        {
            CenterOrigin();
            UpdateWindow();
            InvalidateCanvas();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddFunction();
        }

        private void CenterOrigin()
        {
            Origin.X = Canvas.Width / 2;
            Origin.Y = Canvas.Height / 2;
        }

        private void CenterButton_Click(object sender, EventArgs e)
        {
            CenterOrigin();

            lock (Buffer.Scale)
            {
                Buffer.Scale[0] = 50;
                Buffer.Scale[1] = 50;
            }

            UpdateWindow();
            InvalidateCanvas();
        }

        private void Zoom(int delta)
        {
            Canvas_MouseWheel(null, new MouseEventArgs(MouseButtons.None, 0, Canvas.Width / 2, Canvas.Height / 2, delta));
        }

        private void ZoomInButton_Click(object sender, EventArgs e)
        {
            Zoom(1);
        }

        private void ZoomOutButton_Click(object sender, EventArgs e)
        {
            Zoom(-1);
        }

        private void ToggleAllCheckbox_Click(object sender, EventArgs e)
        {
            bool value = !visible.All(v => v);
            for (int i = 0; i < visible.Count; i++)
            {
                visible[i] = value;
                visibleCheckboxes[i].Checked = value;
            }
            ToggleAllCheckbox.Checked = value;
        }
    }
}
