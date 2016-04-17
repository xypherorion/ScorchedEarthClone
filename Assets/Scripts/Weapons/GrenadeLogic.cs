using Assets.Scripts.UI;
using Assets.Scripts.Weapon.WeaponTypes;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class GrenadeLogic : ShooterLogic
    {
        protected override void Update()
        {
            this.WeaponExplosionLogic.CreateExplosion(ExplosionType.WhiteExplosion, position: transform.position, radius: transform.localScale.x / 2);

            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                if (Util.OutOfBounds(this.gameObject.transform.position))
                {
                    Destroy(this.gameObject);
                    return;
                }

                this.WeaponExplosionLogic.CreateExplosion(ExplosionType.WhiteExplosion);
                Destroy(this.gameObject);
            }
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.gameObject == Player || collision.gameObject.CompareTag("Weapon"))
            {
                Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), collision.collider);
                return;
            }
        }

        public override void Fire()
        {
            base.Fire();
            StartWeaponTimer();
            StartCoroutine(Util.WaitWithDelegate(this.WeaponTimer, () =>
            {
                this.WeaponExplosionLogic.StartHit(this.Damage);
            }));
        }
    }
}
