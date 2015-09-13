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
using Emgu.CV;
using Emgu.CV.Structure;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using RobynsWhiskerTracker.ViewModel.Whisker;
using RobynsWhiskerTracker.ViewModel.WhiskerPoint;

namespace RobynsWhiskerTracker.ViewModel
{
    public class MouseFrameViewModel : ViewModelBase, IDisposable
    {
        private WhiskerViewModel[] m_Whiskers;

        public IMouseFrame Model
        {
            get;
            set;
        }

        public int FrameNumber
        {
            get
            {
                return Model.FrameNumber;
            }
            set
            {
                if (Equals(FrameNumber, value))
                {
                    return;
                }

                Model.FrameNumber = value;

                NotifyPropertyChanged();
            }
        }

        public int IndexNumber
        {
            get
            {
                return Model.IndexNumber;
            }
            set
            {
                if (Equals(IndexNumber, value))
                {
                    return;
                }

                Model.IndexNumber = value;

                NotifyPropertyChanged();
            }
        }

        public Image<Bgr, Byte> Frame
        {
            get
            {
                return Model.Frame;
            }
            set
            {
                if (Equals(Frame, value))
                {
                    return;
                }

                Model.Frame = value;

                NotifyPropertyChanged();
            }
        }

        public WhiskerViewModel[] Whiskers
        {
            get
            {
                return m_Whiskers;
            }
            set
            {
                if (ReferenceEquals(m_Whiskers, value))
                {
                    return;
                }

                m_Whiskers = value;

                NotifyPropertyChanged();
            }
        }

        public MouseFrameViewModel(IMouseFrame model)
        {
            Model = model;
            if (Model.Whiskers != null)
            {
                Whiskers = model.Whiskers.Select(x => new WhiskerViewModel(x)).ToArray();
            }
        }

        public void Dispose()
        {
            if (Model != null)
            {
                Model.Dispose();
            }
        }

        public void SaveWhiskers()
        {
            foreach (WhiskerViewModel whiskerViewModel in Whiskers)
            {
                whiskerViewModel.SaveWhiskerPoints();
            }
        }

        public bool Validate()
        {
            if (Whiskers == null)
            {
                return false;
            }

            if (Whiskers.Any(whisker => whisker.WhiskerPoints.Any(whiskerPoint => !whiskerPoint.PointPlaced)))
            {
                return false;
            }

            return true;
        }

        public bool Validate(ref string message)
        {
            if (Whiskers == null)
            {
                message = "Whiskers not set";
                return false;
            }

            if (Whiskers.Any(whisker => whisker.WhiskerPoints.Any(whiskerPoint => !whiskerPoint.PointPlaced)))
            {
                message = "Whisker point missing";
                return false;
            }

            return true;
        }
    }
}
