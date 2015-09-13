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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobynsWhiskerTracker.Extension;

namespace RobynsWhiskerTracker.Classes
{
    public class StartEndPoint
    {
        public Point StartPoint;
        public Point EndPoint;
        public int Index;

        public StartEndPoint(Point startPoint, Point endPoint, int index)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Index = index;
        }

        public float Distance(Point point)
        {
            float[] distances = new float[2];
            distances[1] = StartPoint.Distance(point);
            distances[2] = EndPoint.Distance(point);

            return distances.Min();
        }

        public void Distance(StartEndPoint points, out float distance, out ClosestPoints order)
        {
            float[] distances = new float[4];
            distances[0] = StartPoint.Distance(points.StartPoint);
            distances[1] = StartPoint.Distance(points.EndPoint);
            distances[2] = EndPoint.Distance(points.StartPoint);
            distances[3] = EndPoint.Distance(points.EndPoint);

            float minDist = distances.Min();

            if (distances[0] == minDist)
            {
                order = ClosestPoints.StartStart;
            }
            else if (distances[1] == minDist)
            {
                order = ClosestPoints.StartEnd;
            }
            else if (distances[2] == minDist)
            {
                order = ClosestPoints.EndStart;
            }
            else
            {
                order = ClosestPoints.EndEnd;
            }

            distance = minDist;
        }
    }

    public enum ClosestPoints
    {
        StartStart,
        StartEnd,
        EndStart,
        EndEnd,
    }
}
