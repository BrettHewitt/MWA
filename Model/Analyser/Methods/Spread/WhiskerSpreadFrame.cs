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
using System.Windows;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.AngleTypes;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngle;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Spread;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Spread
{
    internal class WhiskerSpreadFrame : AnalyserFrame<ISingleWhiskerAngle>, IWhiskerSpreadFrame
    {
        private ISingleWhiskerAngle[] m_LeftWhiskers;
        private ISingleWhiskerAngle[] m_RightWhiskers;

        private double m_LeftAverage;
        private double m_LeftMax;
        private double m_RightAverage;
        private double m_RightMax;

        public ISingleWhiskerAngle[] LeftWhiskers
        {
            get
            {
                return m_LeftWhiskers;
            }
            private set
            {
                if (ReferenceEquals(m_LeftWhiskers, value))
                {
                    return;
                }

                m_LeftWhiskers = value;

                MarkAsDirty();
            }
        }

        public ISingleWhiskerAngle[] RightWhiskers
        {
            get
            {
                return m_RightWhiskers;
            }
            private set
            {
                if (ReferenceEquals(m_RightWhiskers, value))
                {
                    return;
                }

                m_RightWhiskers = value;

                MarkAsDirty();
            }
        }

        public double LeftAverage
        {
            get
            {
                return m_LeftAverage;
            }
            private set
            {
                if (Equals(m_LeftAverage, value))
                {
                    return;
                }

                m_LeftAverage = value;

                MarkAsDirty();
            }
        }

        public double LeftMax
        {
            get
            {
                return m_LeftMax;
            }
            private set
            {
                if (Equals(m_LeftMax, value))
                {
                    return;
                }

                m_LeftMax = value;

                MarkAsDirty();
            }
        }

        public double RightAverage
        {
            get
            {
                return m_RightAverage;
            }
            private set
            {
                if (Equals(m_RightAverage, value))
                {
                    return;
                }

                m_RightAverage = value;

                MarkAsDirty();
            }
        }

        public double RightMax
        {
            get
            {
                return m_RightMax;
            }
            private set
            {
                if (Equals(m_RightMax, value))
                {
                    return;
                }

                m_RightMax = value;

                MarkAsDirty();
            }
        }

        public WhiskerSpreadFrame()
        {
            Name = "Whisker Spread";
        }

        public override object[] ExportData()
        {
            return new object[] { LeftAverage, LeftMax, RightAverage, RightMax };
        }

        public void LoadData(IMouseFrame frame)
        {
            TargetFrame = frame;

            IAngleTypeBase angleType;
            IWhisker noseWhisker = TargetFrame.Whiskers.FirstOrDefault(x => x.WhiskerId == -1);
            IWhisker orientationWhisker = TargetFrame.Whiskers.FirstOrDefault(x => x.WhiskerId == 0);

            if (noseWhisker == null || orientationWhisker == null)
            {
                //can't generate centerline, don't let it work
                return;
            }
            
            ICenterLine centerLine = ModelResolver.Resolve<ICenterLine>();
            centerLine.NosePoint = noseWhisker.WhiskerPoints[0];
            centerLine.OrientationPoint = orientationWhisker.WhiskerPoints[0];
            angleType = centerLine;

            List<ISingleWhiskerAngle> leftWhiskers = new List<ISingleWhiskerAngle>();
            List<ISingleWhiskerAngle> rightWhiskers = new List<ISingleWhiskerAngle>();
            foreach (IWhisker whisker in TargetFrame.Whiskers)
            {
                if (whisker.IsGenericPoint)
                {
                    continue;
                }

                ISingleWhiskerAngle singleWhiskerAngle = ModelResolver.Resolve<ISingleWhiskerAngle>();
                singleWhiskerAngle.Whisker = whisker;
                singleWhiskerAngle.AngleType = angleType;
                Targets.Add(singleWhiskerAngle);

                //Is whisker on left or right?
                double x1 = centerLine.NosePoint.XRatio;
                double y1 = centerLine.NosePoint.YRatio;
                double x2 = centerLine.OrientationPoint.XRatio;
                double y2 = centerLine.OrientationPoint.YRatio;

                double x = singleWhiskerAngle.Whisker.WhiskerPoints[0].XRatio;
                double y = singleWhiskerAngle.Whisker.WhiskerPoints[0].YRatio;

                double determinant = ((x - x1)*(y2 - y1)) - ((y - y1)*(x2 - x1));
                
                if (determinant < 0)
                {
                    //On left side
                    leftWhiskers.Add(singleWhiskerAngle);
                }
                else
                {
                    //On right side
                    rightWhiskers.Add(singleWhiskerAngle);
                }
            }

            LeftWhiskers = leftWhiskers.ToArray();
            RightWhiskers = rightWhiskers.ToArray();
            CalculateData();
        }

        private void CalculateData()
        {
            CalculateLeftWhiskerSpread();
            CalculateRightWhiskerSpread();
        }

        private void CalculateLeftWhiskerSpread()
        {
            if (LeftWhiskers == null)
            {
                ZeroLeftData();
                return;
            }

            int numberOfLeftWhiskers = LeftWhiskers.Length;

            if (numberOfLeftWhiskers == 0)
            {
                ZeroLeftData();
                return;
            }

            double angleCounter = 0;
            double maxCounter = LeftWhiskers[0].Angle;
            double minCounter = LeftWhiskers[0].Angle;

            for (int i = 1; i < LeftWhiskers.Length; i++)
            {
                angleCounter += Math.Abs(LeftWhiskers[i].Angle - LeftWhiskers[i - 1].Angle);

                double absAngle = Math.Abs(LeftWhiskers[i].Angle);
                if (absAngle > maxCounter)
                {
                    maxCounter = absAngle;
                }

                if (absAngle < minCounter)
                {
                    minCounter = absAngle;
                }
            }

            LeftAverage = angleCounter / (numberOfLeftWhiskers - 1);
            LeftMax = maxCounter - minCounter;
        }

        private void CalculateRightWhiskerSpread()
        {
            if (RightWhiskers == null)
            {
                ZeroRightData();
                return;
            }

            int numberOfRightWhiskers = RightWhiskers.Length;

            if (numberOfRightWhiskers == 0)
            {
                ZeroRightData();
                return;
            }

            double angleCounter = 0;
            double maxCounter = RightWhiskers[0].Angle;
            double minCounter = RightWhiskers[0].Angle;

            for (int i = 1; i < RightWhiskers.Length; i++)
            {
                angleCounter += Math.Abs(RightWhiskers[i].Angle - RightWhiskers[i - 1].Angle);

                double absAngle = Math.Abs(RightWhiskers[i].Angle);
                if (absAngle > maxCounter)
                {
                    maxCounter = absAngle;
                }

                if (absAngle < minCounter)
                {
                    minCounter = absAngle;
                }
            }

            RightAverage = angleCounter / (numberOfRightWhiskers - 1);
            RightMax = maxCounter - minCounter;
        }

        private void ZeroLeftData()
        {
            LeftAverage = 0;
            LeftMax = 0;
        }

        private void ZeroRightData()
        {
            RightAverage = 0;
            RightMax = 0;
        }

        public void UpdateTValue(double tValue)
        {
            //foreach (ISingleWhiskerAngle whisker in Targets)
            //{
            //    whisker.TValue = tValue;
            //}

            CalculateData();
        }
    }
}
