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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.ProtractionRetraction;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.ProtractionRetraction
{
    internal class RetractionData : ProtractionRetractionBase, IRetractionData
    {
        public override string Name
        {
            get
            {
                return "Retraction";
            }
        }

        protected override double CalculateMean()
        {
            if (DeltaTime == 0)
            {
                return 0;
            }

            double deltaAngle = MinAngle - MaxAngle;

            return deltaAngle/DeltaTime;
        }
    }
}
