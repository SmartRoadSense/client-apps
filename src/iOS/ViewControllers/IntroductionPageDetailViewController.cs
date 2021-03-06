// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;
using SmartRoadSense.Shared.Api;
using System.Drawing;
using SmartRoadSense.Shared;

namespace SmartRoadSense.iOS
{
	public partial class IntroductionPageDetailViewController : UIViewController
	{
		public IntroductionPageViewController IntroVC { get; set; }
        private const double resizeBottomConst = 8.5;

		public IntroductionPageDetailViewController (IntPtr handle) : base (handle)
		{
		}
			
		public int PageIndex{ get; set; }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = StyleSettings.ThemePrimaryDarkLightenedColor();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Get correct view to show
			switch (PageIndex) {
			case 0:
				var page1VC = new IntroPage1ViewController();
				setPage1View (page1VC);
				break;
			case 1:
				var page2VC = new IntroPage2ViewController ();
				setPage2View (page2VC);
				break;
			case 2:
				var page3VC = new IntroPage3ViewController ();
				setPage3View (page3VC);
				break;
			case 3: 
				var page4VC = new IntroPage4ViewController ();
				setPage4View (page4VC);
				break;
			case 4:
				var pageCalibrationVC = new IntroPageCalibrationViewController ();
				setPageCalibrationView (pageCalibrationVC);
				break;
			case 5:
				var page5VC = new IntroPage5ViewController ();
				setPage5View (page5VC);
				break;
			case 6:
				var page6VC = new IntroPage6ViewController();
				page6VC.IntroVC = IntroVC;
				setPage6View(page6VC);
				break;
			default:
				Log.Debug ("Introduction page not found");
				break;
			}
		}

		private void setPage1View(IntroPage1ViewController content){
			this.AddChildViewController(content);
			UIView destView = ((IntroPage1ViewController)content).View;
			destView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			destView.Frame = new RectangleF(
				0, 
				0, (float)this.View.Frame.Size.Width, 
				(float)this.View.Frame.Size.Height - ((float)(this.View.Frame.Size.Height) / (float)resizeBottomConst));
			this.View.AddSubview(destView);
			(content).DidMoveToParentViewController (this);

			img1.Image = UIImage.FromBundle ("ic_launcher");
			img2.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img3.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img4.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img5.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img6.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img7.Image = UIImage.FromBundle ("ic_launcher_disabled");

		}

		private void setPage2View(IntroPage2ViewController content){
			this.AddChildViewController(content);
			UIView destView = ((IntroPage2ViewController)content).View;
			destView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			destView.Frame = new RectangleF(
				0, 
				0, (float)this.View.Frame.Size.Width, 
				(float)this.View.Frame.Size.Height - ((float)(this.View.Frame.Size.Height) / (float)resizeBottomConst));	
			this.View.AddSubview(destView);
			(content).DidMoveToParentViewController (this);

			img1.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img2.Image = UIImage.FromBundle ("ic_launcher");
			img3.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img4.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img5.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img6.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img7.Image = UIImage.FromBundle ("ic_launcher_disabled");

		}

		private void setPage3View(IntroPage3ViewController content){
			this.AddChildViewController(content);
			UIView destView = ((IntroPage3ViewController)content).View;
			destView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			destView.Frame = new RectangleF(
				0, 
				0, (float)this.View.Frame.Size.Width, 
				(float)this.View.Frame.Size.Height - ((float)(this.View.Frame.Size.Height) / (float)resizeBottomConst));
			this.View.AddSubview(destView);
			(content).DidMoveToParentViewController (this);

			img1.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img2.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img3.Image = UIImage.FromBundle ("ic_launcher");
			img4.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img5.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img6.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img7.Image = UIImage.FromBundle ("ic_launcher_disabled");

		}

		private void setPage4View(IntroPage4ViewController content){
			this.AddChildViewController(content);
			UIView destView = ((IntroPage4ViewController)content).View;
			destView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			destView.Frame = new RectangleF(
				0, 
				0, (float)this.View.Frame.Size.Width, 
				(float)this.View.Frame.Size.Height - ((float)(this.View.Frame.Size.Height) / (float)resizeBottomConst));	
			this.View.AddSubview(destView);
			(content).DidMoveToParentViewController (this);

			img1.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img2.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img3.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img4.Image = UIImage.FromBundle ("ic_launcher");
			img5.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img6.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img7.Image = UIImage.FromBundle ("ic_launcher_disabled");

		}

		private void setPageCalibrationView(IntroPageCalibrationViewController content){
			this.AddChildViewController(content);
			UIView destView = ((IntroPageCalibrationViewController)content).View;
			destView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			destView.Frame = new RectangleF(
				0, 
				0, (float)this.View.Frame.Size.Width, 
				(float)this.View.Frame.Size.Height - ((float)(this.View.Frame.Size.Height) / (float)resizeBottomConst));	
			this.View.AddSubview(destView);
			(content).DidMoveToParentViewController (this);

			img1.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img2.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img3.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img4.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img5.Image = UIImage.FromBundle ("ic_launcher");
			img6.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img7.Image = UIImage.FromBundle("ic_launcher_disabled");

		}

		private void setPage5View(IntroPage5ViewController content){
			this.AddChildViewController(content);
			UIView destView = ((IntroPage5ViewController)content).View;
			destView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			destView.Frame = new RectangleF(
				0, 
				0, (float)this.View.Frame.Size.Width, 
				(float)this.View.Frame.Size.Height - ((float)(this.View.Frame.Size.Height) / (float)resizeBottomConst));
			this.View.AddSubview(destView);
			(content).DidMoveToParentViewController (this);

			img1.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img2.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img3.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img4.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img5.Image = UIImage.FromBundle ("ic_launcher_disabled");
			img6.Image = UIImage.FromBundle ("ic_launcher");
			img7.Image = UIImage.FromBundle("ic_launcher_disabled");

		}

		private void setPage6View(IntroPage6ViewController content)
		{
			this.AddChildViewController(content);
			UIView destView = ((IntroPage6ViewController)content).View;
			destView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			destView.Frame = new RectangleF(
				0,
				0, (float)this.View.Frame.Size.Width,
				(float)this.View.Frame.Size.Height - ((float)(this.View.Frame.Size.Height) / (float)resizeBottomConst));
			this.View.AddSubview(destView);
			(content).DidMoveToParentViewController(this);

			img1.Image = UIImage.FromBundle("ic_launcher_disabled");
			img2.Image = UIImage.FromBundle("ic_launcher_disabled");
			img3.Image = UIImage.FromBundle("ic_launcher_disabled");
			img4.Image = UIImage.FromBundle("ic_launcher_disabled");
			img5.Image = UIImage.FromBundle("ic_launcher_disabled");
			img6.Image = UIImage.FromBundle("ic_launcher_disabled");
			img7.Image = UIImage.FromBundle("ic_launcher");
		}	
	}
}
