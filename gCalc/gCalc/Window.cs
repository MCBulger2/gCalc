namespace gCalc
{
    public class Window
    {
        public PointRange Domain { get; set; }
        public PointRange Range { get; set; }


        public double Top
        {
            get { return Range.End; }
        }
        public double Bottom
        {
            get { return Range.Start; }
        }
        public double Left
        {
            get { return Domain.Start; }
        }
        public double Right
        {
            get { return Domain.End; }
        }

        public FPoint TopLeft
        {
            get { return new FPoint(Left, Top); }
        }
        public FPoint BottomLeft
        {
            get { return new FPoint(Left, Bottom); }
        }
        public FPoint TopRight
        {
            get { return new FPoint(Right, Top); }
        }

        public FPoint BottomRight
        {
            get { return new FPoint(Right, Bottom); }
        }


        public Window(double x0, double x1, double y0, double y1)
        {
            Domain = new PointRange(x0, x1);
            Range = new PointRange(y0, y1);
        }

        public Window(FPoint topLeft, FPoint bottomRight)
        {
            Domain = new PointRange(topLeft.X, bottomRight.X);
            Range = new PointRange(bottomRight.Y, topLeft.Y);
        }
    }
}