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

using RobynsWhiskerTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RobynsWhiskerTracker.ViewModel.Analyser.Methods
{
    public abstract class MethodBaseWithImage : MethodBase, IResize
    {
        private Bitmap m_Image;

        private double m_LastKnownImageWidth;
        private double m_LastKnownImageHeight;

        public double LastKnownImageWidth
        {
            get
            {
                return m_LastKnownImageWidth;
            }
            set
            {
                if (Equals(m_LastKnownImageWidth, value))
                {
                    return;
                }

                m_LastKnownImageWidth = value;

                NotifyPropertyChanged();
            }
        }

        public double LastKnownImageHeight
        {
            get
            {
                return m_LastKnownImageHeight;
            }
            set
            {
                if (Equals(m_LastKnownImageHeight, value))
                {
                    return;
                }

                m_LastKnownImageHeight = value;

                NotifyPropertyChanged();
            }
        }

        public Bitmap Image
        {
            get
            {
                return m_Image;
            }
            protected set
            {
                if (Equals(m_Image, value))
                {
                    return;
                }

                if (m_Image != null)
                {
                    m_Image.Dispose();
                }

                m_Image = value;

                NotifyPropertyChanged();
            }
        }

        protected MethodBaseWithImage(AnalyserViewModel parent, string methodName) : base(parent, methodName)
        {

        }

        public void SizeChanged(object sender, SizeChangedEventArgs e)
        {
            System.Windows.Controls.Image image = sender as System.Windows.Controls.Image;
            if (image != null)
            {
                Image_OnSizeChanged(image, e);
            }
        }

        protected void Image_OnSizeChanged(System.Windows.Controls.Image image, SizeChangedEventArgs e)
        {
            LastKnownImageWidth = image.ActualWidth;
            LastKnownImageHeight = image.ActualHeight;

            UpdateWhiskerPositions(image, e);
        }

        protected abstract void UpdateWhiskerPositions(System.Windows.Controls.Image image, SizeChangedEventArgs e);
    }
}
