using UnityEngine;

namespace Assets.Scripts.UI
{
    public class StartSceneController : MonoBehaviour
    {

        private MainMenuController[] _mmc;
        private GameSettingsController _gsc;

        void Start ()
        {
            _mmc = GetComponentsInChildren<MainMenuController>();
            _gsc = GetComponentInChildren<GameSettingsController>();

        }
	
        void Update () {
	
        }

        public void StartNewGame()
        {
            _mmc[0].IsVisible(false);
            _gsc.IsVisible(true);
        }

        public void Controls()
        {
            _mmc[0].IsVisible(false);
            _mmc[2].IsVisible(true);
        }

        public void About()
        {
            _mmc[0].IsVisible(false);
            _mmc[1].IsVisible(true);
        }

        public void GoBackFromControls()
        {
            _mmc[0].IsVisible(true);
            _mmc[2].IsVisible(false);
        }

        public void GoBackFromAbout()
        {
            _mmc[0].IsVisible(true);
            _mmc[1].IsVisible(false);
        }

        public void GoBackFromSettings()
        {
            _mmc[0].IsVisible(true);
            _gsc.IsVisible(false);
        }

        public void Exit()
        {
            Application.Quit();
        }


    }
}
