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

using Emgu.CV;
using Emgu.CV.Structure;
using RobynsWhiskerTracker.Classes;
using RobynsWhiskerTracker.Commands;
using RobynsWhiskerTracker.Interfaces;
using RobynsWhiskerTracker.Model.ClipSettings;
using RobynsWhiskerTracker.Model.MouseFrame;
using RobynsWhiskerTracker.Model.Settings;
using RobynsWhiskerTracker.ModelInterface.ClipSettings;
using RobynsWhiskerTracker.ModelInterface.MouseFrame;
using RobynsWhiskerTracker.ModelInterface.Whisker;
using RobynsWhiskerTracker.ModelInterface.WhiskerPoint;
using RobynsWhiskerTracker.ModelInterface.WhiskerVideo;
using RobynsWhiskerTracker.Resolver;
using RobynsWhiskerTracker.Services.ExcelService;
using RobynsWhiskerTracker.Services.FileBrowser;
using RobynsWhiskerTracker.View;
using RobynsWhiskerTracker.View.Analyser;
using RobynsWhiskerTracker.View.ClipSettings;
using RobynsWhiskerTracker.View.Help;
using RobynsWhiskerTracker.ViewModel.Analyser;
using RobynsWhiskerTracker.ViewModel.Help;
using RobynsWhiskerTracker.ViewModel.Setings;
using RobynsWhiskerTracker.ViewModel.Whisker;
using RobynsWhiskerTracker.ViewModel.WhiskerPoint;
using RobynsWhiskerTracker.ViewModel.Window.ClipSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace RobynsWhiskerTracker.ViewModel
{
    public class MainWindowViewModel : WindowViewModelBase, IMouseClickedViewModel, IResize
    {
        private ObservableCollection<WhiskerViewModel> m_Whiskers = new ObservableCollection<WhiskerViewModel>();
        private ObservableCollection<WhiskerViewModel> m_PreviousWhiskers = new ObservableCollection<WhiskerViewModel>();
        private ObservableCollection<WhiskerPointViewModel> m_WhiskerPoints = new ObservableCollection<WhiskerPointViewModel>();
        private ObservableCollection<WhiskerPointViewModel> m_AllWhiskerPoints = new ObservableCollection<WhiskerPointViewModel>();
        private ObservableCollection<WhiskerPointViewModel> m_AllPrevWhiskerPoints = new ObservableCollection<WhiskerPointViewModel>();

        private List<WhiskerPointViewModel> m_UndoActions = new List<WhiskerPointViewModel>();

        private WhiskerViewModel m_SelectedWhisker;
        private WhiskerPointViewModel m_SelectedWhiskerPoint;
        private int m_CurrentPoint = 1;

        private Bitmap m_Image;

        private IWhiskerVideo m_WhiskerVideo;
        private MouseFrameViewModel m_CurrentFrame;
        private int m_VideoWidth;
        private int m_VideoHeight;
        private int m_FrameCount;
        private int m_FrameCounter = 1;
        private string m_WorkingFile = string.Empty;

        private double m_LastKnownImageWidth;
        private double m_LastKnownImageHeight;

        private bool m_AutoNextFrame = true;
        private bool m_AutoNextPoint = true;
        private bool m_EqualizeImage = true;
        private bool m_Started = false;
        private bool m_ShowPoints = true;
        private bool m_VideoSelected = false;

        private ActionCommand m_NewSessionCommand;
        private ActionCommand m_OpenVideoCommand;
        private ActionCommand m_NextFrameCommand;
        private ActionCommand m_PreviousFrameCommand;
        private ActionCommand m_ClosingCommand;
        private ActionCommand m_SaveCommand;
        private ActionCommand m_OpenSettingsWindowCommand;
        private ActionCommand m_OpenAnalyserWindowCommand;
        private ActionCommand m_OpenAboutCommand;
        private ActionCommand m_SaveAsCommand;
        private ActionCommand m_CtrlSCommand;
        private ActionCommand m_UndoCommand;
        private ActionCommand m_RepeatLastPointCommand;
        private ActionCommand m_ExitCommand;
        private ActionCommand m_ExportCommand;

        private Dictionary<int, MouseFrameViewModel> m_Frames = new Dictionary<int, MouseFrameViewModel>(); 

        public ActionCommand NextFrameCommand
        {
            get
            {
                return m_NextFrameCommand ?? (m_NextFrameCommand = new ActionCommand()
                {
                    ExecuteAction = GetNextFrame,
                    CanExecuteAction = () => Started && FrameCounter < m_Frames.Count,
                });
            }
        }

        public ActionCommand NewSessionCommand
        {
            get
            {
                return m_NewSessionCommand ?? (m_NewSessionCommand = new ActionCommand()
                {
                    ExecuteAction = NewSession
                });
            }
        }

        public ActionCommand OpenVideoCommand
        {
            get
            {
                return m_OpenVideoCommand ?? (m_OpenVideoCommand = new ActionCommand()
                {
                    ExecuteAction = OpenFile
                });
            }
        }

        public ActionCommand PreviousFrameCommand
        {
            get
            {
                return m_PreviousFrameCommand ?? (m_PreviousFrameCommand = new ActionCommand()
                {
                    ExecuteAction = GetPreviousFrame,
                    CanExecuteAction = () => Started && CurrentFrame != null && CurrentFrame.IndexNumber > 0
                });
            }
        }

        public ActionCommand ClosingCommand
        {
            get
            {
                return m_ClosingCommand ?? (m_ClosingCommand = new ActionCommand()
                {
                    ExecuteAction = OnExit,
                });
            }
        }

        public ActionCommand SaveCommand
        {
            get
            {
                return m_SaveCommand ?? (m_SaveCommand = new ActionCommand()
                {
                    ExecuteAction = SaveFile,
                    CanExecuteAction = () => Started,
                });
            }
        }

        public ActionCommand OpenSettingsWindowCommand
        {
            get
            {
                return m_OpenSettingsWindowCommand ?? (m_OpenSettingsWindowCommand = new ActionCommand()
                {
                    ExecuteAction = OpenSettingsWindow
                });
            }
        }

        public ActionCommand OpenAnalyserWindowCommand
        {
            get
            {
                return m_OpenAnalyserWindowCommand ?? (m_OpenAnalyserWindowCommand = new ActionCommand()
                {
                    ExecuteAction = OpenAnalyserWindow,
                    CanExecuteAction = () => Started,
                });
            }
        }

        public ActionCommand OpenAboutCommand
        {
            get
            {
                return m_OpenAboutCommand ?? (m_OpenAboutCommand = new ActionCommand()
                {
                    ExecuteAction = OpenAboutWindow,
                });
            }
        }

        public ActionCommand SaveAsCommand
        {
            get
            {
                return m_SaveAsCommand ?? (m_SaveAsCommand = new ActionCommand()
                {
                    ExecuteAction = SaveFileAs,
                    CanExecuteAction = () => Started,
                });
            }
        }

        public ActionCommand CtrlSCommand
        {
            get
            {
                return m_CtrlSCommand ?? (m_CtrlSCommand = new ActionCommand()
                {
                    ExecuteAction = SaveFile,
                    CanExecuteAction = () => Started,
                });
            }
        }

        public ActionCommand UndoCommand
        {
            get
            {
                return m_UndoCommand ?? (m_UndoCommand = new ActionCommand()
                {
                    ExecuteAction = UndoLastAction,
                    CanExecuteAction = () => Started && m_UndoActions.Count > 0,
                });
            }
        }

        public ActionCommand RepeatLastPointCommand
        {
            get
            {
                return m_RepeatLastPointCommand ?? (m_RepeatLastPointCommand = new ActionCommand()
                {
                    ExecuteAction = RepeatLastPoint,
                });
            }
        }

        public ActionCommand ExitCommand
        {
            get
            {
                return m_ExitCommand ?? (m_ExitCommand = new ActionCommand()
                {
                    ExecuteAction = ExitApplication,
                });
            }
        }

        public ActionCommand ExportCommand
        {
            get
            {
                return m_ExportCommand ?? (m_ExportCommand = new ActionCommand()
                {
                    ExecuteAction = SaveResults,
                    CanExecuteAction = ValidateResults,
                });
            }
        }

        private IWhiskerVideo WhiskerVideo
        {
            get
            {
                return m_WhiskerVideo;
            }
            set
            {
                if (m_WhiskerVideo != null)
                {
                    m_WhiskerVideo.Dispose();
                }

                m_WhiskerVideo = value;
            }
        }

        private MouseFrameViewModel CurrentFrame
        {
            get
            {
                return m_CurrentFrame;
            }
            set
            {
                if (m_CurrentFrame != null)
                {
                    m_CurrentFrame.Whiskers = Whiskers.ToArray();
                    m_CurrentFrame.SaveWhiskers();
                }

                m_CurrentFrame = value;

                if (m_CurrentFrame != null)
                {
                    if (EqualizeImage)
                    {
                        using (Image<Gray, Byte> equalizedImage = m_CurrentFrame.Frame.Convert<Gray, Byte>())
                        {
                            equalizedImage._EqualizeHist();
                            Image = equalizedImage.ToBitmap();
                        }
                    }
                    else
                    {
                        Image = m_CurrentFrame.Frame.ToBitmap();
                    }
                    
                    if (FrameCounter - 1 >= 0 && m_Frames[FrameCounter - 1].Whiskers != null)
                    {
                        PreviousWhiskers = new ObservableCollection<WhiskerViewModel>(m_Frames[FrameCounter - 1].Whiskers);
                    }
                    else
                    {
                        PreviousWhiskers = new ObservableCollection<WhiskerViewModel>();
                    }

                    Whiskers = new ObservableCollection<WhiskerViewModel>(CurrentFrame.Whiskers);

                    //If all points have been placed, select first whisker and point, else select first point that still needs to be placed
                    WhiskerViewModel whiskerToSelect = null;
                    WhiskerPointViewModel pointToSelect = null;
                    bool pointFound = false;
                    foreach (WhiskerViewModel whisker in Whiskers)
                    {
                        foreach (WhiskerPointViewModel point in whisker.WhiskerPoints)
                        {
                            if (!point.PointPlaced)
                            {
                                whiskerToSelect = whisker;
                                pointToSelect = point;
                                pointFound = true;
                                break;
                            }
                        }

                        if (pointFound)
                        {
                            break;
                        }
                    }

                    if (pointFound)
                    {
                        SelectedWhisker = whiskerToSelect;
                        SelectedWhiskerPoint = pointToSelect;
                    }
                    else
                    {
                        SelectedWhisker = Whiskers.First();
                        SelectedWhiskerPoint = WhiskerPoints.First();
                    }

                    NextFrameCommand.RaiseCanExecuteChangedNotification();
                    PreviousFrameCommand.RaiseCanExecuteChangedNotification();  
                    ExportCommand.RaiseCanExecuteChangedNotification();
                }
            }
        }

        public ObservableCollection<WhiskerViewModel> Whiskers
        {
            get
            {
                return m_Whiskers;
            }
            set
            {
                if (Equals(m_Whiskers, value))
                {
                    return;
                }

                m_Whiskers = value;

                CreateWhiskerPointsList();

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<WhiskerViewModel> PreviousWhiskers
        {
            get
            {
                return m_PreviousWhiskers;
            }
            set
            {
                if (Equals(m_PreviousWhiskers, value))
                {
                    return;
                }

                m_PreviousWhiskers = value;

                CreatePrevWhiskerPointsList();

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<WhiskerPointViewModel> WhiskerPoints
        {
            get
            {
                return m_WhiskerPoints;
            }
            set
            {
                if (Equals(m_WhiskerPoints, value))
                {
                    return;
                }

                m_WhiskerPoints = value;

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<WhiskerPointViewModel> AllWhiskerPoints
        {
            get
            {
                return m_AllWhiskerPoints;
            }
            set
            {
                if (ReferenceEquals(m_AllWhiskerPoints, value))
                {
                    return;
                }

                m_AllWhiskerPoints = value;

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<WhiskerPointViewModel> AllPrevWhiskerPoints
        {
            get
            {
                return m_AllPrevWhiskerPoints;
            }
            set
            {
                if (ReferenceEquals(m_AllPrevWhiskerPoints, value))
                {
                    return;
                }

                m_AllPrevWhiskerPoints = value;

                NotifyPropertyChanged();
            }
        }

        public int CurrentPoint
        {
            get
            {
                return m_CurrentPoint;
            }
            set
            {
                if (Equals(m_CurrentPoint, value))
                {
                    return;
                }

                m_CurrentPoint = value;

                NotifyPropertyChanged();
            }
        }

        public WhiskerViewModel SelectedWhisker
        {
            get
            {
                return m_SelectedWhisker;
            }
            set
            {
                if (Equals(m_SelectedWhisker, value))
                {
                    return;
                }

                m_SelectedWhisker = value;
                NotifyPropertyChanged();

                if (m_SelectedWhisker != null)
                {
                    WhiskerPoints = new ObservableCollection<WhiskerPointViewModel>(m_SelectedWhisker.WhiskerPoints);
                    SelectedWhiskerPoint = WhiskerPoints.First();
                }
            }
        }

        public WhiskerPointViewModel SelectedWhiskerPoint
        {
            get
            {
                return m_SelectedWhiskerPoint;
            }
            set
            {
                if (Equals(m_SelectedWhiskerPoint, value))
                {
                    return;
                }

                m_SelectedWhiskerPoint = value;
                CurrentPoint = WhiskerPoints.IndexOf(SelectedWhiskerPoint) + 1;

                NotifyPropertyChanged();
            }
        }

        public Bitmap Image
        {
            get
            {
                return m_Image;
            }
            set
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

        public bool AutoNextFrame
        {
            get
            {
                return m_AutoNextFrame;
            }
            set
            {
                if (Equals(m_AutoNextFrame, value))
                {
                    return;
                }

                m_AutoNextFrame = value;

                NotifyPropertyChanged();
            }
        }

        public bool AutoNextPoint
        {
            get
            {
                return m_AutoNextPoint;
            }
            set
            {
                if (Equals(m_AutoNextPoint, value))
                {
                    return;
                }

                m_AutoNextPoint = value;

                NotifyPropertyChanged();
            }
        }

        public bool EqualizeImage
        {
            get
            {
                return m_EqualizeImage;
            }
            set
            {
                if (Equals(m_EqualizeImage, value))
                {
                    return;
                }

                m_EqualizeImage = value;

                if (CurrentFrame != null)
                {
                    if (m_EqualizeImage)
                    {
                        using (Image<Gray, Byte> equalizedImage = CurrentFrame.Frame.Convert<Gray, Byte>())
                        {
                            equalizedImage._EqualizeHist();
                            Image = equalizedImage.ToBitmap();
                        }
                    }
                    else
                    {
                        Image = CurrentFrame.Frame.ToBitmap();
                    }
                }

                NotifyPropertyChanged();
            }
        }

        public bool ShowPoints
        {
            get
            {
                return m_ShowPoints;
            }
            set
            {
                if (Equals(m_ShowPoints, value))
                {
                    return;
                }

                m_ShowPoints = value;

                NotifyPropertyChanged();
            }
        }

        public bool VideoSelected
        {
            get
            {
                return m_VideoSelected;
            }
            set
            {
                if (Equals(m_VideoSelected, value))
                {
                    return;
                }

                m_VideoSelected = value;

                NotifyPropertyChanged();
            }
        }

        public int StartFrame
        {
            get
            {
                return GlobalSettings.GlobalSettings.ClipSettings.StartFrame;
            }
        }

        public int EndFrame
        {
            get
            {
                return GlobalSettings.GlobalSettings.ClipSettings.EndFrame;
            }
        }

        public int FrameCount
        {
            get
            {
                return m_FrameCount;
            }
            set
            {
                if (Equals(m_FrameCount, value))
                {
                    return;
                }

                m_FrameCount = value;

                NotifyPropertyChanged();
            }
        }

        public int FrameCounter
        {
            get
            {
                return m_FrameCounter;
            }
            set
            {
                if (Equals(m_FrameCounter, value) || value < 0)
                {
                    return;
                }

                if (value >= m_Frames.Count)
                {
                    if (m_CurrentFrame != null)
                    {
                        m_CurrentFrame.Whiskers = Whiskers.ToArray();
                    }

                    MessageBoxResult result = MessageBox.Show("End of video, would you like to export the results?", "End of video", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        SaveResults();
                    }

                    return;
                }

                m_FrameCounter = value;

                CurrentFrame = m_Frames[m_FrameCounter];

                ClearUndoActions();

                NotifyPropertyChanged();
                NotifyPropertyChanged("ActualFrameNumber");
            }
        }

        public int ActualFrameNumber
        {
            get
            {
                if (CurrentFrame != null)
                {
                    return CurrentFrame.FrameNumber;
                }

                return 0;
            }
        }

        public int FrameInterval
        {
            get
            {
                return GlobalSettings.GlobalSettings.ClipSettings.FrameInterval;
            }
        }

        public bool Started
        {
            get
            {
                return m_Started;
            }
            set
            {
                if (Equals(m_Started, value))
                {
                    return;
                }

                m_Started = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged("OrientationPointEnabled");

                SaveCommand.RaiseCanExecuteChangedNotification();
                SaveAsCommand.RaiseCanExecuteChangedNotification();
                CtrlSCommand.RaiseCanExecuteChangedNotification();
                NextFrameCommand.RaiseCanExecuteChangedNotification();
                OpenAnalyserWindowCommand.RaiseCanExecuteChangedNotification();
                UndoCommand.RaiseCanExecuteChangedNotification();
            }
        }
            
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

        public string WorkingFile
        {
            get
            {
                return m_WorkingFile;
            }
            set
            {
                if (Equals(m_WorkingFile, value))
                {
                    return;
                }

                m_WorkingFile = value;

                NotifyPropertyChanged();
            }
        }

        public MainWindowViewModel(string[] args)
        {
            if (args == null)
            {
                return;
            }

            if (args.Length == 0)
            {
                return;
            }

            string fileName = args[0];

            if (File.Exists(fileName))
            {
                OpenFile(fileName);
            }
        }

        public void MouseClicked(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)sender;
            
            System.Windows.Point point = e.GetPosition(image);

            double xRatio = point.X/image.ActualWidth;
            double yRatio = point.Y/image.ActualHeight;

            AddUndoAction(SelectedWhiskerPoint.Clone());

            SelectedWhiskerPoint.XRatio = xRatio;
            SelectedWhiskerPoint.YRatio = yRatio;

            SelectedWhiskerPoint.CanvasWidth = image.ActualWidth;
            SelectedWhiskerPoint.CanvasHeight = image.ActualHeight;
            
            NotifyPropertyChanged("Whiskers");
            ExportCommand.RaiseCanExecuteChangedNotification();

            if (AutoNextPoint)
            {
                IncreaseWhiskerCounter();
            }
        }

        public void SizeChanged(object sender, SizeChangedEventArgs e)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)sender;

            LastKnownImageWidth = image.ActualWidth;
            LastKnownImageHeight = image.ActualHeight;

            UpdateWhiskerPositions(image.ActualWidth, image.ActualHeight);
        }

        private void UpdateWhiskerPositions(double actualWidth, double actualheight)
        {
            if (CurrentFrame == null)
            {
                return;
            }

            foreach (MouseFrameViewModel mouseFrame in m_Frames.Values)
            {
                foreach (WhiskerViewModel whisker in mouseFrame.Whiskers)
                {
                    whisker.UpdatePositions(actualWidth, actualheight);
                }
            }

            Whiskers = new ObservableCollection<WhiskerViewModel>(CurrentFrame.Whiskers);

            PreviousWhiskers.Clear();

            if (FrameCounter - 1 >= 0 && m_Frames[FrameCounter - 1].Whiskers != null)
            {
                PreviousWhiskers = new ObservableCollection<WhiskerViewModel>(m_Frames[FrameCounter - 1].Whiskers);
            }
        }

        private void IncreaseWhiskerCounter()
        {
            int pointsLength = WhiskerPoints.Count;

            if (CurrentPoint < pointsLength)
            {
                SelectedWhiskerPoint = WhiskerPoints[CurrentPoint];
                return;
            }

            int selectedWhiskerIndex = Whiskers.IndexOf(SelectedWhisker);

            if (selectedWhiskerIndex + 1 < Whiskers.Count)
            {
                SelectedWhisker = Whiskers[selectedWhiskerIndex + 1];
            }
            else if (AutoNextFrame)
            {
                GetNextFrame();
            }
        }

        private void StartPicking(Dictionary<int, MouseFrameViewModel> data = null)
        {
            Started = true;

            m_Frames = new Dictionary<int, MouseFrameViewModel>();
            int indexNumber = 0;
            int frameCounter = 1;
            while (frameCounter <= EndFrame)
            {
                using (Image<Bgr, Byte> currentFrame = WhiskerVideo.GetFrameImage())
                {
                    if (currentFrame == null)
                    {
                        break;
                    }

                    int startRange = frameCounter - StartFrame;
                    if (startRange >= 0)
                    {
                        //We're within range
                        if (startRange % FrameInterval == 0)
                        {
                            //We're on the interval
                            MouseFrameViewModel mouseFrameViewModel;
                            IMouseFrame mouseFrame;
                            if (data == null)
                            {
                                mouseFrame = ModelResolver.Resolve<IMouseFrame>();
                                mouseFrame.FrameNumber = frameCounter;
                                mouseFrame.IndexNumber = indexNumber;
                                mouseFrame.Whiskers = GlobalSettings.GlobalSettings.ClipSettings.CreateEmptyWhiskers(mouseFrame);
                                mouseFrameViewModel = new MouseFrameViewModel(mouseFrame);
                            }
                            else
                            {
                                mouseFrameViewModel = data[indexNumber];
                                mouseFrame = mouseFrameViewModel.Model;
                            }
                            
                            mouseFrame.Frame = currentFrame.Clone();

                            m_Frames.Add(indexNumber, mouseFrameViewModel);
                            indexNumber++;
                        }
                    }
                }

                frameCounter++;
            }

            m_FrameCounter = -1;
            UpdateWhiskerPositions(LastKnownImageWidth, LastKnownImageHeight);
            GetNextFrame();
        }

        private void GetNextFrame()
        {
            FrameCounter++;
        }

        private void GetPreviousFrame()
        {
            FrameCounter--;
        }

        private void NewSession()
        {
            string fileName = FileBrowser.BroseForVideoFiles();

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            WhiskerVideo = ModelResolver.Resolve<IWhiskerVideo>();
            WhiskerVideo.SetVideo(fileName);
            m_VideoWidth = (int)WhiskerVideo.Width;
            m_VideoHeight = (int)WhiskerVideo.Height;
            FrameCount = WhiskerVideo.FrameCount;

            ClipSettingsView clipSettingsView = new ClipSettingsView();
            ClipSettingsWindowViewModel viewModel = new ClipSettingsWindowViewModel(WhiskerVideo);

            clipSettingsView.DataContext = viewModel;

            clipSettingsView.ShowDialog();

            if (viewModel.ExitResult != WindowExitResult.Ok)
            {
                return;
            }

            IClipSettings clipSettings = viewModel.Model;

            GlobalSettings.GlobalSettings.ClipSettings = clipSettings;
            GlobalSettings.GlobalSettings.ClipSettings.Commit();

            GlobalSettings.GlobalSettings.FrameRateSettings.CurrentFrameRate = WhiskerVideo.FrameRate;
            GlobalSettings.GlobalSettings.FrameRateSettings.OriginalFrameRate = viewModel.OriginalFrameRate;

            Image = null;
            Started = false;
            VideoSelected = true;

            StartPicking();

            WorkingFile = string.Empty;
            ClearUndoActions();
        }

        private void OnExit()
        {
            foreach (var whiskerFrame in m_Frames)
            {
                whiskerFrame.Value.Dispose();
            }

            if (WhiskerVideo != null)
            {
                WhiskerVideo.Dispose();
            }
        }

        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }

        private bool ValidateResults()
        {
            foreach (KeyValuePair<int, MouseFrameViewModel> frame in m_Frames)
            {
                MouseFrameViewModel mouseFrame = frame.Value;

                if (!mouseFrame.Validate())
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateResults(out string message)
        {
            message = string.Empty;

            foreach (KeyValuePair<int, MouseFrameViewModel> frame in m_Frames)
            {
                MouseFrameViewModel mouseFrame = frame.Value;

                string mouseFrameMessage = string.Empty;
                if (!mouseFrame.Validate(ref mouseFrameMessage))
                {
                    message = mouseFrameMessage + " from index: " + mouseFrame.IndexNumber;
                    return false;
                }
            }

            return true;
        }

        private void SaveResults()
        {
            string message;

            if (!ValidateResults(out message))
            {
                MessageBox.Show("Unable to save: " + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string fileLocation = FileBrowser.SaveFile("Excel|*.xlsx");

            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }

            int rows = m_Frames.Count + 1;
            int columns = GlobalSettings.GlobalSettings.ClipSettings.TotalNumberOfPoints*2;

            object[,] excelValues = new object[rows, columns];

            IWhisker[] whiskers = m_Frames.First().Value.Model.Whiskers;

            int counter = 0;
            foreach (IWhisker whisker in whiskers)
            {
                foreach (IWhiskerPoint whiskerPoint in whisker.WhiskerPoints)
                {
                    excelValues[0, counter] = whisker.WhiskerName + " - Point " + whiskerPoint.PointId + " X";
                    excelValues[0, counter + 1] = whisker.WhiskerName + " - Point " + whiskerPoint.PointId + " Y";
                    counter += 2;
                }
            }

            int xCounter = 1;
            foreach (var whiskerFrame in m_Frames)
            {
                MouseFrameViewModel frame = whiskerFrame.Value;

                WhiskerViewModel[] whiskersViewModel = frame.Whiskers;
                counter = 0;
                foreach (WhiskerViewModel whisker in whiskersViewModel)
                {
                    foreach (WhiskerPointViewModel whiskerPoint in whisker.WhiskerPoints)
                    {
                        excelValues[xCounter, counter] = whiskerPoint.XRatio * m_VideoWidth * GlobalSettings.GlobalSettings.UnitSettings.UnitsPerPixel;
                        excelValues[xCounter, counter + 1] = whiskerPoint.YRatio * m_VideoHeight * GlobalSettings.GlobalSettings.UnitSettings.UnitsPerPixel;
                        counter += 2;
                    }
                }
                
                xCounter++;
            }

            ExcelService.WriteData(excelValues, fileLocation);
        }

        private void SaveFile()
        {
            if (string.IsNullOrWhiteSpace(WorkingFile))
            {
                SaveFileAs();
                return;
            }

            MouseFrameXml[] frames = m_Frames.Values.Select(x => new MouseFrameXml(x.Model)).ToArray();

            UnitSettingsXml unitSettings = new UnitSettingsXml(GlobalSettings.GlobalSettings.UnitSettings);
            ClipSettingsXml clipSettings = new ClipSettingsXml(GlobalSettings.GlobalSettings.ClipSettings);
            FrameRateSettingsXml frameRateSettings = new FrameRateSettingsXml(GlobalSettings.GlobalSettings.FrameRateSettings);

            WhiskerTrackerXml filXml = new WhiskerTrackerXml(clipSettings, frames, unitSettings, frameRateSettings);
            XmlSerializer serializer = new XmlSerializer(typeof(WhiskerTrackerXml));

            using (StreamWriter writer = new StreamWriter(WorkingFile))
            {
                serializer.Serialize(writer, filXml);
            }
        }

        private void SaveFileAs()
        {
            string filePath = FileBrowser.SaveFile("MWA|*.mwa");

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            MouseFrameXml[] frames = m_Frames.Values.Select(x => new MouseFrameXml(x.Model)).ToArray();

            UnitSettingsXml unitSettings = new UnitSettingsXml(GlobalSettings.GlobalSettings.UnitSettings);
            ClipSettingsXml clipSettings = new ClipSettingsXml(GlobalSettings.GlobalSettings.ClipSettings);
            FrameRateSettingsXml frameRateSettings = new FrameRateSettingsXml(GlobalSettings.GlobalSettings.FrameRateSettings);

            WhiskerTrackerXml filXml = new WhiskerTrackerXml(clipSettings, frames, unitSettings, frameRateSettings);
            XmlSerializer serializer = new XmlSerializer(typeof(WhiskerTrackerXml));

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, filXml);
            }
        }

        private void OpenFile()
        {
            string xmlFile = FileBrowser.BrowseForFile("MWA|*.mwa");

            if (string.IsNullOrWhiteSpace(xmlFile))
            {
                return;
            }

            OpenFile(xmlFile);
        }

        private void OpenFile(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WhiskerTrackerXml));
            WhiskerTrackerXml whiskerTracker;
            using (StreamReader reader = new StreamReader(fileName))
            {
                whiskerTracker = (WhiskerTrackerXml)serializer.Deserialize(reader);
            }

            if (!File.Exists(whiskerTracker.ClipSettings.ClipFilePath))
            {
                MessageBoxResult result = MessageBox.Show("File not found, would you like to browse for it?", "File not found", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    whiskerTracker.ClipSettings.ClipFilePath = FileBrowser.BroseForVideoFiles();

                    if (string.IsNullOrWhiteSpace(whiskerTracker.ClipSettings.ClipFilePath))
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            WorkingFile = fileName;
            WhiskerVideo = ModelResolver.Resolve<IWhiskerVideo>();
            WhiskerVideo.SetVideo(whiskerTracker.ClipSettings.ClipFilePath);
            m_VideoWidth = (int)WhiskerVideo.Width;
            m_VideoHeight = (int)WhiskerVideo.Height;
            FrameCount = WhiskerVideo.FrameCount;

            GlobalSettings.GlobalSettings.ClipSettings = whiskerTracker.ClipSettings.GetClipSettings();
            GlobalSettings.GlobalSettings.ClipSettings.Commit();
            GlobalSettings.GlobalSettings.UnitSettings = whiskerTracker.UnitSettings.GetSettings();
            GlobalSettings.GlobalSettings.UnitSettings.Commit();
            GlobalSettings.GlobalSettings.FrameRateSettings = whiskerTracker.FrameRateSettings.GetSettings();
            GlobalSettings.GlobalSettings.FrameRateSettings.Commit();

            Image = null;
            Started = false;
            VideoSelected = true;
            ClearUndoActions();

            Dictionary<int, MouseFrameViewModel> frames = new Dictionary<int, MouseFrameViewModel>();

            foreach (MouseFrameXml mouseFrame in whiskerTracker.Frames)
            {
                frames.Add(mouseFrame.IndexNumber, new MouseFrameViewModel(mouseFrame.GetMouseFrame()));
            }

            StartPicking(frames);
        }

        private void OpenSettingsWindow()
        {
            SettingsViewModel viewModel = new SettingsViewModel();
            SettingsView view = new SettingsView()
            {
                DataContext = viewModel,
            };

            view.ShowDialog();
        }

        private void OpenAnalyserWindow()
        {
            string message;

            if (!ValidateResults(out message))
            {
                MessageBox.Show("Unable to analyse video: " + message);
                return;
            }

            AnalyserViewModel viewModel = new AnalyserViewModel(m_Frames, WhiskerVideo);
            AnalyserView view = new AnalyserView()
            {
                DataContext = viewModel,
            };

            view.ShowDialog();
        }

        private void CreateWhiskerPointsList()
        {
            ObservableCollection<WhiskerPointViewModel> allWhiskerPoints = new ObservableCollection<WhiskerPointViewModel>();
            foreach (WhiskerViewModel whisker in Whiskers)
            {
                foreach (WhiskerPointViewModel whiskerPoint in whisker.WhiskerPoints)
                {
                    allWhiskerPoints.Add(whiskerPoint);
                }
            }

            AllWhiskerPoints = allWhiskerPoints;
        }

        private void CreatePrevWhiskerPointsList()
        {
            ObservableCollection<WhiskerPointViewModel> allPrevWhiskerPoints = new ObservableCollection<WhiskerPointViewModel>();
            foreach (WhiskerViewModel whisker in PreviousWhiskers)
            {
                foreach (WhiskerPointViewModel whiskerPoint in whisker.WhiskerPoints)
                {
                    allPrevWhiskerPoints.Add(whiskerPoint);
                }
            }

            AllPrevWhiskerPoints = allPrevWhiskerPoints;
        }

        private void OpenAboutWindow()
        {
            AboutViewModel viewModel = new AboutViewModel();
            AboutView view = new AboutView()
            {
                DataContext = viewModel,
            };

            view.Show();
        }

        private void UndoLastAction()
        {
            if (m_UndoActions.Count == 0)
            {
                return;
            }

            WhiskerPointViewModel whiskerPoint = m_UndoActions.Last();

            if (whiskerPoint != null)
            {
                int whiskerId = whiskerPoint.Model.Parent.WhiskerId;
                int pointId = whiskerPoint.Model.PointId;

                WhiskerViewModel whisker = Whiskers.Single(x => x.WhiskerId == whiskerId);

                if (whisker != null)
                {
                    WhiskerPointViewModel whiskerPointToReplace = whisker.WhiskerPoints.Single(x => x.PointId == pointId);

                    if (whiskerPointToReplace != null)
                    {
                        int index = whisker.WhiskerPoints.IndexOf(whiskerPointToReplace);

                        whisker.WhiskerPoints[index] = whiskerPoint;
                        RemoveUndoAction(whiskerPoint);

                        CreateWhiskerPointsList();
                        CreatePrevWhiskerPointsList();

                        //Need to re-select the Whisker and/or Whisker Point
                        SelectedWhisker = whisker;
                        SelectedWhiskerPoint = whiskerPoint;
                    }
                }
            }
        }

        private void AddUndoAction(WhiskerPointViewModel whiskerPoint)
        {
            m_UndoActions.Add(whiskerPoint);
            UndoCommand.RaiseCanExecuteChangedNotification();
        }

        private void RemoveUndoAction(WhiskerPointViewModel whiskerPoint)
        {
            m_UndoActions.Remove(whiskerPoint);
            UndoCommand.RaiseCanExecuteChangedNotification();
        }

        private void ClearUndoActions()
        {
            m_UndoActions.Clear();
            UndoCommand.RaiseCanExecuteChangedNotification();
        }

        private void RepeatLastPoint()
        {
            //Make sure there's a previous frame to repeat action from
            if (CurrentFrame == null || m_Frames == null)
            {
                return;
            }

            if (m_Frames.Count <= 1)
            {
                return;
            }

            if (FrameCounter == 0)
            {
                return;
            }

            MouseFrameViewModel lastFrame = m_Frames[FrameCounter - 1];

            WhiskerViewModel previousWhisker = lastFrame.Whiskers.FirstOrDefault(x => x.WhiskerId == SelectedWhisker.WhiskerId);

            if (previousWhisker == null)
            {
                return;
            }

            WhiskerPointViewModel previousPoint = previousWhisker.WhiskerPoints.FirstOrDefault(x => x.PointId == SelectedWhiskerPoint.PointId);

            if (previousPoint == null)
            {
                return;
            }

            AddUndoAction(SelectedWhiskerPoint.Clone());

            SelectedWhiskerPoint.XRatio = previousPoint.XRatio;
            SelectedWhiskerPoint.YRatio = previousPoint.YRatio;

            SelectedWhiskerPoint.CanvasWidth = previousPoint.CanvasWidth;
            SelectedWhiskerPoint.CanvasHeight = previousPoint.CanvasHeight;

            NotifyPropertyChanged("Whiskers");

            if (AutoNextPoint)
            {
                IncreaseWhiskerCounter();
            }
        }
    }
}
