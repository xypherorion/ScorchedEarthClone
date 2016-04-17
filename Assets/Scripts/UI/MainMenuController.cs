using UnityEngine;

namespace Assets.Scripts.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public float AnimationSpeed;
        public float Alpha = 1.0f;

        private CanvasGroup _cg;

        void Start ()
        {
            _cg = GetComponent<CanvasGroup>();
        }
	
        void Update () {

            if (Mathf.Abs(_cg.alpha - Alpha) > 0.0001)
            {
                _cg.alpha = Mathf.Lerp(_cg.alpha, Alpha, AnimationSpeed);
            }
        }

        public void IsVisible(bool visible)
        {
            Alpha = visible ? 1.0f : 0.0f;
            _cg.interactable = visible;
            _cg.blocksRaycasts = visible;
        }
    }
}
