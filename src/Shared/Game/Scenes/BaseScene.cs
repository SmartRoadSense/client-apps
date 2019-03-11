using System;
using Urho;

namespace SmartRoadSense.Shared
{
    public class BaseScene : Scene
    {
        protected Game GameInstance { get; set; }

        public BaseScene(Game game)
        {
            GameInstance = game;
            CleanUI();
        }

        public virtual void Init(){
            GameInstance.InitUiInfo();
        }

        protected void CleanUI()
        {
            GameInstance.UI.Root.RemoveAllChildren();
        }
    }
}
