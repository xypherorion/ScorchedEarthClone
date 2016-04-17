using Assets.Scripts.Weapon;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UtilityUsedController : MonoBehaviour
    {
        public float AnimationSpeed;
        public Image Image;

        private CanvasGroup _canvasGroup;
        private Sprite[] _sprites;
        private float _alpha = 0.0f;
        private float _y = 5f;
        private bool _toShow;
        private bool _isShowing;

        void Start()
        {
            this._canvasGroup = GetComponent<CanvasGroup>();
            this._sprites =  Resources.LoadAll<Sprite>("Sprites/Inventory/");
        }

        void Update()
        {
            if (this._toShow && Mathf.Abs(_canvasGroup.alpha - 1) < 0.01)
            {
                this._alpha = 0.0f;
                this._toShow = false;
            }

            if (Mathf.Abs(_canvasGroup.alpha - _alpha) > 0.0001)
            {
                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _alpha, AnimationSpeed);
            }
            if (Mathf.Abs(this.transform.localPosition.y - this._y) > 0.0001)
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, Mathf.Lerp(this.transform.localPosition.y, this._y, 0.06f));
        }

        public void ShowUtility(WeaponType weapon)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.gameObject.transform.parent.gameObject.transform.position.y);
            this.Image.sprite = this._sprites[(int) weapon];
            this._alpha = 1.0f;
            this._toShow = true;
        }
    }
}
