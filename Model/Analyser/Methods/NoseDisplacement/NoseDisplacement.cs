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
using RobynsWhiskerTracker.Extension;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.NoseDisplacement;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.NoseDisplacement
{
    internal class NoseDisplacement : ModelObjectBase, INoseDisplacement
    {
        private INoseDisplacementFrame[] m_Frames;
        private string m_Name;
        private double m_DistanceTravelled;

        public INoseDisplacementFrame[] Frames
        {
            get
            {
                return m_Frames;
            }
            private set
            {
                if (ReferenceEquals(m_Frames, value))
                {
                    return;
                }

                m_Frames = value;

                MarkAsDirty();
            }
        }

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

        public double DistanceTravelled
        {
            get
            {
                return m_DistanceTravelled;
            }
            set
            {
                if (Equals(m_DistanceTravelled, value))
                {
                    return;
                }

                m_DistanceTravelled = value;

                MarkAsDirty();
            }
        }

        public NoseDisplacement()
        {
            Name = "Nose Displacement";
        }

        public void LoadData(IMouseFrame[] frames)
        {
            if (frames == null)
            {
                throw new Exception("Frames can not be null");
            }

            if (frames.Length == 0)
            {
                throw new Exception("There must be at least one frame");
            }

            IWhisker nose = frames[0].Whiskers.FirstOrDefault(x => x.WhiskerId == -1);

            if (nose == null)
            {
                return;
            }

            Frames = new INoseDisplacementFrame[frames.Length];

            for (int i = 0; i < Frames.Length; i++)
            {
                INoseDisplacementFrame frame = ModelResolver.Resolve<INoseDisplacementFrame>();
                nose = frames[i].Whiskers.FirstOrDefault(x => x.WhiskerId == -1);
                frame.Nose = nose;
                Frames[i] = frame;
            }

            //Frames have been populated, figure out distance travelled
            double distanceCounter = 0;

            for (int i = 1; i < Frames.Length; i++)
            {
                Point previousPoint = Frames[i - 1].NoseLocation;
                Point currentPoint = Frames[i].NoseLocation;

                distanceCounter += currentPoint.Distance(previousPoint);
            }

            DistanceTravelled = distanceCounter;
        }

        public object[][] ExportData()
        {
            List<object[]> data = new List<object[]>();

            data.Add(new object[]{"Total Displacement", DistanceTravelled});

            return data.ToArray();
        }

        public object[][] ExportMeanData()
        {
            List<object[]> data = new List<object[]>();

            data.Add(new object[] { "Nose Displacement" });
            data.Add(new object[] { "Distance Travelled: ", DistanceTravelled });

            return data.ToArray();
        }
    }
}
