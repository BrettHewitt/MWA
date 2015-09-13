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

using System.Linq;
using RobynsWhiskerTracker.Events.SingleWhiskerEnabled;
using RobynsWhiskerTracker.Extension;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngle;
using System.Windows;
using System.Windows.Media;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using Point = System.Windows.Point;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerAngle
{
    public class SingleWhiskerAngleViewModel : SingleWhiskerBase
    {
        private double m_TValue = -1;
        private double m_X1;
        private double m_Y1;
        private double m_X2;
        private double m_Y2;

        private double m_ImageWidth;
        private double m_ImageHeight;

        private Geometry m_Data;

        private double m_XCanvas;
        private double m_YCanvas;
        private const double m_PointSize = 5;

        private ISingleWhiskerAngle m_Model;

        public double TValue
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

                if (value < 0)
                {
                    value = 0;
                }
                else if (value > 1)
                {
                    value = 1;
                }

                m_TValue = value;
                Model.TValue = TValue;

                CalculateTargetPoint();
                CreateAngleLine();

                NotifyPropertyChanged();
                NotifyPropertyChanged("Angle");
            }
        }

        public double X1
        {
            get
            {
                return m_X1;
            }
            set
            {
                if (Equals(m_X1, value))
                {
                    return;
                }

                m_X1 = value;

                NotifyPropertyChanged();
            }
        }

        public double Y1
        {
            get
            {
                return m_Y1;
            }
            set
            {
                if (Equals(m_Y1, value))
                {
                    return;
                }

                m_Y1 = value;

                NotifyPropertyChanged();
            }
        }

        public double X2
        {
            get
            {
                return m_X2;
            }
            set
            {
                if (Equals(m_X2, value))
                {
                    return;
                }

                m_X2 = value;

                NotifyPropertyChanged();
            }
        }
        public double Y2
        {
            get
            {
                return m_Y2;
            }
            set
            {
                if (Equals(m_Y2, value))
                {
                    return;
                }

                m_Y2 = value;

                NotifyPropertyChanged();
            }
        }

        private double ImageWidth
        {
            get
            {
                return m_ImageWidth;
            }
            set
            {
                if (Equals(m_ImageWidth, value))
                {
                    return;
                }

                m_ImageWidth = value;

                CreateData();
            }
        }

        private double ImageHeight
        {
            get
            {
                return m_ImageHeight;
            }
            set
            {
                if (Equals(m_ImageHeight, value))
                {
                    return;
                }

                m_ImageHeight = value;

                CreateData();
            }
        }

        public double VideoWidth
        {
            get
            {
                return Model.Whisker.Parent.OriginalWidth;
            }
        }

        public double VideoHeight
        {
            get
            {
                return Model.Whisker.Parent.OriginalHeight;
            }
        }

        public Geometry Data
        {
            get
            {
                return m_Data;
            }
            set
            {
                if (Equals(m_Data, value))
                {
                    return;
                }

                m_Data = value;

                NotifyPropertyChanged();
            }
        }

        public double XCanvas
        {
            get
            {
                return m_XCanvas - (PointSize/2d);
            }
            set
            {
                m_XCanvas = value;

                NotifyPropertyChanged();
            }
        }

        public double YCanvas
        {
            get
            {
                return m_YCanvas - (PointSize/2d);
            }
            set
            {
                m_YCanvas = value;

                NotifyPropertyChanged();
            }
        }
        
        public ISingleWhiskerAngle Model
        {
            get
            {
                return m_Model;
            }
            set
            {
                if (Equals(m_Model, value))
                {
                    return;
                }

                m_Model = value;

                CreateData();

                NotifyPropertyChanged();
                NotifyPropertyChanged("Angle");
            }
        }

        public double PointSize
        {
            get
            {
                return m_PointSize;
            }
        }

        public double Angle
        {
            get
            {
                return Model.Angle;
            }
        }

        public override int WhiskerId
        {
            get
            {
                return Model.Whisker.WhiskerId;
            }
        }

        public string WhiskerName
        {
            get
            {
                return Model.Whisker.WhiskerName;
            }
        }

        public SingleWhiskerAngleViewModel(ISingleWhiskerAngle model)
        {
            Model = model;
            TValue = 0;
        }

        public void UpdatePositions(double actualWidth, double actualHeight)
        {
            ImageWidth = actualWidth;
            ImageHeight = actualHeight;
        }

        private void CreateData()
        {
            CreateCurve();
            CreateAngleLine();
            CalculateTargetPoint();
        }

        private void CreateCurve()
        {
            //Make sure we can create the data
            if (Model == null || ImageWidth == 0 || ImageHeight == 0)
            {
                return;
            }

            Point[] points = new Point[Model.ControlPoints.Length];
            for (int i = 0; i < Model.ControlPoints.Length; i++)
            {
                points[i] = new Point(Model.ControlPoints[i].XRatio * ImageWidth, Model.ControlPoints[i].YRatio * ImageHeight);
            }

            Data = DrawingUtility.GenerateBezierCurve(points);
        }

        private void CreateAngleLine()
        {
            Vector gradient = Model.AngleLine;
            gradient.Normalize();
            Point basePoint = Model.BasePoint;
            IWhiskerPoint lastPoint = Model.ControlPoints.First();
            Point targetPoint = new Point(lastPoint.XRatio * ImageWidth, lastPoint.YRatio * ImageHeight);

            basePoint.X = ConvertHorizontalFromVideoToImage(basePoint.X);
            basePoint.Y = ConvertVerticalFromVideoToImage(basePoint.Y);

            Vector distance = new Vector(targetPoint.X - basePoint.X, targetPoint.Y - basePoint.Y);
            double lineLength = distance.Length;

            X1 = basePoint.X - (gradient.X * lineLength);
            Y1 = basePoint.Y - (gradient.Y * lineLength);
            X2 = basePoint.X + (gradient.X * lineLength);
            Y2 = basePoint.Y + (gradient.Y * lineLength);
        }

        private void CalculateTargetPoint()
        {
            XCanvas = ConvertHorizontalFromVideoToImage(Model.TargetPoint.X);
            YCanvas = ConvertVerticalFromVideoToImage(Model.TargetPoint.Y);
        }

        private double ConvertHorizontalFromVideoToImage(double number)
        {
            number /= VideoWidth;
            number *= ImageWidth;

            return number;
        }

        private double ConvertVerticalFromVideoToImage(double number)
        {
            number /= VideoHeight;
            number *= ImageHeight;

            return number;
        }

        public void NotifyAnglePropertyChanged()
        {
            NotifyPropertyChanged("Angle");
        }
    }
}
