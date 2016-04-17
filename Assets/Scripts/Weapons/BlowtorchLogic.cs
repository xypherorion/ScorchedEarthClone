using Assets.Scripts.Weapon;
using Assets.Scripts.Weapon.WeaponTypes;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class BlowtorchLogic : ToolLogic
    {
        protected override void Update()
        {
            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                if (Util.OutOfBounds(this.gameObject.transform.position))
                {
                    Destroy(this.gameObject);
                    return;
                }
                this.WeaponExplosionLogic.CreateExplosion(ExplosionType.LightExplosion, fadeSpeed: 0.3f);
                Destroy(this.gameObject);
            }
        }

        public override void Initialize(WeaponController weaponController)
        {
            base.Initialize(weaponController);
            this.WeaponExplosionLogic.HitRadius = transform.localScale.y * 3;
            var spawnPosition = (this.Player.transform.position + new Vector3(0, transform.localScale.y/3));
            transform.position = this.WeaponController.CalculateFirePoint(this.Player.transform.localScale, spawnPosition);
        }

        public override void Fire()
        {
            base.Fire();
            this.WeaponExplosionLogic.StartHit(this.Damage, this.Player);
        }
    }
}
