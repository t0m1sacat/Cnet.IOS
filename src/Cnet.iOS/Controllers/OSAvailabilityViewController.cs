// This file has been autogenerated from a class added in the UI designer.

using System;

using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cnt.API;
using Cnt.API.Exceptions;
using Cnt.Web.API.Models;

namespace Cnet.iOS
{
	public partial class OSAvailabilityViewController : UIViewController
	{
		#region Private Members
		private static NSString availabilityBlockDetailSegueName = new NSString ("AvailabilityBlockDetail");
		private List<AvailabilityBlock> availabilityBlocks;
		private int selectedRowIndex;
		private bool hasErrors;
		#endregion

		#region Public Properties
		public DateTime SelectedDate { get; set; }
		#endregion

		#region Constructors
		public OSAvailabilityViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Public Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			LoadAvailability ();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == availabilityBlockDetailSegueName) {
				var indexPath = availabilityTable.IndexPathForSelectedRow;
				var selectedAvailabilityBlock = availabilityBlocks [indexPath.Row];
				//var selectedAvailabilityBlock = availabilityBlocks [selectedRowIndex];
				var view = (OSEditAvailabilityViewController)segue.DestinationViewController;
				view.AvailabilityBlockId = selectedAvailabilityBlock.Id;
			}
		}
		#endregion

		#region Event Delegates
		private void DeleteButtonClicked (object sender, EventArgs e)
		{
			UIAlertView alert = new UIAlertView ("Delete Availability", "Are you sure you want to delete this availability?", null, "Cancel", "Confirm");
			alert.Clicked += DeleteConfirmClicked;
			alert.Show ();
		}

		private void DeleteConfirmClicked (object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 1) {
				DeleteAvailabilityBlock ();
				if (!hasErrors) {
					LoadAvailability ();
					availabilityTable.ReloadData ();
				}
			}
		}
		#endregion

		#region Private Methods
		private void DeleteAvailabilityBlock ()
		{
			try {
				var selectedAvailabilityBlock = availabilityBlocks [selectedRowIndex];
				Client client = AuthenticationHelper.GetClient ();
				client.AvailabilityService.DeleteAvailabilityBlock (selectedAvailabilityBlock.Id);
				hasErrors = false;
			} catch (CntResponseException ex) {
				hasErrors = true;
				Utility.ShowError (ex);
			}
		}

		private void LoadAvailability ()
		{
			Client client = AuthenticationHelper.GetClient ();
			availabilityBlocks = new List<AvailabilityBlock> (client.AvailabilityService.GetAvailabilityBlocks ());
			availabilityTable.Source = new OSAvailabilityTableSource (this);
		}
		#endregion

		private class OSAvailabilityTableSource : UITableViewSource
		{
			#region Private Members
			private OSAvailabilityViewController controller;
			private static NSString AvailabilityCellId = new NSString ("AvailabilityCellIdentifier");
			#endregion

			#region Constructors
			public OSAvailabilityTableSource(OSAvailabilityViewController parent) : base()
			{
				controller = parent;
			}
			#endregion

			#region Public Methods
			public override int RowsInSection (UITableView tableview, int section)
			{
				if (controller.availabilityBlocks == null)
					return 1;
				return controller.availabilityBlocks.Count + 1;
			}

			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				if (indexPath.Row == (controller.availabilityBlocks.Count))
					return 50;
				return 105;//165;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = new UITableViewCell ();
				int rowIndex = indexPath.Row;
				if (rowIndex != controller.availabilityBlocks.Count) {
					AvailabilityBlock availabilityBlock = controller.availabilityBlocks [rowIndex];
					cell = tableView.DequeueReusableCell (AvailabilityCellId, indexPath);
					RenderAvailabilityCell ((OSAvailabilityCell)cell, rowIndex, availabilityBlock);
				}
				return cell;
			}
			#endregion

			#region Private Methods
			private void RenderAvailabilityCell(OSAvailabilityCell cell, int row, AvailabilityBlock availabilityBlock)
			{
				cell.RowIndex = row;
				cell.DatesLabel.Text = availabilityBlock.ToDatesString ();
				cell.DaysOfWeek.Text = availabilityBlock.Weekdays;
				foreach(TimeBlock time in availabilityBlock.Times){
					cell.TimesLabel.Text = time.ToTimesString ();
				}
				cell.CloseButton.Hidden = true;
				cell.EditButton.Hidden = true;
				// Remove the event handlers first since this may be a reused cell.
				//cell.EditButton.TouchDown -= (object sender, EventArgs e) => {
				//	controller.selectedRowIndex = cell.RowIndex;
				//	controller.PerformSegue("AvailabilityBlockDetail", this);
				//};
				//cell.CloseButton.TouchUpInside -= controller.DeleteButtonClicked;
				//cell.EditButton.TouchDown += (object sender, EventArgs e) => {
				//	controller.selectedRowIndex = cell.RowIndex;
				//	controller.PerformSegue("AvailabilityBlockDetail", this);
				//};
				//cell.CloseButton.TouchUpInside += controller.DeleteButtonClicked;
			}
			#endregion
		}	
	}
}
