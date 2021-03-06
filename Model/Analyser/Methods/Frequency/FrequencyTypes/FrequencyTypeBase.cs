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

using System.Collections.Generic;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Frequency.FrequencyTypes;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Frequency.FrequencyTypes
{
    internal abstract class FrequencyTypeBase : ModelObjectBase, IFrequencyTypeBase
    {
        private string m_Name;

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

        protected FrequencyTypeBase(string name)
        {
            Name = name;
        }

        public abstract double CalculateFrequency(double[] signal, double frameRate, double frameInterval);

        public virtual Dictionary<double, double> GetExtraData()
        {
            return null;
        }

        public virtual string[] GetExtraDataNames()
        {
            return null;
        }
    }
}
