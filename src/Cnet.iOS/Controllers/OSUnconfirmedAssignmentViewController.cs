// This file has been autogenerated from a class added in the UI designer.

using System;

using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cnt.API;
using Cnt.API.Models;
using Cnt.Web.API.Models;

namespace Cnet.iOS
{
	public partial class OSUnconfirmedAssignmentViewController : UIViewController
	{
		#region Private Members
		private AssignmentStatus assignmentStatus;
		private List<Assignment> assignments;
		private Placement placement;
		#endregion

		public int PlacementId { get; set; }

		public OSUnconfirmedAssignmentViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			if (PlacementId > 0) {
				LoadPlacement ();
				RenderPlacement ();
			}
		}

		#region Event Delegates
		public void CallOffice(object sender, EventArgs e)
		{
			NTMobileAppLoadData data = AuthenticationHelper.UserData;
			UserOfficeInfo office = data.Offices.FirstOrDefault (o => o.Id == placement.OfficeId);
			if (office != null)
				Utility.OpenPhoneDailer (office.Phone);
		}

		public void CallFamily(object sender, EventArgs e)
		{
			if (!Utility.OpenPhoneDailer (placement.ClientMobilePhone))
				Utility.OpenPhoneDailer (placement.ClientHomePhone);
		}

		public void CloseMessage (object sender, EventArgs e)
		{
			messageView.Hidden = true;
			// TODO: Dismiss notification.
		}

		public void ConfirmAssignment(object sender, EventArgs e)
		{
			Client client = AuthenticationHelper.GetClient ();
			if (client != null)
				client.PlacementService.ConfirmPlacement (PlacementId);
		}

		public void DeclineAssignment(object sender, EventArgs e)
		{
			Client client = AuthenticationHelper.GetClient ();
			if (client != null)
				client.PlacementService.DeclinePlacement (PlacementId);
		}

		public void ViewPolicy (object sender, EventArgs e)
		{
			string message = "As a College Nannies and Tutors employee, you are expected to fulfill you responsibilities as outlined in the CNT Role Model Promise. "
				+ "If declining an assignment, you must contact the CNT office to explain the situation. Thank You!";
			new UIAlertView ("Policy", message, null, "ok", null).Show ();
		}
		#endregion

		#region Private Methods
		private void LoadPlacement()
		{
			Client client = AuthenticationHelper.GetClient ();
			placement = client.PlacementService.GetPlacement (PlacementId);
			DateRange currentPayPeriod = AuthenticationHelper.UserData.PayPeriod;
			DateTime endDate = currentPayPeriod.End.Value > DateTime.Now.AddDays (7) ? currentPayPeriod.End.Value : DateTime.Now.AddDays (7);
			assignments = new List<Assignment> (client.PlacementService.GetAssignments (placement, currentPayPeriod.Start.Value, endDate));
			assignmentStatus = assignments.GetStatus ();
		}

		private void RenderPlacement ()
		{
			callOfficeButton.Clicked += CallOffice;

			// Message
			RenderMessage ();

			// Details Table
			detailTable.Source = new OSAssignmentDetailSource(this);

			// Action Button
			RenderActionButton ();
		}

		private void RenderMessage ()
		{
			switch (assignmentStatus) {
			case AssignmentStatus.Canceled:
				messageLabel.Text = "This assignment has been cancelled.";
				messageView.BackgroundColor = Utility.CanceledBackgroundColor;
				messageLabel.TextColor = Utility.CanceledTextColor;
				messageCloseButton.Hidden = true;
				detailTable.AdjustFrame (0, 26, 0, -26);
				break;
			case AssignmentStatus.Updated:
				// TODO: Get update message.
				messageLabel.Text = "The <property> has been updated.";
				messageView.BackgroundColor = Utility.UpdatedBackgroundColor;
				messageLabel.TextColor = Utility.UpdatedTextColor;
				messageCloseButton.TouchUpInside += CloseMessage;
				break;
			default:
				messageView.Hidden = true;
				break;
			}
		}

