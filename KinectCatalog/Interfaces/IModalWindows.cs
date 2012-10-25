using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectCatalog.Interfaces
{

	public interface ISubmitIdeaModalDialog : IModalWindow
	{

	}

	public interface IViewCommentsModalDialog : IModalWindow
	{
	}

	public interface IViewIdeaModalDialog : IModalWindow
	{
	}

	public interface IAddCommentModalDialog : IModalWindow
	{

	}

	public interface IDistributorLoginModalDialog : IModalWindow
	{

	}

	public interface IMessageBoxModalDialog : IModalWindow
	{
		string MessageText { get; set; }
		byte Response { get; set; }
		byte Style { get; set; }
	}

	public interface IModalWindow
	{
		//Guid Handler { get; set; }
		//bool? DialogResult { get; set; }
		//string DialogResultMessage { get; set; }
		//event EventHandler Closed;
		//void Show();
		//object DataContext { get; set; }
		//void Close();

		bool? DialogResult { get; set; }
		event EventHandler Closed;
		void Show();
		object DataContext { get; set; }
		void Close();
	}
}
