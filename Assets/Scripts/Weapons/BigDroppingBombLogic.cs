using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Weapon;
using Assets.Scripts.Weapon.WeaponTypes;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class BigDroppingBombLogic : ShooterLogic
    {
        public Vector3 Velocity { get; set; }
        public int RainbowNumber { get; set; }

        protected override void Update()
        {
            this.WeaponExplosionLogic.CreateExplosion((ExplosionType)this.RainbowNumber, position: transform.position, radius: transform.localScale.x);
            this.RainbowNumber = this.RainbowNumber > 0 ? 0 : 3;

            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                if (Util.OutOfBounds(this.gameObject.transform.position))
                {
                    Destroy(this.gameObject);
                    return;
                }

                this.WeaponExplosionLogic.CreateExplosion(ExplosionType.HitExplosion);
                Destroy(this.gameObject);
            }
        }

        public override void Fire()
        {
            base.Fire();
            StartWeaponTimer();
            StartCoroutine(RunEndlessAfterDelay(0.5f, 0.6f, InitiateSplitBomb));
            StartCoroutine(Util.WaitWithDelegate(this.WeaponTimer, () =>
            {
                this.WeaponExplosionLogic.StartHit(this.Damage);
            }));
        }

        private void InitiateSplitBomb()
        {
            var position = new Vector3(this.transform.position.x, this.transform.position.y - (this.gameObject.transform.localScale.y * 2));
            var weapon = WeaponController.WeaponInventory.GetWeaponInstance(WeaponType.BigDroppingBomb, position);
            var component = weapon.GameObject.GetComponent<BigDroppingBombLogic>();
            weapon.Initialize(this.WeaponController);
            component.Velocity = Util.CalculateVelocity(270);
            component.RunAfterDelay(0.0001f, component.WeaponController.SetWeaponVelocity);
        }

        public void RunAfterDelay(float delay, Action<GameObject, Vector3> runIt)
        {
            StartCoroutine(Util.WaitWithDelegate(delay, () =>
            {
                runIt(this.gameObject, Velocity);
            }));
        }

        private IEnumerator RunEndlessAfterDelay(float waitTime, float tickTime, Action toRun)
        {
            yield return new WaitForSeconds(waitTime);
            var totalTicks = tickTime;
            while (true)
            {
                if (Mathf.Abs(totalTicks - this.weaponTimer) < 1)
                    break;

                toRun();
                totalTicks += tickTime;
                yield return new WaitForSeconds(tickTime);
            }
        }
    }
}
