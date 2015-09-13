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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;

namespace RobynsWhiskerTracker.Model.Analyser.Methods
{
    internal abstract class MethodBase<T> : ModelObjectBase, IMethodBase<T>
    {
        private string m_Name;
        private T[] m_Frames; 

        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                if (Equals(m_Name, value))
                {
                    return;
                }

                m_Name = value;

                MarkAsDirty();
            }
        }

        public T[] Frames
        {
            get
            {
                return m_Frames;
            }
            protected set
            {
                if (ReferenceEquals(m_Frames, value))
                {
                    return;
                }

                m_Frames = value;

                MarkAsDirty();
            }
        }

        protected MethodBase(string name)
        {
            Name = name;
        }

        public void LoadData(IMouseFrame[] frames)
        {
            Frames = CreateData(frames);

            DataLoadComplete();
        }

        protected abstract T[] CreateData(IMouseFrame[] frames);

        public abstract object[][] ExportData();
        public abstract object[][] ExportMeanData();
    }
}
