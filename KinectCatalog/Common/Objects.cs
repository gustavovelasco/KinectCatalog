using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace KinectCatalog.Common
{

	public class HandPosition : INotifyPropertyChanged
	{
		public int _x;

		public int X
		{
			get
			{ return _x; }
			set
			{
				_x = value;
				NotifyPropertyChanged("X");
			}
		}

		public int _y;

		public int Y
		{
			get
			{ return _y; }
			set
			{
				_y = value;
				NotifyPropertyChanged("Y");
			}
		}


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

	}

}
