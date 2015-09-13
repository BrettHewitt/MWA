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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.AngleTypes;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngularVelocity;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Settings;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Angles.WhiskerAngularVelocity
{
    internal class WhiskerAngularVelocity : MethodBase<IWhiskerAngularVelocityFrame>, IWhiskerAngularVelocity
    {
        public WhiskerAngularVelocity() : base("Angular Velocity")
        {
        }

        protected override IWhiskerAngularVelocityFrame[] CreateData(IMouseFrame[] frames)
        {
            int frameCount = frames.Length;

            IWhiskerAngularVelocityFrame[] data = new IWhiskerAngularVelocityFrame[frameCount];
            IWhiskerAngularVelocityFrame previousFrame = null;
            for (int i = 0; i < frameCount; i++)
            {
                IWhiskerAngularVelocityFrame frameData = ModelResolver.Resolve<IWhiskerAngularVelocityFrame>();
                IFrameRateSettings frameRateSettings = GlobalSettings.GlobalSettings.FrameRateSettings;

                frameData.LoadData(frames[i], previousFrame, frameRateSettings);

                data[frames[i].IndexNumber] = frameData;

                previousFrame = frameData;
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

            data.Add(Frames[0].Targets.Select(target => target.Whisker.WhiskerName).Cast<object>().ToArray());
            data.AddRange(Frames.Select(frame => frame.ExportData()));

            return data.ToArray();
        }

        public void UpdateTValue(double tValue)
        {
            foreach (var frame in Frames)
            {
                foreach (var whisker in frame.Targets)
                {
                    whisker.TValue = tValue;
                }
            }
        }

        public void UpdateAngleType(IAngleTypeBase angleType)
        {
            foreach (var frame in Frames)
            {
                frame.UpdateAngleType(angleType);
            }
        }

        public override object[][] ExportMeanData()
        {
            return null;
        }
    }
}
