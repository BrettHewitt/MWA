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
using RobynsWhiskerTracker.Commands;
using RobynsWhiskerTracker.Model;
using RobynsWhiskerTracker.ModelInterface.Settings;
using RobynsWhiskerTracker.Resolver;
using RobynsWhiskerTracker.View.Settings;

namespace RobynsWhiskerTracker.ViewModel.Setings
{
    public class ColorSettingsViewModel : BaseSettingsViewModel
    {
        private ActionCommand m_RevertToDefaultCommand;

        private Color m_NoseColor;
        private Color m_OrientationColor;
        private Color m_Whisker1Color;
        private Color m_Whisker2Color;
        private Color m_Whisker3Color;
        private Color m_Whisker4Color;
        private Color m_Whisker5Color;
        private Color m_Whisker6Color;
        private Color m_Whisker7Color;
        private Color m_Whisker8Color;
        private Color m_Whisker9Color;
        private Color m_Whisker10Color;

        public ActionCommand RevertToDefaultCommand
        {
            get
            {
                return m_RevertToDefaultCommand ?? (m_RevertToDefaultCommand = new ActionCommand()
                {
                    ExecuteAction = () =>
                    {
                        Model.ReturnToDefault();
                        Model.SaveSettings();
                        LoadSettings(null, null);
                    },
                });
            }
        }

        public IColorSettings Model
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

                MarkAsDirtyAndNotifyPropertyChange();
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

                MarkAsDirtyAndNotifyPropertyChange();
            }
        }

        public Color Whisker1Color
        {
            get
            {
                return m_Whisker1Color;
            }
            set
            {
                if (Equals(m_Whisker1Color, value))
                {
                    return;
                }

                m_Whisker1Color = value;

                MarkAsDirtyAndNotifyPropertyChange();
            }
        }

        public Color Whisker2Color
        {
            get
            {
                return m_Whisker2Color;
            }
            set
            {
                if (Equals(m_Whisker2Color, value))
                {
                    return;
                }

                m_Whisker2Color = value;

                MarkAsDirtyAndNotifyPropertyChange();
            }
        }

        public Color Whisker3Color
        {
            get
            {
                return m_Whisker3Color;
            }
            set
            {
                if (Equals(m_Whisker3Color, value))
                {
                    return;
                }

                m_Whisker3Color = value;

                MarkAsDirtyAndNotifyPropertyChange();
            }
        }

        public Color Whisker4Color
        {
            get
            {
                return m_Whisker4Color;
            }
            set
            {
                if (Equals(m_Whisker4Color, value))
                {
                    return;
                }

                m_Whisker4Color = value;

                MarkAsDirtyAndNotifyPropertyChange();
            }
        }

        public Color Whisker5Color
        {
            get
            {
                return m_Whisker5Color;
            }
            set
            {
                if (Equals(m_Whisker5Color, value))
                {
                    return;
                }

                m_Whisker5Color = value;

                MarkAsDirtyAndNotifyPropertyChange();
            }
        }

        public Color Whisker6Color
        {
            get
            {
                return m_Whisker6Color;
            }
            set
            {
                if (Equals(m_Whisker6Color, value))
                {
                    return;
                }

                m_Whisker6Color = value;

                MarkAsDirtyAndNotifyPropertyChange();
            }
        }

        public Color Whisker7Color
        {
            get
            {
                return m_Whisker7Color;
            }
            set
            {
                if (Equals(m_Whisker7Color, value))
                {
                    return;
                }

                m_Whisker7Color = value;

                MarkAsDirtyAndNotifyPropertyChange();
            }
        }

        public Color Whisker8Color
        {
            get
            {
                return m_Whisker8Color;
            }
            set
            {
                if (Equals(m_Whisker8Color, value))
                {
                    return;
                }

                m_Whisker8Color = value;

                MarkAsDirtyAndNotifyPropertyChange();
            }
        }

        public Color Whisker9Color
        {
            get
            {
                return m_Whisker9Color;
            }
            set
            {
                if (Equals(m_Whisker9Color, value))
                {
                    return;
                }

                m_Whisker9Color = value;

                MarkAsDirtyAndNotifyPropertyChange();
            }
        }

        public Color Whisker10Color
        {
            get
            {
                return m_Whisker10Color;
            }
            set
            {
                if (Equals(m_Whisker10Color, value))
                {
                    return;
                }

                m_Whisker10Color = value;

                MarkAsDirtyAndNotifyPropertyChange();
            }
        }

        public ColorSettingsViewModel(IColorSettings model)
        {
            Model = model;
            //Model.ReturnToDefault();
            //Model.SaveSettings();
            Name = "Color Settings";
            SettingsUserControl = new ColorSettingsUserControl();
            SettingsUserControl.DataContext = this;
        }

        public override void LoadSettings(object sender, RoutedEventArgs eventArgs)
        {
            Model.LoadSettings();

            if (Model.ModelObjectState == ModelObjectState.Error)
            {
                MessageBox.Show(Model.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NoseColor = Model.NoseColor;
            OrientationColor = Model.OrientationColor;
            Whisker1Color = Model.Whisker1Color;
            Whisker2Color = Model.Whisker2Color;
            Whisker3Color = Model.Whisker3Color;
            Whisker4Color = Model.Whisker4Color;
            Whisker5Color = Model.Whisker5Color; 
            Whisker6Color = Model.Whisker6Color; 
            Whisker7Color = Model.Whisker7Color;
            Whisker8Color = Model.Whisker8Color;
            Whisker9Color = Model.Whisker9Color;
            Whisker10Color = Model.Whisker10Color;

            Initialise();
        }

        public override void SaveSettings()
        {
            Model.NoseColor = NoseColor;
            Model.OrientationColor = OrientationColor;
            Model.Whisker1Color = Whisker1Color;
            Model.Whisker2Color = Whisker2Color;
            Model.Whisker3Color = Whisker3Color;
            Model.Whisker4Color = Whisker4Color;
            Model.Whisker5Color = Whisker5Color;
            Model.Whisker6Color = Whisker6Color;
            Model.Whisker7Color = Whisker7Color;
            Model.Whisker8Color = Whisker8Color;
            Model.Whisker9Color = Whisker9Color;
            Model.Whisker10Color = Whisker10Color;

            Model.SaveSettings();

            if (Model.ModelObjectState == ModelObjectState.Error)
            {
                MessageBox.Show(Model.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Initialise();
        }
    }
}
