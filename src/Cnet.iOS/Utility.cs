﻿using Cnt.API.Exceptions;
using Cnt.API.Models;
using Cnt.Web.API.Models;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Cnet.iOS
{
	public static class Utility
	{
		#region Static Settings

		public static int AppId = 0; // TODO: Figure out App ID.
		public static UIColor CanceledBackgroundColor = UIColor.FromRGB (255, 0, 87);
		public static UIColor CanceledTextColor = UIColor.FromRGB (134, 15, 56);
		public static UIColor CanceledStatusTextColor = UIColor.FromRGB (255, 0, 67);
		public static UIColor DefaultTextColor = UIColor.FromRGB (77, 77, 77);
		public static UIColor DisabledTextColor = UIColor.FromRGB (217, 217, 217);
		public static UIColor NewTextColor = UIColor.FromRGB (106, 65, 131);
		public static UIColor TimesheetDueTextColor = UIColor.FromRGB (146, 203, 99);
		public static UIColor UpdatedBackgroundColor = UIColor.FromRGB (254, 221, 3);
		public static UIColor UpdatedTextColor = UIColor.FromRGB (160, 116, 85);
		public static UIColor UpdatedStatusTextColor = UIColor.FromRGB (204, 175, 0);

		#endregion

		#region Helper Methods

		public static UIImage UIImageFromUrl (string uri)
		{
			if (String.IsNullOrEmpty (uri))
				return new UIImage ();

			using (var url = new NSUrl (uri))
			using (var data = NSData.FromUrl (url))
				return UIImage.LoadFromData (data);
		}

		public static bool OpenPhoneDailer (string phoneNumber)
		{
			if (String.IsNullOrWhiteSpace (phoneNumber))
				return false;
			var cleanNumber = new String (phoneNumber.Where (Char.IsLetterOrDigit).ToArray ());
			return OpenUrl ("tel:" + cleanNumber);
		}

		public static bool OpenMap (Address address)
		{
			if (!address.AddressHasContents())
				return false;
			string mapUrl = String.Format ("http://maps.google.com/maps?f=d&daddr={0} {1} {2} {3} {4}", address.Line1, address.Line2, address.City, address.State, address.Zip).Replace (' ', '+');
			return OpenUrl (mapUrl);
		}

		public static bool OpenUrl (string url)
		{
			var urlToSend = new NSUrl (url);
			if (UIApplication.SharedApplication.CanOpenUrl (urlToSend)) {
				UIApplication.SharedApplication.OpenUrl (urlToSend);
				return true;
			}
			return false;
		}

		public static bool RateApp ()
		{
			return OpenUrl (String.Format ("itms-apps://itunes.apple.com/app/id{0}", AppId));
		}

		public static void ShowError (CntResponseException exception, string defaultMessage = null)
		{
			string userMessage = defaultMessage ?? "Unknown error. Contact your CNT office for information.";
			var view = new UIAlertView ("Error", userMessage, null, "Ok", "Details");
			view.Clicked += (object sender, UIButtonEventArgs e) => ShowErrorDetail (e, exception);
			view.Show ();
		}

		private static void ShowErrorDetail (UIButtonEventArgs e, CntResponseException exception)
		{
			if (e.ButtonIndex == 1) {
				string message = exception.Message;
				if (exception.Errors != null && exception.Errors.Count () > 1) {
					foreach (ApiError error in exception.Errors) {
						message += "\n\n" + error.Message;
					}
				}
				new UIAlertView ("Error Details", message, null, "Ok", null).Show ();
			}
		}

		#endregion

		#region Extension Methods

		public static void AdjustFrame (this UIView view, float x, float y, float width, float height)
		{
			if (view == null)
				return;
			var frame = view.Frame;
			frame.X += x;
			frame.Y += y;
			frame.Width += width;
			frame.Height += height;
			view.Frame = frame;
		}

		public static bool AddressHasContents (this Address address)
		{
			if (address == null)
				return false;
			List<String> addressFields = new List<String> {
				address.Line1,
				address.Line2,
				address.Line3,
				address.City,
				address.State,
				address.Zip
			};
			foreach (String field in addressFields) {
				if (!String.IsNullOrWhiteSpace (field)) {
					return true;
				}
			}
			return false;
		}

		public static UIView FindFirstResponder (this UIView view)
		{
			if (view == null)
				return null;
			if (view.IsFirstResponder)
				return view;
			foreach (UIView subView in view.Subviews)
			{
				var firstResponder = subView.FindFirstResponder ();
				if (firstResponder != null)
					return firstResponder;
			}
			return null;
		}

		public static void AddPadding (this UITextField textField, float width, float height, UIImage image = null)
		{
			if (textField == null)
				return;
			UIImageView paddingView = new UIImageView (new RectangleF (0, 0, width, height));
			if (image != null) {
				paddingView.Image = image;
				paddingView.ContentMode = UIViewContentMode.Center;
			}
			textField.LeftView = paddingView;
			textField.LeftViewMode = UITextFieldViewMode.Always;
		}

		public static UIImage GetInfoImage (this AssignmentStatus assignmentStatus)
		{
			switch (assignmentStatus) {
			case AssignmentStatus.New:
				return new UIImage ("check-off.png");
			case AssignmentStatus.Canceled:
				return new UIImage ("icon-cancelled.png");
			case AssignmentStatus.Confirmed:
				return new UIImage ("icon-check.png");
			case AssignmentStatus.TimesheetRequired:
				return new UIImage ("check-dollar.png");
			default:
				return new UIImage ();
			}
		}

		public static UIImage GetInfoImage (this Notification notification)
		{
			switch (notification.Type) {
			case "PlacementSubmitTimesheet":
				return new UIImage ("check-dollar.png");
			case "PlacementUpdate":
				return new UIImage ("icon-no-image.png");
			case "PlacementCreated":
			case "PlacementCanceled":
			case "PlacementReminder":
			case "UpdateAvailability":
			case "UpdateProfile":
			default:
				return new UIImage ();
			}
		}

		public static UIImage GetProfileImage (this Notification notification)
		{
			switch (notification.Type) {
			case "PlacementUpdate":
				return new UIImage ("icon-no-image.png");
			case "UpdateAvailability":
				return new UIImage ("icon-calendar-assign.png");
			case "PlacementReminder":
			case "PlacementCanceled":
			case "PlacementCreated":
			case "PlacementSubmitTimesheet":
			case "UpdateProfile":
			default:
				return new UIImage ("icon-no-image.png");
			}
		}

		public static UIImage GetProfileImage (this Placement placement)
		{
			if (placement == null)
				return new UIImage ();
			if (String.IsNullOrWhiteSpace (placement.ClientPhoto))
				return new UIImage ("icon-no-image.png");
			return UIImageFromUrl (placement.ClientPhoto);
		}

		public static UIImage GetProfileImage (this User user)
		{
			if (user == null)
				return new UIImage ();
			if (String.IsNullOrWhiteSpace (user.Photo))
				return new UIImage ("icon-no-image.png");
			return UIImageFromUrl (user.Photo);
		}

		public static AssignmentStatus GetStatus (this List<Assignment> assignments)
		{
			// Default to the lowest priority status.
			AssignmentStatus status = AssignmentStatus.NoTimesheetRequired;
			foreach (Assignment assignment in assignments) {
				AssignmentStatus currentStatus = assignment.Status;
				if ((int)currentStatus < (int)status)
					status = currentStatus;
			}
			return status;
		}

		public static UIImage GetStatusFlagImage (this Assignment assignment)
		{
			if (assignment == null)
				return new UIImage ();
			switch (assignment.Status) {
			case AssignmentStatus.New:
				return new UIImage ("flag-updated.png");
			case AssignmentStatus.Canceled:
				return new UIImage ("flag-cancelled.png");
			default:
				return new UIImage ("pointer-icon-upcoming.png");
			}
		}

		public static UIImage GetStatusImage (this Assignment assignment)
		{
			if (assignment == null)
				return new UIImage ();
			switch (assignment.Status) {
			case AssignmentStatus.New:
				return new UIImage ("icon-updated.png");
			case AssignmentStatus.Canceled:
				return new UIImage ("cancelled-dot.png");
			default:
				return new UIImage ("icon-upcoming.png");
			}
		}

		public static DateTime RoundDown (this DateTime dt, TimeSpan d)
		{
			var delta = dt.Ticks % d.Ticks;
			return new DateTime (dt.Ticks - delta);
		}

		public static DateTime RoundToNearest (this DateTime dt, TimeSpan d)
		{
			var delta = dt.Ticks % d.Ticks;
			bool roundUp = delta > d.Ticks / 2;
			return roundUp ? dt.RoundUp (d) : dt.RoundDown (d);
		}

		public static DateTime RoundUp (this DateTime dt, TimeSpan d)
		{
			var delta = (d.Ticks - (dt.Ticks % d.Ticks)) % d.Ticks;
			return new DateTime (dt.Ticks + delta);
		}

		public static void ShowDatePicker(this UIView view, DateTime date, UIDatePickerMode mode, EventHandler valueChangedHandler)
		{
			var actionSheetDatePicker = new ActionSheetDatePicker (view);
			actionSheetDatePicker.DatePicker.Date = date.ToNSDate();
			actionSheetDatePicker.DatePicker.Mode = mode;
			actionSheetDatePicker.DatePicker.MinuteInterval = 15;
			actionSheetDatePicker.DatePicker.ValueChanged += valueChangedHandler;
			actionSheetDatePicker.Show ();
		}

		public static string ToAgeString (this Student child)
		{
			if (child == null || !child.DateOfBirth.HasValue)
				return String.Empty;

			DateTime dateOfBirth = child.DateOfBirth.Value;
			DateTime currentDate = DateTime.Now;

			int ageInYears = 0;
			int ageInMonths = 0;
			int ageInDays = 0;

			ageInDays = currentDate.Day - dateOfBirth.Day;
			ageInMonths = currentDate.Month - dateOfBirth.Month;
			ageInYears = currentDate.Year - dateOfBirth.Year;

			if (ageInDays < 0) {
				ageInDays += DateTime.DaysInMonth (currentDate.Year, currentDate.Month);
				ageInMonths = ageInMonths--;

				if (ageInMonths < 0) {
					ageInMonths += 12;
					ageInYears--;
				}
			}
			if (ageInMonths < 0) {
				ageInMonths += 12;
				ageInYears--;
			}

			return String.Format ("{0}yo {1}mo", ageInYears, ageInMonths);
		}

		public static string ToDatesString (this AvailabilityBlock availabilityBlock)
		{
			if (availabilityBlock == null)
				return String.Empty;
			string datesString = availabilityBlock.Start.ToString ("MMM d");
			if (availabilityBlock.End.HasValue) {
				if (availabilityBlock.End.Value != availabilityBlock.Start)
					datesString += " - " + availabilityBlock.End.Value.ToString ("MMM dd");
			} else
				datesString += " and After";
			return datesString;
		}

		public static DateTime ToDateTime(this NSDate nsDate)
		{
			DateTime date = DateTime.SpecifyKind (nsDate, DateTimeKind.Unspecified);
			DateTime localDate = date.AddHours (-5); // Adjust for central.
			return localDate;
		}

		public static string ToDurationString (this Timesheet timesheet)
		{
			if (timesheet == null)
				return String.Empty;
			TimeSpan duration = timesheet.End.Subtract (timesheet.Start);
			return String.Format ("{0:%h} hrs {0:%m} min", duration);
		}

		public static string ToFamilyNameString (this Placement placement)
		{
			if (placement == null)
				return String.Empty;
			string clientName = placement.ClientName;
			return clientName.Substring (clientName.LastIndexOf (" ") + 1) + " Family";
		}

		public static string ToLocationString (this Address location, string format)
		{
			if (location == null)
				return String.Empty;
			return String.Format (format, location.Line1, location.Line2, location.City, location.State, location.Zip);
		}

		public static string ToNameString (this User user)
		{
			if (user == null)
				return String.Empty;
			List<string> nameParts = new List<string> ();
			if (!String.IsNullOrWhiteSpace (user.FirstName))
				nameParts.Add (user.FirstName);
			if (!String.IsNullOrWhiteSpace (user.LastName))
				nameParts.Add (user.LastName);
			return String.Join (" ", nameParts);
		}

		public static NSDate ToNSDate(this DateTime date)
		{
			DateTime localDate = date.AddHours (5); // Adjust for central.
			NSDate nsDate = (NSDate)DateTime.SpecifyKind (localDate, DateTimeKind.Utc);
			return nsDate;
		}

		public static string ToStartString (this Assignment assignment)
		{
			if (assignment == null)
				return String.Empty;
			return assignment.Start.ToString ("ddd d MMM");
		}

		public static string ToStatusString (this Assignment assignment)
		{
			if (assignment == null)
				return String.Empty;
			switch (assignment.Status) {
			case AssignmentStatus.New:
				return "Unconfirmed";
			case AssignmentStatus.Canceled:
				return "Cancelled";
			default:
			case AssignmentStatus.Confirmed:
			case AssignmentStatus.Updated:
				return assignment.Placement.Location.ToLocationString ("{2}, {3} {4}");
			}
		}

		public static string ToTimeSinceString (this DateTime dateTime)
		{
			TimeSpan timeSince = DateTime.Now.Subtract (dateTime);
			if (timeSince.Days == 1)
				return "1 day ago";
			if (timeSince.Days > 0)
				return timeSince.Days + " days ago";
			if (timeSince.Hours == 1)
				return "1 hour ago";
			if (timeSince.Hours > 0)
				return timeSince.Hours + " hours ago";
			if (timeSince.Minutes == 1)
				return "1 min ago";
			if (timeSince.Minutes > 0)
				return timeSince.Minutes + " mins ago";
			return String.Empty;
		}

		public static string ToTimesString (this Assignment assignment)
		{
			if (assignment == null)
				return String.Empty;
			DateTime end = assignment.Start.AddSeconds (assignment.Duration);
			return assignment.Start.ToString ("h:mmtt").ToLower () + " - " + end.ToString ("h:mmtt").ToLower ();
		}

		public static string ToTimesString (this TimeBlock timeBlock)
		{
			if (timeBlock == null)
				return String.Empty;
			DateTime start = DateTime.Today + TimeSpan.FromSeconds (timeBlock.Start);
			DateTime end = start.AddSeconds (timeBlock.Duration);
			return start.ToString ("h:mmtt").ToLower () + " - " + end.ToString ("h:mmtt").ToLower ();
		}

		public static string ToTimeUntilString (this Assignment assignment)
		{
			if (assignment == null)
				return String.Empty;
			TimeSpan timeUntil = assignment.Start.Subtract (DateTime.Now);
			if (timeUntil.Days == 1)
				return "1 day";
			if (timeUntil.Days > 0)
				return timeUntil.Days + " days";
			if (timeUntil.Hours == 1)
				return "1 hour";
			if (timeUntil.Hours > 0)
				return timeUntil.Hours + " hours";
			return String.Empty;
		}

		#endregion
	}
}

