// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Cnet.iOS
{
	[Register ("OSChildrenCell")]
	partial class OSChildrenCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel childrenLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (childrenLabel != null) {
				childrenLabel.Dispose ();
				childrenLabel = null;
			}
		}
	}
}
