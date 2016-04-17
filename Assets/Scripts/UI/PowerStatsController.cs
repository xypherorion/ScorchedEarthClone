using System;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class PowerStatsController : MonoBehaviour
    {
        public bool IsVisible;
        public float AnimationSpeed = 0.5f;
        public Text TextPower;
        public Text TextAngle;
        public Image CrosshairImage;
        public GameObject PowerPanel;
        public GameObject AnglePanel;

        private PlayerBehaviour _player;
        private CanvasGroup _canvasGroup;
        private float _currentAngle = 0;
        private float _initX;
        private bool _isFacingLeft = true;

        void Awake () {
            _canvasGroup = GetComponent<CanvasGroup>();
            _player = GetComponentInParent<PlayerBehaviour>();
            _initX = PowerPanel.transform.localPosition.x;
        }
	
        void Update ()
        {
            SetAlpha();
            UpdateInfo();
            PositionLogic();
            CrosshairLogic();
        }

        private void SetAlpha()
        {
            float alpha = IsVisible ? 1.0f : 0.0f;

            if (Math.Abs(_canvasGroup.alpha - alpha) > 0.001)
                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, alpha, 0.5f);
        }

        private void PositionLogic()
        {
            if (!IsVisible)
                return;

            _isFacingLeft = (_player.WeaponController.ShootingAngle > 90 && _player.WeaponController.ShootingAngle < 270);

            float x = _isFacingLeft ? _initX : _initX*-1;

            if (Mathf.Abs(PowerPanel.transform.localPosition.x - x) > 0.0001)
            {
                PowerPanel.transform.localPosition = new Vector3(
                    Mathf.Lerp(PowerPanel.transform.localPosition.x, x, AnimationSpeed), PowerPanel.transform.localPosition.y, PowerPanel.transform.localPosition.z);
                AnglePanel.transform.localPosition = new Vector3(
                    Mathf.Lerp(AnglePanel.transform.localPosition.x, x, AnimationSpeed), AnglePanel.transform.localPosition.y, AnglePanel.transform.localPosition.z);
            }
        }

        private void UpdateInfo()
        {
            if (!IsVisible)
                return;

            if (Math.Abs(_currentAngle - _player.WeaponController.ShootingAngle) > 0.01)
            {
                
                int angleWithOffset = (270 + (int)_player.WeaponController.ShootingAngle) % 360;
                TextAngle.text = angleWithOffset + "°";
            }
            
            TextPower.text = _player.WeaponController.ShootingPower + "%";
        }

        private void CrosshairLogic()
        {
            if (!IsVisible)
                return;

            DisplayCrosshair(true);

            if ( Math.Abs(_currentAngle - _player.WeaponController.ShootingAngle) > 0.01)
            {
                _currentAngle = _player.WeaponController.ShootingAngle;
                CrosshairImage.transform.position = _player.WeaponController.CalculateFirePoint(new Vector3(1, 1));
            } 
        }

        private void DisplayCrosshair(bool enable)
        {
            if (this._player.Stats.ShowTrajectory)
                enable = false;

            if (this.CrosshairImage.enabled != enable)
                this.CrosshairImage.enabled = enable;
        }
    }
}
