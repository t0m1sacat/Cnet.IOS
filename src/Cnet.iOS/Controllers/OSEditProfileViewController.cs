// This file has been autogenerated from a class added in the UI designer.

using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Drawing;

namespace Cnet.iOS
{
	public partial class OSEditProfileViewController : UIViewController
	{
		public OSEditProfileViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Indent text/set image in text views here
			List<UITextField> textFieldList = new List<UITextField> {
				firstNameTextField,
				lastNameTextField,
				emailTextField,
				phoneTextField,
				emergencyContactTextField,
				ecPhoneTextField};

			List<String> imageNameList = new List<String> {
				"icon-user.png",
				"icon-user.png",
				"icon-mail.png",
				"icon-mobile.png",
				"icon-user.png",
				"icon-phone.png"};

			for (int i = 0; i < textFieldList.Count; i++)
			{
				UITextField tempTextField = textFieldList[i];
				UIImageView spacerView = new UIImageView (new RectangleF (0, 0, 40, 24));

				tempTextField.LeftViewMode = UITextFieldViewMode.Always;
				spacerView.Image = new UIImage (imageNameList [i]);
				spacerView.ContentMode = UIViewContentMode.Center;
				textFieldList [i].LeftView = spacerView;
			}
		}

		partial void addNewPhonePressed (NSObject sender)
		{
			float frameAdjustment = 161.0f;
			List<UIView> adjustedViewsList = new List<UIView>{
				addPhoneButton, addressLabel, addressTextField, addressLine2TextField,
				cityTextField, stateTextField, zipCodeTextField, addAddressButton,
				emergencyContactLabel, emergencyContactTextField, ecPhoneTextField };

			foreach (UIView view in adjustedViewsList)
			{
				view.AdjustFrame(0, frameAdjustment, 0 ,0);
			}

			// Adjust content size, not frame for scroll view
			SizeF scrollViewContent = editProfileScrollView.ContentSize;
			scrollViewContent.Height += frameAdjustment;
			editProfileScrollView.ContentSize = scrollViewContent;

			// Add new ui elements for phone
			UITextField newPhoneTextField = new UITextField (addPhoneButton.Frame);
			newPhoneTextField.AdjustFrame(0, (-1 * frameAdjustment), 0 ,0);
			UIButton newPhoneCarrierField = new UIButton (newPhoneTextField.Frame);
			newPhoneCarrierField.AdjustFrame(0, 51, 0, 0);
		}

		partial void addAlternateAddressPressed (NSObject sender)
		{
			float frameAdjustment = 241.0f;
			List<UIView> adjustedViewsList = new List<UIView>{ addAddressButton, emergencyContactLabel, emergencyContactTextField, ecPhoneTextField };

			foreach (UIView view in adjustedViewsList)
			{
				view.AdjustFrame(0, frameAdjustment, 0 ,0);
			}

			// Adjust content size, not frame for scroll view
			SizeF scrollViewContent = editProfileScrollView.ContentSize;
			scrollViewContent.Height += frameAdjustment;
			editProfileScrollView.ContentSize = scrollViewContent;
		}
	}
}
