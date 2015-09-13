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

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using RobynsWhiskerTracker.Commands;
using RobynsWhiskerTracker.ModelInterface.ClipSettings;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using RobynsWhiskerTracker.ModelInterface.WhiskerVideo;
using RobynsWhiskerTracker.Resolver;
using RobynsWhiskerTracker.ViewModel.ClipSettings;

namespace RobynsWhiskerTracker.ViewModel.Window.ClipSettings
{
    public class ClipSettingsWindowViewModel : WindowViewModelBase
    {
        private ActionCommand m_OkCommand;
        private ActionCommand m_CancelCommand;
        private ActionCommand m_AddPointCommand;
        private ActionCommand m_RemovePointCommand;

        private int m_FrameCount;
        private int m_SelectedNumberOfWhiskers;
        private double m_OriginalFrameRate;
        private ObservableCollection<IWhiskerClipSettings> m_Whiskers = new ObservableCollection<IWhiskerClipSettings>();
        private List<IWhiskerClipSettings> m_GenericWhiskerPoints = new List<IWhiskerClipSettings>(); 

        private readonly ObservableCollection<int> m_NumberOfWhiskersList = new ObservableCollection<int>()
        {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
        };

        private readonly ObservableCollection<int> m_NumberOfPointsPerWhiskerList = new ObservableCollection<int>()
        {
            1,
            2,
            3,
            4,
        };

        private readonly ObservableCollection<int> m_FrameIntervalChoiceList = new ObservableCollection<int>()
        {
            1,
            2,
            3,
            4,
            5,
        }; 

        public ObservableCollection<int> NumberOfPointsPerWhiskerList
        {
            get
            {
                return m_NumberOfPointsPerWhiskerList;
            }
        }

        public ObservableCollection<int> NumberOfWhiskersList
        {
            get
            {
                return m_NumberOfWhiskersList;
            }
        }

        public ObservableCollection<int> FrameIntervalChoiceList
        {
            get
            {
                return m_FrameIntervalChoiceList;
            }
        }

        public ObservableCollection<IWhiskerClipSettings> Whiskers
        {
            get
            {
                return m_Whiskers;
            }
            set
            {
                if (ReferenceEquals(m_Whiskers, value))
                {
                    return;
                }

                m_Whiskers = value;

                NotifyPropertyChanged();
                OkCommand.RaiseCanExecuteChangedNotification();
            }
        }

        public ActionCommand OkCommand
        {
            get
            {
                return m_OkCommand ?? (m_OkCommand = new ActionCommand()
                {
                    ExecuteAction = OkPressed,
                    CanExecuteAction = CanOkPress,
                });
            }
        }

        public ActionCommand CancelCommand
        {
            get
            {
                return m_CancelCommand ?? (m_CancelCommand = new ActionCommand()
                {
                    ExecuteAction = CancelPressed,
                });
            }
        }

        public ActionCommand AddPointCommand
        {
            get
            {
                return m_AddPointCommand ?? (m_AddPointCommand = new ActionCommand()
                {
                    ExecuteAction = AddPoint
                });
            }
        }

        public ActionCommand RemovePointCommand
        {
            get
            {
                return m_RemovePointCommand ?? (m_RemovePointCommand = new ActionCommand()
                {
                    ExecuteAction = RemovePoint,
                    CanExecuteAction = () => GenericPoints.Count > 0,
                });
            }
        }

        public ClipSettingsViewModel ViewModel
        {
            get;
            set;
        }

        public IClipSettings Model
        {
            get
            {
                return ViewModel.Model;
            }
        }

        public int FrameCount
        {
            get
            {
                return m_FrameCount;
            }
            set
            {
                if (Equals(m_FrameCount, value))
                {
                    return;
                }

                m_FrameCount = value;

                NotifyPropertyChanged();
            }
        }

        public int SelectedNumberOfWhiskers
        {
            get
            {
                return m_SelectedNumberOfWhiskers;
            }
            set
            {
                if (Equals(m_SelectedNumberOfWhiskers, value))
                {
                    return;
                }

                m_SelectedNumberOfWhiskers = value;
                ViewModel.NumberOfWhiskers = value;

                NotifyPropertyChanged();

                UpdateNumberOfWhiskers();
            }
        }

        public int SelectedNumberOfPointsPerWhisker
        {
            get
            {
                return ViewModel.NumberOfPointsPerWhisker;
            }
            set
            {
                if (Equals(SelectedNumberOfPointsPerWhisker, value))
                {
                    return;
                }

                ViewModel.NumberOfPointsPerWhisker = value;

                NotifyPropertyChanged();
                UpdateNumberOfWhiskers();
            }
        }

        public bool IncludeNosePoint
        {
            get
            {
                return ViewModel.IncludeNosePoint;
            }
            set
            {
                if (Equals(ViewModel.IncludeNosePoint, value))
                {
                    return;
                }

                ViewModel.IncludeNosePoint = value;
                UpdateNumberOfWhiskers();
                NotifyPropertyChanged();
                NotifyPropertyChanged("IncludeOrientationPoint");
            }
        }

