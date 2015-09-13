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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Amplitude;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerAmplitude
{
    public class SingleWhiskerAmplitudeViewModel : SingleWhiskerBase
    {
        private ISingleWhiskerAmplitude m_Model;

        public ISingleWhiskerAmplitude Model
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
            }
        }

        public string WhiskerName
        {
            get
            {
                return Model.Whisker.WhiskerName;
            }
        }

        public double MinAngle
        {
            get
            {
                return Model.MinAngle;
            }
        }

        public double MaxAngle
        {
            get
            {
                return Model.MaxAngle;
            }
        }

        public double MaxAmplitude
        {
            get
            {
                return Model.MaxAmplitude;
            }
        }

        public double CurrentMinAngle
        {
            get
            {
                return Model.CurrentMinAngle;
            }
        }

        public double CurrentMaxAngle
        {
            get
            {
                return Model.CurrentMaxAngle;
            }
        }

        public double CurrentAmplitude
        {
            get
            {
                return Model.CurrentAmplitude;
            }
        }

        public double MeanAmplitude
        {
            get
            {
                return Model.AverageAmplitude;
            }
        }

        public double RMS
        {
            get
            {
                return Model.RMS;
            }
        }

        public override int WhiskerId
        {
            get
            {
                return Model.Whisker.WhiskerId;
            }
        }

        public SingleWhiskerAmplitudeViewModel(ISingleWhiskerAmplitude model)
        {
            Model = model;
        }

        public void NotifyAmplitudeChanged()
        {
            NotifyPropertyChanged("CurrentAmplitude");
            NotifyPropertyChanged("CurrentMaxAngle");
            NotifyPropertyChanged("CurrentMinAngle");
            NotifyPropertyChanged("MaxAmplitude");
            NotifyPropertyChanged("MaxAngle");
            NotifyPropertyChanged("MinAngle");
            NotifyPropertyChanged("RMS");
        }

        public void UpdateFrameNumber(int frameNumber)
        {
            Model.UpdateFrameNumber(frameNumber);
            NotifyAmplitudeChanged();
        }
    }
}
