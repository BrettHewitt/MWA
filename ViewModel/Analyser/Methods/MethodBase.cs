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
using System.Windows.Controls;
using RobynsWhiskerTracker.ModelInterface;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods
{
    public abstract class MethodBase : ViewModelBase
    {
        private string m_MethodName;
        private UserControl m_MethodControl;
        private bool m_Loaded = false;
        private bool m_ShowFrameSlider = true;

        public string MethodName
        {
            get
            {
                return m_MethodName;
            }
            set
            {
                if (Equals(m_MethodName, value))
                {
                    return;
                }

                m_MethodName = value;

                NotifyPropertyChanged();
            }
        }

        public UserControl MethodControl
        {
            get
            {
                return m_MethodControl;
            }
            set
            {
                if (Equals(m_MethodControl, value))
                {
                    return;
                }

                m_MethodControl = value;

                NotifyPropertyChanged();
            }
        }

        public bool Loaded
        {
            get
            {
                return m_Loaded;
            }
            set
            {
                if (Equals(m_Loaded, value))
                {
                    return;
                }

                m_Loaded = value;

                NotifyPropertyChanged();
            }
        }

        public bool ShowFrameSlider
        {
            get
            {
                return m_ShowFrameSlider;
            }
            set
            {
                if (Equals(m_ShowFrameSlider, value))
                {
                    return;
                }

                m_ShowFrameSlider = value;

                NotifyPropertyChanged();
            }
        }

        public AnalyserViewModel Parent
        {
            get;
            private set;
        }

        protected MethodBase(AnalyserViewModel parent, string methodName)
        {
            MethodName = methodName;
            Parent = parent;
        }

        public virtual void PropagateFrameChangedNotifications(int indexNumber) {}

        public virtual void LoadData()
        {
            Loaded = true;
        }

        public abstract object[,] ExportResults();

        public virtual object[,] ExportMeanResults()
        {
            return null;
        }

        public abstract void PropagateWhiskerEnabledNotification(int whiskerId, bool value);
    }
}
