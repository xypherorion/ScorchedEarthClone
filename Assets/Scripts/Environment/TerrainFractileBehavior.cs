using UnityEngine;

namespace Assets.Scripts.Environment
{
    public class TerrainFractileBehavior : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2D;
        private ExplosionBehavior _explosionBehavior;

        private void Awake ()
        {
            this._rigidbody2D = GetComponent<Rigidbody2D>();
            this._explosionBehavior = GetComponent<ExplosionBehavior>();
        }

        private void Start()
        {
            this.SetVelocity();
        }

        private void Update()
        {
            if (Util.OutOfBounds(this.gameObject.transform.position))
            {
                Destroy(this.gameObject);
            }
        }

        public void SetVelocity()
        {
            this._rigidbody2D.velocity = Util.CalculateVelocity(Random.Range(1, 360), 40);

            _explosionBehavior.FadeSpeed = Random.Range(0.1f, 0.5f);
            _explosionBehavior.SetStartAlpha();
            _explosionBehavior.StartFade();
        }
    }
}
