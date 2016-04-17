using Assets.Scripts.Extra;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class LoadbarController : MonoBehaviour, IUiControl
    {
        public GameObject Loadbar;
        public Text StatusText;
        public float AnimationSpeed;
        public bool IsVisible;

        public float _nextValue = 0;
        private CanvasGroup _canvasGroup;

        void Awake () {
            this._canvasGroup = GetComponent<CanvasGroup>();
            this.Loadbar.transform.localScale = new Vector3(0,1,1);
            this.StatusText.text = Global.InitializeGameProgressStatusText;
        }
	
        void Update ()
        {
            VisibilityLogic();
            ProgressLogic();
        }

        private void ProgressLogic()
        {
            if (!Global.PlayersReady)
            {
                if (Mathf.Abs(Loadbar.transform.localScale.x - Global.InitializeGameProgress) > 0.0001)
                {
                    this.StatusText.text = Global.InitializeGameProgressStatusText;
                    Loadbar.transform.localScale = new Vector3(Mathf.Lerp(Loadbar.transform.localScale.x, Global.InitializeGameProgress, AnimationSpeed), 1, 1);
                }
            }
        }

        private void VisibilityLogic()
        {
            float alpha = IsVisible ? 1.0f : 0.0f;
            if (Mathf.Abs(_canvasGroup.alpha - alpha) > 0.0001)
            {
                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, alpha, AnimationSpeed);
            }
        }
    }
}
