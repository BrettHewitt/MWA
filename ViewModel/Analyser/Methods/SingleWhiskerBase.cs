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
using RobynsWhiskerTracker.Events.SingleWhiskerEnabled;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods
{
    public abstract class SingleWhiskerBase : ViewModelBase
    {
        private bool m_Enabled = true;
        public event SingleWhiskerEnabledHandler EnabledChanged;

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

                if (EnabledChanged != null)
                {
                    EnabledChanged(this, new SingleWhiskerEnabledChangeEventArgs(WhiskerId, Enabled));
                }

                NotifyPropertyChanged();
            }
        }

        public abstract int WhiskerId
        {
            get;
        }
    }
}
