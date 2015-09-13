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

using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Spread;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;
using RobynsWhiskerTracker.View.Analyser.Methods;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.Spread;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Shapes;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods
{
    public class WhiskerSpreadViewModel : MethodBaseWithImage
    {
        private IWhiskerSpread m_Model;

        private ObservableCollection<WhiskerSpreadFrameViewModel> m_Frames = new ObservableCollection<WhiskerSpreadFrameViewModel>();
        private WhiskerSpreadFrameViewModel m_CurrentFrame;

        private double m_TValue;
        private Line m_CenterLine;

        public IWhiskerSpread Model
        {
            get
            {
                return m_Model;
            }
            private set
            {
                if (Equals(m_Model, value))
                {
                    return;
                }

                m_Model = value;

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<WhiskerSpreadFrameViewModel> Frames
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

        public WhiskerSpreadFrameViewModel CurrentFrame
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

                NotifyPropertyChanged();
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

                UpdateTValue();

                NotifyPropertyChanged();
            }
        }

        public WhiskerSpreadViewModel(AnalyserViewModel parent) : base(parent, "Whisker Spread")
        {
            IWhisker nosePoint = Parent.CurrentFrame.Whiskers.Select(x => x.Model).FirstOrDefault(x => x.WhiskerId == -1);
            IWhisker orientationPoint = Parent.CurrentFrame.Whiskers.Select(x => x.Model).FirstOrDefault(x => x.WhiskerId == 0);

            if (nosePoint == null || orientationPoint == null)
            {
                //Can't work!
                return;
            }

            Model = ModelResolver.Resolve<IWhiskerSpread>();
            Model.LoadData(parent.Frames.Values.Select(x => x.Model).ToArray());
            Model.UpdateTValue(0);
            CreateFrames();

            MethodControl = new WhiskerSpreadView()
            {
                DataContext = this,
            };

            Initialise();
        }

        private void CreateFrames()
        {
            ObservableCollection<WhiskerSpreadFrameViewModel> frames = new ObservableCollection<WhiskerSpreadFrameViewModel>();

            foreach (IWhiskerSpreadFrame frame in Model.Frames)
            {
                frames.Add(new WhiskerSpreadFrameViewModel(frame, this));
            }

            Frames = frames;
            CurrentFrame = Frames.First();
        }

        protected override void UpdateWhiskerPositions(System.Windows.Controls.Image image, System.Windows.SizeChangedEventArgs e)
        {
            foreach (WhiskerSpreadFrameViewModel whisker in Frames)
            {
                whisker.UpdatePositions(LastKnownImageWidth, LastKnownImageHeight);
            }
        }

        public override object[,] ExportResults()
        {
            object[][] modelData = Model.ExportData();
            object[,] data = new object[modelData.Length, 5];

            for (int i = 0; i < modelData.Length; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    data[i, j] = modelData[i][j];
                }
            }

            return data;
        }

        public override void PropagateWhiskerEnabledNotification(int whiskerId, bool value)
        {
            
        }

        public override void PropagateFrameChangedNotifications(int indexNumber)
        {
            CurrentFrame = Frames[indexNumber];
        }

        private void UpdateTValue()
        {
            foreach (WhiskerSpreadFrameViewModel whisker in Frames)
            {
                whisker.UpdateTValue(TValue);
            }
            Model.UpdateTValue(TValue);
        }
    }
}
