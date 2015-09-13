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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngularVelocity;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.ProtractionRetraction;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods.ProtractionRetraction
{
    public class SingleWhiskerProtractionRetractionViewModel : SingleWhiskerBase
    {
        private ISingleWhiskerProtractionRetraction m_Model;
        private ProtractionRetractionDataBaseViewModel m_CurrentProtractionRetraction;

        public ISingleWhiskerProtractionRetraction Model
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

        public ProtractionRetractionDataBaseViewModel CurrentProtractionRetraction
        {
            get
            {
                return m_CurrentProtractionRetraction;
            }
            set
            {
                if (Equals(m_CurrentProtractionRetraction, value))
                {
                    return;
                }

                m_CurrentProtractionRetraction = value;

                NotifyPropertyChanged();
            }
        }

        public override int WhiskerId
        {
            get
            {
                return Model.WhiskerId;
            }
        }

        public string WhiskerName
        {
            get
            {
                return Model.Whisker.WhiskerName;
            }
        }

        public double MeanProtractionVelocity
        {
            get
            {
                return Model.MeanProtractionVelocity;
            }
        }

        public double MeanRetractionVelocity
        {
            get
            {
                return Model.MeanRetractionVelocity;
            }
        }

        public SingleWhiskerProtractionRetractionViewModel(ISingleWhiskerProtractionRetraction model)
        {
            Model = model;
        }

        public void NotifyProtractionRetractionChanged()
        {
            NotifyPropertyChanged("MeanProtractionVelocity");
            NotifyPropertyChanged("MeanRetractionVelocity");
        }

        public void UpdateFrameNumber(int frameNumber)
        {
            IProtractionRetractionBase data = Model.GetCurrentProtractionRetraction(frameNumber);

            CurrentProtractionRetraction = data == null ? null : new ProtractionRetractionDataBaseViewModel(data);
        }
    }
}
