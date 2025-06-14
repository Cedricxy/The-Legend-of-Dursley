using UnityEngine;
using UnityEngine.SceneManagement;
using ResourceSystem.Storage;
using SaveAndLoad;

namespace GameManagement
{
    public class MainMenuManager : MonoBehaviour
    {
        public GameObject mainMenuCanvas;

        public void OnPlayGame()
        {
            Time.timeScale = 1f;
            string sceneToLoad = "Tutorial";

            if (LoadGame.Instance != null)
            {
                // Stellt sicher, dass die neuesten Daten geladen sind.
                LoadGame.Instance.LoadGameData(); 
                if (!string.IsNullOrEmpty(LoadGame.Instance.LastScene))
                {
                    sceneToLoad = LoadGame.Instance.LastScene;
                }
            }
            else if (ResourceStorageManager.Instance != null && !string.IsNullOrEmpty(ResourceStorageManager.Instance.LastScene))
            {
                sceneToLoad = ResourceStorageManager.Instance.LastScene;
            }
            
            if (mainMenuCanvas != null)
                mainMenuCanvas.SetActive(false);
            SceneManager.LoadScene(sceneToLoad);      
        }
        
        public void OnQuitGame()
        {
            Application.Quit();
        }

        public void OnNewGame()
        {
            Time.timeScale = 1f;
            GameController.ForceNewGameReset = true;
            
            if (mainMenuCanvas != null)
                mainMenuCanvas.SetActive(false);
            SceneManager.LoadScene("Tutorial");
        }

        public void OnLoadGame()
        {
            OnPlayGame(); 
        }
    }
}