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
using RobynsWhiskerTracker.ModelInterface.ClipSettings;

namespace RobynsWhiskerTracker.ViewModel.ClipSettings
{
    public class ClipSettingsViewModel : ViewModelBase
    {
        public IClipSettings Model
        {
            get;
            set;
        }

        public int StartFrame
        {
            get
            {
                return Model.StartFrame;
            }
            set
            {
                if (Equals(StartFrame, value))
                {
                    return;
                }

                Model.StartFrame = value;

                NotifyPropertyChanged();
            }
        }

        public int EndFrame
        {
            get
            {
                return Model.EndFrame;
            }
            set
            {
                if (Equals(EndFrame, value))
                {
                    return;
                }

                Model.EndFrame = value;

                NotifyPropertyChanged();
            }
        }

        public int FrameInterval
        {
            get
            {
                return Model.FrameInterval;
            }
            set
            {
                if (Equals(FrameInterval, value))
                {
                    return;
                }

                Model.FrameInterval = value;

                NotifyPropertyChanged();
            }
        }

        public int NumberOfWhiskers
        {
            get
            {
                return Model.NumberOfWhiskers;
            }
            set
            {
                if (Equals(NumberOfWhiskers, value))
                {
                    return;
                }

                Model.NumberOfWhiskers = value;

                NotifyPropertyChanged();
            }
        }

        public int NumberOfPointsPerWhisker
        {
            get
            {
                return Model.NumberOfPointsPerWhisker;
            }
            set
            {
                if (Equals(NumberOfPointsPerWhisker, value))
                {
                    return;
                }

                Model.NumberOfPointsPerWhisker = value;

                NotifyPropertyChanged();
            }
        }

        public bool IncludeNosePoint
        {
            get
            {
                return Model.IncludeNosePoint;
            }
            set
            {
                if (Equals(IncludeNosePoint, value))
                {
                    return;
                }

                Model.IncludeNosePoint = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged("IncludeOrientationPoint");
            }
        }

        public bool IncludeOrientationPoint
        {
            get
            {
                return Model.IncludeOrientationPoint;
            }
            set
            {
                if (Equals(IncludeOrientationPoint, value))
                {
                    return;
                }

                Model.IncludeOrientationPoint = value;

                NotifyPropertyChanged();
            }
        }

        public int NumberOfGenericPoints
        {
            get
            {
                return Model.NumberOfGenericPoints;
            }
            set
            {
                if (Equals(NumberOfGenericPoints))
                {
                    return;
                }

                Model.NumberOfGenericPoints = value;

                NotifyPropertyChanged();
            }
        }
        

        public ClipSettingsViewModel(IClipSettings model)
        {
            Model = model;
        }
    }
}
