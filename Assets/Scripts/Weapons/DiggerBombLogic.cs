using System;
using Assets.Scripts.Weapon;
using Assets.Scripts.Weapon.WeaponTypes;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class DiggerBombLogic : ShooterLogic
    {
        public int BombNumber { get; set; }
        public Vector3 Velocity { get; set; }

        protected override void Update()
        {
            this.WeaponExplosionLogic.CreateExplosion(ExplosionType.EarthExplosion, position: transform.position, radius: transform.localScale.x/2, fadeSpeed: 0.2f);
            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                if (Util.OutOfBounds(this.gameObject.transform.position))
                {
                    Destroy(this.gameObject);
                    return;
                }

                if (this.BombNumber < 8)
                    InitiateDiggerBomb();

                this.WeaponExplosionLogic.CreateExplosion(ExplosionType.EarthExplosion);
                Destroy(this.gameObject);
            }
        }

        private void InitiateDiggerBomb()
        {
            var weapon = WeaponController.WeaponInventory.GetWeaponInstance(WeaponType.DiggerBomb, this.transform.position);
            var component = weapon.GameObject.GetComponent<DiggerBombLogic>();
            weapon.Initialize(this.WeaponController);
            component.Velocity = Util.CalculateVelocity(270);
            component.BombNumber = this.BombNumber + 1;
            component.RunAfterDelay(0.01f, component.WeaponController.SetWeaponVelocity);
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
