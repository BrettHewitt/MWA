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

using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Velocity;
using RobynsWhiskerTracker.Resolver;
using RobynsWhiskerTracker.View.Analyser.Methods;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerVelocity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Image = System.Windows.Controls.Image;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods
{
    public class WhiskerVelocityViewModel : MethodBaseWithImage
    {
        private IWhiskerVelocity m_Model;

        private ObservableCollection<WhiskerVelocityFrameViewModel> m_Frames = new ObservableCollection<WhiskerVelocityFrameViewModel>();
        private WhiskerVelocityFrameViewModel m_CurrentFrame;
        private ObservableCollection<WhiskersEnabledViewModel> m_DisplayWhiskers; 

        public IWhiskerVelocity Model
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

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<WhiskerVelocityFrameViewModel> Frames
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

        public WhiskerVelocityFrameViewModel CurrentFrame
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

        public WhiskerVelocityViewModel(AnalyserViewModel parent) : base(parent, "Whisker Velocity")
        {
            Model = ModelResolver.Resolve<IWhiskerVelocity>();
            Model.LoadData(parent.Frames.Values.Select(x => x.Model).ToArray());
            CreateFrames();

            MethodControl = new WhiskerVelocityView()
            {
                DataContext = this,
            };

            Initialise();
        }

        private void CreateFrames()
        {
            ObservableCollection<WhiskerVelocityFrameViewModel> frames = new ObservableCollection<WhiskerVelocityFrameViewModel>();

            foreach (IWhiskerVelocityFrame frame in Model.Frames)
            {
                frames.Add(new WhiskerVelocityFrameViewModel(frame, this));
            }

            Frames = frames;
            WhiskerVelocityFrameViewModel firstFrame = Frames.First();
            ObservableCollection<WhiskersEnabledViewModel> displayWhiskers = new ObservableCollection<WhiskersEnabledViewModel>();

            foreach (SingleWhiskerVelocityViewModel whisker in firstFrame.Whiskers)
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

        protected override void UpdateWhiskerPositions(Image image, SizeChangedEventArgs e)
        {
            foreach (WhiskerVelocityFrameViewModel frame in Frames)
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

        public void PropagateHeadCompensationNotification(int whiskerId, bool value)
        {
            foreach (var frame in Frames)
            {
                frame.Whiskers.First(x => x.Model.Whisker.WhiskerId == whiskerId).CompensateHeadMovement = value;
            }
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
    }
}
