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
using RobynsWhiskerTracker.Interfaces;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Frequency;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.Resolver;
using RobynsWhiskerTracker.View.Analyser.Methods;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerAngle.AngleTypes;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerFrequency;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerFrequency.FrequencyTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods
{
    public class WhiskerFrequencyViewModel : MethodBaseWithImage, IUpdateAngle, IUpdateFrequency, IUpdateTValue
    {
        private IWhiskerFrequency m_Model;
        private ObservableCollection<SingleWhiskerFrequencyViewModel> m_Whiskers;
        private ObservableCollection<WhiskersEnabledViewModel> m_DisplayWhiskers;
        private ObservableCollection<FrequencyTypeBaseViewModel> m_FrequencyTypes;
        private ObservableCollection<AngleTypeBase> m_AngleOptions; 
        private FrequencyTypeBaseViewModel m_SelectedFrequencyType;
        private AngleTypeBase m_SelectedAngleOption;

        private ObservableCollection<KeyValuePair<double, double>> m_AngleData;
        private ObservableCollection<KeyValuePair<double, double>> m_ExtraData;
        private SingleWhiskerFrequencyViewModel m_SelectedWhisker;
        private string m_ExtraDataName;
        private string m_ExtraDataXAxisTitle;
        private string m_ExtraDataYAxisTitle;

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

        public IWhiskerFrequency Model
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

        public ObservableCollection<SingleWhiskerFrequencyViewModel> Whiskers
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

        public ObservableCollection<FrequencyTypeBaseViewModel> FrequencyTypes
        {
            get
            {
                return m_FrequencyTypes;
            }
            set
            {
                if (ReferenceEquals(m_FrequencyTypes, value))
                {
                    return;
                }

                m_FrequencyTypes = value;

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

                CreateGraphData();
                FrequencyChanged();

                NotifyPropertyChanged();
            }
        }

        public FrequencyTypeBaseViewModel SelectedFrequencyType
        {
            get
            {
                return m_SelectedFrequencyType;
            }
            set
            {
                if (Equals(m_SelectedFrequencyType, value))
                {
                    return;
                }

                m_SelectedFrequencyType = value;

                Model.FrequencyMethod = m_SelectedFrequencyType.Model;
                string[] methodNames = m_SelectedFrequencyType.Model.GetExtraDataNames();
                ExtraDataName = methodNames[0];
                ExtraDataXAxisTitle = methodNames[1];
                ExtraDataYAxisTitle = methodNames[2];
                CreateGraphData();
                FrequencyChanged();

                NotifyPropertyChanged();
            }
        }

        public SingleWhiskerFrequencyViewModel SelectedWhisker
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

        public ObservableCollection<KeyValuePair<double, double>> ExtraData
        {
            get
            {
                return m_ExtraData;
            }
            set
            {
                if (ReferenceEquals(m_ExtraData, value))
                {
                    return;
                }

                m_ExtraData = value;

                NotifyPropertyChanged();
            }
        }

        public string ExtraDataName
        {
            get
            {
                return m_ExtraDataName;
            }
            set
            {
                if (Equals(m_ExtraDataName, value))
                {
                    return;
                }

                m_ExtraDataName = value;

                NotifyPropertyChanged();
            }
        }

        public string ExtraDataXAxisTitle
        {
            get
            {
                return m_ExtraDataXAxisTitle;
            }
            set
            {
                if (Equals(m_ExtraDataXAxisTitle, value))
                {
                    return;
                }

                m_ExtraDataXAxisTitle = value;

                NotifyPropertyChanged();
            }
        }

        public string ExtraDataYAxisTitle
        {
            get
            {
                return m_ExtraDataYAxisTitle;
            }
            set
            {
                if (Equals(m_ExtraDataYAxisTitle, value))
                {
                    return;
                }

                m_ExtraDataYAxisTitle = value;

                NotifyPropertyChanged();
            }
        }

        public WhiskerFrequencyViewModel(AnalyserViewModel parent) : base(parent, "Whisker Frequency")
        {
            FrequencyTypes = new ObservableCollection<FrequencyTypeBaseViewModel>()
            {
                new AutocorrelogramViewModel(),
                new DiscreteFourierTransformViewModel(),
            };

            Model = ModelResolver.Resolve<IWhiskerFrequency>();
            Model.LoadData(parent.Frames.Values.Select(x => x.Model).ToArray());
            SelectedFrequencyType = FrequencyTypes.First();

            CreateFrames();

            MethodControl = new WhiskerFrequencyView()
            {
                DataContext = this,
            };

            Initialise();
        }

        private void CreateFrames()
        {
            ObservableCollection<SingleWhiskerFrequencyViewModel> whiskers = new ObservableCollection<SingleWhiskerFrequencyViewModel>();
            foreach (ISingleWhiskerFrequency frequency in Model.Whiskers)
            {
                SingleWhiskerFrequencyViewModel frequencyViewModel = new SingleWhiskerFrequencyViewModel(frequency);
                //frequencyViewModel.EnabledChanged += PropagateWhiskerEnabledNotification;
                whiskers.Add(frequencyViewModel);
            }

            Whiskers = whiskers;
            SelectedWhisker = Whiskers.First();

            ObservableCollection<WhiskersEnabledViewModel> displayWhiskers = new ObservableCollection<WhiskersEnabledViewModel>();
            foreach (SingleWhiskerFrequencyViewModel whisker in Whiskers)
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

            CreateGraphData();
        }

        private void CreateGraphData()
        {
            if (SelectedWhisker == null)
            {
                return;
            }

            ObservableCollection<KeyValuePair<double, double>> graph = new ObservableCollection<KeyValuePair<double, double>>();
            double[] signal = SelectedWhisker.Model.Signal;
            int counter = 0;
            foreach (double value in signal)
            {
                graph.Add(new KeyValuePair<double, double>(counter, value));
                counter++;
            }

            ObservableCollection<KeyValuePair<double, double>> extra = new ObservableCollection<KeyValuePair<double, double>>();
            Dictionary<double, double> extraData = SelectedWhisker.Model.ExtraData;

            counter = 0;
            int dataInterval = extraData.Count/1000;

            if (dataInterval == 0)
            {
                dataInterval = 1;
            }

            while (counter < extraData.Count)
            {
                KeyValuePair<double, double> kvp = extraData.ElementAt(counter);
                extra.Add(new KeyValuePair<double, double>(Math.Round(kvp.Key, 2), kvp.Value));
                counter += dataInterval;
            }

            AngleData = graph;
            ExtraData = extra;
        }

        protected override void UpdateWhiskerPositions(System.Windows.Controls.Image image, System.Windows.SizeChangedEventArgs e)
        {
            
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
                    disabledColumns.Add(i);
                }
            }

            //Create storage
            object[,] data = new object[numberOfRows, numberOfColumns - disabledColumns.Count];

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

            object[,] data = new object[rowCount + 1, columnCount];
            data[0, 0] = "Frequency";
            for (int i = 0; i < rowCount; i++)
            {
                object[] rowData = rawData[i];
                for (int j = 0; j < rowData.Length; j++)
                {
                    data[i + 1, j] = rowData[j];
                }
            }

            return data;
        }

        public override void PropagateWhiskerEnabledNotification(int whiskerId, bool value)
        {
            
        }

        private void TValueChanged(object param)
        {
            if (param is double)
            {
                TValue = (double) param;
            }
        }

        private void FrequencyChanged()
        {
            if (Whiskers == null)
            {
                return;
            }

            foreach (var whisker in Whiskers)
            {
                whisker.NotifyFrequencyChanged();
            }
        }

        private void AngleMethodChanged()
        {
            Model.UpdateAngleType(SelectedAngleOption.Model);
            CreateGraphData();
            FrequencyChanged();
        }
    }
}
