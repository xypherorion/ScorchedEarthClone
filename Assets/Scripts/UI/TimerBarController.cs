using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TimerBarController : MonoBehaviour, IVisible, IUiControl
    {
        public Text TimeText;

        private CanvasGroup _canvasGroup;
        private float _alpha = 0.0f;
        private bool _ready;

        private GameObject _objectToTrack;

        private void Awake()
        {
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
        }

        private void Update()
        {
            CheckVisibilityChanges();

            if (this._alpha > 0f && this._objectToTrack != null)
                this.transform.position = new Vector3(_objectToTrack.transform.position.x, _objectToTrack.transform.position.y, 0);

            if(this._ready && this._objectToTrack == null)
                Destroy(this.gameObject);
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


        public void DisplayTimer(GameObject obj, int seconds)
        {
            this._objectToTrack = obj;
            this.transform.position = new Vector3(_objectToTrack.transform.position.x, _objectToTrack.transform.position.y, 0);
            this._ready = true;
            SetTime(seconds);
        }

        private void SetTime(int seconds)
        {
            StartCoroutine(SetTimeDelay(1f, value =>
            {
                this.TimeText.text = (seconds - value).ToString();
            }, seconds));

        }

        private IEnumerator SetTimeDelay(float tickDuration, Action<int> toRun, float maxDuration = Single.MaxValue)
        {
            IsVisible(true);
            for (float i = 0; i < maxDuration; i++)
            {
                toRun((int)i);
                yield return new WaitForSeconds(tickDuration);
            }
            Destroy(this.gameObject);
        }
    }
}
