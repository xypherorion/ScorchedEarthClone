using System;
using Assets.Scripts.Weapon;
using Assets.Scripts.Weapon.WeaponTypes;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class SunBombLogic : ShooterLogic
    {
        public int BombNumber { get; set; }
        public Vector3 Velocity { get; set; }
        public float Angle { get; set; }

        protected override void Update()
        {
            if (this.BombNumber < 1)
                this.WeaponExplosionLogic.CreateExplosion(ExplosionType.OrangeExplosion, position: transform.position, radius: transform.localScale.x / 2, fadeSpeed: 0.2f);
            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                if (Util.OutOfBounds(this.gameObject.transform.position))
                {
                    Destroy(this.gameObject);
                    return;
                }

                if (this.BombNumber < 4)
                    InitiateExBomb();

                this.WeaponExplosionLogic.CreateExplosion(this.BombNumber == 0 ? ExplosionType.OrangeExplosion : ExplosionType.LightExplosion, fadeSpeed: 0.1f);
                Destroy(this.gameObject);
            }
        }

        private void InitiateExBomb()
        {
            if (BombNumber < 1)
            {
                for (int i = 0; i < 6; i++)
                {
                    var angle = i == 0 ? 60f : (i * 60) + 60f;
                    var newWeapon = WeaponController.WeaponInventory.GetWeaponInstance(WeaponType.SunBomb, this.transform.position);
                    var newComponent = newWeapon.GameObject.GetComponent<SunBombLogic>();
                    newWeapon.Initialize(this.WeaponController);
                    newComponent.Collider.enabled = false;
                    newComponent.Angle = angle;
                    newComponent.Velocity = Util.CalculateVelocity(angle);
                    newComponent.BombNumber = this.BombNumber + 1;
                    newComponent.RunAfterDelay(0.01f, newComponent.WeaponController.SetWeaponVelocity);
                }
                return;
            }

            var weapon = WeaponController.WeaponInventory.GetWeaponInstance(WeaponType.SunBomb, this.transform.position);
            var component = weapon.GameObject.GetComponent<SunBombLogic>();
            weapon.Initialize(this.WeaponController);
            component.Collider.enabled = false;
            component.Angle = this.Angle;
            component.Velocity = Util.CalculateVelocity(this.Angle, 20);
            component.BombNumber = this.BombNumber + 1;
            component.RunAfterDelay(0.01f, component.WeaponController.SetWeaponVelocity);
        }

        public void RunAfterDelay(float delay, Action<GameObject, Vector3> runIt)
        {
            StartCoroutine(Util.WaitWithDelegate(delay, () =>
            {
                runIt(this.gameObject, Velocity);
                StartCoroutine(Util.WaitWithDelegate(0.1f, () =>
                {
                    this.WeaponExplosionLogic.HitRadius *= 0.2f;
                    this.WeaponExplosionLogic.StartHit(this.Damage * 0.5f);
                }));
            }));
        }
    }
}
