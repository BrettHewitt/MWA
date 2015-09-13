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

using RobynsWhiskerTracker.Model.Analyser.Methods.Curvature;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.AngleTypes;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngle;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using RobynsWhiskerTracker.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Angles.WhiskerAngle
{
    internal class SingleWhiskerAngle : SingleWhiskerCurve, ISingleWhiskerAngle
    {
        private double m_Angle;
        private IAngleTypeBase m_AngleType;
        private Vector m_AngleLine;

        private Point m_BasePoint;
        private Point m_TargetPoint;

        public override double TValue
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

                CalculateAngle();

                MarkAsDirty();
            }
        }

        public double Angle
        {
            get
            {
                return m_Angle;
            }
            protected set
            {
                if (Equals(m_Angle, value))
                {
                    return;
                }

                m_Angle = value;

                MarkAsDirty();
            }
        }

        public IAngleTypeBase AngleType
        {
            get
            {
                return m_AngleType;
            }
            set
            {
                if (Equals(m_AngleType, value))
                {
                    return;
                }

                m_AngleType = value;

                CalculateAngle();

                MarkAsDirty();
            }
        }

        public Vector AngleLine
        {
            get
            {
                return m_AngleLine;
            }
            protected set
            {
                if (Equals(m_AngleLine, value))
                {
                    return;
                }

                m_AngleLine = value;

                MarkAsDirty();
            }
        }

        public Point BasePoint
        {
            get
            {
                return m_BasePoint;
            }
            set
            {
                if (Equals(m_BasePoint, value))
                {
                    return;
                }

                m_BasePoint = value;

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

        public SingleWhiskerAngle()
        {
           
        }

        protected void CalculateAngle()
        {
            //Create line from the base of the whisker to the point on the whisker
            IWhiskerPoint baseWhiskerPoint = ControlPoints.Last();
            BasePoint = new Point(baseWhiskerPoint.XRatio * VideoWidth, baseWhiskerPoint.YRatio * VideoHeight);
            Point anglePoint;
            Vector gradient;

            if (TValue == 1)
            {
                //If TValue == 1 (Point on whisker == base), calculate gradient at TValue = 1
                gradient = GetBezierGradientVideoSize();
                gradient *= -1;
                anglePoint = BasePoint;
                anglePoint.X += gradient.X;
                anglePoint.Y += gradient.Y;
            }
            else
            {
                anglePoint = GetTValuePoint();
                gradient = new Vector(anglePoint.X - BasePoint.X, anglePoint.Y - BasePoint.Y);
            }

            //Create line which will determine angle
            gradient.Normalize();
            TargetPoint = anglePoint;

            AngleLine = gradient;
            Angle = AngleType.CalculateAngle(AngleLine);
        }

        public Point GetTValuePoint()
        {
            switch (ControlPoints.Length)
            {
                case 2:

                    return GetTValuePointForLinear();

                case 3:

                    return GetTValuePointForQuadratic();

                case 4:

                    return GetTValuePointForCubic();
            }

            throw new Exception("Trying to get TValue point but control points don't consist of either 2, 3 or 4 points");
        }

        private Point GetTValuePointForLinear()
        {
            Point point = new Point();

            Point p0 = new Point(ControlPoints[0].XRatio * VideoWidth, ControlPoints[0].YRatio * VideoHeight);
            Point p1 = new Point(ControlPoints[1].XRatio * VideoWidth, ControlPoints[1].YRatio * VideoHeight);

            double xValue = p0.X + (TValue * (p1.X - p0.X));
            double yValue = p0.Y + (TValue * (p1.Y - p0.Y));

            point.X = xValue;
            point.Y = yValue;

            return point;
        }

        private Point GetTValuePointForQuadratic()
        {
            Point point = new Point();

            Point p0 = new Point((ControlPoints[0].XRatio * VideoWidth), (ControlPoints[0].YRatio * VideoHeight));
            Point p1 = new Point((ControlPoints[1].XRatio * VideoWidth), (ControlPoints[1].YRatio * VideoHeight));
            Point p2 = new Point((ControlPoints[2].XRatio * VideoWidth), (ControlPoints[2].YRatio * VideoHeight));

            double xValue = (Math.Pow(1 - TValue, 2) * p0.X) + (2 * (1 - TValue) * TValue * p1.X) + (Math.Pow(TValue, 2) * p2.X);
            double yValue = (Math.Pow(1 - TValue, 2) * p0.Y) + (2 * (1 - TValue) * TValue * p1.Y) + (Math.Pow(TValue, 2) * p2.Y);

            point.X = xValue;
            point.Y = yValue;

            return point;
        }

        private Vector GetBezierGradientVideoSize()
        {
            int numberOfPoints = ControlPoints.Length;
            Vector result;

            if (numberOfPoints == 2)
            {
                result = new Vector((ControlPoints[1].XRatio * VideoWidth) - (ControlPoints[0].XRatio * VideoWidth), (ControlPoints[1].YRatio * VideoHeight) - (ControlPoints[0].YRatio * VideoHeight));
            }
            else if (numberOfPoints == 3)
            {
                result = GetQuadraticBezierGradientVideoSize(TValue);
                ;
            }
            else if (numberOfPoints == 4)
            {
                result = GetCubicBezierGradientVideoSize(TValue);
            }
            else
            {
                throw new Exception("Number of points must be equal to 2, 3 or 4");
            }

            return result;
        }

        private Vector GetQuadraticBezierGradientVideoSize(double tValue)
        {
            Point p0 = new Point((float)(ControlPoints[0].XRatio * VideoWidth), (float)(ControlPoints[0].YRatio * VideoHeight));
            Point p1 = new Point((float)(ControlPoints[1].XRatio * VideoWidth), (float)(ControlPoints[1].YRatio * VideoHeight));
            Point p2 = new Point((float)(ControlPoints[2].XRatio * VideoWidth), (float)(ControlPoints[2].YRatio * VideoHeight));

            double xValue = (2 * (1 - tValue) * (p1.X - p0.X)) + (2 * tValue * (p2.X - p1.X));
            double yValue = (2 * (1 - tValue) * (p1.Y - p0.Y)) + (2 * tValue * (p2.Y - p1.Y));

            return new Vector(xValue, yValue);
        }

        private Vector GetCubicBezierGradientVideoSize(double tValue)
        {
            Point p0 = new Point((float)(ControlPoints[0].XRatio * VideoWidth), (float)(ControlPoints[0].YRatio * VideoHeight));
            Point p1 = new Point((float)(ControlPoints[1].XRatio * VideoWidth), (float)(ControlPoints[1].YRatio * VideoHeight));
            Point p2 = new Point((float)(ControlPoints[2].XRatio * VideoWidth), (float)(ControlPoints[2].YRatio * VideoHeight));
            Point p3 = new Point((float)(ControlPoints[3].XRatio * VideoWidth), (float)(ControlPoints[3].YRatio * VideoHeight));
            
            double xValue = (3 * Math.Pow(1 - tValue, 2) * (p1.X - p0.X)) + (6 * (1 - tValue) * tValue * (p2.X - p1.X)) + (3 * Math.Pow(tValue, 2) * (p3.X - p2.X));
            double yValue = (3 * Math.Pow(1 - tValue, 2) * (p1.Y - p0.Y)) + (6 * (1 - tValue) * tValue * (p2.Y - p1.Y)) + (3 * Math.Pow(tValue, 2) * (p3.Y - p2.Y));

            return new Vector(xValue, yValue);
        }
    }
}
