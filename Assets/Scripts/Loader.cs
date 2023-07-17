using UnityEngine.SceneManagement;

public static class Loader
{
   public enum Scene
   {
      MainMenu,
      LoadingScene, 
      GamePlay,
   }
   private static Scene _targetScene;

   public static void Load(Scene targetSceneName)
   {
      _targetScene = targetSceneName;
      SceneManager.LoadScene(Scene.LoadingScene.ToString());
   }

   public static void LoaderCallback()
   {
      SceneManager.LoadScene(_targetScene.ToString());
   }
   
}
