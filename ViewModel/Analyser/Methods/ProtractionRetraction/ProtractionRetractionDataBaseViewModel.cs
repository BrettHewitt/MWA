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

using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.ProtractionRetraction;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods.ProtractionRetraction
{
    public class ProtractionRetractionDataBaseViewModel : ViewModelBase
    {
        private IProtractionRetractionBase m_Model;

        public string Name
        {
            get
            {
                return Model.Name;
            }
        }

        public double MeanAngularVelocity
        {
            get
            {
                return Model.MeanAngularVelocity;
            }
        }

        public double Amplitude
        {
            get
            {
                return Model.Amplitude;
            }
        }

        public double MaxAngle
        {
            get
            {
                return Model.MaxAngle;
            }
        }

        public double MinAngle
        {
            get
            {
                return Model.MinAngle;
            }
        }

        public IProtractionRetractionBase Model
        {
            get
            {
                return m_Model;
            }
            private set
            {
                if (Equals(m_Model, value))
                {
                    return;
                }

                m_Model = value;

                NotifyPropertyChanged();
            }
        }

        public ProtractionRetractionDataBaseViewModel(IProtractionRetractionBase model)
        {
            Model = model;
        }
    }
}
