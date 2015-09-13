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
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using RobynsWhiskerTracker.ViewModel.WhiskerPoint;

namespace RobynsWhiskerTracker.ViewModel.Whisker
{
    public class WhiskerViewModel : ViewModelBase
    {
        private ObservableCollection<WhiskerPointViewModel> m_WhiskerPoints;

        public int WhiskerId
        {
            get
            {
                return Model.WhiskerId;
            }
            set
            {
                Model.WhiskerId = value;
            }
        }

        public string WhiskerName
        {
            get
            {
                return Model.WhiskerName;
            }
            set
            {
                Model.WhiskerName = value;
            }
        }

        public bool IsGenericPoint
        {
            get
            {
                return Model.IsGenericPoint;
            }
            set
            {
                Model.IsGenericPoint = value;
            }
        }

        public IWhisker Model
        {
            get;
            set;
        }

        public ObservableCollection<WhiskerPointViewModel> WhiskerPoints
        {
            get
            {
                return m_WhiskerPoints;
            }
            set
            {
                if (ReferenceEquals(m_WhiskerPoints, value))
                {
                    return;
                }

                m_WhiskerPoints = value;

                NotifyPropertyChanged();
            }
        }

        public WhiskerViewModel(IWhisker model)
        {
            Model = model;
            WhiskerPoints = new ObservableCollection<WhiskerPointViewModel>(Model.WhiskerPoints.Select(x => x != null ? new WhiskerPointViewModel(x, this) : null));
        }

        public void UpdatePositions(double actualWidth, double actualHeight)
        {
            foreach (WhiskerPointViewModel whiskerPointViewModel in WhiskerPoints)
            {
                if (whiskerPointViewModel != null)
                {
                    whiskerPointViewModel.UpdatePositions(actualWidth, actualHeight);
                }
            }
        }

        public void SaveWhiskerPoints()
        {
            Model.WhiskerPoints = WhiskerPoints.Select(x => x.Model).ToArray();
        }
    }
}
