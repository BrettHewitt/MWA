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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobynsWhiskerTracker.Events.SingleWhiskerEnabled;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngle;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Spread;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerAngle;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods.Spread
{
    public class WhiskerSpreadFrameViewModel : ViewModelBase
    {
        private IWhiskerSpreadFrame m_Model;
        private ObservableCollection<SingleWhiskerAngleViewModel> m_Whiskers;

        public IWhiskerSpreadFrame Model
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

        public ObservableCollection<SingleWhiskerAngleViewModel> Whiskers
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

        public double LeftAverage
        {
            get
            {
                return Model.LeftAverage;
            }
        }

        public double LeftMax
        {
            get
            {
                return Model.LeftMax;
            }
        }

        public double RightAverage
        {
            get
            {
                return Model.RightAverage;
            }
        }

        public double RightMax
        {
            get
            {
                return Model.RightMax;
            }
        }

        public WhiskerSpreadViewModel Parent
        {
            get;
            set;
        }

        public WhiskerSpreadFrameViewModel(IWhiskerSpreadFrame model, WhiskerSpreadViewModel parent)
        {
            Model = model;
            Parent = parent;
            CreateData();
        }

        private void CreateData()
        {
            ObservableCollection<SingleWhiskerAngleViewModel> whiskers = new ObservableCollection<SingleWhiskerAngleViewModel>();
            foreach (ISingleWhiskerAngle whisker in Model.Targets)
            {
                SingleWhiskerAngleViewModel singleWhiskerAngleViewModel = new SingleWhiskerAngleViewModel(whisker);
                singleWhiskerAngleViewModel.EnabledChanged += PropagateWhiskerEnabledNotification;
                whiskers.Add(singleWhiskerAngleViewModel);
            }

            Whiskers = whiskers;
        }

        private void PropagateWhiskerEnabledNotification(object sender, SingleWhiskerEnabledChangeEventArgs e)
        {
            Parent.PropagateWhiskerEnabledNotification(e.WhiskerId, e.Enabled);
        }

        public void UpdatePositions(double width, double height)
        {
            foreach (SingleWhiskerAngleViewModel whisker in Whiskers)
            {
                whisker.UpdatePositions(width, height);
            }
        }

        public void UpdateTValue(double tValue)
        {
            foreach (SingleWhiskerAngleViewModel whisker in Whiskers)
            {
                whisker.TValue = tValue;
            }

            NotifyDataChanged();
        }

        public void NotifyDataChanged()
        {
            NotifyPropertyChanged("LeftAverage");
            NotifyPropertyChanged("LeftMax");
            NotifyPropertyChanged("RightMax");
            NotifyPropertyChanged("RightAverage");
        }
    }
}
