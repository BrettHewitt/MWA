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

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods.AnalyseEverything
{
    public class AnalyseEverythingBaseViewModel : ViewModelBase
    {
        private bool m_IsEnabled = true;
        private MethodBase m_Method;

        public bool IsEnabled
        {
            get
            {
                return m_IsEnabled;
            }
            set
            {
                if (Equals(m_IsEnabled, value))
                {
                    return;
                }

                m_IsEnabled = value;

                NotifyPropertyChanged();
            }
        }

        public MethodBase Method
        {
            get
            {
                return m_Method;
            }
            set
            {
                if (Equals(m_Method, value))
                {
                    return;
                }

                m_Method = value;

                NotifyPropertyChanged();
            }
        }

        public AnalyseEverythingViewModel Parent
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                return Method.MethodName;
            }
        }

        public AnalyseEverythingBaseViewModel(AnalyseEverythingViewModel parent, MethodBase method)
        {
            Parent = parent;
            Method = method;
        }
    }
}
