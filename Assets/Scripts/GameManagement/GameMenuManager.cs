using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement
{
    public class GameMenuManager : MonoBehaviour
    {
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