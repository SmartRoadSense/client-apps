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

        /// <summary>
        /// Sets the x.
        /// </summary>
        /// <returns>The x.</returns>
        /// <param name="pixels">Pixels.</param>
        public int SetX(int pixels)
        {
            return (int)(XScreenRatio * pixels);
        }

        /// <summary>
        /// Sets the y.
        /// </summary>
        /// <returns>The y.</returns>
        /// <param name="pixels">Pixels.</param>
        public int SetY(int pixels)
        {
            return (int)(YScreenRatio * pixels);
        }

        /// <summary>
        /// Sets the x.
        /// </summary>
        /// <returns>The x.</returns>
        /// <param name="position">Position.</param>
        public float SetX(float position) {
            return XScreenRatio * position;
        }

        /// <summary>
        /// Sets the y.
        /// </summary>
        /// <returns>The y.</returns>
        /// <param name="position">Position.</param>
        public float SetY(float position) {
            return YScreenRatio * position;
        }
    }
}
