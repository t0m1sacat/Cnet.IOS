// This file has been autogenerated from a class added in the UI designer.

using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Cnet.iOS
{
	public partial class OSContactUsCell : UITableViewCell
	{
		public OSContactUsCell (IntPtr handle) : base (handle)
		{
		}

		public UILabel NameLabel { get { return nameLabel; } set { nameLabel = value; } }
		public UILabel AddressLabel { get { return addressLabel; } set { addressLabel = value; } }
		public UIButton PhoneButton { get { return phoneButton; } set { phoneButton = value; } }
		public UILabel FaxLabel { get { return faxLabel; } set { faxLabel = value; } }
		public UIButton EmailButton { get { return emailButton; } set { emailButton = value; } }

		partial void emailClicked (MonoTouch.UIKit.UIButton sender)
		{
			Utility.OpenUrl ("mailto:" + sender.Title(UIControlState.Normal));
		}

		partial void phoneClicked (MonoTouch.UIKit.UIButton sender)
		{
			Utility.OpenPhoneDailer (sender.Title(UIControlState.Normal));
		}
	}
}
