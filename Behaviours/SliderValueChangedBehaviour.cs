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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace RobynsWhiskerTracker.Behaviours
{
    /// <summary>
    /// Helps find the user-selected value of a slider only when the keyboard/mouse gesture has ended.
    /// </summary>
    public class SliderValueChangedBehavior : Behavior<Slider>
    {
        /// <summary>
        /// Keys down.
        /// </summary>
        private int keysDown;

        /// <summary>
        /// Indicate whether to capture the value on latest key up.
        /// </summary>
        private bool applyKeyUpValue;

        #region Dependency property Value

        /// <summary>
        /// DataBindable value.
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double),
            typeof(SliderValueChangedBehavior),
            new PropertyMetadata(default(double), OnValuePropertyChanged));

        #endregion

        #region Dependency property Value

        /// <summary>
        /// DataBindable Command
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(SliderValueChangedBehavior),
            new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// On behavior attached.
        /// </summary>
        protected override void OnAttached()
        {
            this.AssociatedObject.KeyUp += this.OnKeyUp;
            this.AssociatedObject.KeyDown += this.OnKeyDown;
            this.AssociatedObject.ValueChanged += this.OnValueChanged;

            base.OnAttached();
        }

        /// <summary>
        /// On behavior detaching.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.KeyUp -= this.OnKeyUp;
            this.AssociatedObject.KeyDown -= this.OnKeyDown;
            this.AssociatedObject.ValueChanged -= this.OnValueChanged;
        }

        /// <summary>
        /// On Value dependency property change.
        /// </summary>
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = (SliderValueChangedBehavior)d;
            if (me.AssociatedObject != null)
                me.Value = (double)e.NewValue;
        }

        /// <summary>
        /// Occurs when the slider's value change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Mouse.Captured != null)
            {
                this.AssociatedObject.LostMouseCapture += this.OnLostMouseCapture;
            }
            else if (this.keysDown != 0)
            {
                this.applyKeyUpValue = true;
            }
            else
            {
                this.ApplyValue();
            }
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            this.AssociatedObject.LostMouseCapture -= this.OnLostMouseCapture;
            this.ApplyValue();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (this.keysDown-- != 0)
            {
                this.ApplyValue();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            this.keysDown++;
        }

        /// <summary>
        /// Applies the current value in the Value dependency property and raises the command.
        /// </summary>
        private void ApplyValue()
        {
            this.Value = this.AssociatedObject.Value;

            if (this.Command != null)
                this.Command.Execute(this.Value);
        }
    }
}