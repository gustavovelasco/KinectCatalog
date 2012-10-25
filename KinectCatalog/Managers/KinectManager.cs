using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using KinectCatalog.Interfaces;
using KinectCatalog.Common.Events;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace KinectCatalog.Managers
{
	public class KinectManager : IKinectManager
	{

		#region Internals
		private KinectSensor _sensor;
		private WriteableBitmap _ColorImageBitmap;
		private Int32Rect _ColorImageBitmapRect;
		private int _ColorImageStride;
		private Skeleton[] FrameSkeletons;
		//private Image videoStream;
		#endregion

		#region Properties

		public KinectSensor Sensor
		{
			get { return this._sensor; }

			set
			{
				if (this._sensor != value)
				{
					if (this._sensor != null)
					{
						//Uninitialize
						UninitializeKinectSensor(this._sensor);
						this._sensor = null;
					}
					if (value != null && value.Status == KinectStatus.Connected)
					{
						this._sensor = value;
						InitializeKinectSensor(this._sensor);
					}
				}
			}
		}

		#endregion Properties

		#region Events
		public event EventHandler<KinectSensorStatusChangedEventArgs> RaiseSensorStatusChangedEvent;
		public event EventHandler<ButtonStatusChangedEventArgs> RaiseButtonStatusChangedEvent;
		public event EventHandler<ColorImageBitmapChangedEventArgs> RaiseColorImageBitmapChangedEvent;
		
		#endregion


		#region Methods

		// =======================================================================================
		public void InitializeSensor()
		{
		}

		// =======================================================================================
		public void UnloadSensor()
		{
		}

		// =======================================================================================
		public void DiscoverKinectSensor()
		{
			KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
			this.Sensor = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);

			OnRaiseSensorStatusChangedEvent(new KinectSensorStatusChangedEventArgs
			{
				OperationSuccessful = true,
				Status = KinectStatus.Connected.ToString()
			});

		}

		// =======================================================================================
		void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
		{

			switch (e.Status)
			{
				case KinectStatus.Connected:
					if (this.Sensor == null)
					{
						this.Sensor = e.Sensor;
					}
					break;
				case KinectStatus.Disconnected:
					if (this.Sensor == e.Sensor)
					{
						this.Sensor = null;
						this.Sensor = KinectSensor.KinectSensors
						.FirstOrDefault(x => x.Status == KinectStatus.Connected);
						if (this.Sensor == null)
						{
							//Notify the user that the sensor is disconnected
						}
					}
					break;
				//Handle all other statuses according to needs
			}

			OnRaiseSensorStatusChangedEvent(new KinectSensorStatusChangedEventArgs
			{
				OperationSuccessful = true,
				Status = e.Status.ToString()
			});

		}

		// =======================================================================================

		private void UninitializeKinectSensor(KinectSensor kinectSensor)
		{
			if (kinectSensor != null)
			{
				kinectSensor.Stop();
				kinectSensor.ColorFrameReady -= Kinect_ColorFrameReady;
				kinectSensor.SkeletonFrameReady -= Kinect_SkeletonFrameReady;
			}
		}

		// =======================================================================================
		private void InitializeKinectSensor(KinectSensor kinectSensor)
		{
			if (kinectSensor != null)
			{
				ColorImageStream colorStream = kinectSensor.ColorStream;
				colorStream.Enable();
				this._ColorImageBitmap = new WriteableBitmap(colorStream.FrameWidth, colorStream.FrameHeight,
					96, 96, PixelFormats.Bgr32, null);
				this._ColorImageBitmapRect = new Int32Rect(0, 0, colorStream.FrameWidth, colorStream.FrameHeight);
				this._ColorImageStride = colorStream.FrameWidth * colorStream.FrameBytesPerPixel;
				//videoStream.Source = this._ColorImageBitmap;

				OnColorImageBitmapChangedEvent(new ColorImageBitmapChangedEventArgs
				{
					ColorImageBitmap= this._ColorImageBitmap
				});

				kinectSensor.SkeletonStream.Enable(new TransformSmoothParameters()
				{
					Correction = 0.5f,
					JitterRadius = 0.05f,
					MaxDeviationRadius = 0.04f,
					Smoothing = 0.5f
				});

				kinectSensor.SkeletonFrameReady += Kinect_SkeletonFrameReady;
				kinectSensor.ColorFrameReady += Kinect_ColorFrameReady;
				kinectSensor.Start();
				this.FrameSkeletons = new Skeleton[this._sensor.SkeletonStream.FrameSkeletonArrayLength];

			}
		}

		// =======================================================================================
		private void Kinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
		{
			using (ColorImageFrame frame = e.OpenColorImageFrame())
			{
				if (frame != null)
				{
					byte[] pixelData = new byte[frame.PixelDataLength];
					frame.CopyPixelDataTo(pixelData);
					this._ColorImageBitmap.WritePixels(this._ColorImageBitmapRect, pixelData,
						this._ColorImageStride, 0);
				}
			}
		}

		// =======================================================================================
		private void Kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
		{
			using (SkeletonFrame frame = e.OpenSkeletonFrame())
			{
				if (frame != null)
				{
					frame.CopySkeletonDataTo(this.FrameSkeletons);
					Skeleton skeleton = GetPrimarySkeleton(this.FrameSkeletons);

					if (skeleton == null)
					{
						//kinectButton.Visibility = Visibility.Collapsed;
					}
					else
					{
						Joint primaryHand = GetPrimaryHand(skeleton);
						TrackHand(primaryHand);

					}
				}
			}
		}

		// =======================================================================================
		//get the hand closest to the Kinect sensor
		private static Joint GetPrimaryHand(Skeleton skeleton)
		{
			Joint primaryHand = new Joint();
			if (skeleton != null)
			{
				primaryHand = skeleton.Joints[JointType.HandLeft];
				Joint rightHand = skeleton.Joints[JointType.HandRight];
				if (rightHand.TrackingState != JointTrackingState.NotTracked)
				{
					if (primaryHand.TrackingState == JointTrackingState.NotTracked)
					{
						primaryHand = rightHand;
					}
					else
					{
						if (primaryHand.Position.Z > rightHand.Position.Z)
						{
							primaryHand = rightHand;
						}
					}
				}
			}
			return primaryHand;
		}

		// =======================================================================================
		//track and display hand
		private void TrackHand(Joint hand)
		{

			Visibility handVisibility;

			if (hand.TrackingState == JointTrackingState.NotTracked)
			{
				handVisibility = System.Windows.Visibility.Collapsed;
			}
			else
			{
				handVisibility = System.Windows.Visibility.Visible;

				DepthImagePoint point = this.Sensor.MapSkeletonPointToDepth(hand.Position, DepthImageFormat.Resolution640x480Fps30);

				OnRaiseButtonStatusChangedEvent(new ButtonStatusChangedEventArgs
				{
					ButtonVisibility = handVisibility,
					JType = hand.JointType,
					Point = point
				});

				//handX = (int)((point.X * LayoutRoot.ActualWidth / this.Kinect.DepthStream.FrameWidth) -
				//    (kinectButton.ActualWidth / 2.0));

				//handY = (int)((point.Y * LayoutRoot.ActualHeight / this.Kinect.DepthStream.FrameHeight) -
				//    (kinectButton.ActualHeight / 2.0));

				//Canvas.SetLeft(kinectButton, handX);
				//Canvas.SetTop(kinectButton, handY);

				//if (isHandOver(kinectButton, buttons)) kinectButton.Hovering();
				//else kinectButton.Release();

				//if (hand.JointType == JointType.HandRight)
				//{

				//    kinectButton.ImageSource = "/Images/RightHand.png";
				//    kinectButton.ActiveImageSource = "/Images/RightHand.png";
				//}
				//else
				//{
				//    kinectButton.ImageSource = "/Images/LeftHand.png";
				//    kinectButton.ActiveImageSource = "/Images/LeftHand.png";
				//}

			}
		}

		// =======================================================================================
		//get the skeleton closest to the Kinect sensor
		private static Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
		{
			Skeleton skeleton = null;
			if (skeletons != null)
			{
				for (int i = 0; i < skeletons.Length; i++)
				{
					if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
					{
						if (skeleton == null)
						{
							skeleton = skeletons[i];
						}
						else
						{
							if (skeleton.Position.Z > skeletons[i].Position.Z)
							{
								skeleton = skeletons[i];
							}
						}
					}
				}
			}
			return skeleton;
		}

		// =======================================================================================
		#endregion


		#region Event Manager

		// =======================================================================================
		protected virtual void OnRaiseSensorStatusChangedEvent(KinectSensorStatusChangedEventArgs e)
		{
			EventHandler<KinectSensorStatusChangedEventArgs> handler = RaiseSensorStatusChangedEvent;

			// Event will be null if there are no subscribers
			if (handler != null)
			{
				handler(this, e);
			}

		} // protected virtual void OnRaiseDistributorLoggedInEvent(DistributorLogedInEventArgs e)

		// =======================================================================================
		protected virtual void OnRaiseButtonStatusChangedEvent(ButtonStatusChangedEventArgs e)
		{
			EventHandler<ButtonStatusChangedEventArgs> handler = RaiseButtonStatusChangedEvent;

			// Event will be null if there are no subscribers
			if (handler != null)
			{
				handler(this, e);
			}

		} // 

		// =======================================================================================
		protected virtual void OnColorImageBitmapChangedEvent(ColorImageBitmapChangedEventArgs e)
		{
			EventHandler<ColorImageBitmapChangedEventArgs> handler = RaiseColorImageBitmapChangedEvent;

			// Event will be null if there are no subscribers
			if (handler != null)
			{
				handler(this, e);
			}

		} // 

		// =======================================================================================

		#endregion

	}
}
