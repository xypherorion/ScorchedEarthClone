using System.Collections.Generic;
using Assets.Scripts.Weapon;
using Assets.Scripts.Weapon.WeaponTypes;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class ShotgunLogic : ShooterLogic
    {
        protected override void Update()
        {
            this.WeaponExplosionLogic.CreateExplosion(ExplosionType.WhiteExplosion, position: transform.position, radius: transform.localScale.x / 3, fadeSpeed: 0.2f);
            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                if (Util.OutOfBounds(this.gameObject.transform.position))
                {
                    Destroy(this.gameObject);
                    return;
                }
                this.WeaponExplosionLogic.CreateExplosion(ExplosionType.WhiteExplosion, fadeSpeed: 0.2f);
                Destroy(this.gameObject);
            }
        }

        private List<IWeapon> weapons = new List<IWeapon>();
        public override void Fire()
        {
            weapons.Add(this);
            weapons.Add(WeaponController.WeaponInventory.GetWeaponInstance(WeaponType.Shotgun, this.transform.position));
            weapons.Add(WeaponController.WeaponInventory.GetWeaponInstance(WeaponType.Shotgun, this.transform.position));

            float offset = -1f;
            foreach (var obj in weapons)
            {
                obj.Initialize(this.WeaponController);
                WeaponController.SetWeaponVelocity(obj.GameObject, new Vector3(WeaponController.ShootingVelocity.x+offset, WeaponController.ShootingVelocity.y+offset, WeaponController.ShootingVelocity.z));
                offset += 1f;
            }
        }
    }
}
