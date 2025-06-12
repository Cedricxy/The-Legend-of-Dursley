using UnityEngine;
using UnityEngine.SceneManagement;
using ResourceSystem.Storage;

namespace GameManagement
{
    public class MainMenuManager : MonoBehaviour
    {
        public void OnPlayGame()
        {
            string lastScene = "Tutorial";
            if (ResourceStorageManager.Instance != null && 
                !string.IsNullOrEmpty(ResourceStorageManager.Instance.LastScene))
            {
                lastScene = ResourceStorageManager.Instance.LastScene;
            }
            SceneManager.LoadScene(lastScene);      
        }
        
        public void OnQuitGame()
        {
            Application.Quit();
        }

        public void OnNewGame()
        {
            SceneManager.LoadScene("Tutorial");
        }

        public void OnLoadGame()
        {
            SaveAndLoad.LoadGame.IsGameLoaded = true;
        }
    }
}