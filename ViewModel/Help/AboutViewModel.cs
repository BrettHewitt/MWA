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

using RobynsWhiskerTracker.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobynsWhiskerTracker.ViewModel.Help
{
    public class AboutViewModel : WindowViewModelBase
    {
        private ActionCommand m_OkCommand;
        private string m_AboutText;
        
        public ActionCommand OkCommand
        {
            get
            {
                return m_OkCommand ?? (m_OkCommand = new ActionCommand()
                {
                    ExecuteAction = OkPressed,
                });
            }
        }

        public string AboutText
        {
            get
            {
                return m_AboutText;
            }
            set
            {
                if (Equals(m_AboutText, value))
                {
                    return;
                }

                m_AboutText = value;

                NotifyPropertyChanged();
            }
        }

        public AboutViewModel()
        {
            AboutText = "Manual Whisker Tracker was developed at Manchester Metropolitan University.\nCreated by Brett Hewitt, Robyn Grant and Moi Hoon Yap";
        }

        private void OkPressed()
        {
            Close = true;
        }
    }
}
