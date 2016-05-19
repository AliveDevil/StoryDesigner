/* Copyright: Ashley Davis (http://www.codeproject.com/Articles/182683/NetworkView-A-WPF-custom-control-for-visualizing-a) */
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StoryDesigner
{
	public class CurvedPath : Shape
	{
		public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(PointCollection), typeof(CurvedPath), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public PointCollection Points
		{
			get { return (PointCollection)GetValue(PointsProperty); }
			set { SetValue(PointsProperty, value); }
		}

		protected override Geometry DefiningGeometry
		{
			get
			{
				if (Points == null || Points.Count < 2)
				{
					return new GeometryGroup();
				}

				return GenerateGeometry();
			}
		}

		protected Geometry GenerateGeometry()
		{
			PathGeometry pathGeometry = new PathGeometry();

			if (Points.Count == 2 || Points.Count == 3)
			{
				PathFigure fig = new PathFigure();
				fig.IsClosed = false;
				fig.IsFilled = false;
				fig.StartPoint = Points[0];

				for (int i = 1; i < Points.Count; ++i)
				{
					fig.Segments.Add(new LineSegment(Points[i], true));
				}

				pathGeometry.Figures.Add(fig);
			}
			else
			{
				PointCollection adjustedPoints = new PointCollection();
				adjustedPoints.Add(Points[0]);
				for (int i = 1; i < Points.Count; ++i)
				{
					adjustedPoints.Add(Points[i]);
				}

				if (adjustedPoints.Count == 4)
				{
					PathFigure fig = new PathFigure();
					fig.IsClosed = false;
					fig.IsFilled = false;
					fig.StartPoint = adjustedPoints[0];
					fig.Segments.Add(new BezierSegment(adjustedPoints[1], adjustedPoints[2], adjustedPoints[3], true));

					pathGeometry.Figures.Add(fig);
				}
				else if (adjustedPoints.Count >= 5)
				{
					PathFigure fig = new PathFigure();
					fig.IsClosed = false;
					fig.IsFilled = false;
					fig.StartPoint = adjustedPoints[0];

					adjustedPoints.RemoveAt(0);

					while (adjustedPoints.Count > 3)
					{
						Point generatedPoint = adjustedPoints[1] + ((adjustedPoints[2] - adjustedPoints[1]) / 2);

						fig.Segments.Add(new BezierSegment(adjustedPoints[0], adjustedPoints[1], generatedPoint, true));

						adjustedPoints.RemoveAt(0);
						adjustedPoints.RemoveAt(0);
					}

					if (adjustedPoints.Count == 2)
					{
						fig.Segments.Add(new BezierSegment(adjustedPoints[0], adjustedPoints[0], adjustedPoints[1], true));
					}
					else
					{
						//Trace.Assert(adjustedPoints.Count == 2);

						fig.Segments.Add(new BezierSegment(adjustedPoints[0], adjustedPoints[1], adjustedPoints[2], true));
					}

					pathGeometry.Figures.Add(fig);
				}
			}

			return pathGeometry;
		}
	}
}
