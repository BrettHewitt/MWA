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
using System.Windows.Controls;
using System.Windows.Data;
using RobynsWhiskerTracker.Commands;
using RobynsWhiskerTracker.Interfaces;
using RobynsWhiskerTracker.View.Analyser.Methods;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.AnalyseEverything;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerAngle.AngleTypes;
using RobynsWhiskerTracker.ViewModel.Analyser.Methods.WhiskerFrequency.FrequencyTypes;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods
{
    public class AnalyseEverythingViewModel : MethodBase
    {
        private ObservableCollection<AnalyseEverythingBaseViewModel> m_AnalysisMethods; 
        private ObservableCollection<AnalyseEverythingBaseViewModel> m_ItemsToAnalyse;
        private ObservableCollection<DataGridColumn> m_Columns;
        private ObservableCollection<object[]> m_ColumnData; 

        private ActionCommandWithParameter m_AddCommand;
        private ActionCommandWithParameter m_RemoveCommand;

        private double m_TValue;
        private ObservableCollection<AngleTypeBase> m_AngleOptions;
        private ObservableCollection<FrequencyTypeBaseViewModel> m_FrequencyTypes;
        private AngleTypeBase m_SelectedAngleOption;
        private FrequencyTypeBaseViewModel m_SelectedFrequencyType;

        private object[,] FinalData
        {
            get;
            set;
        }

        public ActionCommandWithParameter AddCommand
        {
            get
            {
                return m_AddCommand ?? (m_AddCommand = new ActionCommandWithParameter()
                {
                    ExecuteAction = AddButton,
                });
            }
        }

        public ActionCommandWithParameter RemoveCommand
        {
            get
            {
                return m_RemoveCommand ?? (m_RemoveCommand = new ActionCommandWithParameter()
                {
                    ExecuteAction = RemoveButton,
                });
            }
        }

        public ObservableCollection<AnalyseEverythingBaseViewModel> AnalysisMethods
        {
            get
            {
                return m_AnalysisMethods;
            }
            set
            {
                if (ReferenceEquals(m_AnalysisMethods, value))
                {
                    return;
                }

                m_AnalysisMethods = value;

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<AnalyseEverythingBaseViewModel> ItemsToAnalyse
        {
            get
            {
                return m_ItemsToAnalyse;
            }
            set
            {
                if (ReferenceEquals(m_ItemsToAnalyse, value))
                {
                    return;
                }

                m_ItemsToAnalyse = value;

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<DataGridColumn> Columns
        {
            get
            {
                return m_Columns;
            }
            set
            {
                if (ReferenceEquals(m_Columns, value))
                {
                    return;
                }

                m_Columns = value;

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<object[]> ColumnData
        {
            get
            {
                return m_ColumnData;
            }
            private set
            {
                if (ReferenceEquals(m_ColumnData, value))
                {
                    return;
                }

                m_ColumnData = value;

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

                m_TValue = value;

                CreateColumnsAndData();

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

                CreateColumnsAndData();

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

                CreateColumnsAndData();

                NotifyPropertyChanged();
            }
        }

        public AnalyseEverythingViewModel(AnalyserViewModel parent) : base(parent, "Analyse Everything")
        {
            ObservableCollection<AnalyseEverythingBaseViewModel> analysisMethods = new ObservableCollection<AnalyseEverythingBaseViewModel>();

            foreach (MethodBase method in parent.AnalysisMethods)
            {
                if (method is NoseDisplacementViewModel || method is WhiskerFrequencyViewModel || method is WhiskerMeanOffsetViewModel || method is WhiskerAmplitudeViewModel || method is WhiskerProtractionRetractionViewModel)
                {
                    analysisMethods.Add(new AnalyseEverythingBaseViewModel(this, method));
                }
            }
            
            AnalysisMethods = analysisMethods;

            ItemsToAnalyse = new ObservableCollection<AnalyseEverythingBaseViewModel>();

            MethodControl = new AnalyseEverythingView()
            {
                DataContext = this,
            };

            ShowFrameSlider = false;

            ObservableCollection<AngleTypeBase> angleOptions = new ObservableCollection<AngleTypeBase>();
            angleOptions.Add(new CenterLineViewModel());
            angleOptions.Add(new VerticalViewModel());
            angleOptions.Add(new HorizontalViewModel());
            AngleOptions = angleOptions;
            SelectedAngleOption = AngleOptions.First();

            ObservableCollection<FrequencyTypeBaseViewModel> frequencyTypes = new ObservableCollection<FrequencyTypeBaseViewModel>();
            frequencyTypes.Add(new AutocorrelogramViewModel());
            frequencyTypes.Add(new DiscreteFourierTransformViewModel());
            FrequencyTypes = frequencyTypes;
            SelectedFrequencyType = FrequencyTypes.First();

            Initialise();
        }

        private void CreateColumnsAndData()
        {
            List<object[,]> data = new List<object[,]>();

            int numberOfRows = 0;
            int maxColumns = 0;

            foreach (AnalyseEverythingBaseViewModel item in ItemsToAnalyse)
            {
                if (!item.Method.Loaded)
                {
                    item.Method.LoadData();
                }

                IUpdateAngle updateAngle = item.Method as IUpdateAngle;
                if (updateAngle != null)
                {
                    updateAngle.SelectedAngleOption = SelectedAngleOption;
                }

                IUpdateTValue tValue = item.Method as IUpdateTValue;
                if (tValue != null)
                {
                    tValue.TValue = TValue;
                }

                IUpdateFrequency updateFrequency = item.Method as IUpdateFrequency;
                if (updateFrequency != null)
                {
                    updateFrequency.SelectedFrequencyType = SelectedFrequencyType;
                }

                object[,] methodData = item.Method.ExportMeanResults();

                if (methodData == null)
                {
                    continue;
                }

                numberOfRows += methodData.Length + 3;
                int columnCount = methodData.GetLength(1);
                if (columnCount > maxColumns)
                {
                    maxColumns = columnCount;
                }

                data.Add(methodData);
            }

            ObservableCollection<DataGridColumn> columns = new ObservableCollection<DataGridColumn>();
            for (int column = 0; column < maxColumns; column++)
            {
                DataGridTextColumn currentColumn = new DataGridTextColumn();
                currentColumn.Binding = new Binding("[" + column + "]");
                columns.Add(currentColumn);
            }

            int rowCounter = 0;
            ObservableCollection<object[]> columnData = new ObservableCollection<object[]>();
            foreach (object[,] methodData in data)
            {
                int rowCount = methodData.GetLength(0);
                int columnCount = methodData.GetLength(1);

                for (int i = 0; i < rowCount; i++)
                {
                    object[] currentRow = new object[columnCount];
                    for (int j = 0; j < columnCount; j++)
                    {
                        currentRow[j] = methodData[i, j];
                    }

                    columnData.Add(currentRow);
                }
            }

            Columns = columns;
            ColumnData = columnData;

            object[,] finalData = new object[numberOfRows, maxColumns];

            //int rowCounter = 0;
            foreach (object[,] methodData in data)
            {
                int rowCount = methodData.GetLength(0);
                int columnCount = methodData.GetLength(1);

                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        finalData[i + rowCounter, j] = methodData[i, j];
                    }
                }

                rowCounter += rowCount;
                rowCounter += 1;
                finalData[rowCounter, 0] = "--------------------------------------------";
                rowCounter += 2;
            }

            FinalData = finalData;
        }

        public override object[,] ExportResults()
        {
            return FinalData;
        }

        public override void PropagateWhiskerEnabledNotification(int whiskerId, bool value)
        {
            
        }

        private void AddButton(object item)
        {
            AnalyseEverythingBaseViewModel method = item as AnalyseEverythingBaseViewModel;

            if (method == null)
            {
                return;
            }

            method.IsEnabled = false;
            ItemsToAnalyse.Add(method);

            CreateColumnsAndData();

            NotifyPropertyChanged("ItemsToAnalyse");
        }

        private void RemoveButton(object item)
        {
            AnalyseEverythingBaseViewModel method = item as AnalyseEverythingBaseViewModel;

            if (method == null)
            {
                return;
            }

            method.IsEnabled = true;
            ItemsToAnalyse.Remove(method);

            CreateColumnsAndData();

            NotifyPropertyChanged("ItemsToAnalyse");
        }
    }
}
