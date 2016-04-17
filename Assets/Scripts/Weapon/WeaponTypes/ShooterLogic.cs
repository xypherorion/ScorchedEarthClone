using Assets.Scripts.Weapon.WeaponInteraction;
using UnityEngine;

namespace Assets.Scripts.Weapon.WeaponTypes
{
    public class ShooterLogic : WeaponLogic, IShooter
    {
        public float damage;
        public float Damage { get { return this.damage; } }

        public BoxCollider2D Collider
        {
            get
            {
                if (this._collider != null)
                    return this._collider;
                this._collider = GetComponent<BoxCollider2D>();
                return this._collider;
            }
        }

        private BoxCollider2D _collider;

        protected WeaponExplosionLogic WeaponExplosionLogic { get; set; }

        protected virtual void Awake()
        {
            this.WeaponExplosionLogic = GetComponent<WeaponExplosionLogic>();
            this._collider = GetComponent<BoxCollider2D>();
        }

        protected virtual void Update()
        {
            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.gameObject == Player || collision.gameObject.CompareTag("Weapon"))
            {
                Physics2D.IgnoreCollision(Collider, collision.collider);
                return;
            }
            this.WeaponExplosionLogic.StartHit(this.Damage);
        }

        public override void Fire()
        {
            base.Fire();
            WeaponController.SetWeaponVelocity(this.gameObject, WeaponController.ShootingVelocity);
        }
    }
}
