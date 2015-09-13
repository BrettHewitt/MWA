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
using Microsoft.Office.Interop.Excel;
using RobynsWhiskerTracker.Extension;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.HeadOrientation;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using Point = System.Windows.Point;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods.HeadOrientation
{
    public class SingleHeadOrientationViewModel : SingleWhiskerBase
    {
        private double m_ImageWidth;
        private double m_ImageHeight;
        private double m_VideoWidth;
        private double m_VideoHeight;

        private ISingleHeadOrientation m_Model;

        private Point m_NosePoint;
        private Point m_OrientationPoint;

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

        public ISingleHeadOrientation Model
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

        public override int WhiskerId
        {
            get
            {
                return -1;
            }
        }

        public Point NosePoint
        {
            get
            {
                return m_NosePoint;
            }
            set
            {
                if (Equals(m_NosePoint, value))
                {
                    return;
                }

                m_NosePoint = value;

                NotifyPropertyChanged();
            }
        }

        public Point OrientationPoint
        {
            get
            {
                return m_OrientationPoint;
            }
            set
            {
                if (Equals(m_OrientationPoint, value))
                {
                    return;
                }

                m_OrientationPoint = value;

                NotifyPropertyChanged();
            }
        }

        public double Orientation
        {
            get
            {
                return Model.Orientation;
            }
        }

        public SingleHeadOrientationViewModel(ISingleHeadOrientation model)
        {
            Model = model;
            VideoWidth = Model.NoseWhisker.Parent.OriginalWidth;
            VideoHeight = Model.NoseWhisker.Parent.OriginalHeight;
        }

        private void CreateData()
        {
            IWhisker noseWhisker = Model.NoseWhisker;
            IWhisker orientationWhisker = Model.OrientationWhisker;
            if (noseWhisker == null || orientationWhisker == null)
            {
                return;
            }

            IWhiskerPoint nose = noseWhisker.WhiskerPoints[0];
            NosePoint = new Point(nose.XRatio * ImageWidth, nose.YRatio * ImageHeight);

            IWhiskerPoint orientation = orientationWhisker.WhiskerPoints[0];
            OrientationPoint = new Point(orientation.XRatio * ImageWidth, orientation.YRatio * ImageHeight);
        }

        public void UpdatePositions(double imageWidth, double imageHeight)
        {
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
        }
    }
}
