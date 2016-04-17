using Assets.Scripts.Player;
using Assets.Scripts.UI;
using Assets.Scripts.Weapon.WeaponInteraction;
using UnityEngine;

namespace Assets.Scripts.Weapon.WeaponTypes
{
    public class WeaponLogic : MonoBehaviour
    {
        public GameObject GameObject { get { return this.gameObject; } }
        public GameObject Player { get; set; }
        public GameObject WeaponTimerBar;

        public bool cameraTrack;
        public bool projectileTrack;
        public bool displayHud;
        public string weaponName;
        public string weaponDescription;
        public int weaponPrice;
        public int weaponTimer;
        public Trigger trigger;
        public CrosshairData crosshair;
        public PowerData power;

        public bool DisplayHud          { get { return this.displayHud; } }
        public bool CameraTrack         { get { return this.cameraTrack; } }
        public bool ProjectileTrack     { get { return this.projectileTrack; } }
        public string WeaponName        { get { return this.weaponName; } }
        public string WeaponDescription { get { return this.weaponDescription; } }
        public int WeaponPrice          { get { return this.weaponPrice; } }
        public int WeaponTimer          { get { return this.weaponTimer; } }
        public Trigger Trigger          { get { return this.trigger; } }
        public CrosshairData Crosshair  { get { return this.crosshair; } }
        public PowerData Power          { get { return this.power; } }

        protected PlayerBehaviour PlayerBehavior { get; set; }
        protected WeaponController WeaponController { get; set; }

        public virtual void Initialize(WeaponController weaponController)
        {
            this.WeaponController = weaponController;
            this.Player = weaponController.gameObject;
            this.PlayerBehavior = this.Player.GetComponent<PlayerBehaviour>();
            AssignOwner();
        }

        public virtual void Fire()
        {
            if (this.PlayerBehavior.State != (this.Trigger.IsTrigger ? PlayerState.IsFiringWithTrigger : PlayerState.IsFiring))
                return;
        }

        private void AssignOwner()
        {
            var hitLogic = GetComponent<HitLogic>();
            if(hitLogic != null)
                hitLogic.Owner = this.PlayerBehavior;
        }

        protected void StartWeaponTimer()
        {
            if (!(this.WeaponTimer > 0))
                return;

            if (this.WeaponTimerBar == null)
                return;

            var timerBar = Instantiate(this.WeaponTimerBar, Vector3.zero, Quaternion.identity) as GameObject;
            if(timerBar != null)
                timerBar.GetComponent<TimerBarController>().DisplayTimer(this.gameObject, this.WeaponTimer);
        }
    }
}
