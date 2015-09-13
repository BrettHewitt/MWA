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

using RobynsWhiskerTracker.Model.Analyser.Methods.Amplitude;
using RobynsWhiskerTracker.Model.Analyser.Methods.Angles.AngleTypes;
using RobynsWhiskerTracker.Model.Analyser.Methods.Angles.WhiskerAngle;
using RobynsWhiskerTracker.Model.Analyser.Methods.Angles.WhiskerAngularVelocity;
using RobynsWhiskerTracker.Model.Analyser.Methods.Curvature;
using RobynsWhiskerTracker.Model.Analyser.Methods.Frequency;
using RobynsWhiskerTracker.Model.Analyser.Methods.Frequency.FrequencyTypes;
using RobynsWhiskerTracker.Model.Analyser.Methods.HeadOrientation;
using RobynsWhiskerTracker.Model.Analyser.Methods.Mean;
using RobynsWhiskerTracker.Model.Analyser.Methods.NoseDisplacement;
using RobynsWhiskerTracker.Model.Analyser.Methods.ProtractionRetraction;
using RobynsWhiskerTracker.Model.Analyser.Methods.Spread;
using RobynsWhiskerTracker.Model.Analyser.Methods.Velocity;
using RobynsWhiskerTracker.Model.ClipSettings;
using RobynsWhiskerTracker.Model.GenericPoint;
using RobynsWhiskerTracker.Model.MouseFrame;
using RobynsWhiskerTracker.Model.Settings;
using RobynsWhiskerTracker.Model.Whisker;
using RobynsWhiskerTracker.Model.WhiskerPoint;
using RobynsWhiskerTracker.Model.WhiskerVideo;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Amplitude;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.AngleTypes;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngle;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Angles.WhiskerAngularVelocity;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Curvature;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Frequency;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Frequency.FrequencyTypes;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.HeadOrientation;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Mean;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.NoseDisplacement;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.ProtractionRetraction;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Spread;
using RobynsWhiskerTracker.ModelInterface.Analyser.Methods.Velocity;
using RobynsWhiskerTracker.ModelInterface.ClipSettings;
using RobynsWhiskerTracker.ModelInterface.GenericPoint;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Settings;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using RobynsWhiskerTracker.ModelInterface.WhiskerVideo;
using System;
using System.Collections.Generic;

namespace RobynsWhiskerTracker.Resolver
{
    public static class ModelResolver
    {
        private static Dictionary<Type, Func<object>> _TypeDictionary = new Dictionary<Type, Func<object>>(); 

        public static T Resolve<T>() where T : class
        {
            return _TypeDictionary[typeof(T)].Invoke() as T;
        }

        static ModelResolver()
        {
            _TypeDictionary.Add(typeof(IWhiskerVideo), () => new WhiskerVideo());
            _TypeDictionary.Add(typeof(IColorSettings), () => new ColorSettings());
            _TypeDictionary.Add(typeof(IWhiskerPoint), () => new WhiskerPoint());
            _TypeDictionary.Add(typeof(IUnitSettings), () => new UnitSettings());
            _TypeDictionary.Add(typeof(IMouseFrame), () => new MouseFrame());
            _TypeDictionary.Add(typeof(IClipSettings), () => new ClipSettings());
            _TypeDictionary.Add(typeof(IWhisker), () => new Whisker());
            _TypeDictionary.Add(typeof(IWhiskerClipSettings), () => new WhiskerClipSettings());
            _TypeDictionary.Add(typeof(IGenericPoint), () => new GenericPoint());
            _TypeDictionary.Add(typeof(IFrameRateSettings), () => new FrameRateSettings());
            _TypeDictionary.Add(typeof(IVertical), () => new Vertical());
            _TypeDictionary.Add(typeof(IHorizontal), () => new Horizontal());
            _TypeDictionary.Add(typeof(ICenterLine), () => new CenterLine());
            _TypeDictionary.Add(typeof(ISingleWhiskerCurve), () => new SingleWhiskerCurve());
            _TypeDictionary.Add(typeof(IWhiskerCurveFrame), () => new WhiskerCurveFrame());
            _TypeDictionary.Add(typeof(IWhiskerCurvature), () => new WhiskerCurvature());
            _TypeDictionary.Add(typeof(ISingleWhiskerAngle), () => new SingleWhiskerAngle());
            _TypeDictionary.Add(typeof(IWhiskerAngleFrame), () => new WhiskerAngleFrame());
            _TypeDictionary.Add(typeof(IWhiskerAngle), () => new WhiskerAngle());
            _TypeDictionary.Add(typeof(ISingleWhiskerAngularVelocity), () => new SingleWhiskerAngularVelocity());
            _TypeDictionary.Add(typeof(IWhiskerAngularVelocityFrame), () => new WhiskerAngularVelocityFrame());
            _TypeDictionary.Add(typeof(IWhiskerAngularVelocity), () => new WhiskerAngularVelocity());
            _TypeDictionary.Add(typeof(ISingleWhiskerVelocity), () => new SingleWhiskerVelocity());
            _TypeDictionary.Add(typeof(IWhiskerVelocityFrame), () => new WhiskerVelocityFrame());
            _TypeDictionary.Add(typeof(IWhiskerVelocity), () => new WhiskerVelocity());
            _TypeDictionary.Add(typeof(ISingleHeadOrientation), () => new SingleHeadOrientation());
            _TypeDictionary.Add(typeof(IHeadOrientationFrame), () => new HeadOrientationFrame());
            _TypeDictionary.Add(typeof(IHeadOrientation), () => new HeadOrientation());
            _TypeDictionary.Add(typeof(IWhiskerFrequency), () => new WhiskerFrequency());
            _TypeDictionary.Add(typeof(ISingleWhiskerFrequency), () => new SingleWhiskerFrequency());
            _TypeDictionary.Add(typeof(INoseDisplacement), () => new NoseDisplacement());
            _TypeDictionary.Add(typeof(INoseDisplacementFrame), () => new NoseDisplacementFrame());
            _TypeDictionary.Add(typeof(IDiscreteFourierTransform), () => new DiscreteFourierTransform());
            _TypeDictionary.Add(typeof(IAutoCorrelogram), () => new AutoCorrelogram());
            _TypeDictionary.Add(typeof(IWhiskerMeanOffset), () => new WhiskerMeanOffset());
            _TypeDictionary.Add(typeof(ISingleWhiskerMeanOffset), () => new SingleWhiskerMeanOffset());
            _TypeDictionary.Add(typeof(IWhiskerProtractionRetraction), () => new WhiskerProtractionRetraction());
            _TypeDictionary.Add(typeof(ISingleWhiskerProtractionRetraction), () => new SingleWhiskerProtractionRetraction());
            _TypeDictionary.Add(typeof(IProtractionData), () => new ProtractionData());
            _TypeDictionary.Add(typeof(IRetractionData), () => new RetractionData());
            _TypeDictionary.Add(typeof(IWhiskerAmplitude), () => new WhiskerAmplitude());
            _TypeDictionary.Add(typeof(ISingleWhiskerAmplitude), () => new SingleWhiskerAmplitude());
            _TypeDictionary.Add(typeof(IWhiskerSpread), () => new WhiskerSpread());
            _TypeDictionary.Add(typeof(IWhiskerSpreadFrame), () => new WhiskerSpreadFrame());
        }
    }
}
