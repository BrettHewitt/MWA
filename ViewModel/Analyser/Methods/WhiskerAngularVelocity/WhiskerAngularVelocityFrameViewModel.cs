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

using RobynsWhiskerTracker.Events.SingleWhiskerEnabled;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngularVelocity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerAngle.AngleTypes;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerAngularVelocity
{
    public class WhiskerAngularVelocityFrameViewModel : ViewModelBase
    {
        private IWhiskerAngularVelocityFrame m_Model;
        private ObservableCollection<SingleWhiskerAngularVelocityViewModel> m_Whiskers;

        public IWhiskerAngularVelocityFrame Model
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

        public ObservableCollection<SingleWhiskerAngularVelocityViewModel> Whiskers
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

        public WhiskerAngularVelocityViewModel Parent
        {
            get;
            set;
        }

        public WhiskerAngularVelocityFrameViewModel(IWhiskerAngularVelocityFrame model, WhiskerAngularVelocityViewModel parent)
        {
            Parent = parent;
            Model = model;
            CreateData();
        }

        public void CreateData()
        {
            ObservableCollection<SingleWhiskerAngularVelocityViewModel> whiskers = new ObservableCollection<SingleWhiskerAngularVelocityViewModel>();
            foreach (ISingleWhiskerAngularVelocity whisker in Model.Targets)
            {
                SingleWhiskerAngularVelocityViewModel singleWhiskerAngularVelocity = new SingleWhiskerAngularVelocityViewModel(whisker);
                singleWhiskerAngularVelocity.EnabledChanged += PropagateWhiskerEnabledNotification;
                whiskers.Add(singleWhiskerAngularVelocity);
            }

            Whiskers = whiskers;
        }

        private void PropagateWhiskerEnabledNotification(object sender, SingleWhiskerEnabledChangeEventArgs e)
        {
            Parent.PropagateWhiskerEnabledNotification(e.WhiskerId, e.Enabled);
        }

        public void UpdatePositions(double width, double height)
        {
            foreach (SingleWhiskerAngularVelocityViewModel whisker in Whiskers)
            {
                whisker.UpdatePositions(width, height);
            }
        }

        public void UpdateTValue(double tValue)
        {
            foreach (SingleWhiskerAngularVelocityViewModel whisker in Whiskers)
            {
                whisker.UpdateTValue(tValue);
            }
        }

        public void UpdateAngleType(AngleTypeBase angleMethod)
        {
            //Model.UpdateAngleType(angleMethod.Model);

            foreach (SingleWhiskerAngularVelocityViewModel whisker in Whiskers)
            {
                whisker.NotifyAngularVelocityPropertyChanged();
            }
        }
    }
}
