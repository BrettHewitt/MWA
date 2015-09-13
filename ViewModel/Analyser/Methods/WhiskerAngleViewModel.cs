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
using System.Windows.Documents;
using RobynsWhiskerTracker.Interfaces;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngle;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;
using RobynsWhiskerTracker.View.Analyser.Methods;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerAngle;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerAngle.AngleTypes;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods
{
    public class WhiskerAngleViewModel : MethodBaseWithImage, IUpdateAngle, IUpdateTValue
    {
        private IWhiskerAngle m_Model;

        private ObservableCollection<WhiskerAngleFrameViewModel> m_Frames = new ObservableCollection<WhiskerAngleFrameViewModel>();
        private WhiskerAngleFrameViewModel m_CurrentFrame;
        private ObservableCollection<WhiskersEnabledViewModel> m_DisplayWhiskers;

        private ObservableCollection<AngleTypeBase> m_AngleOptions = new ObservableCollection<AngleTypeBase>();
        private AngleTypeBase m_SelectedAngleOption;
        private double m_TValue;
        private Line m_CenterLine;

        public ObservableCollection<WhiskerAngleFrameViewModel> Frames
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

        public WhiskerAngleFrameViewModel CurrentFrame
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
                CreateCenterLine();

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

        public IWhiskerAngle Model
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

        public ObservableCollection<AngleTypeBase> AngleOptions
        {
            get
            {
                return m_AngleOptions;
            }
            set
            {
                if (ReferenceEquals(m_AngleOptions, value))
                {
                    return;
                }

                m_AngleOptions = value;

                NotifyPropertyChanged();
            }
        }

        public AngleTypeBase SelectedAngleOption
        {
            get
            {
                return m_SelectedAngleOption;
            }
            set
            {
                if (Equals(m_SelectedAngleOption, value))
                {
                    return;
                }

                m_SelectedAngleOption = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged("CenterLineSelected");
                AngleMethodChanged();
            }
        }

        public bool CenterLineSelected
        {
            get
            {
                return SelectedAngleOption is CenterLineViewModel;
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

        public Line CenterLine
        {
            get
            {
                return m_CenterLine;
            }
            set
            {
                if (Equals(m_CenterLine, value))
                {
                    return;
                }

                m_CenterLine = value;

                NotifyPropertyChanged();
            }
        }

        public WhiskerAngleViewModel(AnalyserViewModel parent, string name = "Whisker Angle")  : base(parent, name)
        {
            Model = ModelResolver.Resolve<IWhiskerAngle>();
            Model.LoadData(parent.Frames.Values.Select(x => x.Model).ToArray());
            CreateFrames();

            MethodControl = new WhiskerAngleMethodView()
            {
                DataContext = this,
            };

            Initialise();

            ObservableCollection<AngleTypeBase> angleOptions = new ObservableCollection<AngleTypeBase>();

            IWhisker nosePoint = Parent.CurrentFrame.Whiskers.Select(x => x.Model).FirstOrDefault(x => x.WhiskerId == -1);
            IWhisker orientationPoint = Parent.CurrentFrame.Whiskers.Select(x => x.Model).FirstOrDefault(x => x.WhiskerId == 0);

            if (nosePoint != null && orientationPoint != null)
            {
                angleOptions.Add(new CenterLineViewModel());
            }

            angleOptions.Add(new VerticalViewModel());
            angleOptions.Add(new HorizontalViewModel());

            AngleOptions = angleOptions;
            SelectedAngleOption = AngleOptions.First();
        }

        private void CreateFrames()
        {
            ObservableCollection<WhiskerAngleFrameViewModel> frames = new ObservableCollection<WhiskerAngleFrameViewModel>();

            foreach (IWhiskerAngleFrame frame in Model.Frames)
            {
                frames.Add(new WhiskerAngleFrameViewModel(frame, this));
            }

            Frames = frames;
            WhiskerAngleFrameViewModel firstFrame = Frames.First();
            ObservableCollection<WhiskersEnabledViewModel> displayWhiskers = new ObservableCollection<WhiskersEnabledViewModel>();

            foreach (SingleWhiskerAngleViewModel whisker in firstFrame.Whiskers)
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
            foreach (WhiskerAngleFrameViewModel whisker in Frames)
            {
                whisker.UpdatePositions(LastKnownImageWidth, LastKnownImageHeight);
            }

            CreateCenterLine();
        }

        protected void AngleMethodChanged()
        {
            foreach (WhiskerAngleFrameViewModel whisker in Frames)
            {
                whisker.UpdateAngleType(SelectedAngleOption);
            }
        }

        public override void PropagateFrameChangedNotifications(int indexNumber)
        {
            CurrentFrame = Frames[indexNumber];
        }

        protected void UpdateWhiskerTValue()
        {
            foreach (WhiskerAngleFrameViewModel whisker in Frames)
            {
                whisker.UpdateTValue(TValue);
            }
        }

        protected void CreateCenterLine()
        {
            IWhisker nosePoint = Parent.CurrentFrame.Whiskers.Select(x => x.Model).FirstOrDefault(x => x.WhiskerId == -1);
            IWhisker orientationPoint = Parent.CurrentFrame.Whiskers.Select(x => x.Model).FirstOrDefault(x => x.WhiskerId == 0);

            if (nosePoint == null || orientationPoint == null)
            {
                return;
            }

            double x0 = orientationPoint.WhiskerPoints[0].XRatio * LastKnownImageWidth;
            double y0 = orientationPoint.WhiskerPoints[0].YRatio * LastKnownImageHeight;
            double x1 = nosePoint.WhiskerPoints[0].XRatio * LastKnownImageWidth;
            double y1 = nosePoint.WhiskerPoints[0].YRatio * LastKnownImageHeight;

            Line centerLine = new Line();
            centerLine.X1 = x0;
            centerLine.Y1 = y0;
            centerLine.X2 = x1;
            centerLine.Y2 = y1;

            CenterLine = centerLine;
        }

        public string[] GetColumnTitles()
        {
            string[] titles = new string[NumberOfColumns()];

            for (int i = 0; i < titles.Length; i ++)
            {
                titles[i] = CurrentFrame.Whiskers[i].WhiskerName;
            }

            return titles;
        }

        public int NumberOfColumns()
        {
            return DisplayWhiskers.Count;
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

        public override void PropagateWhiskerEnabledNotification(int whiskerId, bool value)
        {
            foreach (var frame in Frames)
            {
                frame.Whiskers.First(x => x.Model.Whisker.WhiskerId == whiskerId).Enabled = value;
            }
        }
    }
}
