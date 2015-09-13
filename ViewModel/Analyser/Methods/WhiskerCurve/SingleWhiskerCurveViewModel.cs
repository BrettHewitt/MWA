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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using MathNet.Numerics.LinearAlgebra;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using RobynsWhiskerTracker.Extension;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Curvature;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerCurve
{
    public class SingleWhiskerCurveViewModel : SingleWhiskerBase
    {
        private double m_TValue = -1;
        private double m_ImageWidth;
        private double m_ImageHeight;
        private double m_VideoWidth;
        private double m_VideoHeight;

        private double m_XCanvas;
        private double m_YCanvas;
        private const double m_PointSize = 5;

        private Geometry m_Data;

        private ISingleWhiskerCurve m_Model;

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
                
                NotifyPropertyChanged();
                NotifyPropertyChanged("Curvature");
            }
        }

        public double PointSize
        {
            get
            {
                return m_PointSize;
            }
        }

        public double Curvature
        {
            get
            {
                return Model.Curvature;
            }
        }

        public double XCanvas
        {
            get
            {
                return m_XCanvas - (PointSize / 2d);
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
                return m_YCanvas - (PointSize / 2d);
            }
            set
            {
                m_YCanvas = value;

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
                return m_VideoWidth;
            }
            set
            {
                if (Equals(m_VideoWidth, value))
                {
                    return;
                }

                m_VideoWidth = value;

                NotifyPropertyChanged();
            }
        }

        public double VideoHeight
        {
            get
            {
                return m_VideoHeight;
            }
            set
            {
                if (Equals(m_VideoHeight, value))
                {
                    return;
                }

                m_VideoHeight = value;

                NotifyPropertyChanged();
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

        public ISingleWhiskerCurve Model
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

                NotifyPropertyChanged();

                CreateData();
            }
        }

        public string WhiskerName
        {
            get
            {
                return Model.Whisker.WhiskerName;
            }
        }

        public override int WhiskerId
        {
            get
            {
                return Model.Whisker.WhiskerId;
            }
        }

        public SingleWhiskerCurveViewModel(ISingleWhiskerCurve model)
        {
            Model = model;
            VideoWidth = Model.Whisker.Parent.OriginalWidth;
            VideoHeight = Model.Whisker.Parent.OriginalHeight;
        }

        private void CreateData()
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
            CalculateTargetPoint();
        }

        public void UpdatePositions(double actualWidth, double actualHeight)
        {
            ImageWidth = actualWidth;
            ImageHeight = actualHeight;
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
    }
}
