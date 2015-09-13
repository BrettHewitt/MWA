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

using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.HeadOrientation;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.HeadOrientation
{
    internal class HeadOrientation : MethodBase<IHeadOrientationFrame>, IHeadOrientation
    {
        public HeadOrientation() : base("Head Orientation")
        {
            
        }

        protected override IHeadOrientationFrame[] CreateData(IMouseFrame[] frames)
        {
            int frameCount = frames.Length;

            IHeadOrientationFrame[] data = new IHeadOrientationFrame[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                IHeadOrientationFrame frameData = ModelResolver.Resolve<IHeadOrientationFrame>();
                frameData.LoadData(frames[i]);

                data[frames[i].IndexNumber] = frameData;
            }

            return data;
        }

        public override object[][] ExportData()
        {
            if (Frames == null)
            {
                return null;
            }

            if (Frames.Length == 0)
            {
                return null;
            }

            List<object[]> data = new List<object[]>();

            data.Add(new object[]{"Head Orientation"});

            data.AddRange(Frames.Select(frame => frame.ExportData()));

            return data.ToArray();
        }

        public override object[][] ExportMeanData()
        {
            return null;
        }
    }
}
