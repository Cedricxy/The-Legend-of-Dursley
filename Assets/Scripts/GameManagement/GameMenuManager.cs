using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement
{
    public class GameMenuManager : MonoBehaviour
    {
        public GameObject mainMenu;
        
        public void OnMenuEnable()
        {
            Time.timeScale = 0f;
            if(mainMenu != null)
                mainMenu.SetActive(true);
        }

        public void OnMenuDisable()
        {
            Time.timeScale = 1f;
            if(mainMenu != null)
                mainMenu.SetActive(false);

        }
        
        public void OnSaveGame()
        {
            //Szene speichern!     
        }

        public void OnMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}