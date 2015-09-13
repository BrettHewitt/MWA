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

using RobynsWhiskerTracker.ModelInterface.GenericPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RobynsWhiskerTracker.ViewModel.GenericPoint
{
    public class GenericPointViewModel : ViewModelBase
    {
        private const float Size = 5;

        private double m_CanvasXPosition;
        private double m_CanvasYPosition;
        private double m_CanvasWidth;
        private double m_CanvasHeight;

        public IGenericPoint Model
        {
            get;
            set;
        }

        public int PointId
        {
            get
            {
                return Model.PointId;
            }
            set
            {
                if (Equals(PointId, value))
                {
                    return;
                }

                Model.PointId = value;

                NotifyPropertyChanged();
            }
        }

        public double XRatio
        {
            get
            {
                return Model.XRatio;
            }
            set
            {
                if (Equals(XRatio, value))
                {
                    return;
                }

                Model.XRatio = value;
                CanvasXPosition = CanvasWidth * XRatio;

                NotifyPropertyChanged();
                NotifyPropertyChanged("XCanvas");
                NotifyPropertyChanged("PointPlaced");
            }
        }

        public double YRatio
        {
            get
            {
                return Model.YRatio;
            }
            set
            {
                if (Equals(YRatio, value))
                {
                    return;
                }

                Model.YRatio = value;
                CanvasYPosition = CanvasHeight * YRatio;

                NotifyPropertyChanged();
                NotifyPropertyChanged("YCanvas");
                NotifyPropertyChanged("PointPlaced");
            }
        }

        public double CanvasXPosition
        {
            get
            {
                return m_CanvasXPosition;
            }
            private set
            {
                if (Equals(m_CanvasXPosition, value))
                {
                    return;
                }

                m_CanvasXPosition = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged("XCanvas");
            }
        }

        public double CanvasYPosition
        {
            get
            {
                return m_CanvasYPosition;
            }
            private set
            {
                if (Equals(m_CanvasYPosition, value))
                {
                    return;
                }

                m_CanvasYPosition = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged("YCanvas");
            }
        }

        public double XCanvas
        {
            get
            {
                return CanvasXPosition - (Width / 2);
            }
        }

        public double YCanvas
        {
            get
            {
                return CanvasYPosition - (Height / 2);
            }
        }

        public double CanvasWidth
        {
            get
            {
                return m_CanvasWidth;
            }
            set
            {
                m_CanvasWidth = value;
                CanvasXPosition = XRatio * m_CanvasWidth;
            }
        }

        public double CanvasHeight
        {
            get
            {
                return m_CanvasHeight;
            }
            set
            {
                m_CanvasHeight = value;
                CanvasYPosition = YRatio * m_CanvasHeight;
            }
        }

        public bool PointPlaced
        {
            get
            {
                return Model.XRatio != 0 && Model.YRatio != 0;
            }
        }

        public float Width
        {
            get
            {
                return Size;
            }
        }

        public float Height
        {
            get
            {
                return Size;
            }
        }

        public Brush Color
        {
            get;
            set;
        }

        public GenericPointViewModel(IGenericPoint model, int color = 1)
        {
            Model = model;
            Color = new SolidColorBrush(GetColorFromDrawingColor(GlobalSettings.GlobalSettings.ColorSettings.GetColorFromId(color)));
        }

        private Color GetColorFromDrawingColor(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
        }

        public void UpdatePositions(double imageWidth, double imageHeight)
        {
            CanvasWidth = imageWidth;
            CanvasHeight = imageHeight;
        }
    }
}
