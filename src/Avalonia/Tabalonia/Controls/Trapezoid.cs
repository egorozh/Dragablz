using Avalonia.Controls;
using Avalonia.Media;
using System;
using Avalonia;
using Avalonia.Styling;

namespace Tabalonia;

public class Trapezoid : ContentControl, IStyleable
{
    private PathGeometry _pathGeometry;

    #region IStyleable

    Type IStyleable.StyleKey => typeof(Trapezoid);

    #endregion

    static Trapezoid()
    {
        AffectsMeasure<Trapezoid>(PenBrushProperty, LongBasePenBrushProperty, PenThicknessProperty);
        AffectsRender<Trapezoid>(BackgroundProperty);
    }

    #region Avalonia Properties

    public static readonly StyledProperty<Brush> PenBrushProperty =
        AvaloniaProperty.Register<Trapezoid, Brush>(nameof(PenBrush), (Brush) Brushes.Transparent);

    public static readonly StyledProperty<Brush> LongBasePenBrushProperty =
        AvaloniaProperty.Register<Trapezoid, Brush>(nameof(LongBasePenBrush), (Brush) Brushes.Transparent);

    public static readonly StyledProperty<double> PenThicknessProperty =
        AvaloniaProperty.Register<Trapezoid, double>(nameof(PenThickness));

    #endregion

    #region Public Properties

    public Brush PenBrush
    {
        get => GetValue(PenBrushProperty);
        set => SetValue(PenBrushProperty, value);
    }

    public Brush LongBasePenBrush
    {
        get => GetValue(LongBasePenBrushProperty);
        set => SetValue(LongBasePenBrushProperty, value);
    }

    public double PenThickness
    {
        get => GetValue(PenThicknessProperty);
        set => SetValue(PenThicknessProperty, value);
    }

    #endregion

    protected override Size MeasureOverride(Size availableSize)
    {
        var contentDesiredSize = base.MeasureOverride(availableSize);

        if (contentDesiredSize.Width == 0 || double.IsInfinity(contentDesiredSize.Width)
                                          || contentDesiredSize.Height == 0 ||
                                          double.IsInfinity(contentDesiredSize.Height))

            return contentDesiredSize;

        _pathGeometry = CreateGeometry(contentDesiredSize);
        Clip = _pathGeometry;

        return _pathGeometry.GetRenderBounds(new Pen(PenBrush, 1)
        {
            //EndLineCap = PenLineCap.Flat,
            MiterLimit = 1
        }).Size;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        context.DrawGeometry(Background, CreatePen(), _pathGeometry);

        if (_pathGeometry == null) return;
        context.DrawGeometry(Background, new Pen(LongBasePenBrush, PenThickness)
            {
                //EndLineCap = PenLineCap.Flat,
                MiterLimit = 1
            },
            new LineGeometry(_pathGeometry.Bounds.BottomLeft + new Vector(3, 0),
                _pathGeometry.Bounds.BottomRight + new Vector(-3, 0)));
    }

    private Pen CreatePen()
    {
        return new Pen(PenBrush, PenThickness)
        {
            //EndLineCap = PenLineCap.Flat,
            MiterLimit = 1
        };
    }

    private static PathGeometry CreateGeometry(Size contentDesiredSize)
    {
        //TODO Make better :)  do some funky beziers or summit
        const double cheapRadiusBig = 6.0;
        const double cheapRadiusSmall = cheapRadiusBig / 2;

        const int angle = 20;
        const double radians = angle * (Math.PI / 180);

        var startPoint = new Point(0, contentDesiredSize.Height + cheapRadiusSmall + cheapRadiusSmall);

        //clockwise starting at bottom left
        var bottomLeftSegment = new ArcSegment
        {
            Point = new Point(startPoint.X + cheapRadiusSmall, startPoint.Y - cheapRadiusSmall),
            Size = new Size(cheapRadiusSmall, cheapRadiusSmall),
            RotationAngle = 315,
            IsLargeArc = false,
            SweepDirection = SweepDirection.CounterClockwise
        };
        var triangleX = Math.Tan(radians) * (contentDesiredSize.Height);
        var leftSegment =
            new LineSegment
            {
                Point = new Point(bottomLeftSegment.Point.X + triangleX,
                    bottomLeftSegment.Point.Y - contentDesiredSize.Height)
            };
        var topLeftSegment =
            new ArcSegment
            {
                Point = new Point(leftSegment.Point.X + cheapRadiusBig, leftSegment.Point.Y - cheapRadiusSmall),
                Size = new Size(cheapRadiusBig, cheapRadiusBig),
                RotationAngle = 120,
                IsLargeArc = false,
                SweepDirection = SweepDirection.Clockwise
            };
        var topSegment =
            new LineSegment
            {
                Point = new Point(contentDesiredSize.Width + cheapRadiusBig + cheapRadiusBig, 0)
            };
        var topRightSegment =
            new ArcSegment
            {
                Point = new Point(contentDesiredSize.Width + cheapRadiusBig + cheapRadiusBig + cheapRadiusBig,
                    cheapRadiusSmall),
                Size = new Size(cheapRadiusBig, cheapRadiusBig),
                IsLargeArc = false,
                RotationAngle = 40,
                SweepDirection = SweepDirection.Clockwise
            };

        triangleX = Math.Tan(radians) * (contentDesiredSize.Height);
        //triangleX = Math.Tan(radians)*(contentDesiredSize.Height - topRightSegment.Point.Y);
        var rightSegment =
            new LineSegment
            {
                Point = new Point(topRightSegment.Point.X + triangleX,
                    topRightSegment.Point.Y + contentDesiredSize.Height)
            };

        var bottomRightPoint = new Point(rightSegment.Point.X + cheapRadiusSmall,
            rightSegment.Point.Y + cheapRadiusSmall);

        var bottomRightSegment = new ArcSegment
        {
            Point = bottomRightPoint,
            Size = new Size(cheapRadiusSmall, cheapRadiusSmall),
            IsLargeArc = false,
            RotationAngle = 25,
            SweepDirection = SweepDirection.CounterClockwise
        };
        var bottomLeftPoint = new Point(0, bottomRightSegment.Point.Y);
        var bottomSegment = new LineSegment
        {
            Point = bottomLeftPoint
        };

        var pathSegmentCollection = new PathSegments
        {
            bottomLeftSegment, leftSegment, topLeftSegment, topSegment, topRightSegment, rightSegment,
            bottomRightSegment, bottomSegment
        };

        var pathFigure = new PathFigure
        {
            StartPoint = startPoint,
            IsClosed = true,
            Segments = pathSegmentCollection,
            IsFilled = true
        };

        return new PathGeometry()
        {
            Figures = new PathFigures {pathFigure}
        };
    }
}