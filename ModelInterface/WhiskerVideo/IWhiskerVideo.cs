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
using Emgu.CV;
using Emgu.CV.Structure;

namespace RobynsWhiskerTracker.ModelInterface.WhiskerVideo
{
    public interface IWhiskerVideo : IModelObjectBase, IDisposable
    {
        int FrameNumber
        {
            get;
        }

        int FrameCount
        {
            get;
        }

        double ElpasedTime
        {
            get;
        }

        double FrameRate
        {
            get;
        }

        double Width
        {
            get;
        }

        double Height
        {
            get;
        }

        string FilePath
        {
            get;
        }

        Image<Bgr, Byte> GetFrameImage();
        Image<Gray, Byte> GetGrayFrameImage();

        void SetVideo(string filepath);
        void Reset();
        void SetFrame(int frameNumber);
        void SetFrame(double relativePosition);
    }
}
