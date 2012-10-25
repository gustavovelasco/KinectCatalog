using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectCatalog.Interfaces
{
	public interface IKinectManager
	{

		KinectSensor Sensor { get; set; }

		void InitializeSensor();

		void UnloadSensor();

	}
}
