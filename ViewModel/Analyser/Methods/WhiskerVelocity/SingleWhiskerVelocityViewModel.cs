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

using System.Drawing;
using RobynsWhiskerTracker.Events.SingleWhiskerEnabled;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Velocity;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using Point = System.Windows.Point;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerVelocity
{
    public class SingleWhiskerVelocityViewModel : SingleWhiskerBase
    {
        private double m_ImageWidth;
        private double m_ImageHeight;
        private double m_VideoWidth;
        private double m_VideoHeight;
        
        private ISingleWhiskerVelocity m_Model;

        private Point m_VelocityPoint;
        private Vector m_VelocityDirection;

        public event SingleWhiskerEnabledHandler HeadCompensationChanged;

        public Vector Velocity
        {
            get
            {
                return Model.Velocity;
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

        public bool CompensateHeadMovement
        {
            get
            {
                return Model.CompensateForHeadMovement;
            }
            set
            {
                if (Equals(Model.CompensateForHeadMovement, value))
                {
                    return;
                }

                Model.CompensateForHeadMovement = value;

                CreateData();

                if (HeadCompensationChanged != null)
                {
                    HeadCompensationChanged(this, new SingleWhiskerEnabledChangeEventArgs(WhiskerId, value));
                }

                NotifyPropertyChanged();
                NotifyPropertyChanged("DisplayVelocity");
            }
        }

        public bool IsHeadPoint
        {
            get
            {
                return Model.IsHeadPoint;
            }
        }

        public ISingleWhiskerVelocity Model
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

        public Point VelocityPoint
        {
            get
            {
                return m_VelocityPoint;
            }
            set
            {
                if (Equals(m_VelocityPoint, value))
                { 
                    return;
                }

                m_VelocityPoint = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged("XCanvas");
                NotifyPropertyChanged("YCanvas");
                NotifyPropertyChanged("VelocityDirectionPoint");
            }
        }

        public Vector VelocityDirection
        {
            get
            {
                return m_VelocityDirection;
            }
            set
            {
                if (Equals(m_VelocityDirection, value))
                {
                    return;
                }

                m_VelocityDirection = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged("VelocityDirectionPoint");
            }
        }

        public Point VelocityDirectionPoint
        {
            get
            {
                return new Point(VelocityPoint.X + VelocityDirection.X, VelocityPoint.Y + VelocityDirection.Y);
            }
        }
        
        public string DisplayVelocity
        {
            get
            {
                return Model.DisplayVelocity;
            }
        }

        private double VideoWidth
        {
            get;
            set;
        }

        private double VideoHeight
        {
            get;
            set;
        }

        public double Size
        {
            get
            {
                return 5;
            }
        }

        public double XCanvas
        {
            get
            {
                return VelocityPoint.X - (Size / 2);
            }
        }

        public double YCanvas
        {
            get
            {
                return VelocityPoint.Y -(Size / 2);
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

        public SingleWhiskerVelocityViewModel(ISingleWhiskerVelocity model)
        {
            Model = model;
            VideoWidth = Model.Whisker.Parent.Frame.Width;
            VideoHeight = Model.Whisker.Parent.Frame.Height;
        }

        private void CreateData()
        {
            if (Model == null || ImageWidth == 0 || ImageHeight == 0)
            {
                return;
            }

            IWhiskerPoint velocityPoint = Model.VelocityPoint;
            VelocityPoint = new Point(velocityPoint.XRatio * ImageWidth, velocityPoint.YRatio * ImageHeight);
            VelocityDirection = ModifyVideoToImageVector(Model.Velocity);
        }

        public void UpdatePositions(double imageWidth, double imageHeight)
        {
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
        }

        private Vector ModifyVideoToImageVector(Vector vector)
        {
            return new Vector(ModifyVideoToImageWidth(vector.X), ModifyVideoToImageHeight(vector.Y));
        }

        private double ModifyVideoToImageWidth(double value)
        {
            return (value/VideoWidth) * ImageWidth;
        }

        private double ModifyVideoToImageHeight(double value)
        {
            return (value/VideoHeight) * ImageHeight;
        }
    }
}