		private void RenderActionButton ()
		{
			policyButton.TouchUpInside += ViewPolicy;

			switch (assignmentStatus) {
			case AssignmentStatus.New:
				actionButton.SetTitle ("Confirm", UIControlState.Normal);
				actionButton.TouchUpInside += ConfirmAssignment;
				declineButton.TouchUpInside += DeclineAssignment;
				break;
			case AssignmentStatus.TimesheetRequired:
				actionButton.SetTitle ("Submit Timesheet", UIControlState.Normal);
				declineButton.Hidden = true;
				break;
			default:
				actionButton.Hidden = true;
				declineButton.Hidden = true;
				policyView.AdjustFrame (0, 45, 0, 0);
				detailTable.AdjustFrame (0, 0, 0, 45);
				break;
			}
		}
		#endregion

		private class OSAssignmentDetailSource : UITableViewSource
		{
			#region Private Members
			private OSUnconfirmedAssignmentViewController controller;
			private static NSString FamilyCellId = new NSString ("FamilyCellIdentifier");
			private static NSString StartEndCellId = new NSString ("StartEndCellIdentifier");
			private static NSString TimesCellId = new NSString ("TimesCellIdentifier");
			private static NSString LocationCellId = new NSString ("LocationCellIdentifier");
			private static NSString ChildrenCellId = new NSString ("ChildrenCellIdentifier");
			private static NSString InfoCellId = new NSString ("InfoCellIdentifier");
			private static NSString DetailsCellId = new NSString ("DetailsCellIdentifier");

			private const decimal CharactersPerLine = 35;
			private int timesLineCount;
			private NSMutableAttributedString timesText;
			private int locationLineCount;
			private int childrenLineCount;
			private string childrenText;
			private int notesLineCount;
			private int detailsLineCount;
			#endregion

			public OSAssignmentDetailSource(OSUnconfirmedAssignmentViewController parent) : base()
			{
				controller = parent;

				LoadTimesData();
				LoadLocationData();
				LoadChildrenData();
				LoadDetailData();
			}

			#region Public Methods
			public override int RowsInSection (UITableView tableview, int section)
			{
				return 7;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = new UITableViewCell ();
				switch (indexPath.Row) {
				case 0: // Family
					cell = tableView.DequeueReusableCell (FamilyCellId, indexPath);
					RenderFamilyCell ((OSFamilyCell)cell);
					break;
				case 1: // Start End
					cell = tableView.DequeueReusableCell (StartEndCellId, indexPath);
					RenderStartEndCell ((OSStartEndCell)cell);
					break;
				case 2: // Times
					cell = tableView.DequeueReusableCell (TimesCellId, indexPath);
					RenderTimesCell ((OSTimesCell)cell);
					break;
				case 3: // Location
					cell = tableView.DequeueReusableCell (LocationCellId, indexPath);
					RenderLocationCell ((OSLocationCell)cell);
					break;
				case 4: // Children
					cell = tableView.DequeueReusableCell (ChildrenCellId, indexPath);
					RenderChildrenCell ((OSChildrenCell)cell);
					break;
				case 5: // Info
					cell = tableView.DequeueReusableCell (InfoCellId, indexPath);
					RenderInfoCell ((OSInfoCell)cell);
					break;
				case 6: // Details
					cell = tableView.DequeueReusableCell (DetailsCellId, indexPath);
					RenderDetailsCell ((OSDetailsCell)cell);
					break;
				}

				return cell;
			}

			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				switch (indexPath.Row) {
				case 2: // Times
					return 25 + (25 * timesLineCount);
				case 3: // Location
					return 33 + (17 * locationLineCount);
				case 4: // Children
					return 33 + (17 * childrenLineCount);
				case 5: // Info
					return 50;
				case 6: // Details
					return 40 + (notesLineCount > 0 ? 40 : 0) + (17 * notesLineCount) + 25 + (17 * detailsLineCount);
				case 0: // Family
				case 1: // Start End
				default: 
					return 75;
				}
			}
			#endregion

