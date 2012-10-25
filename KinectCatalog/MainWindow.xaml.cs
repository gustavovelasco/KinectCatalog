using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KinectCatalog.Controls.MessageBox;
using KinectCatalog.Managers;
using System.ComponentModel;
using KinectCatalog.Common;

namespace KinectCatalog
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{

		#region Privates

		KinectManager _manager = new KinectManager();

		#endregion

		#region Properties

		private HandPosition _rightHandPosition;

		public HandPosition RightHandPosition
		{
			get { return _rightHandPosition; }
			set
			{
				_rightHandPosition = value;
				NotifyPropertyChanged("RightHandPosition");
			}
		}

		private HandPosition _leftHandPosition;

		public HandPosition LeftHandPosition
		{
			get { return _leftHandPosition; }
			set
			{
				_leftHandPosition = value;
				NotifyPropertyChanged("LeftHandPosition");
			}
		}

		private string _sensorStatus;

		public string SensorStatus
		{
			get
			{
				return _sensorStatus;
			}
			set
			{
				_sensorStatus = value;
				NotifyPropertyChanged("SensorStatus");
			}
		}

		private string _debug;

		public string Debug
		{
			get
			{
				return _debug;
			}
			set
			{
				_debug = value;
				NotifyPropertyChanged("Debug");
			}
		}

		#endregion

		// =======================================================================================
		public MainWindow()
		{
			InitializeComponent();

			this.DataContext = this;

			HookUpEvents();
			this.Loaded += new RoutedEventHandler(MainWindow_Loaded);

		}

		#region Methods

		// =======================================================================================
		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{

			this.RightHandPosition = new HandPosition { X = 100, Y = 0 };
			this.LeftHandPosition = new HandPosition { X = 150, Y = 0 };

			_manager.DiscoverKinectSensor();
		}

		// =======================================================================================
		private void HookUpEvents()
		{
			this._manager.RaiseSensorStatusChangedEvent += new EventHandler<Common.Events.KinectSensorStatusChangedEventArgs>(_manager_RaiseSensorStatusChangedEvent);
			this._manager.RaiseColorImageBitmapChangedEvent += new EventHandler<Common.Events.ColorImageBitmapChangedEventArgs>(_manager_RaiseColorImageBitmapChangedEvent);
			this._manager.RaiseButtonStatusChangedEvent += new EventHandler<Common.Events.ButtonStatusChangedEventArgs>(_manager_RaiseButtonStatusChangedEvent);
		}

		// =======================================================================================
		#endregion

		#region Events

		// =======================================================================================
		void _manager_RaiseButtonStatusChangedEvent(object sender, Common.Events.ButtonStatusChangedEventArgs e)
		{

			string debugText = string.Format("Point x: {0} y:{1}, Joint Type: {2}", e.Point.X, e.Point.Y, e.JType.ToString());
			int frameWidth = this._manager.Sensor.DepthStream.FrameWidth;
			int frameHeight = this._manager.Sensor.DepthStream.FrameHeight;

			if (e.JType == Microsoft.Kinect.JointType.HandRight)
			{
				double kinectButtonRHWidth = kinectButtonRH.ActualWidth;
				double kinectButtonRHHeight = kinectButtonRH.ActualHeight;

				RightHandPosition.X = (int)((e.Point.X * LayoutRoot.ActualWidth / frameWidth) -
					(kinectButtonRHWidth / 2.0));

				RightHandPosition.Y = (int)((e.Point.Y * LayoutRoot.ActualHeight / frameHeight) -
					(kinectButtonRHHeight / 2.0));
			}
			
			if (e.JType == Microsoft.Kinect.JointType.HandLeft)
			{
				double kinectButtonLHWidth = kinectButtonLH.ActualWidth;
				double kinectButtonLHHeight = kinectButtonLH.ActualHeight;

				LeftHandPosition.X = (int)((e.Point.X * LayoutRoot.ActualWidth / frameWidth) -
					(kinectButtonLHWidth / 2.0));

				LeftHandPosition.Y = (int)((e.Point.Y * LayoutRoot.ActualHeight / frameHeight) -
					(kinectButtonLHHeight / 2.0));
			}

			this.Debug = debugText;

		}

		// =======================================================================================
		void _manager_RaiseColorImageBitmapChangedEvent(object sender, Common.Events.ColorImageBitmapChangedEventArgs e)
		{
			this.videoStream.Source = e.ColorImageBitmap;
		}

		// =======================================================================================
		void _manager_RaiseSensorStatusChangedEvent(object sender, Common.Events.KinectSensorStatusChangedEventArgs e)
		{
			this.SensorStatus = ((Common.Events.KinectSensorStatusChangedEventArgs)e).Status;
		}

		// =======================================================================================

		#endregion

		// =======================================================================================
		// =======================================================================================

		// =======================================================================================

		#region INotifyPropertyChanged Implementation

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		#endregion

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			(new CustomMessageBoxAlert()
			 	{
			 		MessageText = "Hello"
			 	}
				).Show();
		}

		// =======================================================================================

	}
}
