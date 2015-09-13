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
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RobynsWhiskerTracker.ModelInterface.Settings;
using RobynsWhiskerTracker.Repository;
using RobynsWhiskerTracker.RepositoryInterface;

namespace RobynsWhiskerTracker.Model.Settings
{
    internal class ColorSettings : SettingsBase, IColorSettings
    {
        private readonly Color NoseColorDefault = Color.Yellow;
        private readonly Color OrientationColorDefault = Color.White;
        private readonly Color WhiskerColor1Default = Color.Red;
        private readonly Color WhiskerColor2Default = Color.Blue;
        private readonly Color WhiskerColor3Default = Color.LightSeaGreen;
        private readonly Color WhiskerColor4Default = Color.Purple;
        private readonly Color WhiskerColor5Default = Color.Green;
        private readonly Color WhiskerColor6Default = Color.Cyan;
        private readonly Color WhiskerColor7Default = Color.Orange;
        private readonly Color WhiskerColor8Default = Color.Teal;
        private readonly Color WhiskerColor9Default = Color.RosyBrown;
        private readonly Color WhiskerColor10Default = Color.White;

        private Color m_NoseColor;
        private Color m_OrientationColor;
        private Color m_WhiskerColor1;
        private Color m_WhiskerColor2;
        private Color m_WhiskerColor3;
        private Color m_WhiskerColor4;
        private Color m_WhiskerColor5;
        private Color m_WhiskerColor6;
        private Color m_WhiskerColor7;
        private Color m_WhiskerColor8;
        private Color m_WhiskerColor9;
        private Color m_WhiskerColor10;

        private Dictionary<int, Color> WhiskerColors
        {
            get;
            set;
        }

        public Color NoseColor
        {
            get
            {
                return m_NoseColor;
            }
            set
            {
                if (Equals(m_NoseColor, value))
                {
                    return;
                }

                m_NoseColor = value;

                MarkAsDirty();
            }
        }

        public Color OrientationColor
        {
            get
            {
                return m_OrientationColor;
            }
            set
            {
                if (Equals(m_OrientationColor, value))
                {
                    return;
                }

                m_OrientationColor = value;

                MarkAsDirty();
            }
        }

        public Color Whisker1Color
        {
            get
            {
                return m_WhiskerColor1;
            }
            set
            {
                if (Equals(m_WhiskerColor1, value))
                {
                    return;
                }

                m_WhiskerColor1 = value;

                MarkAsDirty();
            }
        }

        public Color Whisker2Color
        {
            get
            {
                return m_WhiskerColor2;
            }
            set
            {
                if (Equals(m_WhiskerColor2, value))
                {
                    return;
                }

                m_WhiskerColor2 = value;

                MarkAsDirty();
            }
        }

        public Color Whisker3Color
        {
            get
            {
                return m_WhiskerColor3;
            }
            set
            {
                if (Equals(m_WhiskerColor3, value))
                {
                    return;
                }

                m_WhiskerColor3 = value;

                MarkAsDirty();
            }
        }

        public Color Whisker4Color
        {
            get
            {
                return m_WhiskerColor4;
            }
            set
            {
                if (Equals(m_WhiskerColor4, value))
                {
                    return;
                }

                m_WhiskerColor4 = value;

                MarkAsDirty();
            }
        }

        public Color Whisker5Color
        {
            get
            {
                return m_WhiskerColor5;
            }
            set
            {
                if (Equals(m_WhiskerColor5, value))
                {
                    return;
                }

                m_WhiskerColor5 = value;

                MarkAsDirty();
            }
        }

        public Color Whisker6Color
        {
            get
            {
                return m_WhiskerColor6;
            }
            set
            {
                if (Equals(m_WhiskerColor6, value))
                {
                    return;
                }

                m_WhiskerColor6 = value;

                MarkAsDirty();
            }
        }

        public Color Whisker7Color
        {
            get
            {
                return m_WhiskerColor7;
            }
            set
            {
                if (Equals(m_WhiskerColor7, value))
                {
                    return;
                }

                m_WhiskerColor7 = value;

                MarkAsDirty();
            }
        }

        public Color Whisker8Color
        {
            get
            {
                return m_WhiskerColor8;
            }
            set
            {
                if (Equals(m_WhiskerColor8, value))
                {
                    return;
                }

                m_WhiskerColor8 = value;

                MarkAsDirty();
            }
        }

        public Color Whisker9Color
        {
            get
            {
                return m_WhiskerColor9;
            }
            set
            {
                if (Equals(m_WhiskerColor9, value))
                {
                    return;
                }

                m_WhiskerColor9 = value;

                MarkAsDirty();
            }
        }

