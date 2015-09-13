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

using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.AngleTypes;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngle;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Frequency;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Frequency.FrequencyTypes;
using RobynsWhiskerTracker.ModelInterface.ClipSettings;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Settings;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobynsWhiskerTracker.Services.ExcelService;
using RobynsWhiskerTracker.Services.Maths;

namespace RobynsWhiskerTracker.Model.Analyser.Methods.Frequency
{
    internal class WhiskerFrequency : ModelObjectBase, IWhiskerFrequency
    {
        private ISingleWhiskerFrequency[] m_Whiskers;
        private IWhiskerAngle m_WhiskerAngle;
        private IFrequencyTypeBase m_FrequencyMethod;
        private double m_TValue;
        private double[][] m_WhiskerIdFrame;
        
        private string m_Name;

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

        public ISingleWhiskerFrequency[] Whiskers
        {
            get
            {
                return m_Whiskers;
            }
            private set
            {
                if (ReferenceEquals(m_Whiskers, value))
                {
                    return;
                }

                m_Whiskers = value;

                MarkAsDirty();
            }
        }
        
        public IWhiskerAngle WhiskerAngle
        {
            get
            {
                return m_WhiskerAngle;
            }
            private set
            {
                if (Equals(m_WhiskerAngle, value))
                {
                    return;
                }

                m_WhiskerAngle = value;

                MarkAsDirty();
            }
        }

        public IFrequencyTypeBase FrequencyMethod
        {
            get
            {
                return m_FrequencyMethod;
            }
            set
            {
                if (Equals(m_FrequencyMethod, value))
                {
                    return;
                }

                m_FrequencyMethod = value;

                UpdateFrequencyMethod();

                MarkAsDirty();
            }
        }

        public double TValue
        {
            get
            {
                return m_TValue;
            }
            set
            {
                if (Equals(m_TValue, value))
                {
                    return;
                }

                m_TValue = value;

                UpdateTValue();

                MarkAsDirty();
            }
        }

        public double[][] WhiskerIdFrame
        {
            get
            {
                return m_WhiskerIdFrame;
            }
            private set
            {
                if (ReferenceEquals(m_WhiskerIdFrame, value))
                {
                    return;
                }

                m_WhiskerIdFrame = value;

                MarkAsDirty();
            }
        }

        public WhiskerFrequency()
        {
            Name = "Frequency";
            WhiskerAngle = ModelResolver.Resolve<IWhiskerAngle>();
            
        }

        public void LoadData(IMouseFrame[] frames)
        {
            //Load Whisker Angle Data
            WhiskerAngle.LoadData(frames);

            //Get whiskers
            double frameRate = GlobalSettings.GlobalSettings.FrameRateSettings.OriginalFrameRate;
            double frameInterval = GlobalSettings.GlobalSettings.ClipSettings.FrameInterval;

            List<ISingleWhiskerFrequency> whiskers = new List<ISingleWhiskerFrequency>();
            foreach (IWhisker whisker in frames[0].Whiskers)
            {
                if (whisker.IsGenericPoint)
                {
                    continue;
                }

                ISingleWhiskerFrequency singleWhisker = ModelResolver.Resolve<ISingleWhiskerFrequency>();
                singleWhisker.Whisker = whisker;
                singleWhisker.FrameRate = frameRate;
                singleWhisker.FrameInterval = frameInterval;
                whiskers.Add(singleWhisker);
            }

            Whiskers = whiskers.ToArray();

            CreateWhiskers();
            UpdateTValue();
        }

        private void CreateWhiskers()
        {
            //Iterate over whisker angles and build graph values for individual whiskers
            int numberOfWhiskers = Whiskers.Length;
            WhiskerIdFrame = new double[numberOfWhiskers][];

            for (int i = 0; i < numberOfWhiskers; i++)
            {
                WhiskerIdFrame[i] = new double[WhiskerAngle.Frames.Length];
            }
        }

        private void UpdateTValue()
        {
            WhiskerAngle.UpdateTValue(TValue);
            for (int j = 0; j < Whiskers.Length; j++)
            {
                for (int i = 0; i < WhiskerAngle.Frames.Length; i++)
                {
                    ISingleWhiskerAngle whiskerAngle = WhiskerAngle.Frames[i].Targets.Single(x => x.Whisker.WhiskerId == Whiskers[j].WhiskerId);
                    WhiskerIdFrame[j][i] = whiskerAngle.Angle;
                }

                Whiskers[j].Signal = WhiskerIdFrame[j];
            }
        }

        private void UpdateFrequencyMethod()
        {
            if (FrequencyMethod == null)
            {
                return;
            }

            //Update frequency method for each whisker
            for (int i = 0; i < WhiskerIdFrame.Length; i++)
            {
                Whiskers[i].FrequencyType = FrequencyMethod;
            }
        }

        public double[] GetWhiskerFrequencySignal(int whiskerId)
        {
            return Whiskers.Single(x => x.WhiskerId == whiskerId).Signal;
        }
        
        public object[][] ExportData()
        {
            List<object[]> data = new List<object[]>();

            data.Add(Whiskers.Select(target => target.Whisker.WhiskerName).Cast<object>().ToArray());
            data.Add(Whiskers.Select(whisker => whisker.Frequency).Cast<object>().ToArray());

            return data.ToArray();
        }

        public object[][] ExportMeanData(out int rowCount, out int columnCount)
        {
            rowCount = 2;
            columnCount = Whiskers.Length;
            return ExportData();
        }

        public void UpdateAngleType(IAngleTypeBase angleType)
        {
            WhiskerAngle.UpdateAngleType(angleType);
            UpdateTValue();
        }
    }
}
