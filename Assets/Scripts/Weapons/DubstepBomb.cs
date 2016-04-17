using System;
using System.Collections;
using Assets.Scripts.Weapon.WeaponTypes;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class DubstepBomb : ShooterLogic
    {
        private int _count = 0;
        private int _currentCount = 1;
        protected override void Update()
        {
            this._count++;
            if (this._count % 2 == 0)
            {
                this._currentCount = this._currentCount + 1 > 3 ? 1 : this._currentCount + 1;
                this.WeaponExplosionLogic.CreateExplosion((ExplosionType)2, position: transform.position, radius: transform.localScale.x * this._currentCount, fadeSpeed: 0.1f, delay: 0.1f, startAlpha: 1f);
            }
            if (this._count % 11 == 0)
            {
                this.WeaponExplosionLogic.CreateExplosion((ExplosionType)4, position: transform.position, radius: transform.localScale.x * 4, fadeSpeed: 0.1f, delay: 0.1f, startAlpha: 0.7f);
            }
            
            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                if (Util.OutOfBounds(this.gameObject.transform.position))
                {
                    Destroy(this.gameObject);
                    return;
                }
                this.WeaponExplosionLogic.CreateExplosion(ExplosionType.GreenExplosion);
                Destroy(this.gameObject);
            }
        }
    }
}
