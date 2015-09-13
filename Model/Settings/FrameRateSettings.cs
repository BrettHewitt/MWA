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

using RobynsWhiskerTracker.ModelInterface.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobynsWhiskerTracker.Model.Settings
{
    internal class FrameRateSettings : SettingsBase, IFrameRateSettings
    {
        private double m_OriginalFrameRate;
        private double m_CurrentFrameRate;
        private double m_ModifierRatio;

        public double OriginalFrameRate
        {
            get
            {
                return m_OriginalFrameRate;
            }
            set
            {
                if (Equals(m_OriginalFrameRate, value))
                {
                    return;
                }

                m_OriginalFrameRate = value;
                CalculateModifierRatio();

                MarkAsDirty();                
            }
        }

        public double CurrentFrameRate
        {
            get
            {
                return m_CurrentFrameRate;
            }
            set
            {
                if (Equals(m_CurrentFrameRate, value))
                {
                    return;
                }

                m_CurrentFrameRate = value;

                MarkAsDirty();                
            }
        }

        public double ModifierRatio
        {
            get
            {
                return m_ModifierRatio;
            }
            set
            {
                if (Equals(m_ModifierRatio, value))
                {
                    return;
                }

                m_ModifierRatio = value;
                CalculateOriginalFrameRate();

                MarkAsDirty();                
            }
        }

        private void CalculateModifierRatio()
        {
            if (OriginalFrameRate == 0 || CurrentFrameRate == 0)
            {
                return;
            }

            m_ModifierRatio = OriginalFrameRate / CurrentFrameRate;
        }

        private void CalculateOriginalFrameRate()
        {
            if (ModifierRatio == 0 || CurrentFrameRate == 0)
            {
                return;
            }

            m_OriginalFrameRate = CurrentFrameRate * ModifierRatio;
        }

        public override void LoadSettings()
        {
            DataLoadComplete();
        }

        public override void SaveSettings()
        {
            DataLoadComplete();
        }
    }
}
