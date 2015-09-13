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
using RobynsWhiskerTracker.Commands;
using RobynsWhiskerTracker.Interfaces;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Amplitude;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;
using RobynsWhiskerTracker.View.Analyser.Methods;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerAmplitude;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerAngle.AngleTypes;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods
{
    public class WhiskerAmplitudeViewModel : MethodBaseWithImage, IUpdateAngle, IUpdateTValue
    {
        private ObservableCollection<SingleWhiskerAmplitudeViewModel> m_Whiskers;
        private SingleWhiskerAmplitudeViewModel m_SelectedWhisker;
        private ObservableCollection<WhiskersEnabledViewModel> m_DisplayWhiskers;

        private ObservableCollection<KeyValuePair<double, double>> m_AngleData;
        private ObservableCollection<KeyValuePair<double, double>> m_CurrentFrameIndicator;

        private ObservableCollection<AngleTypeBase> m_AngleOptions;
        private AngleTypeBase m_SelectedAngleOption;

        private IWhiskerAmplitude m_Model;

        private ActionCommandWithParameter m_ValueChangedCommand;

        public ActionCommandWithParameter ValueChangedCommand
        {
            get
            {
                return m_ValueChangedCommand ?? (m_ValueChangedCommand = new ActionCommandWithParameter()
                {
                    ExecuteAction = TValueChanged
                });
            }
        }

        public IWhiskerAmplitude Model
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

        public ObservableCollection<SingleWhiskerAmplitudeViewModel> Whiskers
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
            }
        }

        public SingleWhiskerAmplitudeViewModel SelectedWhisker
        {
            get
            {
                return m_SelectedWhisker;
            }
            set
            {
                if (Equals(m_SelectedWhisker, value))
                {
                    return;
                }

                m_SelectedWhisker = value;

                CreateGraphData();

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

        public ObservableCollection<KeyValuePair<double, double>> AngleData
        {
            get
            {
                return m_AngleData;
            }
            set
            {
                if (ReferenceEquals(m_AngleData, value))
                {
                    return;
                }

                m_AngleData = value;

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<KeyValuePair<double, double>> CurrentFrameIndicator
        {
            get
            {
                return m_CurrentFrameIndicator;
            }
            set
            {
                if (ReferenceEquals(m_CurrentFrameIndicator, value))
                {
                    return;
                }

                m_CurrentFrameIndicator = value;

                NotifyPropertyChanged();
            }
        }

        public double TValue
        {
            get
            {
                return Model.TValue;
            }
            set
            {
                if (Equals(Model.TValue, value))
                {
                    return;
                }

                Model.TValue = value;

                AmplitudeChanged();

                NotifyPropertyChanged();
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
                AngleMethodChanged();
            }
        }

        private double MaxValue
        {
            get;
            set;
        }

        private double MinValue
        {
            get;
            set;
        }

        public WhiskerAmplitudeViewModel(AnalyserViewModel parent) : base(parent, "Whisker Amplitude")
        {
            Model = ModelResolver.Resolve<IWhiskerAmplitude>();

            MethodControl = new WhiskerAmplitudeView()
            {
                DataContext = this,
            };
        }


        public override void LoadData()
        {
            Model.LoadData(Parent.Frames.Values.Select(x => x.Model).ToArray());

            //CheckForErrors();

            CreateFrames();

            SelectedWhisker = Whiskers.First();

            Initialise();

            Loaded = true;
        }

        private void CreateFrames()
        {
            ObservableCollection<SingleWhiskerAmplitudeViewModel> whiskers = new ObservableCollection<SingleWhiskerAmplitudeViewModel>();
            foreach (ISingleWhiskerAmplitude whisker in Model.Whiskers)
            {
                SingleWhiskerAmplitudeViewModel meanOffsetViewModel = new SingleWhiskerAmplitudeViewModel(whisker);
                whiskers.Add(meanOffsetViewModel);
            }

            Whiskers = whiskers;

            ObservableCollection<WhiskersEnabledViewModel> displayWhiskers = new ObservableCollection<WhiskersEnabledViewModel>();
            foreach (SingleWhiskerAmplitudeViewModel whisker in Whiskers)
            {
                WhiskersEnabledViewModel displayWhisker = new WhiskersEnabledViewModel(this, whisker);
                displayWhiskers.Add(displayWhisker);
            }

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

            DisplayWhiskers = displayWhiskers;
        }

        protected override void UpdateWhiskerPositions(System.Windows.Controls.Image image, System.Windows.SizeChangedEventArgs e)
        {
            
        }

        public override void PropagateWhiskerEnabledNotification(int whiskerId, bool value)
        {
            
        }

        public override void PropagateFrameChangedNotifications(int indexNumber)
        {
            UpdateFrameIndicator(indexNumber);
            foreach (var whisker in Whiskers)
            {
                whisker.UpdateFrameNumber(indexNumber);
            }
        }

        private void UpdateFrameIndicator()
        {
            if (CurrentFrameIndicator == null || CurrentFrameIndicator.Count == 0)
            {
                return;
            }

            ObservableCollection<KeyValuePair<double, double>> currentFrameIndicator = new ObservableCollection<KeyValuePair<double, double>>();
            currentFrameIndicator.Add(new KeyValuePair<double, double>(CurrentFrameIndicator[0].Key, MinValue));
            currentFrameIndicator.Add(new KeyValuePair<double, double>(CurrentFrameIndicator[1].Key, MaxValue));

            CurrentFrameIndicator = currentFrameIndicator;
        }

        private void UpdateFrameIndicator(int frameNumber)
        {
            ObservableCollection<KeyValuePair<double, double>> frameIndicator = new ObservableCollection<KeyValuePair<double, double>>();

            frameIndicator.Add(new KeyValuePair<double, double>(frameNumber, MinValue));
            frameIndicator.Add(new KeyValuePair<double, double>(frameNumber, MaxValue));

            CurrentFrameIndicator = frameIndicator;
        }

        private void TValueChanged(object tValue)
        {
            if (tValue is double)
            {
                TValue = (double)tValue;
            }
        }

        private void AmplitudeChanged()
        {
            if (Whiskers == null)
            {
                return;
            }

            CreateGraphData();

            foreach (SingleWhiskerAmplitudeViewModel whisker in Whiskers)
            {
                whisker.NotifyAmplitudeChanged();
            }
        }

        private void CreateGraphData()
        {
            if (SelectedWhisker == null)
            {
                return;
            }

            ObservableCollection<KeyValuePair<double, double>> angleData = new ObservableCollection<KeyValuePair<double, double>>();
            double minValue = 1000;
            double maxValue = -1000;
            for (int i = 0; i < SelectedWhisker.Model.Signal.Length; i++)
            {
                double angularVelocity = SelectedWhisker.Model.Signal[i];
                angleData.Add(new KeyValuePair<double, double>(i, angularVelocity));

                if (angularVelocity < minValue)
                {
                    minValue = angularVelocity;
                }

                if (angularVelocity > maxValue)
                {
                    maxValue = angularVelocity;
                }
            }

            MinValue = minValue;
            MaxValue = maxValue;

            UpdateFrameIndicator();
            AngleData = angleData;
        }

        private void AngleMethodChanged()
        {
            Model.UpdateAngleType(SelectedAngleOption.Model);
            CreateGraphData();
            AmplitudeChanged();
        }

        public override object[,] ExportResults()
        {
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
                    disabledColumns.Add(1 + (i * 5));
                    disabledColumns.Add(2 + (i * 5));
                    disabledColumns.Add(3 + (i * 5));
                    disabledColumns.Add(4 + (i * 5));
                    disabledColumns.Add(5 + (i * 5));
                }
            }

            //Create storage
            object[,] data = new object[numberOfRows, numberOfColumns - (disabledColumns.Count)];

            //Add frame counter, and only the enabled whiskers
            for (int i = 0; i < numberOfRows; i++)
            {
                object[] currentRow = rawData[i];

                int disabledColumnCounter = 0;
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

            return data;
        }

        public override object[,] ExportMeanResults()
        {
            int rowCount, columnCount;
            object[][] rawData = Model.ExportMeanData(out rowCount, out columnCount);

            object[,] data = new object[rowCount, columnCount];

            for (int i = 0; i < rowCount; i++)
            {
                object[] rowData = rawData[i];
                for (int j = 0; j < rowData.Length; j++)
                {
                    data[i, j] = rowData[j];
                }
            }

            return data;
        }
    }
}
