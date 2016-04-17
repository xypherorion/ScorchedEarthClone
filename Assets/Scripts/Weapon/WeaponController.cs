using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Extra;
using Assets.Scripts.Player;
using Assets.Scripts.Weapon.WeaponInteraction;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class WeaponController : MonoBehaviour
    {
        public PlayerBehaviour PlayerBehavior { get; set; }
        public WeaponInventory WeaponInventory { get; set; }

        public float ShootingAngle;
        public float ShootingPower;

        public Vector3 ShootingVelocity;
        public Vector3 ShotStartingPoint;

        public LineRenderer Trajectory;
        public GameObject ProjectileToTrack { get; set; }
        private bool IsTriggerCoroutineRunning { get; set; }
        private IEnumerator TriggerCoroutine { get; set; }

        void Awake()
        {
            this.PlayerBehavior = GetComponent<PlayerBehaviour>();
            this.WeaponInventory = GetComponent<WeaponInventory>();
            this.Trajectory = GetComponent<LineRenderer>();
            this.Trajectory.enabled = false;
            if(this.PlayerBehavior.State != PlayerState.Inactive)
                CalculateVelocity();
        }

        public void Fire()
        {
            if (this.PlayerBehavior.State != PlayerState.IsActive)
                return;

            InitializeShot();

            if (WeaponInventory.CurrentWeaponInstance.Trigger.IsTrigger)
            {
                FireWithTrigger();
                return;
            }

            if (WeaponInventory.CurrentWeaponInstance is IUtility)
            {
                UseUtility();
                return;
            }

            this.PlayerBehavior.ChangeState(PlayerState.IsFiring);
            CheckTrajectory(this.ShotStartingPoint, this.ShootingVelocity, Physics2D.gravity);

            var activeWeapon = WeaponInventory.GetWeaponInstance(this.ShotStartingPoint);

            if(WeaponInventory.CurrentWeaponInstance.CameraTrack)
                Global.TrackWeaponPosition = WeaponInventory.CurrentWeaponInstance.CameraTrack;
            else if (WeaponInventory.CurrentWeaponInstance.ProjectileTrack)
            {
                Global.TrackProjectilePosition = WeaponInventory.CurrentWeaponInstance.ProjectileTrack;
                this.ProjectileToTrack = activeWeapon.GameObject;
            }
            
            this.WeaponInventory.LastUsedWeapon = WeaponInventory.CurrentWeaponInstance;
            activeWeapon.Initialize(this);
            activeWeapon.Fire();
            this.WeaponInventory.UpdateWeaponQuantity();

        }

        public void FireWithTrigger()
        {
            if (this.PlayerBehavior.State != PlayerState.IsActive)
                return;
            this.PlayerBehavior.ChangeState(PlayerState.IsFiringWithTrigger, true);
            this.WeaponInventory.LastUsedWeapon = WeaponInventory.CurrentWeaponInstance;

            this.IsTriggerCoroutineRunning = true;
            this.TriggerCoroutine = Util.WaitWithTicksDelegateContinueWithDelegate(WeaponInventory.CurrentWeaponInstance.Trigger.TimeBetweenTicks, () =>
            {
                var activeWeapon = WeaponInventory.GetWeaponInstance(this.ShotStartingPoint);
                activeWeapon.Initialize(this);
                activeWeapon.Fire();
            }, () => { StopFire(); }, WeaponInventory.CurrentWeaponInstance.Trigger.TotalTicks);
            StartCoroutine(this.TriggerCoroutine);
            this.WeaponInventory.UpdateWeaponQuantity();
        }

        public void UseUtility()
        {
            if (this.PlayerBehavior.State != PlayerState.IsActive)
                return;
            this.PlayerBehavior.ChangeState(PlayerState.IsUsingUtility, false, true);

            if (this.PlayerBehavior.HasUsedUtility)
                return;

            this.PlayerBehavior.HasUsedUtility = true;
            var activeWeapon = WeaponInventory.GetWeaponInstance(this.ShotStartingPoint);
            this.WeaponInventory.LastUsedWeapon = WeaponInventory.CurrentWeaponInstance;
            activeWeapon.Initialize(this);
            activeWeapon.Fire();
            this.WeaponInventory.UpdateWeaponQuantity();
            this.WeaponInventory.ChangeCurrentWeapon(this.WeaponInventory.LastUsedWeaponType);
        }


        public void StopFire(bool playerDead = false)
        {
            if (this.TriggerCoroutine != null)
                StopCoroutine(this.TriggerCoroutine);
            this.IsTriggerCoroutineRunning = false;

            if (this.PlayerBehavior.State != PlayerState.IsFiringWithTrigger)
                return;

            if(!playerDead && !(WeaponInventory.LastUsedWeapon is IShooter))
                this.PlayerBehavior.ChangeState(PlayerState.IsEndingTurn);
        }

        public void ChangeDirection(Direction direction)
        {
            var nextAngle = Direction.Up == direction ? this.ShootingAngle + 1 : this.ShootingAngle - 1;

            if (!IsAngleAllowed(nextAngle))
                return;

            this.ShootingAngle = nextAngle >= 360 ? 0 : nextAngle <= 0 ? 359 : nextAngle;
            InitializeShot();
        }

        public void ChangePower(Direction direction)
        {
            var nextPower = Direction.Up == direction ? this.ShootingPower + 2 : this.ShootingPower - 2;
            if (!IsPowerAllowed(nextPower))
                return;
            
            this.ShootingPower = nextPower >= 100 ? 100 : nextPower <= 0 ? 2 : nextPower;
            InitializeShot();
        }

        public bool IsAngleAllowed(float angle)
        {
            var minAngle = WeaponInventory.CurrentWeaponInstance.Crosshair.From;
            var maxAngle = WeaponInventory.CurrentWeaponInstance.Crosshair.To;
            var a = (angle - minAngle) % 360 < 0 ? (angle - minAngle) + 360 : (angle - minAngle);
            var b = (maxAngle - minAngle) % 360 < 0 ? (maxAngle - minAngle) + 360 : (maxAngle - minAngle);

            return a <= b;
        }

        public bool IsPowerAllowed(float power)
        {
            var minPower = WeaponInventory.CurrentWeaponInstance.Power.From;
            var maxPower = WeaponInventory.CurrentWeaponInstance.Power.To;

            return power <= maxPower && power >= minPower;
        }

        public void SetWeaponVelocity(GameObject weaponObject, Vector3 velocity)
        {
            weaponObject.GetComponent<Rigidbody2D>().velocity = velocity;
        }

        public void IsCurrentlyFiring(bool withTrigger = false)
        {
            if (!(this.WeaponInventory.LastUsedWeapon is IShooter))
            {
                if (withTrigger)
                    return;

                this.PlayerBehavior.ChangeState(PlayerState.IsEndingTurn);
                return;
            }

            var weapons = GameObject.FindGameObjectsWithTag("Weapon");
            foreach (var weapon in weapons)
            {
                if (weapon == null)
                    continue;

                var component = (IShooter)(IWeapon)weapon.GetComponent(typeof(IWeapon));

                if (component == null)
                    continue;

                if (component.Player == this.gameObject)
                    return;
            }

            if (this.IsTriggerCoroutineRunning)
                return;

            this.PlayerBehavior.ChangeState(PlayerState.IsEndingTurn);
        }

        public void InitializeShot()
        {
            CalculateVelocity();
            this.ShotStartingPoint = CalculateFirePoint(transform.localScale);

            if(this.PlayerBehavior.Stats.ShowTrajectory)
                CheckTrajectory(this.ShotStartingPoint, this.ShootingVelocity, Physics2D.gravity);
        }

        private void CalculateVelocity()
        {
            var radian = (Mathf.PI / 180) * this.ShootingAngle;
            var velocity = new Vector3((float)Math.Cos(radian), (float)Math.Sin(radian));
            var power = this.ShootingPower <= 0 ? 2 : this.ShootingPower;
            this.ShootingVelocity = new Vector3(velocity.x * ((power * 4)/10), velocity.y * (power * 2/10));
        }

        public Vector3 CalculateFirePoint(Vector3 distance, Vector3 position = default(Vector3), bool flipAngle = false)
        {
            position = position == default(Vector3) ? transform.position : position;
            var degree = Mathf.Atan2((position.x + this.ShootingVelocity.x) - position.x, (position.y + this.ShootingVelocity.y) - position.y) * 180 / Mathf.PI;
            var fittedDegree = degree > 90 ? Mathf.Abs((degree - 450) % -360) : Mathf.Abs((degree - 90) % 360);
            fittedDegree = flipAngle ? fittedDegree + 180 > 360 ? fittedDegree + 180 - 360 : fittedDegree + 180 : fittedDegree;
            return AngleToPoint(fittedDegree, distance, position);
        }

        private Vector3 AngleToPoint(float degree, Vector3 distance, Vector3 position = default(Vector3))
        {
            position = position == default(Vector3) ? transform.position : position;
            var resultY = position.y + (distance.y) * Mathf.Sin(degree * (Mathf.PI / 180));
            var resultX = position.x + (distance.x) * Mathf.Cos(degree * (Mathf.PI / 180));
            return new Vector3(resultX, resultY, 0);
        }

        private void CheckTrajectory(Vector3 initialPosition, Vector3 initialVelocity, Vector3 gravity, int maxSteps = 1000)
        {
            Vector3 position = initialPosition;
            Vector3 velocity = initialVelocity;
            var pathPositions = new List<Vector3> { position };

            float magnitude = 0.2f / velocity.magnitude;
            for (int i = 0; i < maxSteps; ++i)
            {
                if (position.y < Global.TerrainNullPoint.y || position.x < Global.TerrainNullPoint.x || position.x > Global.TerrainEndPoint.x)
                    break;

                var oldPosition = new Vector3(position.x, position.y, position.z);
                position += velocity * magnitude + 0.5f * gravity * magnitude * magnitude;
                velocity += gravity * magnitude;

                pathPositions.Add(position);

                if (CheckTrajectoryCollision(oldPosition, position, pathPositions))
                    break;
            }

            BuildTrajectoryLine(pathPositions);
            Global.NextProjectileCollision = position;
        }

        private bool CheckTrajectoryCollision(Vector3 oldPosition, Vector3 newPosition, List<Vector3> positions)
        {
            bool returntype = false;
            var hasHitSomething = Physics2D.Linecast(oldPosition, newPosition);
            if (hasHitSomething)
                if (hasHitSomething.transform.gameObject != PlayerBehavior.gameObject)
                {
                    //positions.Add(hasHitSomething.point);
                    returntype = true;
                }
            positions.Add(newPosition);

            return returntype;
        }


        private void BuildTrajectoryLine(List<Vector3> positions)
        {
            this.Trajectory.material = new Material(Shader.Find("Particles/Additive"));
            this.Trajectory.material.color = Color.white;
            this.Trajectory.SetWidth(0.1f, 0.1f);
            this.Trajectory.SetVertexCount(positions.Count);
            for (var i = 0; i < positions.Count; ++i)
                this.Trajectory.SetPosition(i, positions[i]);
        }

        public void SetTrajectory(bool enable = true)
        {
            if (this.Trajectory.enabled != enable)
            {
                if (enable)
                {
                    if (this.PlayerBehavior.Stats.ShowTrajectory)
                        this.Trajectory.enabled = enable;
                }
                else
                    this.Trajectory.enabled = enable;

                InitializeShot();
            }
        }
    }

    public class Hit
    {
        public HitLogic Sender { get; set; }
        public Vector3 TargetPosition { get; set; }
        public float HitForce { get; set; }
        public float HitDamage { get; set; }
        public float HitRadius { get; set; }
        public Vector3 HitCenter { get; set; }
        public List<HitPoint> HitPoints { get; set; }
    }

    public enum WeaponType
    {
        Bomb, BigBomb, Grenade, RollingBomb, SplitBomb, Shotgun, Machinegun, DiggerBomb, DroppingBomb, BigDroppingBomb, RainbowBomb, DubstepBomb, ExBomb, SpiralBomb, SunBomb, Spade, Blowtorch, Digger, SpeedIncrease, JumpIncrease, FallDecrease, DamageIncrease, AimingPoint, SmallHealthPotion, LargeHealthPotion
    }

    public interface IWeapon
    {
        GameObject GameObject { get; }
        GameObject Player { get; set; }
        string WeaponName { get; }
        string WeaponDescription { get; }
        int WeaponPrice { get; }
        int WeaponTimer { get; }
        bool CameraTrack { get; }
        bool ProjectileTrack { get; }
        bool DisplayHud { get; }
        Trigger Trigger { get; }
        CrosshairData Crosshair { get; }
        PowerData Power { get; }

        void Initialize(WeaponController weaponController);
        void Fire();
    }

    public interface IShooter : IWeapon
    {
        float Damage { get;}
    }

    public interface IMine : IWeapon
    {
        float Damage { get; }
    }

    public interface ITool : IWeapon
    {
        float Damage { get; }
    }

    public interface IMelee : IWeapon
    {
        float Damage { get; }
    }

    public interface IUtility : IWeapon
    {
        float IncreaseBy { get; }
    }
}
