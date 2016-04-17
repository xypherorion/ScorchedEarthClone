using System;
using System.Collections.Generic;
using Assets.Scripts.Weapon;
using Assets.Scripts.Weapon.WeaponTypes;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class RainbowBomb : ShooterLogic
    {
        public bool IsSplitBomb { get; set; }
        public Vector3 Velocity { get; set; }
        public int RainbowNumber { get; set; }

        protected override void Update()
        {
            this.WeaponExplosionLogic.CreateExplosion((ExplosionType)this.RainbowNumber, position: transform.position, radius: transform.localScale.x, fadeSpeed: 0.2f, delay: 0.2f);
            if(!IsSplitBomb)
                this.RainbowNumber = this.RainbowNumber + 1 > 4 ? 0 : this.RainbowNumber + 1;

            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                if (Util.OutOfBounds(this.gameObject.transform.position))
                {
                    Destroy(this.gameObject);
                    return;
                }

                if (!this.IsSplitBomb)
                    InitiateSplitBombs();

                this.WeaponExplosionLogic.CreateExplosion(this.IsSplitBomb ? (ExplosionType)this.RainbowNumber : ExplosionType.WhiteExplosion);
                Destroy(this.gameObject);
            }
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            if (this.IsSplitBomb)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), collision.collider);
                    return;
                }
            }
            base.OnCollisionEnter2D(collision);
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

        private void InitiateSplitBombs()
        {
            var weapons = new List<IWeapon>
            {
                WeaponController.WeaponInventory.GetWeaponInstance(WeaponType.RainbowBomb, this.transform.position),
                WeaponController.WeaponInventory.GetWeaponInstance(WeaponType.RainbowBomb, this.transform.position),
                WeaponController.WeaponInventory.GetWeaponInstance(WeaponType.RainbowBomb, this.transform.position),
                WeaponController.WeaponInventory.GetWeaponInstance(WeaponType.RainbowBomb, this.transform.position),
                WeaponController.WeaponInventory.GetWeaponInstance(WeaponType.RainbowBomb, this.transform.position)
            };

            int i = 360/5;
            int j = 1;
            foreach (var weapon in weapons)
            {
                var component = weapon.GameObject.GetComponent<RainbowBomb>();
                weapon.Initialize(this.WeaponController);
                component.Velocity = Util.CalculateVelocity(i);
                component.IsSplitBomb = true;
                component.RainbowNumber = j - 1;
                component.RunAfterDelay(j * 0.02f, component.WeaponController.SetWeaponVelocity);
                i = i + (360/5);
                j++;
            }
        }

        public void RunAfterDelay(float delay, Action<GameObject, Vector3> runIt)
        {
            StartCoroutine(Util.WaitWithDelegate(delay, () =>
            {
                runIt(this.gameObject, Velocity);
            }));
        }
    }
}
