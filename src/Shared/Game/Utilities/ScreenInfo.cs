using System;
namespace SmartRoadSense.Shared
{
    public static class ScreenInfo
    {
        public static int DefaultScreenWidth = 1920;
        public static int DefaultScreenHeight = 1080;
    }

    public class ScreenInfoRatio
    {
        float _xScreenRatio;
        float _yScreenRatio;

        public ScreenInfoRatio(int screenWidth, int screenHeight)
        {
            if(screenWidth > screenHeight) 
            {
                XScreenRatio = (float)screenWidth / (float)ScreenInfo.DefaultScreenWidth;
                YScreenRatio = (float)screenHeight / (float)ScreenInfo.DefaultScreenHeight;
            }
            else
            {
                XScreenRatio = (float)screenWidth / (float)ScreenInfo.DefaultScreenHeight;
                YScreenRatio = (float)screenHeight / (float)ScreenInfo.DefaultScreenWidth;
            }
                
        }

        public float XScreenRatio
        {
            get => _xScreenRatio;
            private set => _xScreenRatio = value;
        }

        public float YScreenRatio
        {
            get => _yScreenRatio;
            private set => _yScreenRatio = value;
        }

        public int SetX(int pixels)
        {
            return (int)(XScreenRatio * pixels);
        }

        public int SetY(int pixels)
        {
            return (int)(YScreenRatio * pixels);
        }
    }
}