			#region Private Methods
			private void LoadTimesData()
			{
				timesLineCount = controller.assignments.Count;

				var boldAttribute = new UIStringAttributes {
					Font = UIFont.FromName ("Helvetica-Bold", 14f),
					ParagraphStyle = new NSMutableParagraphStyle () { LineSpacing = 8.0f }
				};
				var regularAttribute = new UIStringAttributes { 
					Font = UIFont.FromName ("Helvetica-Regular", 14f),
					ParagraphStyle = new NSMutableParagraphStyle () { LineSpacing = 8.0f }
				};
				string timesString = String.Empty;
				List<NSRange> ranges = new List<NSRange> ();
				int index = 0;
				for (int i = 0; i < timesLineCount; i++) {
					Assignment assignment = controller.assignments [i];
					string date = assignment.ToStartString ();
					string times = " - " + assignment.ToTimesString ();
					if (i < (timesLineCount - 1))
						times += "\n";
					timesString += date + times;
					ranges.Add (new NSRange (index, date.Length));
					index += date.Length;
					ranges.Add (new NSRange (index, times.Length));
					index += times.Length;
				}

				timesText = new NSMutableAttributedString (timesString);
				for (int i = 0; i < ranges.Count; i++) {
					timesText.SetAttributes ((i % 2 == 0 ? boldAttribute : regularAttribute), ranges [i]);
				}
			}

			private void LoadLocationData()
			{
				if (controller.assignmentStatus == AssignmentStatus.New)
					locationLineCount = 1;
				else if (String.IsNullOrWhiteSpace (controller.placement.Location.Line2))
					locationLineCount = 2;
				else
					locationLineCount = 3;
			}

			private void LoadChildrenData()
			{
				List<string> childStrings = new List<string> ();
				foreach (Student child in controller.placement.Students) {
					if (controller.assignmentStatus == AssignmentStatus.New) {
						childStrings.Add (String.Format ("{0}, {1}", child.Gender, child.ToAgeString ()));
					} else {
						childStrings.Add (String.Format ("{0} {1}, {2}", child.Name.Substring (0, child.Name.IndexOf (" ")), child.Gender, child.ToAgeString ()));
					}
				}
				childrenLineCount = (int)Math.Ceiling (childStrings.Count / 2m);
				childrenText = String.Empty;
				for (int i = 0; i < childStrings.Count; i++) {
					childrenText += childStrings [i] + (i % 2 == 0 ? "\t" : "\n");
				}
			}

			private void LoadDetailData()
			{
				notesLineCount = 0;
				if (!String.IsNullOrWhiteSpace (controller.placement.Notes))
					notesLineCount = (int)Math.Ceiling (controller.placement.Notes.ToCharArray ().Length / CharactersPerLine);

				detailsLineCount = 0;
				if (!String.IsNullOrWhiteSpace (controller.placement.ImportantDetails))
					detailsLineCount = (int)Math.Ceiling (controller.placement.ImportantDetails.ToCharArray ().Length / CharactersPerLine);
			}

			private void RenderFamilyCell (OSFamilyCell cell)
			{
				cell.ProfileImage.Image = controller.placement.GetProfileImage ();
				if (controller.assignmentStatus == AssignmentStatus.TimesheetRequired)
					cell.InfoImage.Image = controller.assignmentStatus.GetInfoImage ();
				else
					cell.InfoImage.Hidden = true;

				cell.FamilyNameLabel.Text = controller.placement.ToFamilyNameString ();

				switch (controller.assignmentStatus) {
				case AssignmentStatus.New:
					cell.StatusLabel.Text = "Unconfirmed";
					cell.StatusLabel.TextColor = Utility.NewTextColor;
					break;
				case AssignmentStatus.Updated:
					cell.StatusLabel.Text = "Updated: ";
					// TODO: Get updated message.
					cell.StatusLabel.TextColor = Utility.UpdatedStatusTextColor;
					cell.StatusLabel.Font = UIFont.FromName ("Helvetica-Bold", 12);
					break;
				case AssignmentStatus.TimesheetRequired:
					cell.StatusLabel.Text = "Timesheet due";
					cell.StatusLabel.TextColor = Utility.TimesheetDueTextColor;
					break;
				case AssignmentStatus.Canceled:
					cell.StatusLabel.Text = "Cancelled";
					cell.StatusLabel.TextColor = Utility.CanceledStatusTextColor;
					break;
				default:
					cell.StatusLabel.Hidden = true;
					break;
				}

				if (controller.assignmentStatus == AssignmentStatus.Confirmed || controller.assignmentStatus == AssignmentStatus.Updated)
					cell.CallButton.TouchUpInside += controller.CallFamily;
				else
					cell.CallButton.Hidden = true;
			}

