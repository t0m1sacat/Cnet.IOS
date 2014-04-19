// This file has been autogenerated from a class added in the UI designer.

using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Linq;
using Cnt.API.Models;
using Cnt.Web.API.Models;
using Cnt.API;

namespace Cnet.iOS
{
	public partial class OSTimesheetViewController : UIViewController
	{
		#region Private Members
		private const string TimesheetDetailSegueName = "TimesheetDetail";
		private List<Assignment> completedAssignments;
		private List<Timesheet> timesheets;
		#endregion

		public OSTimesheetViewController (IntPtr handle) : base (handle)
		{
		}

		#region Public Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			LoadTimesheets ();
			RenderPayPeriod ();
			RenderTimesheets ();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == TimesheetDetailSegueName) {
				var indexPath = timesheetsTable.IndexPathForSelectedRow;
				var selectedAssignment = completedAssignments [indexPath.Row];
				var selectedTimesheet = GetTimesheet (selectedAssignment);
				var view = (OSNewTimesheetViewController)segue.DestinationViewController;
				view.PlacementId = selectedAssignment.Placement.Id;
				if (selectedTimesheet != null)
					view.TimesheetId = selectedTimesheet.Id;
			}
		}
		#endregion

		#region Private Methods
		private void LoadTimesheets ()
		{
			Client client = AuthenticationHelper.GetClient ();
			DateRange currentPayPeriod = AuthenticationHelper.UserData.PayPeriod;
			completedAssignments = new List<Assignment> (client.PlacementService.GetCompletedAssignments (currentPayPeriod.Start.Value, currentPayPeriod.End.Value));
			timesheets = new List<Timesheet> (client.TimesheetService.GetTimesheets ());
		}

		private void RenderPayPeriod ()
		{
			DateRange currentPayPeriod = AuthenticationHelper.UserData.PayPeriod;
			currentPayPeriodLabel.Text = String.Format ("{0:MMM} {0:dd} - {1:dd}, {0:yyyy}", currentPayPeriod.Start.Value, currentPayPeriod.End.Value);
		}

		private void RenderTimesheets ()
		{
			timesheetsTable.Source = new OSTimesheetTableSource (this);
		}

		private Timesheet GetTimesheet(Assignment assignment)
		{
			return timesheets.FirstOrDefault (t => assignment.Placement.Id == t.PlacementId && assignment.Start.Date >= t.Start && assignment.Start <= t.End);
		}
		#endregion

		private class OSTimesheetTableSource : UITableViewSource
		{
			#region Private Members
			private OSTimesheetViewController controller;
			private static NSString OSTimesheetCellId = new NSString ("TimesheetCellIdentifier");
			#endregion

			public OSTimesheetTableSource(OSTimesheetViewController parent) : base()
			{
				controller = parent;
			}

			#region Public Methods
			public override int RowsInSection (UITableView tableView, int section)
			{
				return controller.completedAssignments.Count;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				OSTimesheetCell cell = (OSTimesheetCell)tableView.DequeueReusableCell (OSTimesheetCellId, indexPath);

				Assignment assignment = controller.completedAssignments [indexPath.Row];

				cell.DayLabel.Text = assignment.Start.ToString("dd");
				cell.MonthLabel.Text = assignment.Start.ToString ("MMM");
				cell.FamilyNameLable.Text = assignment.Placement.ToFamilyNameString ();
				cell.ProfileImage.Image = assignment.Placement.GetProfileImage ();

				Timesheet timesheet = controller.GetTimesheet(assignment);
				if (timesheet == null) {
					cell.TimesLabel.Text = assignment.ToTimesString ();
					cell.CheckImage.Image = new UIImage ("icon-check-off.png");
					cell.IconClockImage.Image = new UIImage ("icon-clock-off.png");
				} else {
					cell.TimesLabel.Text = timesheet.ToDurationString ();
					cell.CheckImage.Image = new UIImage ("icon-check-on.png");
					cell.IconClockImage.Image = new UIImage ("icon-clock-on.png");
					cell.DayLabel.TextColor = cell.MonthLabel.TextColor = cell.FamilyNameLable.TextColor = cell.TimesLabel.TextColor = Utility.DefaultTextColor;
				}

				return cell;
			}
			#endregion
		}
	}
}
