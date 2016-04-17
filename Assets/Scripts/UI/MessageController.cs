using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class MessageController : MonoBehaviour, IUiControl
    {
        public float AnimationSpeed = 0.3f;
        public bool IsVisible;

        private Text _text;
        private CanvasGroup _canvasGroup;
        private bool _isTimedMessage;
        private DateTime _endTime;

        void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _text = GetComponentInChildren<Text>();
            _endTime = DateTime.Now;
        }

        void Update()
        {
            DisplayLogic();
        }

        private void DisplayLogic()
        {
            if (_isTimedMessage && _endTime < DateTime.Now)
                IsVisible = false;

            float alpha = IsVisible ? 1.0f : 0.0f;
            if(_canvasGroup.alpha != alpha)
                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, alpha, AnimationSpeed);
        }

        public void DisplayMessage(string message, int time = 0, bool overwrite = false)
        {
            if (this._endTime != null)
            {
                if (_endTime > DateTime.Now && !overwrite)
                    return;
            }
            _text.text = message;
            _isTimedMessage = time != 0;
            _endTime = DateTime.Now.Add(TimeSpan.FromSeconds(time));
            IsVisible = true;
        }
    }
}
