using System;
using UIKit;

namespace SmartRoadSense.iOS
{
	public class PageDataSource : UIPageViewControllerDataSource
	{
		public PageDataSource(IntroductionPageViewController parentController)
		{
			this.parentController = parentController;
		}

		private IntroductionPageViewController parentController;

		public override UIViewController GetPreviousViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			IntroductionPageDetailViewController currentPageController = referenceViewController as IntroductionPageDetailViewController;

			// Determine if we are on the first page
			if (currentPageController.PageIndex <= 0)
			{
				// We are on the first page, so there is no need for a controller before that
				return null;
			} else
			{
				int previousPageIndex = currentPageController.PageIndex - 1;
				IntroductionPageDetailViewController pageController = UIStoryboard.FromName ("MainStoryboard", null).InstantiateViewController ("IntroductionPageDetailViewController") as IntroductionPageDetailViewController;
				pageController.IntroVC = parentController;
				pageController.PageIndex = previousPageIndex;
				return pageController;
			}//end if else
		}

		public override UIViewController GetNextViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			IntroductionPageDetailViewController currentPageController = referenceViewController as IntroductionPageDetailViewController;
			// Determine if we are on the last page
			if (currentPageController.PageIndex >= (this.parentController.TotalPages - 1))
			{
				// We are on the last page, so there is no need for a controller after that
				return null;
			} else
			{
				int nextPageIndex = currentPageController.PageIndex + 1;
				IntroductionPageDetailViewController pageController = UIStoryboard.FromName ("MainStoryboard", null).InstantiateViewController ("IntroductionPageDetailViewController") as IntroductionPageDetailViewController;
				pageController.IntroVC = parentController;
				pageController.PageIndex = nextPageIndex;
				return pageController;			
			}//end if else
		}
	}
}