        public bool IncludeOrientationPoint
        {
            get
            {
                return ViewModel.IncludeOrientationPoint;
            }
            set
            {
                if (Equals(ViewModel.IncludeOrientationPoint, value))
                {
                    return;
                }

                ViewModel.IncludeOrientationPoint = value;
                UpdateNumberOfWhiskers();
                NotifyPropertyChanged();
            }
        }

        private List<IWhiskerClipSettings> GenericPoints
        {
            get
            {
                return m_GenericWhiskerPoints;
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

                NotifyPropertyChanged();
            }
        }

        public ClipSettingsWindowViewModel(IWhiskerVideo video)
        {
            IClipSettings clipSettings = ModelResolver.Resolve<IClipSettings>();
            clipSettings.ClipFilePath = video.FilePath;
            clipSettings.StartFrame = 1;
            int frameCount = video.FrameCount;
            clipSettings.EndFrame = frameCount < 100 ? frameCount : 100;
            clipSettings.IncludeNosePoint = true;
            clipSettings.IncludeOrientationPoint = true;
            
            clipSettings.NumberOfPointsPerWhisker = 3;
            clipSettings.FrameInterval = 1;
            FrameCount = frameCount;

            ViewModel = new ClipSettingsViewModel(clipSettings);
            SelectedNumberOfWhiskers = 4;

            OriginalFrameRate = video.FrameRate;
        }

        public ClipSettingsWindowViewModel(IClipSettings model, int frameCount)
        {
            ViewModel = new ClipSettingsViewModel(model);
            FrameCount = frameCount;
        }

        private void OkPressed()
        {
            string errors = "";

            if (!Validate(ref errors))
            {
                MessageBox.Show(errors, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ExitResult = WindowExitResult.Ok;

            Model.Whiskers = Whiskers.ToArray();

            CloseWindow();
        }

        private bool CanOkPress()
        {
            if (Whiskers.Count == 0)
            {
                return false;
            }

            return true;
        }

        private void CancelPressed()
        {
            ExitResult = WindowExitResult.Cancel;
            CloseWindow();
        }

        private bool Validate(ref string error)
        {
            return true;
        }

        private void AddPoint()
        {
            IWhiskerClipSettings whisker = ModelResolver.Resolve<IWhiskerClipSettings>();
            whisker.IsGenericPoint = true;
            whisker.NumberOfPoints = 1;
            int genericStartId = ViewModel.NumberOfWhiskers + 1;
            genericStartId += GenericPoints.Count;
            whisker.WhiskerId = genericStartId;
            whisker.WhiskerName = "Point " + (GenericPoints.Count + 1);
            AddWhisker(whisker);
            GenericPoints.Add(whisker);
            RemovePointCommand.RaiseCanExecuteChangedNotification();
        }

        private void RemovePoint()
        {
            if (GenericPoints.Count == 0)
            {
                return;
            }

            Whiskers.RemoveAt(Whiskers.Count - 1);
            GenericPoints.RemoveAt(GenericPoints.Count - 1);

            OkCommand.RaiseCanExecuteChangedNotification();
            RemovePointCommand.RaiseCanExecuteChangedNotification();
        }

        private void UpdateNumberOfWhiskers()
        {
            int startIndex = 1;
            if (ViewModel.IncludeNosePoint)
            {
                startIndex = -1;
            }

            ObservableCollection<IWhiskerClipSettings> whiskers = new ObservableCollection<IWhiskerClipSettings>();
            for (int index = startIndex; index <= ViewModel.NumberOfWhiskers; index++)
            {
                string name;
                bool isGeneric;
                int numberOfPoints = SelectedNumberOfPointsPerWhisker;
                if (index == -1)
                {
                    name = "Nose Point";
                    isGeneric = true;
                    numberOfPoints = 1;
                }
                else if (index == 0)
                {
                    if (!ViewModel.IncludeOrientationPoint)
                    {
                        continue;
                    }

                    name = "Orientation Point";
                    isGeneric = true;
                    numberOfPoints = 1;
                }
                else
                {
                    name = "Whisker " + index;
                    isGeneric = false;
                }

                IWhiskerClipSettings whisker = ModelResolver.Resolve<IWhiskerClipSettings>();
                whisker.WhiskerId = index;
                whisker.WhiskerName = name;
                whisker.IsGenericPoint = isGeneric;
                whisker.NumberOfPoints = numberOfPoints;
                whiskers.Add(whisker);
            }

            if (GenericPoints.Count > 0)
            {
                int genericStartId = ViewModel.NumberOfWhiskers + 1;
                int delta = genericStartId - GenericPoints.Min(x => x.WhiskerId);
                foreach (IWhiskerClipSettings genericWhisker in GenericPoints)
                {
                    genericWhisker.WhiskerId += delta;
                    whiskers.Add(genericWhisker);
                }
            }

            Whiskers = whiskers;
        }

        private void AddWhisker(IWhiskerClipSettings whisker)
        {
            Whiskers.Add(whisker);
            OkCommand.RaiseCanExecuteChangedNotification();
        }
    }
}
