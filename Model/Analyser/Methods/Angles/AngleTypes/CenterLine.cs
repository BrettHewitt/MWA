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
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Angles.AngleTypes
{
    internal class CenterLine : AngleTypeBase, ICenterLine
    {
        private IWhiskerPoint m_NosePoint;
        private IWhiskerPoint m_OrientationPoint;

        private Vector CenterLineVector
        {
            get;
            set;
        }

        public IWhiskerPoint NosePoint
        {
            get
            {
                return m_NosePoint;
            }
            set
            {
                if (Equals(m_NosePoint, value))
                {
                    return;
                }

                m_NosePoint = value;

                CreateCenterLine();
            }
        }

        public IWhiskerPoint OrientationPoint
        {
            get
            {
                return m_OrientationPoint;
            }
            set
            {
                if (Equals(m_OrientationPoint, value))
                {
                    return;
                }

                m_OrientationPoint = value;

                CreateCenterLine();
            }
        }

        public CenterLine() : base("Center Line")
        {

        }

        public override double CalculateAngle(Vector vector)
        {
            return Math.Abs(Vector.AngleBetween(CenterLineVector, vector));
        }

        public override void LoadData(IMouseFrame frame)
        {
            IWhisker nose = frame.Whiskers.FirstOrDefault(x => x.WhiskerId == -1);
            IWhisker orientation = frame.Whiskers.FirstOrDefault(x => x.WhiskerId == 0);

            if (nose == null || orientation == null)
            {
                return;
            }

            NosePoint = nose.WhiskerPoints[0];
            OrientationPoint = orientation.WhiskerPoints[0];
        }

        public override IAngleTypeBase GetInstance()
        {
            return ModelResolver.Resolve<ICenterLine>();
        }

        private void CreateCenterLine()
        {
            if (NosePoint == null || OrientationPoint == null)
            {
                return;
            }

            double originalWidth = NosePoint.Parent.Parent.OriginalWidth;
            double originalHeight = NosePoint.Parent.Parent.OriginalHeight;

            CenterLineVector = new Vector((OrientationPoint.XRatio * originalWidth) - (NosePoint.XRatio * originalWidth), (OrientationPoint.YRatio * originalHeight) - (NosePoint.YRatio * originalHeight));
        }
    }
}
