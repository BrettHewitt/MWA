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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Curvature;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerCurve;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods
{
    public class WhiskersEnabledViewModel : ViewModelBase
    {
        private bool m_Enabled = true;
        private SingleWhiskerBase m_Whisker;

        public bool Enabled
        {
            get
            {
                return m_Enabled;
            }
            set
            {
                if (Equals(m_Enabled, value))
                {
                    return;
                }

                m_Enabled = value;

                Parent.PropagateWhiskerEnabledNotification(Whisker.WhiskerId, Enabled);

                NotifyPropertyChanged();
            }
        }

        public SingleWhiskerBase Whisker
        {
            get
            {
                return m_Whisker;
            }
            set
            {
                if (Equals(m_Whisker, value))
                {
                    return;
                }

                m_Whisker = value;

                NotifyPropertyChanged();
            }
        }

        public MethodBase Parent
        {
            get;
            set;
        }

        public WhiskersEnabledViewModel(MethodBase parent, SingleWhiskerBase whisker)
        {
            Parent = parent;
            Whisker = whisker;
        }
    }
}
