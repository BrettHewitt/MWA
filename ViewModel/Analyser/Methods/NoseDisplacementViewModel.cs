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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.NoseDisplacement;
using RobynsWhiskerTracker.ModelInterface.Settings;
using RobynsWhiskerTracker.Resolver;
using RobynsWhiskerTracker.View.Analyser.Methods;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods
{
    public class NoseDisplacementViewModel : MethodBaseWithImage
    {
        private INoseDisplacement m_Model;

        public INoseDisplacement Model
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

        public double DistanceTravelled
        {
            get
            {
                return Model.DistanceTravelled;
            }
        }

        public string DisplayDistanceTravelled
        {
            get
            {
                IUnitSettings unitSettings = GlobalSettings.GlobalSettings.UnitSettings;
                return (DistanceTravelled * unitSettings.UnitsPerPixel) + " " + unitSettings.UnitsName;
            }
        }

        public NoseDisplacementViewModel(AnalyserViewModel parent) : base(parent, "Nose Displacement")
        {
            Model = ModelResolver.Resolve<INoseDisplacement>();
            Model.LoadData(parent.Frames.Values.Select(x => x.Model).ToArray());

            MethodControl = new NoseDisplacementView()
            {
                DataContext = this,
            };

            ShowFrameSlider = false;

            Initialise();
        }

        protected override void UpdateWhiskerPositions(System.Windows.Controls.Image image, System.Windows.SizeChangedEventArgs e)
        {
            
        }

        public override object[,] ExportResults()
        {
            object[][] data = Model.ExportData();

            object[,] returnData = new object[2, 2];
            returnData[0, 0] = data[0][0];
            returnData[1, 0] = data[1][0];
            returnData[1, 1] = data[1][1];

            return returnData;
        }

        public override void PropagateWhiskerEnabledNotification(int whiskerId, bool value)
        {
            
        }

        public override object[,] ExportMeanResults()
        {
            object[][] data = Model.ExportMeanData();

            object[,] returnData = new object[2,2];
            returnData[0, 0] = data[0][0];
            returnData[1, 0] = data[1][0];
            returnData[1, 1] = data[1][1];

            return returnData;
        }
    }
}
