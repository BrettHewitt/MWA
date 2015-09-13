/*
Manual Whisker Annotator - A program to manually annotate whiskers and analyse them
Copyright (C) 2015 Brett Michael Hewitt

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using RobynsWhiskerTracker.Extension;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Curvature;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Curvature
{
    internal class SingleWhiskerCurve : ModelObjectBase, ISingleWhiskerCurve
    {
        protected double m_TValue = 1;
        private IWhisker m_Whisker;
        private IWhiskerPoint[] m_ControlPoints;
        private double m_VideoWidth;
        private double m_VideoHeight;

        private double m_Curvature;
        private Point m_TargetPoint;

        public virtual double TValue
        {
            get
            {
                return m_TValue;
            }
            set
            {
                if (Equals(m_TValue, value))
                {
                    return;
                }

                m_TValue = value;

                CalculateCurvature();

                MarkAsDirty();
            }
        }

        public Point TargetPoint
        {
            get
            {
                return m_TargetPoint;
            }
            set
            {
                if (Equals(m_TargetPoint, value))
                {
                    return;
                }

                m_TargetPoint = value;

                MarkAsDirty();
            }
        }

        public IWhisker Whisker
        {
            get
            {
                return m_Whisker;
            }
            set
            {
                if (Equals(m_Whisker, value))
                {
                    return;
                }

                m_Whisker = value;

                m_VideoWidth = Whisker.Parent.OriginalWidth;
                m_VideoHeight = Whisker.Parent.OriginalHeight;

                MarkAsDirty();

                GenerateCurve();
            }
        }

        public IWhiskerPoint[] ControlPoints
        {
            get
            {
                return m_ControlPoints;
            }
            private set
            {
                if (ReferenceEquals(m_ControlPoints, value))
                {
                    return;
                }

                m_ControlPoints = value;

                MarkAsDirty();
            }
        }

        public double Curvature
        {
            get
            {
                return m_Curvature;
            }
            private set
            {
                if (Equals(m_Curvature, value))
                {
                    return;
                }

                m_Curvature = value;

                MarkAsDirty();
            }
        }

        protected double VideoWidth
        {
            get
            {
                return m_VideoWidth;
            }
        }

        protected double VideoHeight
        {
            get
            {
                return m_VideoHeight;
            }
        }

        protected void GenerateCurve()
        {
            //Make sure we have all the info we need
            if (Whisker == null || Whisker.WhiskerPoints == null)
            {
                return;
            }

            if (Whisker.WhiskerPoints.Length == 0)
            {
                return;
            }

            if (Whisker.Parent.OriginalWidth == 0 || Whisker.Parent.OriginalHeight == 0)
            {
                return;
            }

            //Calculate Control Points
            IWhiskerPoint firstPoint = Whisker.WhiskerPoints[0].Clone();

            ControlPoints = new IWhiskerPoint[Whisker.WhiskerPoints.Length];
            ControlPoints[0] = firstPoint;

            double videoWidth = Whisker.Parent.OriginalWidth;
            double videoHeight = Whisker.Parent.OriginalHeight;

            if (ControlPoints.Length == 2)
            {
                IWhiskerPoint lWhiskerPoint1 = Whisker.WhiskerPoints[1].Clone();                
                ControlPoints[1] = lWhiskerPoint1;
                TargetPoint = GetTValuePointForLinear();
            }
            
            if (ControlPoints.Length == 3)
            {
                IWhiskerPoint qWhiskerPoint1 = Whisker.WhiskerPoints[1].Clone();
                IWhiskerPoint qWhiskerPoint2 = Whisker.WhiskerPoints[2].Clone();

                Point[] points = new Point[3];
                points[0] = new Point(firstPoint.XRatio * videoWidth, firstPoint.YRatio * videoHeight);
                points[1] = new Point(qWhiskerPoint1.XRatio * videoWidth, qWhiskerPoint1.YRatio * videoHeight);
                points[2] = new Point(qWhiskerPoint2.XRatio * videoWidth, qWhiskerPoint2.YRatio * videoHeight);

                Point point1;
                DrawingUtility.GetControlPointsForQuadraticBezier(points, out point1);

                qWhiskerPoint1.XRatio = point1.X / videoWidth;
                qWhiskerPoint1.YRatio = point1.Y / videoHeight;

                ControlPoints[1] = qWhiskerPoint1;
                ControlPoints[2] = qWhiskerPoint2;
                TargetPoint = GetTValuePointForQuadratic();
            }
            
            if (ControlPoints.Length == 4)
            {
                IWhiskerPoint cWhiskerPoint1 = Whisker.WhiskerPoints[1].Clone();
                IWhiskerPoint cWhiskerPoint2 = Whisker.WhiskerPoints[2].Clone();
                IWhiskerPoint cWhiskerPoint3 = Whisker.WhiskerPoints[3].Clone();

                Point[] points = new Point[4];
                points[0] = new Point(firstPoint.XRatio * videoWidth, firstPoint.YRatio * videoHeight);
                points[1] = new Point(cWhiskerPoint1.XRatio * videoWidth, cWhiskerPoint1.YRatio * videoHeight);
                points[2] = new Point(cWhiskerPoint2.XRatio * videoWidth, cWhiskerPoint2.YRatio * videoHeight);
                points[3] = new Point(cWhiskerPoint3.XRatio * videoWidth, cWhiskerPoint3.YRatio * videoHeight);

                Point point1, point2;
                DrawingUtility.GetControlPointsForCubicBezier(points, out point1, out point2);

                cWhiskerPoint1.XRatio = point1.X / videoWidth;
                cWhiskerPoint1.YRatio = point1.Y / videoHeight;
                cWhiskerPoint2.XRatio = point2.X / videoWidth;
                cWhiskerPoint2.YRatio = point2.Y / videoHeight;

                ControlPoints[1] = cWhiskerPoint1;
                ControlPoints[2] = cWhiskerPoint2;
                ControlPoints[3] = cWhiskerPoint3;
                TargetPoint = GetTValuePointForCubic();
            }
        }

        private void CalculateCurvature()
        {
            Curvature = 0;

            if (ControlPoints.Length == 2)
            {
                TargetPoint = GetTValuePointForLinear();
            }
            else if (ControlPoints.Length == 3)
            {
                IWhiskerPoint cWhiskerPoint0 = Whisker.WhiskerPoints[0];
                IWhiskerPoint cWhiskerPoint1 = Whisker.WhiskerPoints[1];
                IWhiskerPoint cWhiskerPoint2 = Whisker.WhiskerPoints[2];

                Point[] points = new Point[3];
                points[0] = new Point(cWhiskerPoint0.XRatio * VideoWidth, cWhiskerPoint0.YRatio * VideoHeight);
                points[1] = new Point(cWhiskerPoint1.XRatio * VideoWidth, cWhiskerPoint1.YRatio * VideoHeight);
                points[2] = new Point(cWhiskerPoint2.XRatio * VideoWidth, cWhiskerPoint2.YRatio * VideoHeight);

                Curvature = DrawingUtility.GetQuadraticBezierCurvature(points, TValue);

                TargetPoint = GetTValuePointForQuadratic();
            }
            else
            {
                IWhiskerPoint cWhiskerPoint0 = Whisker.WhiskerPoints[0];
                IWhiskerPoint cWhiskerPoint1 = Whisker.WhiskerPoints[1];
                IWhiskerPoint cWhiskerPoint2 = Whisker.WhiskerPoints[2];
                IWhiskerPoint cWhiskerPoint3 = Whisker.WhiskerPoints[3];

                Point[] points = new Point[4];
                points[0] = new Point(cWhiskerPoint0.XRatio * VideoWidth, cWhiskerPoint0.YRatio * VideoHeight);
                points[1] = new Point(cWhiskerPoint1.XRatio * VideoWidth, cWhiskerPoint1.YRatio * VideoHeight);
                points[2] = new Point(cWhiskerPoint2.XRatio * VideoWidth, cWhiskerPoint2.YRatio * VideoHeight);
                points[3] = new Point(cWhiskerPoint3.XRatio * VideoWidth, cWhiskerPoint3.YRatio * VideoHeight);

                Curvature = DrawingUtility.GetCubicBezierCurvature(points, TValue);
                TargetPoint = GetTValuePointForCubic();
            }
        }

        protected Point GetTValuePointForLinear()
        {
            Point point = new Point();

            Point p0 = new Point((ControlPoints[0].XRatio * VideoWidth), (ControlPoints[0].YRatio * VideoHeight));
            Point p1 = new Point((ControlPoints[1].XRatio * VideoWidth), (ControlPoints[1].YRatio * VideoHeight));

            double xValue = p0.X + (TValue*(p1.X - p0.X));
            double yValue = p0.Y + (TValue*(p1.Y - p0.Y));

            point.X = xValue;
            point.Y = yValue;

            return point;
        }

        protected Point GetTValuePointForQuadratic()
        {
            Point point = new Point();

            Point p0 = new Point((ControlPoints[0].XRatio*VideoWidth), (ControlPoints[0].YRatio*VideoHeight));
            Point p1 = new Point((ControlPoints[1].XRatio*VideoWidth), (ControlPoints[1].YRatio*VideoHeight));
            Point p2 = new Point((ControlPoints[2].XRatio*VideoWidth), (ControlPoints[2].YRatio*VideoHeight));

            double xValue = (Math.Pow(1 - TValue, 2) * p0.X) + (2 * (1 - TValue) * TValue * p1.X) + (Math.Pow(TValue, 2) * p2.X);
            double yValue = (Math.Pow(1 - TValue, 2) * p0.Y) + (2 * (1 - TValue) * TValue * p1.Y) + (Math.Pow(TValue, 2) * p2.Y);

            point.X = xValue;
            point.Y = yValue;

            return point;
        }

        protected Point GetTValuePointForCubic()
        {
            Point point = new Point();

            Point p0 = new Point((ControlPoints[0].XRatio * VideoWidth), (ControlPoints[0].YRatio * VideoHeight));
            Point p1 = new Point((ControlPoints[1].XRatio * VideoWidth), (ControlPoints[1].YRatio * VideoHeight));
            Point p2 = new Point((ControlPoints[2].XRatio * VideoWidth), (ControlPoints[2].YRatio * VideoHeight));
            Point p3 = new Point((ControlPoints[3].XRatio * VideoWidth), (ControlPoints[3].YRatio * VideoHeight));

            double xValue = (Math.Pow(1 - TValue, 3) * p0.X) + (3 * Math.Pow(1 - TValue, 2) * TValue * p1.X) + (3 * (1 - TValue) * Math.Pow(TValue, 2) * p2.X) + (Math.Pow(TValue, 3) * p3.X);
            double yValue = (Math.Pow(1 - TValue, 3) * p0.Y) + (3 * Math.Pow(1 - TValue, 2) * TValue * p1.Y) + (3 * (1 - TValue) * Math.Pow(TValue, 2) * p2.Y) + (Math.Pow(TValue, 3) * p3.Y);

            point.X = xValue;
            point.Y = yValue;

            return point;
        }
    }
}
