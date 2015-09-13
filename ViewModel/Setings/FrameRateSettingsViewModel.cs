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

using RobynsWhiskerTracker.Commands;
using RobynsWhiskerTracker.ModelInterface.Settings;
using RobynsWhiskerTracker.View.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobynsWhiskerTracker.ViewModel.Setings
{
    public class FrameRateSettingsViewModel : BaseSettingsViewModel
    {
        private ActionCommand m_RevertToDefaultCommand;

        private IFrameRateSettings Model
        {
            get;
            set;
        }

        private double m_FrameRateModifier;
        private double m_OriginalFrameRate;
        private double m_CurrentFrameRate;

        private bool m_FrameRateModifierHasError = false;
        private bool m_OriginalFrameRateHasError = false;

        public bool FrameRateModifierHasError
        {
            get
            {
                return m_FrameRateModifierHasError;
            }
            set
            {
                if (Equals(m_FrameRateModifierHasError, value))
                {
                    return;
                }

                m_FrameRateModifierHasError = value;
                
                NotifyPropertyChanged();
            }
        }

        public bool OriginalFrameRateHasError
        {
            get
            {
                return m_OriginalFrameRateHasError;
            }
            set
            {
                if (Equals(m_OriginalFrameRateHasError, value))
                {
                    return;
                }

                m_OriginalFrameRateHasError = value;

                NotifyPropertyChanged();
            }
        }

        public ActionCommand RevertToDefaultCommand
        {
            get
            {
                return m_RevertToDefaultCommand ?? (m_RevertToDefaultCommand = new ActionCommand()
                {
                    ExecuteAction = RevertToDefault,
                });
            }
        }

        public double FrameRateModifier
        {
            get
            {
                return m_FrameRateModifier;
            }
            set
            {
                if (Equals(m_FrameRateModifier, value))
                {
                    return;
                }

                m_FrameRateModifier = value;

                CalculateOriginalFrameRate();

                MarkAsDirtyAndNotifyPropertyChange();
            }
        }

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

                CalculateFrameRateModifier();

                MarkAsDirtyAndNotifyPropertyChange();
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

                MarkAsDirtyAndNotifyPropertyChange();
            }
        }

        public FrameRateSettingsViewModel(IFrameRateSettings model)
        {
            Model = model;
            Name = "Frame Rate Settings";
            SettingsUserControl = new FrameRateSettingsUserControl();
            SettingsUserControl.DataContext = this;
        }

        public override void LoadSettings(object sender, System.Windows.RoutedEventArgs e)
        {
            OriginalFrameRate = Model.OriginalFrameRate;
            CurrentFrameRate = Model.CurrentFrameRate;
            FrameRateModifier = Model.ModifierRatio;
            Initialise();
        }

        public override void SaveSettings()
        {
            Model.OriginalFrameRate = OriginalFrameRate;
            Model.ModifierRatio = FrameRateModifier;
        }

        private void RevertToDefault()
        {
            OriginalFrameRate = CurrentFrameRate;
        }

        private void CalculateOriginalFrameRate()
        {
            if (FrameRateModifier == 0 || CurrentFrameRate == 0)
            {
                return;
            }

            m_OriginalFrameRate = CurrentFrameRate * FrameRateModifier;
            NotifyPropertyChanged("OriginalFrameRate");
        }

        private void CalculateFrameRateModifier()
        {
            if (OriginalFrameRate == 0 || CurrentFrameRate == 0)
            {
                return;
            }

            m_FrameRateModifier = OriginalFrameRate / CurrentFrameRate;
            NotifyPropertyChanged("FrameRateModifier");
        }

        public override bool Validate(ref string message)
        {
            bool result = true;

            if (FrameRateModifierHasError)
            {
                message += "Frame Rate Modifier is not valid\r\n";
                result = false;
            }

            if (OriginalFrameRateHasError)
            {
                message += "Original Frame Rate is not valid\r\n";
                result = false;
            }

            return result;
        }
    }
}
