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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngle;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Spread;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Spread
{
    internal class WhiskerSpread : MethodBase<IWhiskerSpreadFrame>, IWhiskerSpread
    {
        public WhiskerSpread() : base("Whisker Spread")
        {

        }

        public void UpdateTValue(double tValue)
        {
            foreach (IWhiskerSpreadFrame frame in Frames)
            {
                frame.UpdateTValue(tValue);
            }
        }

        protected override IWhiskerSpreadFrame[] CreateData(IMouseFrame[] frames)
        {
            int frameCount = frames.Length;

            IWhiskerSpreadFrame[] data = new IWhiskerSpreadFrame[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                IWhiskerSpreadFrame frameData = ModelResolver.Resolve<IWhiskerSpreadFrame>();
                frameData.LoadData(frames[i]);

                data[frames[i].IndexNumber] = frameData;
            }

            return data;
        }

        public override object[][] ExportData()
        {
            object[][] data = new object[Frames.Length + 6][];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new object[5];

                if (i < 6)
                {
                    continue;
                }

                object[] rowData = Frames[i - 6].ExportData();

                for (int j = 0; j < 5; j++)
                {
                    if (j == 0)
                    {
                        data[i][j] = i - 5;
                    }
                    else
                    {
                        data[i][j] = rowData[j - 1];
                    }
                }
            }

            object[][] meanData = ExportMeanData();
            data[0][0] = "Left Average: ";
            data[0][1] = meanData[0][1];

            data[1][0] = "Left Maximum: ";
            data[1][1] = meanData[1][1];

            data[2][0] = "Right Average: ";
            data[2][1] = meanData[2][1];

            data[3][0] = "Right Maximum: ";
            data[3][1] = meanData[3][1];

            data[5][0] = "Frame";
            data[5][1] = "Left Average";
            data[5][2] = "Left Maximum";
            data[5][3] = "Right Average";
            data[5][4] = "Right Maximum";

            return data;
        }

        public override object[][] ExportMeanData()
        {
            object[][] data = new object[4][];

            double leftAverageCounter = 0;
            double leftMaxCounter = 0;
            double rightAverageCounter = 0;
            double rightMaxCounter = 0;
            int frameCount = Frames.Length;

            for (int i = 0; i < frameCount; i++)
            {
                leftAverageCounter += Frames[i].LeftAverage;
                rightAverageCounter += Frames[i].RightAverage;
                leftMaxCounter += Frames[i].LeftMax;
                rightMaxCounter += Frames[i].RightMax;
            }

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new object[2];
            }

            data[0][0] = "Left Average: ";
            data[1][0] = "Left Maximum: ";
            data[2][0] = "Right Average: ";
            data[3][0] = "Right Maximum: ";

            data[0][1] = leftAverageCounter/frameCount;
            data[1][1] = leftMaxCounter/frameCount;
            data[2][1] = rightAverageCounter/frameCount;
            data[3][1] = rightMaxCounter/frameCount;

            return data;
        }
    }
}
