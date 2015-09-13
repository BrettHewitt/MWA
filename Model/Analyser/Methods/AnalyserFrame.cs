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

using RobynsWhiskerTracker.ModelInterface.Analyser.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;

namespace RobynsWhiskerTracker.Model.Analyser.Methods
{
    internal abstract class AnalyserFrame<T> : ModelObjectBase, IAnalyserFrame<T>
    {
        private string m_Name;
        private List<T> m_Targets;
        private IMouseFrame m_TargetFrame;

        public string Name
        {
            get
            {
                return m_Name;
            }
            protected set
            {
                if (Equals(m_Name, value))
                {
                    return;
                }

                m_Name = value;

                MarkAsDirty();
            }
        }

        public List<T> Targets
        {
            get
            {
                return m_Targets;
            }
            set
            {
                if (ReferenceEquals(m_Targets, value))
                {
                    return;
                }

                m_Targets = value;

                MarkAsDirty();
            }
        }

        public IMouseFrame TargetFrame
        {
            get
            {
                return m_TargetFrame;
            }
            set
            {
                if (Equals(m_TargetFrame, value))
                {
                    return;
                }

                m_TargetFrame = value;

                MarkAsDirty();
            }
        }

        protected AnalyserFrame()
        {
            Targets = new List<T>();
        }

        public abstract object[] ExportData();
    }
}
