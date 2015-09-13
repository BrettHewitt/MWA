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
using System.Collections.ObjectModel;
using System.Linq;
using RobynsWhiskerTracker.Commands;
using RobynsWhiskerTracker.Interfaces;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Mean;
using RobynsWhiskerTracker.Resolver;
using RobynsWhiskerTracker.View.Analyser.Methods;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.Mean;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods
{
    public class WhiskerMeanOffsetViewModel : MethodBaseWithImage, IUpdateTValue
    {
        private IWhiskerMeanOffset m_Model;
        private ObservableCollection<SingleWhiskerMeanOffsetViewModel> m_Whiskers;
        private ObservableCollection<WhiskersEnabledViewModel> m_DisplayWhiskers;

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

        public IWhiskerMeanOffset Model
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

        public ObservableCollection<SingleWhiskerMeanOffsetViewModel> Whiskers
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

                MeanOffsetChanged();

                NotifyPropertyChanged();
            }
        }

        public WhiskerMeanOffsetViewModel(AnalyserViewModel parent) : base(parent, "Whisker Mean Offset")
        {
            Model = ModelResolver.Resolve<IWhiskerMeanOffset>();
            Model.LoadData(parent.Frames.Values.Select(x => x.Model).ToArray());

            MethodControl = new WhiskerMeanOffsetView()
            {
                DataContext = this,
            };

            CreateFrames();

            Initialise();
        }

        private void CreateFrames()
        {
            ObservableCollection<SingleWhiskerMeanOffsetViewModel> whiskers = new ObservableCollection<SingleWhiskerMeanOffsetViewModel>();
            foreach (ISingleWhiskerMeanOffset meanOffset in Model.Whiskers)
            {
                SingleWhiskerMeanOffsetViewModel meanOffsetViewModel = new SingleWhiskerMeanOffsetViewModel(meanOffset);
                whiskers.Add(meanOffsetViewModel);
            }

            Whiskers = whiskers;

            ObservableCollection<WhiskersEnabledViewModel> displayWhiskers = new ObservableCollection<WhiskersEnabledViewModel>();
            foreach (SingleWhiskerMeanOffsetViewModel whisker in Whiskers)
            {
                WhiskersEnabledViewModel displayWhisker = new WhiskersEnabledViewModel(this, whisker);
                displayWhiskers.Add(displayWhisker);
            }

            DisplayWhiskers = displayWhiskers;

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
            object[][] data = Model.ExportMeanData();

            int rows = data.Length;
            int columns = 0;

            foreach (object[] column in data)
            {
                if (column.Length > columns)
                {
                    columns = column.Length;
                }
            }

            object[,] returnData = new object[rows, columns];

            for (int i = 0; i < data.Length; i++)
            {
                object[] currentRow = data[i];

                for (int j = 0; j < currentRow.Length; j++)
                {
                    returnData[i, j] = currentRow[j];
                }
            }

            return returnData;
        }

        public override void PropagateWhiskerEnabledNotification(int whiskerId, bool value)
        {
            
        }

        private void TValueChanged(object param)
        {
            if (param is double)
            {
                TValue = (double)param;
            }
        }

        private void MeanOffsetChanged()
        {
            if (Whiskers == null)
            {
                return;
            }

            foreach (var whisker in Whiskers)
            {
                whisker.NotifyMeanOffsetChanged();
            }
        }
    }
}
