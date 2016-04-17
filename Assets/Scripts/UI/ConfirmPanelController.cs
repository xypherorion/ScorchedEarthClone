using Assets.Scripts.Extra;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
    public class ConfirmPanelController : MonoBehaviour {

        public float AnimationSpeed;
        public float Alpha = 1.0f;
        public GameObject HighScoreGameobject;

        private CanvasGroup _cg;
        private HighScoreController _hsc;

        void Start()
        {
            _cg = GetComponent<CanvasGroup>();
            _hsc = HighScoreGameobject.GetComponent<HighScoreController>();
        }

        void Update()
        {
            if (Mathf.Abs(_cg.alpha - Alpha) > 0.0001)
            {
                _cg.alpha = Mathf.Lerp(_cg.alpha, Alpha, AnimationSpeed);
            }
        }

        public void ReturnToMainMenu()
        {
            _hsc.IsVisible(false);
            this.IsVisible(true);
        }

        public void ReturnToGame()
        {
            if (Global.IsGameOver)
            {
                _hsc.IsVisible(true);
            }
            Global.IsGamePaused = false;
            this.IsVisible(false);
        }

        public void ActualReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenuScene");
        }

        public void IsVisible(bool visible)
        {
            Alpha = visible ? 1.0f : 0.0f;
            _cg.interactable = visible;
            _cg.blocksRaycasts = visible;
        }
    }
}
