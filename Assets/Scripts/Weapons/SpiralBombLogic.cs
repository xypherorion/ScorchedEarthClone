using System;
using Assets.Scripts.Weapon;
using Assets.Scripts.Weapon.WeaponTypes;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class SpiralBombLogic : ShooterLogic
    {
        public int BombNumber { get; set; }
        public Vector3 Velocity { get; set; }
        public int Angle { get; set; }

        protected override void Update()
        {
            if (this.BombNumber < 1)
                this.WeaponExplosionLogic.CreateExplosion(ExplosionType.PinkExplosion, position: transform.position, radius: transform.localScale.x / 2, fadeSpeed: 0.2f);
            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                if (Util.OutOfBounds(this.gameObject.transform.position))
                {
                    Destroy(this.gameObject);
                    return;
                }

                if (this.BombNumber < 6)
                    InitiateExBomb();

                this.WeaponExplosionLogic.CreateExplosion(ExplosionType.PinkExplosion, fadeSpeed: 0.1f);
                Destroy(this.gameObject);
            }
        }

        private void InitiateExBomb()
        {
            if (BombNumber < 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    int angle = i == 0 ? 45 : (i * 90) + 45;
                    var newWeapon = WeaponController.WeaponInventory.GetWeaponInstance(WeaponType.SpiralBomb, this.transform.position);
                    var newComponent = newWeapon.GameObject.GetComponent<SpiralBombLogic>();
                    newWeapon.Initialize(this.WeaponController);
                    newComponent.Collider.enabled = false;
                    newComponent.Angle = angle;
                    newComponent.Velocity = Util.CalculateVelocity(angle);
                    newComponent.BombNumber = this.BombNumber + 1;
                    newComponent.RunAfterDelay(0.01f, newComponent.WeaponController.SetWeaponVelocity);
                }
                return;
            }

            var weapon = WeaponController.WeaponInventory.GetWeaponInstance(WeaponType.SpiralBomb, this.transform.position);
            var component = weapon.GameObject.GetComponent<SpiralBombLogic>();
            weapon.Initialize(this.WeaponController);
            component.Collider.enabled = false;
            component.Angle = this.Angle + 20;
            component.Velocity = Util.CalculateVelocity(this.Angle + 20, 35);
            component.BombNumber = this.BombNumber + 1;
            component.RunAfterDelay(0.01f, component.WeaponController.SetWeaponVelocity);
        }

        public void RunAfterDelay(float delay, Action<GameObject, Vector3> runIt)
        {
            StartCoroutine(Util.WaitWithDelegate(delay, () =>
            {
                runIt(this.gameObject, Velocity);
                StartCoroutine(Util.WaitWithDelegate(0.06f, () =>
                {
                    this.WeaponExplosionLogic.StartHit(this.Damage);
                }));
            }));
        }
    }
}
