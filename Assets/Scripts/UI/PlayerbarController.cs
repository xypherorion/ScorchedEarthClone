using System;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class PlayerbarController : MonoBehaviour
    {
        public float AnimationSpeed = 0.1f;
        public bool IsVisible = true;
        public bool IsCurrentlyVisible { get { return _canvasGroup.alpha > 0.9f; } }
        public Image HealthBar;
        public Text PlayerNameText;
        public Text HealthLostText;

        private CanvasGroup _canvasGroup;

        private PlayerBehaviour _player;
        private bool _updateHealth;
        private float _currentHealth;

        void Start()
        {
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
            _player = GetComponentInParent<PlayerBehaviour>();
            this._currentHealth = _player.MaxHealth;
            SetPlayerName(_player.Name);
        }

        void Update()
        {
            SetAlpha();
            SetHealth();
            UpdateHealth();
        }

        private void SetAlpha()
        {
            float alpha = IsVisible ? 1.0f : 0.0f;

            if (_canvasGroup.alpha != alpha)
                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, alpha, AnimationSpeed);
        }

        private void UpdateHealth()
        {
            if (this._updateHealth)
            {
                float h = Mathf.Lerp(HealthBar.transform.localScale.x, (_player.Health / _player.MaxHealth <= 0 ? 0 : _player.Health / _player.MaxHealth), AnimationSpeed);
                HealthBar.transform.localScale = new Vector3(h, 1, 1);

                if (!(Math.Abs((HealthBar.transform.localScale.x * this._player.MaxHealth <= 0 ? 0 : HealthBar.transform.localScale.x * this._player.MaxHealth) - _player.Health) > 0.0001))
                    this._updateHealth = false;
            }

            var alpha = this._updateHealth ? 1f : 0.0f;
            if (HealthLostText.color.a != alpha)
            {
                float speed = alpha == 0.0f ? AnimationSpeed / 2.0f : AnimationSpeed * 2;
                HealthLostText.color = new Color(255, 255, 255, Mathf.Lerp(HealthLostText.color.a, alpha, speed));
            }
        }

        public void SetHealth()
        {
            if (!(Math.Abs(this._currentHealth - _player.Health) > 0.0001))
                return;

            var positive = _player.Health > _currentHealth;
            int health = positive ? (int) (_player.Health - _currentHealth) : (int)(_currentHealth - _player.Health);
            HealthLostText.text = (positive ? "+" : "-") + health;

            this._currentHealth = _player.Health;
            this._updateHealth = true;
        }

        private void SetPlayerName(string playerName)
        {
            PlayerNameText.text = playerName.Length > 15 ? playerName.Substring(0, 15) : playerName;
        }
    }
}
