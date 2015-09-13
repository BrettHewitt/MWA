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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Velocity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerVelocity
{
    public class WhiskerVelocityFrameViewModel : ViewModelBase
    {
        private IWhiskerVelocityFrame m_Model;
        private ObservableCollection<SingleWhiskerVelocityViewModel> m_Whiskers;

        public IWhiskerVelocityFrame Model
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

        public ObservableCollection<SingleWhiskerVelocityViewModel> Whiskers
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

        public WhiskerVelocityViewModel Parent
        {
            get;
            set;
        }

        public WhiskerVelocityFrameViewModel(IWhiskerVelocityFrame model, WhiskerVelocityViewModel parent)
        {
            Model = model;
            Parent = parent;
            CreateData();
        }

        public void CreateData()
        {
            ObservableCollection<SingleWhiskerVelocityViewModel> whiskers = new ObservableCollection<SingleWhiskerVelocityViewModel>();
            foreach (ISingleWhiskerVelocity whisker in Model.Targets)
            {
                SingleWhiskerVelocityViewModel singleWhiskerCurve = new SingleWhiskerVelocityViewModel(whisker);
                singleWhiskerCurve.EnabledChanged += PropagateWhiskerEnabledNotification;
                singleWhiskerCurve.HeadCompensationChanged += PropagateHeadCompensationNotification;
                whiskers.Add(singleWhiskerCurve);
            }

            Whiskers = whiskers;
        }

        private void PropagateWhiskerEnabledNotification(object sender, SingleWhiskerEnabledChangeEventArgs e)
        {
            Parent.PropagateWhiskerEnabledNotification(e.WhiskerId, e.Enabled);
        }

        private void PropagateHeadCompensationNotification(object sender, SingleWhiskerEnabledChangeEventArgs e)
        {
            Parent.PropagateHeadCompensationNotification(e.WhiskerId, e.Enabled);
        }

        public void UpdatePositions(double width, double height)
        {
            foreach (SingleWhiskerVelocityViewModel whisker in Whiskers)
            {
                whisker.UpdatePositions(width, height);
            }
        }
    }
}
