using System;
using Urho;
using Urho.Gui;

namespace SmartRoadSense.Shared
{
    public class SceneManager
    {
        public BaseScene Scene;

        public static SceneManager Instance = new SceneManager();

        public void SetScene(BaseScene scene)
        {
            var deleted = Scene?.IsDeleted;
            if(deleted.HasValue && !deleted.Value)
                Scene?.Clear();

            Scene = scene;
            Scene.Init();
        }
    }
}
