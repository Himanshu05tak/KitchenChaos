using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loader
{
   public static class Loader
   {
      public enum Scene
      {
         MainMenu,
         LoadingScene, 
         GamePlay,
         LobbyScene,
         CharacterSelectScene
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

      public static void LoadNetwork(Scene targetScene)
      {
         NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
      }
   
      public static void GameSave(string key, string val)
      {
         if (!PlayerPrefs.HasKey(key))
         {
            PlayerPrefs.SetString(key,val);
         }
         else
         {
            PlayerPrefs.GetString(key, val);
         }
         PlayerPrefs.Save();
      }
   
   }
}
