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

using System.Collections.Generic;
using RobynsWhiskerTracker.Interfaces;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Curvature;
using RobynsWhiskerTracker.Resolver;
using RobynsWhiskerTracker.View.Analyser.Methods;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerCurve;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods
{
    public class WhiskerCurvatureViewModel : MethodBaseWithImage, IUpdateTValue
    {
        private IWhiskerCurvature m_Model;

        private ObservableCollection<WhiskerCurveFrameViewModel> m_Frames = new ObservableCollection<WhiskerCurveFrameViewModel>();
        private WhiskerCurveFrameViewModel m_CurrentFrame;
        private ObservableCollection<WhiskersEnabledViewModel> m_DisplayWhiskers;
        private double m_TValue = -1;

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

                if (value < 0)
                {
                    value = 0;
                }
                else if (value > 1)
                {
                    value = 1;
                }

                m_TValue = value;

                UpdateWhiskerTValue();

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<WhiskerCurveFrameViewModel> Frames
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

                NotifyPropertyChanged();
            }
        }

        public WhiskerCurveFrameViewModel CurrentFrame
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
                Image = CurrentFrame.Model.TargetFrame.Frame.ToBitmap();
                UpdateDisplayWhiskers();

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<WhiskersEnabledViewModel> DisplayWhiskers
        {
            get
            {
                return m_DisplayWhiskers;
            }
            set
            {
                if (ReferenceEquals(m_DisplayWhiskers, value))
                {
                    return;
                }

                m_DisplayWhiskers = value;
                
                NotifyPropertyChanged();
            }
        }

        public IWhiskerCurvature Model
        {
            get
            {
                return m_Model;
            }
            set
            {
                if (Equals(m_Model, value))
                {
                    return;
                }

                m_Model = value;

                MarkAsDirtyAndNotifyPropertyChange();
            }
        }

        public WhiskerCurvatureViewModel(AnalyserViewModel parent) : base(parent, "Whisker Curvature")
        {
            Model = ModelResolver.Resolve<IWhiskerCurvature>();     
            Model.LoadData(parent.Frames.Values.Select(x => x.Model).ToArray());
            CreateFrames();

            MethodControl = new WhiskerCurveMethodView()
            {
                DataContext = this,
            };

            TValue = 0;
            Initialise();
        }

        private void CreateFrames()
        {
            ObservableCollection<WhiskerCurveFrameViewModel> frames = new ObservableCollection<WhiskerCurveFrameViewModel>();

            foreach (IWhiskerCurveFrame frame in Model.Frames)
            {
                frames.Add(new WhiskerCurveFrameViewModel(frame, this));
            }

            Frames = frames;
            WhiskerCurveFrameViewModel firstFrame = Frames.First();
            ObservableCollection<WhiskersEnabledViewModel> displayWhiskers = new ObservableCollection<WhiskersEnabledViewModel>();

            foreach (SingleWhiskerCurveViewModel whisker in firstFrame.Whiskers)
            {
                WhiskersEnabledViewModel displayWhisker = new WhiskersEnabledViewModel(this, whisker);
                displayWhiskers.Add(displayWhisker);
            }

            DisplayWhiskers = displayWhiskers;

            CurrentFrame = firstFrame;
        }

        private void UpdateDisplayWhiskers()
        {
            foreach (WhiskersEnabledViewModel whisker in DisplayWhiskers)
            {
                whisker.Whisker = CurrentFrame.Whiskers.Single(x => x.Model.Whisker.WhiskerId == whisker.Whisker.WhiskerId);
            }
        }

        protected override void UpdateWhiskerPositions(System.Windows.Controls.Image image, SizeChangedEventArgs e)
        {
            UpdateWhiskerPositions();
        }

        private void UpdateWhiskerPositions()
        {
            foreach (WhiskerCurveFrameViewModel frame in Frames)
            {
                frame.UpdatePositions(LastKnownImageWidth, LastKnownImageHeight);
            }
        }

        public override void PropagateFrameChangedNotifications(int indexNumber)
        {
            CurrentFrame = Frames[indexNumber];
        }

        public override void PropagateWhiskerEnabledNotification(int whiskerId, bool value)
        {
            foreach (var frame in Frames)
            {
                frame.Whiskers.First(x => x.Model.Whisker.WhiskerId == whiskerId).Enabled = value;
            }
        }

        public string[] GetColumnTitles()
        {
            string[] titles = new string[1];

            return titles;
        }

        public int NumberOfColumns()
        {
            return 0;
        }

        public override object[,] ExportResults()
        {
            //Get raw data
            object[][] rawData = Model.ExportData();

            //Get dimensions
            int numberOfColumns = rawData[0].Length + 1;
            int numberOfRows = rawData.Length;

            //Don't want to save disabled whiskers, find out which ones are disabled
            List<int> disabledColumns = new List<int>();
            for (int i = 0; i < DisplayWhiskers.Count; i++)
            {
                if (!DisplayWhiskers[i].Enabled)
                {
                    disabledColumns.Add(i);
                }
            }

            //Create storage
            object[,] data = new object[numberOfRows, numberOfColumns - disabledColumns.Count];

            //Add frame counter, and only the enabled whiskers
            for (int i = 0; i < numberOfRows; i++)
            {
                object[] currentRow = rawData[i];

                data[i, 0] = i;
                int disabledColumnCounter = 1;
                for (int j = 0; j < currentRow.Length; j++)
                {
                    if (disabledColumns.Contains(j))
                    {
                        disabledColumnCounter--;
                        continue;
                    }

                    data[i, j + disabledColumnCounter] = currentRow[j];
                }
            }

            //0,0 is always "Frame"
            data[0, 0] = "Frame";

            return data;
        }

        protected void UpdateWhiskerTValue()
        {
            foreach (WhiskerCurveFrameViewModel whisker in Frames)
            {
                whisker.UpdateTValue(TValue);
            }
        }
    }
}
