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
using RobynsWhiskerTracker.Classes;

namespace RobynsWhiskerTracker.Extension
{
    public static class MathExtension
    {
        public static int ClosestPowerOfTwo(int x)
        {
            x--;
            x |= (x >> 1);
            x |= (x >> 2);
            x |= (x >> 4);
            x |= (x >> 8);
            x |= (x >> 16);

            return (x + 1);
        }

        public static float[] NormalizeData(IEnumerable<float> data, float min, float max)
        {
            float dataMax = data.Max();
            float dataMin = data.Min();
            float range = dataMax - dataMin;

            return data.Select(d => (d - dataMin) / range).Select(n => ((1 - n) * min + n * max)).ToArray();
        }

        public static float[] NormalizeData(IEnumerable<double> data, float min, float max)
        {
            double dataMax = data.Max();
            double dataMin = data.Min();
            double range = dataMax - dataMin;

            return data.Select(d => (d - dataMin) / range).Select(n => (float)((1 - n) * min + n * max)).ToArray();
        }

        //public static float GetAreaFromPoints(Point[] points)
        //{
        //    return Math.Abs(points.Take(points.Length - 1).Select((p, i2) => (points[i2 + 1].X - p.X) * (points[i2 + 1].Y + p.Y)).Sum() / 2);
        //}

        public static float PolygonArea(Point[] polygon)
        {
            int i, j;
            float area = 0;

            for (i = 0; i < polygon.Length; i++)
            {
                j = (i + 1) % polygon.Length;

                area += polygon[i].X * polygon[j].Y;
                area -= polygon[i].Y * polygon[j].X;
            }

            area /= 2;
            return (area < 0 ? -area : area);
        }

        public static float PolygonArea(PointF[] polygon)
        {
            int i, j;
            float area = 0;

            for (i = 0; i < polygon.Length; i++)
            {
                j = (i + 1) % polygon.Length;

                area += polygon[i].X * polygon[j].Y;
                area -= polygon[i].Y * polygon[j].X;
            }

            area /= 2;
            return (area < 0 ? -area : area);
        }

        public static PointSideVector FindSide(Point p1, Point p2, Point pointToCheck)
        {
            float result = (p2.X - p1.X) * (pointToCheck.Y - p1.Y) - (p2.Y - p1.Y) * (pointToCheck.X - p1.X);

            if (result < 0)
            {
                return PointSideVector.Below;
            }

            if (result > 0)
            {
                return PointSideVector.Above;
            }

            return PointSideVector.On;
        }

        public static PointSideVector FindSide(PointF p1, PointF p2, PointF pointToCheck)
        {
            float result = (p2.X - p1.X) * (pointToCheck.Y - p1.Y) - (p2.Y - p1.Y) * (pointToCheck.X - p1.X);

            if (result < 0)
            {
                return PointSideVector.Below;
            }

            if (result > 0)
            {
                return PointSideVector.Above;
            }

            return PointSideVector.On;
        }

        public static bool PolygonContainsPoint(Point[] polygon, Point point)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < point.Y && polygon[j].Y >= point.Y || polygon[j].Y < point.Y && polygon[i].Y >= point.Y)
                {
                    if (polygon[i].X + (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < point.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        /// <summary>
        /// Rotates one point around another
        /// </summary>
        /// <param name="pointToRotate">The point to rotate.</param>
        /// <param name="centerPoint">The centre point of rotation.</param>
        /// <param name="angleInDegrees">The rotation angle in degrees.</param>
        /// <returns>Rotated point</returns>
        public static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X =
                    (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
    }
}
