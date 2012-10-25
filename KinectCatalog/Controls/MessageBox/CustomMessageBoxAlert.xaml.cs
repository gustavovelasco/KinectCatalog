using System;
using System.ComponentModel;
using KinectCatalog.Interfaces;

namespace KinectCatalog.Controls.MessageBox
{
	/// <summary>
	/// Interaction logic for CustomMessageBoxAlert.xaml
	/// </summary>
	public partial class CustomMessageBoxAlert : IMessageBoxModalDialog, INotifyPropertyChanged
	{

		#region Properties

		private string _MessageText;
		public string MessageText
		{
			get { return _MessageText; }
			set
			{
				_MessageText = value;
				NotifyPropertyChanged("MessageText");
			}
		}

		public byte Response { get; set; }
		public new byte Style { get; set; }

		#endregion

		public CustomMessageBoxAlert()
		{
			InitializeComponent();
			LayoutRoot.DataContext = this;

			//DistributorService.Instance.RaiseDistributorLogoutEvent += RaiseDistributorLogoutEvent;
		//	ButtonClickCommand = new DelegateCommand(ButtonClick, x => true);

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
