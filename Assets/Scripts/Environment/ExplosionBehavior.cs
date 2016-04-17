using UnityEngine;

namespace Assets.Scripts.Environment
{
    public class ExplosionBehavior : MonoBehaviour
    {
        public float FadeSpeed;
        public float FadeDelay;

        private bool _startFade;
        private Renderer _renderer;
        private float _alphaEnd;

        private void Awake()
        {
            this._renderer = GetComponent<Renderer>();
            SetStartAlpha(1f);
        }

        private void Update()
        {
            if (_startFade)
                CheckVisibilityChanges();
        }

        public void SetStartAlpha(float alpha = 1f)
        {
            this._renderer.material.color = new Color(this._renderer.material.color.r, this._renderer.material.color.g, this._renderer.material.color.b, alpha);
            this._alphaEnd = 0;
        }

        public void StartFade()
        {
            if (!(this.FadeDelay > 0))
                this._startFade = true;
            else
                StartCoroutine(Util.WaitWithDelegate(this.FadeDelay, () =>
                {
                    this._startFade = true;
                }));
        }

        public void CheckVisibilityChanges()
        {
            if (this._renderer.material.color.a > 0.01)
            {
                var color = new Color(this._renderer.material.color.r, this._renderer.material.color.g, this._renderer.material.color.b, Mathf.Lerp(this._renderer.material.color.a, this._alphaEnd, this.FadeSpeed));
                this._renderer.material.color = color;
                return;
            }
            Destroy(this.gameObject);
        }
    }
}
