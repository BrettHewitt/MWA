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
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using RobynsWhiskerTracker.ViewModel;

namespace RobynsWhiskerTracker.Model.MouseFrame
{
    internal class MouseFrame : ModelObjectBase, IMouseFrame, IDisposable
    {
        private int m_FrameNumber;
        private int m_IndexNumber;
        private Image<Bgr, Byte> m_Frame;
        private IWhisker[] m_Whiskers;

        public int FrameNumber
        {
            get
            {
                return m_FrameNumber;
            }
            set
            {
                if (Equals(m_FrameNumber, value))
                {
                    return;
                }

                m_FrameNumber = value;

                MarkAsDirty();
            }
        }

        public int IndexNumber
        {
            get
            {
                return m_IndexNumber;
            }
            set
            {
                if (Equals(m_IndexNumber, value))
                {
                    return;
                }

                m_IndexNumber = value;

                MarkAsDirty();
            }
        }

        public Image<Bgr, Byte> Frame
        {
            get
            {
                return m_Frame;
            }
            set
            {
                if (Equals(m_Frame, value))
                {
                    return;
                }

                m_Frame = value;

                MarkAsDirty();
            }
        }

        public double OriginalWidth
        {
            get
            {
                return Frame.Width;
            }
        }

        public double OriginalHeight
        {
            get
            {
                return Frame.Height;
            }
        }

        public IWhisker[] Whiskers
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

                MarkAsDirty();
            }
        }

        public void Dispose()
        {
            if (Frame != null)
            {
                Frame.Dispose();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            IMouseFrame mouseFrame = obj as IMouseFrame;

            if (mouseFrame == null)
            {
                return false;
            }

            return Equals(mouseFrame);
        }

        public bool Equals(IMouseFrame mouseFrame)
        {
            return FrameNumber == mouseFrame.FrameNumber;
        }

        public override int GetHashCode()
        {
            return FrameNumber;
        }
    }
}
