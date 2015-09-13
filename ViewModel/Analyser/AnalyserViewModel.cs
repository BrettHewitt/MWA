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
using RobynsWhiskerTracker.ModelInterface.WhiskerVideo;
using RobynsWhiskerTracker.Services.ExcelService;
using RobynsWhiskerTracker.Services.FileBrowser;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.Mean;

namespace RobynsWhiskerTracker.ViewModel.Analyser
{
    public class AnalyserViewModel : WindowViewModelBase
    {
        private ActionCommand m_ExportCommand;

        private ObservableCollection<MethodBase> m_AnalysisMethods = new ObservableCollection<MethodBase>();
        private MethodBase m_SelectedMethod;
        private UserControl m_SelectedMethodControl;

        private IWhiskerVideo m_Video;
        private Dictionary<int, MouseFrameViewModel> m_Frames = new Dictionary<int, MouseFrameViewModel>();
        private MouseFrameViewModel m_CurrentFrame;

        private int m_IndexNumber;
        private int m_MaxIndex;

        public ActionCommand ExportCommand
        {
            get
            {
                return m_ExportCommand ?? (m_ExportCommand = new ActionCommand()
                {
                    ExecuteAction = Export,
                    CanExecuteAction = CanExport,
                });
            }
        }

        public ObservableCollection<MethodBase> AnalysisMethods
        {
            get
            {
                return m_AnalysisMethods;
            }
            set
            {
                if (Equals(m_AnalysisMethods, value))
                {
                    return;
                }

                m_AnalysisMethods = value;

                NotifyPropertyChanged();
            }
        }

        public MethodBase SelectedMethod
        {
            get
            {
                return m_SelectedMethod;
            }
            set
            {
                if (Equals(m_SelectedMethod, value))
                {
                    return;
                }

                m_SelectedMethod = value;

                if (m_SelectedMethod != null)
                {
                    if (!m_SelectedMethod.Loaded)
                    {
                        m_SelectedMethod.LoadData();
                    }

                    SelectedMethodControl = m_SelectedMethod.MethodControl;
                }

                NotifyPropertyChanged();
                NotifyPropertyChanged("ShowFrameSlider");
                ExportCommand.RaiseCanExecuteChangedNotification();
            }
        }

        public bool ShowFrameSlider
        {
            get
            {
                if (SelectedMethod == null)
                {
                    return false;
                }

                return SelectedMethod.ShowFrameSlider;
            }
        }

        public UserControl SelectedMethodControl
        {
            get
            {
                return m_SelectedMethodControl;
            }
            set
            {
                if (Equals(m_SelectedMethodControl, value))
                {
                    return;
                }

                m_SelectedMethodControl = value;

                SelectedMethod.PropagateFrameChangedNotifications(IndexNumber);

                NotifyPropertyChanged();
            }
        }

        public Dictionary<int, MouseFrameViewModel> Frames
        {
            get
            {
                return m_Frames;
            }
            set
            {
                if (ReferenceEquals(m_Frames, value))
                {
                    return;
                }

                m_Frames = value;

                if (m_Frames != null && m_Frames.Count > 0)
                {
                    CurrentFrame = m_Frames[0];
                }

                NotifyPropertyChanged();
            }
        }

        public MouseFrameViewModel CurrentFrame
        {
            get
            {
                return m_CurrentFrame;
            }
            set
            {
                if (Equals(m_CurrentFrame, value))
                {
                    return;
                }

                m_CurrentFrame = value;
                
                NotifyPropertyChanged();
                NotifyFrameChanged();
            }
        }

        public IWhiskerVideo Video
        {
            get
            {
                return m_Video;
            }
            set
            {
                if (Equals(m_Video, value))
                {
                    return;
                }

                m_Video = value;

                NotifyPropertyChanged();
            }
        }

        public int IndexNumber
        {
            get
            {
                return m_IndexNumber;
            }
            set
            {
                if (Equals(m_IndexNumber, value) || value > MaxIndex)
                {
                    return;
                }

                m_IndexNumber = value;

                CurrentFrame = Frames[IndexNumber];

                NotifyPropertyChanged();
            }
        }

        public int MaxIndex
        {
            get
            {
                return m_MaxIndex;
            }
            set
            {
                if (Equals(m_MaxIndex, value))
                {
                    return;
                }

                m_MaxIndex = value;

                NotifyPropertyChanged();
            }
        }

        public AnalyserViewModel(Dictionary<int, MouseFrameViewModel> frames, IWhiskerVideo video)
        {
            Video = video;
            Frames = frames;
            MaxIndex = Frames.Count - 1;

            bool hasNosePoint = frames[0].Model.Whiskers.Any(x => x.WhiskerId == -1);
            bool hasOrientationPoint = frames[0].Model.Whiskers.Any(x => x.WhiskerId == 0);
            bool hasWhiskers = frames[0].Model.Whiskers.Any(x => !x.IsGenericPoint);

            AnalysisMethods.Add(new None());

            if (hasNosePoint)
            {
                AnalysisMethods.Add(new NoseDisplacementViewModel(this));

                if (hasOrientationPoint)
                {
                    AnalysisMethods.Add(new HeadOrientationViewModel(this));

                    if (hasWhiskers)
                    {
                        AnalysisMethods.Add(new WhiskerSpreadViewModel(this));
                    }
                }
            }

            if (hasWhiskers)
            {
                AnalysisMethods.Add(new WhiskerCurvatureViewModel(this));
                AnalysisMethods.Add(new WhiskerAngleViewModel(this));
                AnalysisMethods.Add(new WhiskerAngularVelocityViewModel(this));
                AnalysisMethods.Add(new WhiskerFrequencyViewModel(this));
                AnalysisMethods.Add(new WhiskerMeanOffsetViewModel(this));
                AnalysisMethods.Add(new WhiskerAmplitudeViewModel(this));
                AnalysisMethods.Add(new WhiskerProtractionRetractionViewModel(this));
            }

            if (hasWhiskers || hasNosePoint)
            {
                AnalysisMethods.Add(new WhiskerVelocityViewModel(this));
            }

            AnalysisMethods.Add(new AnalyseEverythingViewModel(this));
            SelectedMethod = AnalysisMethods.First();            
        }

        private void NotifyFrameChanged()
        {
            if (SelectedMethod != null)
            {
                SelectedMethod.PropagateFrameChangedNotifications(IndexNumber);
            }
        }

        private void Export()
        {
            //Get save location
            string fileLocation = FileBrowser.SaveFile("Excel|*.xlsx");

            //Make sure user hasn't pressed cancel
            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }

            //Get data
            object[,] data = SelectedMethod.ExportResults();

            if (data == null)
            {
                return;
            }
            
            //Export
            ExcelService.WriteData(data, fileLocation, false);
        }

        private bool CanExport()
        {
            return !(SelectedMethod is None);
        }
    }
}
