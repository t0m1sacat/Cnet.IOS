// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cnt.API;
using Cnt.API.Exceptions;
using Cnt.Web.API.Models;

namespace Cnet.iOS
{
	// TODO: Add ability to add phones and addresses.
	public partial class OSEditProfileViewController : UIViewController
	{
		#region Private Members

		private NSString DoneSegueName = new NSString ("Profile");
		private bool hasErrors;
		private List<string> mobileCarriers;
		private User user;

		#endregion

		public OSEditProfileViewController (IntPtr handle) : base (handle)
		{
		}

		#region Public Methods

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			LoadUser ();
			WireUpView ();
			RenderUser ();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == DoneSegueName)
				SubmitForm ();
			base.PrepareForSegue (segue, sender);
		}

		public override bool ShouldPerformSegue (string segueIdentifier, NSObject sender)
		{
			if (segueIdentifier == DoneSegueName)
				return !hasErrors;

			return base.ShouldPerformSegue (segueIdentifier, sender);
		}

		#endregion

		#region Event Delegates

		partial void addNewPhonePressed (NSObject sender)
		{
			float frameAdjustment = 161.0f;
			List<UIView> adjustedViewsList = new List<UIView> {
				addPhoneButton, addressLabel, addressTextField, addressLine2TextField,
				cityTextField, stateTextField, zipCodeTextField, addAddressButton,
				emergencyContactLabel, emergencyContactTextField, ecPhoneTextField
			};

			foreach (UIView view in adjustedViewsList) {
				view.AdjustFrame (0, frameAdjustment, 0, 0);
			}

			// Adjust content size, not frame for scroll view
			SizeF scrollViewContent = editProfileScrollView.ContentSize;
			scrollViewContent.Height += frameAdjustment;
			editProfileScrollView.ContentSize = scrollViewContent;

			// Add new ui elements for phone
			var frame = new RectangleF (addPhoneButton.Frame.X, (addPhoneButton.Frame.Y - frameAdjustment), addPhoneButton.Frame.Width, addPhoneButton.Frame.Height);
			UITextField newPhoneTextField = new UITextField (frame);
			newPhoneTextField.Background = new UIImage ("profile-fields.png");
			newPhoneTextField.AddPadding (40, 24, new UIImage ("icon-mobile.png"));
			newPhoneTextField.Placeholder = "Phone #";
			newPhoneTextField.Font = UIFont.FromName ("HelveticaNeue", 15f);
			newPhoneTextField.TextColor = UIColor.DarkGray;

			UIButton newPhoneCarrierButton = new UIButton (newPhoneTextField.Frame);
			newPhoneCarrierButton.AdjustFrame (0, 51, 0, 0);
			UILabel newPhoneCarrierLabel = new UILabel (new RectangleF (60, newPhoneCarrierButton.Frame.Y + 11, 120, 21));
			newPhoneCarrierButton.SetBackgroundImage (new UIImage ("profile-carrier.png"), UIControlState.Normal);
			newPhoneCarrierLabel.Text = "Choose Carrier...";
			newPhoneCarrierLabel.Font = UIFont.FromName ("HelveticaNeue", 15f);
			newPhoneCarrierLabel.TextColor = UIColor.DarkGray;

			UIImageView newPhoneSMSImage = new UIImageView (newPhoneCarrierButton.Frame);
			newPhoneSMSImage.AdjustFrame (0, 51, 0, 0);
			UILabel smsLabel = new UILabel (new RectangleF (60, newPhoneSMSImage.Frame.Y + 11, 95, 21));
			UIImageView smsIcon = new UIImageView (new RectangleF (28, newPhoneSMSImage.Frame.Y + 10, 24, 24));
			UISwitch smsSwitch = new UISwitch (new RectangleF (234, newPhoneSMSImage.Frame.Y + 6, 51, 31));
			smsLabel.Text = "Text Message";
			smsLabel.Font = UIFont.FromName ("HelveticaNeue", 15f);
			smsLabel.TextColor = UIColor.DarkGray;
			newPhoneSMSImage.Image = new UIImage ("profile-fields.png");
			smsIcon.Image = new UIImage ("icon-text.png");
			smsSwitch.OnTintColor = UIColor.FromRGB (135, 198, 86);

			editProfileScrollView.AddSubview (newPhoneTextField);
			editProfileScrollView.AddSubview (newPhoneCarrierButton);
			editProfileScrollView.AddSubview (newPhoneCarrierLabel);
			editProfileScrollView.AddSubview (newPhoneSMSImage);
			editProfileScrollView.AddSubview (smsLabel);
			editProfileScrollView.AddSubview (smsIcon);
			editProfileScrollView.AddSubview (smsSwitch);
		}

		partial void addAlternateAddressPressed (NSObject sender)
		{
			float frameAdjustment = 241.0f;
			List<UIView> adjustedViewsList = new List<UIView> {
				addAddressButton,
				emergencyContactLabel,
				emergencyContactTextField,
				ecPhoneTextField
			};

			foreach (UIView view in adjustedViewsList) {
				view.AdjustFrame (0, frameAdjustment, 0, 0);
			}

			// Adjust content size, not frame for scroll view
			SizeF scrollViewContent = editProfileScrollView.ContentSize;
			scrollViewContent.Height += frameAdjustment;
			editProfileScrollView.ContentSize = scrollViewContent;
		}

		private void PhoneCarrierClick (object sender, EventArgs e)
		{
			ShowCarrierPicker ();
		}

		#endregion

		#region Private Methods

		private void LoadUser ()
		{
			try {
				Client client = AuthenticationHelper.GetClient ();
				mobileCarriers = new List<string> (client.MobileCarrierService.GetMobileCarriers ().OrderBy (c => c));
				user = client.UserService.GetUser (AuthenticationHelper.UserData.UserId);
			} catch (CntResponseException ex) {
				Utility.ShowError (ex);
			}
		}

		private void RenderUser ()
		{
			List<UITextField> textFieldList = new List<UITextField> {
				firstNameTextField,
				lastNameTextField,
				emailTextField,
				phoneTextField,
				emergencyContactTextField,
				ecPhoneTextField
			};
			List<UITextField> addressFieldList = new List<UITextField> {
				addressTextField,
				addressLine2TextField,
				cityTextField, 
				stateTextField,
				zipCodeTextField
			};

			List<String> imageNameList = new List<String> {
				"icon-user.png",
				"icon-user.png",
				"icon-mail.png",
				"icon-mobile.png",
				"icon-user.png",
				"icon-phone.png"
			};

			// Indent text/set image in text views here
			for (int i = 0; i < textFieldList.Count; i++) {
				textFieldList [i].AddPadding (40, 24, new UIImage (imageNameList [i]));
			}

			for (int i = 0; i < addressFieldList.Count; i++) {
				addressFieldList [i].AddPadding (10, 24);
			}

			profileImage.Image = user.GetProfileImage ();

			firstNameTextField.Text = user.FirstName;
			lastNameTextField.Text = user.LastName;
			emailTextField.Text = user.Email;
			phoneTextField.Text = user.MobilePhone;
			phoneCarrierLabel.Text = user.MobileCarrier;
			textMessageSwitch.On = user.AllowTexts;

			addressTextField.Text = user.AddressCurrent.Line1;
			addressLine2TextField.Text = user.AddressCurrent.Line2;
			cityTextField.Text = user.AddressCurrent.City;
			stateTextField.Text = user.AddressCurrent.State;
			zipCodeTextField.Text = user.AddressCurrent.Zip;

			emergencyContactTextField.Text = user.EmergencyContactName;
			ecPhoneTextField.Text = user.EmergencyContactPhone;
		}

		private void ShowCarrierPicker ()
		{
			var actionSheetCarrierPicker = new ActionSheetListPicker (this.View);
			var pickerDataModel = new ListPickerViewModel<string> (mobileCarriers);
			actionSheetCarrierPicker.ListPicker.Source = pickerDataModel;
			actionSheetCarrierPicker.ListPicker.Select (mobileCarriers.IndexOf (phoneCarrierLabel.Text), 0, false);
			pickerDataModel.ValueChanged += (object sender, EventArgs e) => phoneCarrierLabel.Text = (sender as ListPickerViewModel<string>).SelectedItem;
			actionSheetCarrierPicker.Show ();
		}

		private void SubmitForm ()
		{
			user.FirstName = firstNameTextField.Text;
			user.LastName = lastNameTextField.Text;
			user.Email = emailTextField.Text;
			user.MobilePhone = phoneTextField.Text;
			user.MobileCarrier = phoneCarrierLabel.Text;
			user.AllowTexts = textMessageSwitch.On;

			user.AddressCurrent.Line1 = addressTextField.Text;
			user.AddressCurrent.Line2 = addressLine2TextField.Text;
			user.AddressCurrent.City = cityTextField.Text;
			user.AddressCurrent.State = stateTextField.Text;
			user.AddressCurrent.Zip = zipCodeTextField.Text;

			user.EmergencyContactName = emergencyContactTextField.Text;
			user.EmergencyContactPhone = ecPhoneTextField.Text;

			try {
				Client client = AuthenticationHelper.GetClient ();
				client.UserService.UpdateUser (user);
				hasErrors = false;
			} catch (CntResponseException ex) {
				hasErrors = true;
				Utility.ShowError (ex);
			}
		}

		private void WireUpView ()
		{
			phoneCarrierButton.TouchUpInside += PhoneCarrierClick;
		}

		#endregion
	}
}
