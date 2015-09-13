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

using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Frequency.FrequencyTypes;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Frequency
{
    public interface ISingleWhiskerFrequency : IModelObjectBase
    {
        IWhisker Whisker
        {
            get;
            set;
        }

        int WhiskerId
        {
            get;
        }

        double Frequency
        {
            get;
        }

        double[] Signal
        {
            get;
            set;
        }

        IFrequencyTypeBase FrequencyType
        {
            get;
            set;
        }

        double FrameRate
        {
            get;
            set;
        }

        double FrameInterval
        {
            get;
            set;
        }

        Dictionary<double, double> ExtraData
        {
            get;
        }
    }
}
