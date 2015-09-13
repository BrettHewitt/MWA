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
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Velocity;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Settings;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Velocity
{
    internal class WhiskerVelocityFrame : AnalyserFrame<ISingleWhiskerVelocity>, IWhiskerVelocityFrame
    {
        public WhiskerVelocityFrame()
        {
            Name = "Velocity";
        }

        public void LoadData(IMouseFrame frame, IWhiskerVelocityFrame previousFrame, IFrameRateSettings frameRateSettings, IUnitSettings unitSettings)
        {
            TargetFrame = frame;

            int numberOfWhiskers = TargetFrame.Whiskers.Length;

            int previousFrameCounter = 0;
            ISingleWhiskerVelocity nosePoint = null;
            for (int i = 0; i < numberOfWhiskers; i++)
            {
                IWhisker whisker = TargetFrame.Whiskers[i];

                ISingleWhiskerVelocity singleWhiskerVelocity = ModelResolver.Resolve<ISingleWhiskerVelocity>();
                singleWhiskerVelocity.Whisker = whisker;
                singleWhiskerVelocity.FrameRateSettings = frameRateSettings;
                singleWhiskerVelocity.UnitSettings = unitSettings;
                singleWhiskerVelocity.NosePoint = nosePoint;

                if (whisker.WhiskerId == -1)
                {
                    nosePoint = singleWhiskerVelocity;
                }

                if (previousFrame != null)
                {
                    singleWhiskerVelocity.PreviousFrame = previousFrame.Targets[previousFrameCounter];
                }

                Targets.Add(singleWhiskerVelocity);
                previousFrameCounter++;
            }
        }

        public override object[] ExportData()
        {
            int length = Targets.Count;
            object[] data = new object[length];

            for (int i = 0; i < length; i++)
            {
                data[i] = Targets[i].Velocity.Length;
            }

            return data;
        }
    }
}
