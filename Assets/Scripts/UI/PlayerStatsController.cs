using Assets.Scripts.Extra;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class PlayerStatsController : MonoBehaviour, IVisible, IUiControl
    {
        public Image HealthBar;
        public Text Name;
        public Text Weapon;
        public float AnimationSpeed;

        private CanvasGroup _canvasGroup;
        private float _alpha = 0.0f;
        private float _nextHealth;

        private void Awake()
        {
            this._canvasGroup = GetComponentInParent<CanvasGroup>();
        }

        void Update ()
        {
            if (Global.CurrentPlayer == null)
                return;

            CheckVisibilityChanges();
            SetHealth();
            SetWeapon();
            SetName();
        }

        public bool CheckVisibilityChanges()
        {
            if (Mathf.Abs(_canvasGroup.alpha - _alpha) > 0.0001)
            {
                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _alpha, 0.1f);
                return true;
            }
            return false;
        }

        public void IsVisible(bool visible)
        {
            _alpha = visible ? 1.0f : 0.0f;
        }

        private void SetHealth()
        {
            if (HealthBar.transform.localScale.x != Global.CurrentPlayer.Health)
            {
                _nextHealth = _nextHealth == Global.CurrentPlayer.Health ? _nextHealth : Mathf.Clamp(Global.CurrentPlayer.Health, 0.0f, Global.CurrentPlayer.MaxHealth);
                float health = Mathf.Lerp(HealthBar.transform.localScale.x, (_nextHealth / Global.CurrentPlayer.MaxHealth), AnimationSpeed);
                HealthBar.transform.localScale = new Vector3(health, 1, 1);
            }
        }

        private void SetWeapon()
        {
            Weapon.text = Weapon.text == Global.CurrentPlayer.WeaponController.WeaponInventory.CurrentWeaponInstance.WeaponName
                ? Weapon.text
                : Global.CurrentPlayer.WeaponController.WeaponInventory.CurrentWeaponInstance.WeaponName;
        }

        public void SetName()
        {
            Name.text = Name.text == Global.CurrentPlayer.Name ? Name.text : Global.CurrentPlayer.Name;
        }
    }
}
