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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RobynsWhiskerTracker.Commands;

namespace RobynsWhiskerTracker.ViewModel.Setings
{
    public class SettingsViewModel : WindowViewModelBase
    {
        private ActionCommand m_OkCommand;
        private ActionCommand m_ApplyCommand;
        private ActionCommand m_CancelCommand;

        private ObservableCollection<BaseSettingsViewModel> m_SettingsList = new ObservableCollection<BaseSettingsViewModel>();

        private BaseSettingsViewModel m_SelectedSettings;

        private UserControl m_SelectedSettingsUserControl;

        public ActionCommand OkCommand
        {
            get
            {
                return m_OkCommand ?? (m_OkCommand = new ActionCommand()
                {
                    ExecuteAction = Ok,
                });
            }
        }

        public ActionCommand ApplyCommand
        {
            get
            {
                return m_ApplyCommand ?? (m_ApplyCommand = new ActionCommand()
                {
                    ExecuteAction = ValidateAndApply,
                });
            }
        }

        public ActionCommand CancelCommand
        {
            get
            {
                return m_CancelCommand ?? (m_CancelCommand = new ActionCommand()
                {
                    ExecuteAction = Cancel,
                });
            }
        }

        public ObservableCollection<BaseSettingsViewModel> SettingsList
        {
            get
            {
                return m_SettingsList;
            }
        }

        public BaseSettingsViewModel SelectedSettings
        {
            get
            {
                return m_SelectedSettings;
            }
            set
            {
                if (Equals(m_SelectedSettings, value))
                {
                    return;
                }

                m_SelectedSettings = value;
                SelectedSettingsUserControl = SelectedSettings.SettingsUserControl;

                NotifyPropertyChanged();
            }
        }

        public UserControl SelectedSettingsUserControl
        {
            get
            {
                return m_SelectedSettingsUserControl;
            }
            set
            {
                if (Equals(m_SelectedSettingsUserControl, value))
                {
                    return;
                }

                m_SelectedSettingsUserControl = value;

                NotifyPropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            SettingsList.Add(new ColorSettingsViewModel(GlobalSettings.GlobalSettings.ColorSettings));
            SettingsList.Add(new UnitSettingsViewModel(GlobalSettings.GlobalSettings.UnitSettings));
            SettingsList.Add(new FrameRateSettingsViewModel(GlobalSettings.GlobalSettings.FrameRateSettings));

            foreach (BaseSettingsViewModel viewModel in SettingsList)
            {
                viewModel.LoadSettings(null, null);
            }
        }

        private void Ok()
        {
            if (Validate())
            {
                Apply();
                Close = true;
            }
        }

        private void ValidateAndApply()
        {
            if (!Validate())
            {
                return;
            }

            Apply();
        }

        private void Apply()
        {
            foreach (BaseSettingsViewModel setting in SettingsList)
            {
                if (setting.ViewModelState == ViewModelState.Dirty)
                {
                    setting.SaveSettings();
                }                
            }
        }

        private bool Validate()
        {
            bool result = true;
            string message = string.Empty;
            foreach (BaseSettingsViewModel setting in SettingsList)
            {
                if (!setting.Validate(ref message))
                {
                    result = false;
                }
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return result;
        }

        private void Cancel()
        {
            Close = true;
        }
    }
}
