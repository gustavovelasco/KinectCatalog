using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Kinect;
using System.Windows.Media.Imaging;

namespace KinectCatalog.Common.Events
{

	// =======================================================================================
	public class BaseEvent : EventArgs
	{
		public bool OperationSuccessful { get; set; }
		public Exception ex;
	}

	// =======================================================================================
	public class KinectSensorStatusChangedEventArgs : BaseEvent
	{
		public string Status { get; set; }
	}

	// =======================================================================================
	public class ButtonStatusChangedEventArgs : BaseEvent
	{
		//public KinectButton Button { get; set; }

		public Visibility ButtonVisibility { get; set; }
		public DepthImagePoint Point { get; set; }
		public JointType JType { get; set; }

	}

	// =======================================================================================
	public class ColorImageBitmapChangedEventArgs : BaseEvent
	{
		public WriteableBitmap ColorImageBitmap { get; set; }
	}

	// =======================================================================================
}
