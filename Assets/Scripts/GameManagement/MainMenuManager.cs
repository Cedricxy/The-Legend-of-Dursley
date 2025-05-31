using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement
{
    public class MainMenuManager : MonoBehaviour
    {
        public void OnPlayGame()
        {
            //Vorherige, letze Szene laden!      
        }
        
        public void OnQuitGame()
        {
            Application.Quit();
        }

        public void OnNewGame()
        {
            SceneManager.LoadScene("Tutorial");
            //Vorherige Saves löschen!
        }

        public void OnLoadGame()
        {
            OnPlayGame();
        }
    }
}