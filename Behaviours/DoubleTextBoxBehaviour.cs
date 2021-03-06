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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RobynsWhiskerTracker.Behaviours
{
    public class DoubleTextBoxBehaviour
    {
        public static readonly DependencyProperty MVVMHasErrorProperty=
                DependencyProperty.RegisterAttached("MVVMHasError", 
                typeof(bool), typeof(DoubleTextBoxBehaviour), 
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceMVVMHasError));

        public static bool GetMVVMHasError(DependencyObject d)
        {
            return (bool)d.GetValue(MVVMHasErrorProperty);
        }

        public static void SetMVVMHasError(DependencyObject d, bool value)
        {
            d.SetValue(MVVMHasErrorProperty, value);
        }

        private static object CoerceMVVMHasError(DependencyObject d,Object baseValue)
        {
            bool ret=(bool)baseValue ;
            if (BindingOperations.IsDataBound(d,MVVMHasErrorProperty))
            {
                if (GetHasErrorDescriptor(d) == null)
                {
                    DependencyPropertyDescriptor desc=DependencyPropertyDescriptor.FromProperty(Validation.HasErrorProperty, d.GetType());
                    desc.AddValueChanged(d,OnHasErrorChanged);
                    SetHasErrorDescriptor(d, desc);
                    ret = Validation.GetHasError(d);
                }   
            }
            else
            {
                if (GetHasErrorDescriptor(d) != null)
                {
                    DependencyPropertyDescriptor desc= GetHasErrorDescriptor(d);
                    desc.RemoveValueChanged(d, OnHasErrorChanged);
                    SetHasErrorDescriptor(d, null);
                }
            }

            return ret;
        }

        private static readonly DependencyProperty HasErrorDescriptorProperty = 
            DependencyProperty.RegisterAttached("HasErrorDescriptor", 
            typeof(DependencyPropertyDescriptor), typeof(DoubleTextBoxBehaviour));

        private static DependencyPropertyDescriptor GetHasErrorDescriptor(DependencyObject d)
        {
            var ret=d.GetValue(HasErrorDescriptorProperty);
            return ret as DependencyPropertyDescriptor;
        }

        private static void OnHasErrorChanged(object sender, EventArgs e)
        {
            DependencyObject d = sender as DependencyObject;
            if (d != null)
            {
                d.SetValue(MVVMHasErrorProperty, d.GetValue(Validation.HasErrorProperty));
            }
        }

        private static void SetHasErrorDescriptor(DependencyObject d, DependencyPropertyDescriptor value)
        {
            var ret = d.GetValue(HasErrorDescriptorProperty);
            d.SetValue(HasErrorDescriptorProperty, value);
        }
    }
}