        public Color Whisker10Color
        {
            get
            {
                return m_WhiskerColor10;
            }
            set
            {
                if (Equals(m_WhiskerColor10, value))
                {
                    return;
                }

                m_WhiskerColor10 = value;

                MarkAsDirty();
            }
        }

        public override void LoadSettings()
        {
            try
            {
                IRepository repository = RepositoryResolver.Resolve<IRepository>();
                NoseColor = repository.GetValue<Color>("NoseColorSetting");
                OrientationColor = repository.GetValue<Color>("OrientationColor");
                Whisker1Color = repository.GetValue<Color>("Whisker1ColorSetting");
                Whisker2Color = repository.GetValue<Color>("Whisker2ColorSetting");
                Whisker3Color = repository.GetValue<Color>("Whisker3ColorSetting");
                Whisker4Color = repository.GetValue<Color>("Whisker4ColorSetting");
                Whisker5Color = repository.GetValue<Color>("Whisker5ColorSetting");
                Whisker6Color = repository.GetValue<Color>("Whisker6ColorSetting");
                Whisker7Color = repository.GetValue<Color>("Whisker7ColorSetting");
                Whisker8Color = repository.GetValue<Color>("Whisker8ColorSetting");
                Whisker9Color = repository.GetValue<Color>("Whisker9ColorSetting");
                Whisker10Color = repository.GetValue<Color>("Whisker10ColorSetting");
            }
            catch (SettingsPropertyNotFoundException)
            {
                ErrorOccured("Settings file is missing or corrupt, reverting to default values");
                ReturnToDefault();
                return;
            }

            WhiskerColors = new Dictionary<int, Color>();
            WhiskerColors.Add(-1, NoseColor);
            WhiskerColors.Add(0, OrientationColor);
            WhiskerColors.Add(1, Whisker1Color);
            WhiskerColors.Add(2, Whisker2Color);
            WhiskerColors.Add(3, Whisker3Color);
            WhiskerColors.Add(4, Whisker4Color);
            WhiskerColors.Add(5, Whisker5Color);
            WhiskerColors.Add(6, Whisker6Color);
            WhiskerColors.Add(7, Whisker7Color);
            WhiskerColors.Add(8, Whisker8Color);
            WhiskerColors.Add(9, Whisker9Color);
            WhiskerColors.Add(10, Whisker10Color);

            DataLoadComplete();
        }

        public void ReturnToDefault()
        {
            NoseColor = NoseColorDefault;
            OrientationColor = OrientationColorDefault;
            Whisker1Color = WhiskerColor1Default;
            Whisker2Color = WhiskerColor2Default;
            Whisker3Color = WhiskerColor3Default;
            Whisker4Color = WhiskerColor4Default;
            Whisker5Color = WhiskerColor5Default;
            Whisker6Color = WhiskerColor6Default;
            Whisker7Color = WhiskerColor7Default;
            Whisker8Color = WhiskerColor8Default;
            Whisker9Color = WhiskerColor9Default;
            Whisker10Color = WhiskerColor10Default;
        }

        public override void SaveSettings()
        {
            if (ModelObjectState == ModelObjectState.Clean)
            {
                return;
            }

            try
            {
                IRepository repository = RepositoryResolver.Resolve<IRepository>();
                repository.SetValue("NoseColorSetting", NoseColor);
                repository.SetValue("OrientationColor", OrientationColor);
                repository.SetValue("Whisker1ColorSetting", Whisker1Color);
                repository.SetValue("Whisker2ColorSetting", Whisker2Color);
                repository.SetValue("Whisker3ColorSetting", Whisker3Color);
                repository.SetValue("Whisker4ColorSetting", Whisker4Color);
                repository.SetValue("Whisker5ColorSetting", Whisker5Color);
                repository.SetValue("Whisker6ColorSetting", Whisker6Color);
                repository.SetValue("Whisker7ColorSetting", Whisker7Color);
                repository.SetValue("Whisker8ColorSetting", Whisker8Color);
                repository.SetValue("Whisker9ColorSetting", Whisker9Color);
                repository.SetValue("Whisker10ColorSetting", Whisker10Color);
                repository.Save();
            }
            catch (SettingsPropertyNotFoundException)
            {
                ErrorOccured("Settings file is missing or corrupt, reverting to default values");
                return;
            }

            DataLoadComplete();
        }

        public Color GetColorFromId(int id)
        {
            if (WhiskerColors.ContainsKey(id))
            {
                return WhiskerColors[id];
            }

            return WhiskerColors[0];
        }
    }
}