			private void RenderStartEndCell (OSStartEndCell cell)
			{
				cell.StartLabel.Text = controller.placement.Start.ToString ("ddd d MMM");
				if (controller.placement.End.HasValue)
					cell.EndLabel.Text = controller.placement.End.Value.ToString ("ddd d MMM");
				else 
					cell.EndLabel.Text = String.Empty;
			}

			private void RenderTimesCell (OSTimesCell cell)
			{
				int labelHeight = (25 * timesLineCount) - 8;
				if (labelHeight != cell.TimesLabel.Frame.Height)
					cell.TimesLabel.AdjustFrame (0, 0, 0, labelHeight - cell.TimesLabel.Frame.Height);
				cell.TimesLabel.Lines = timesLineCount;
				cell.TimesLabel.AttributedText = timesText;
			}

			private void RenderLocationCell (OSLocationCell cell)
			{
				int labelHeight = 17 * locationLineCount;
				if (labelHeight != cell.LocationLabel.Frame.Height)
					cell.LocationLabel.AdjustFrame (0, 0, 0, labelHeight - cell.LocationLabel.Frame.Height);
				cell.LocationLabel.Lines = locationLineCount;

				string format = "{2}, {3} {4}";
				if (locationLineCount == 2)
					format = "{0}\n{2}, {3} {4}";
				else if (locationLineCount == 3)
					format = "{0}\n{1}\n{2}, {3} {4}";
				cell.LocationLabel.Text = controller.placement.Location.ToLocationString (format);
			}

			private void RenderChildrenCell (OSChildrenCell cell)
			{
				int labelHeight = 17 * childrenLineCount;
				if (labelHeight != cell.ChildrenLabel.Frame.Height)
					cell.ChildrenLabel.AdjustFrame (0, 0, 0, labelHeight - cell.ChildrenLabel.Frame.Height);
				cell.ChildrenLabel.Lines = childrenLineCount;
				cell.ChildrenLabel.Text = childrenText;
			}

			private void RenderInfoCell (OSInfoCell cell)
			{
				cell.InfoLabel.Text = controller.placement.Service + " - " + controller.placement.SubService;
			}

			private void RenderDetailsCell (OSDetailsCell cell)
			{
				if (String.IsNullOrWhiteSpace (controller.placement.Notes)) {
					cell.OrderNotesHeaderLabel.Hidden = true;
					cell.OrderNotesLabel.Hidden = true;
				} else {
					int notesLabelHeight = 17 * notesLineCount;
					if (notesLabelHeight != cell.OrderNotesLabel.Frame.Height)
						cell.OrderNotesLabel.AdjustFrame (0, 0, 0, notesLabelHeight - cell.OrderNotesLabel.Frame.Height);
					cell.OrderNotesLabel.Lines = notesLineCount;
					cell.OrderNotesLabel.Text = controller.placement.Notes;
				}

				if (String.IsNullOrWhiteSpace (controller.placement.ImportantDetails)) {
					// Do nothing?
				} else {
					int detailsHeaderLabelY = 20 + (notesLineCount > 0 ? 40 : 0) + (17 * notesLineCount);
					int detailsLabelY = detailsHeaderLabelY + 25;
					int detailsLabelHeight = 17 * detailsLineCount;
					if (detailsHeaderLabelY != cell.ImportantDetailsHeaderLabel.Frame.Y)
						cell.ImportantDetailsHeaderLabel.AdjustFrame (0, detailsHeaderLabelY - cell.ImportantDetailsHeaderLabel.Frame.Y, 0, 0);
					if (detailsLabelY != cell.ImportantDetailsLabel.Frame.Y || detailsLabelHeight != cell.ImportantDetailsLabel.Frame.Height)
						cell.ImportantDetailsLabel.AdjustFrame (0, detailsLabelY - cell.ImportantDetailsLabel.Frame.Y, 0, detailsLabelHeight - cell.ImportantDetailsLabel.Frame.Height);
					cell.ImportantDetailsLabel.Lines = detailsLineCount;
					cell.ImportantDetailsLabel.Text = controller.placement.ImportantDetails;
				}
			}
			#endregion
		}
	}
}
