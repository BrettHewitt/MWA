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

using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.AngleTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Angles.AngleTypes
{
    internal class Vertical : AngleTypeBase, IVertical
    {
        public Vertical() : base ("Vertical")
        {

        }

        public override double CalculateAngle(Vector vector)
        {
            Vector horizontalVector = new Vector(0, 1);
            return Vector.AngleBetween(horizontalVector, vector);
        }

        public override IAngleTypeBase GetInstance()
        {
            return ModelResolver.Resolve<IVertical>();
        }
    }
}
