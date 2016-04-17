using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extra;
using Assets.Scripts.UI;
using Assets.Scripts.Weapon;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerBehaviour : MonoBehaviour
    {
        public int Id;
        public float Health;
        public float MaxHealth;
        public string Name;
        public PlayerType Type;
        public PlayerState State;

        public Stats Stats;
        public Score Score;
        public bool AllowMovement;
        public bool PlayerBeenHit;
        public bool HasUsedUtility;

        private Stats OriginalStats { get; set; }
        private List<Hit> Hits = new List<Hit>();
        private float FallHealthLoss { get; set; }
        private bool HasCalculatedFallDamage { get; set; }
        private bool IsFalling { get; set; }

        private Vector3 _lastPosition;
        private bool _isFirstIteration = true;
        
        public WeaponController WeaponController { get; set; }
        public PowerStatsController PowerStatsController { get; set; }
        public PlayerbarController PlayerbarController { get; set; }
        public UtilityUsedController UtilityUsedController { get; set; }
        public PlayersController PlayersController { get; set; }

        public Rigidbody2D RigidBody { get; set; }
        private Action ActionToRun { get; set; }

        private void Awake()
        {
            this.WeaponController = GetComponent<WeaponController>();
            this.PlayerbarController = GetComponentInChildren<PlayerbarController>();
            this.PowerStatsController = GetComponentInChildren<PowerStatsController>();
            this.UtilityUsedController = GetComponentInChildren<UtilityUsedController>();
            this.RigidBody = GetComponent<Rigidbody2D>();
            this.OriginalStats = new Stats(this.Stats);
        }

        private void Update()
        {
            if (Global.IsGameOver || !Global.PlayersReady || Global.IsGamePaused)
                return;

            if(this.ActionToRun != null)
                ActionToRun();

            switch (this.State)
            {
                case PlayerState.Idle:
                    this._isFirstIteration = true;
                    ResetTurn();
                    ToDoIfMoved();
                    break;

                case PlayerState.HasLostHealth:
                    ToDoIfMoved();
                    if (FallDamageCalculationComplete())
                    {
                        IsFirstIteration(() => { this.ActionToRun = RemoveHealth; });
                        this.HasCalculatedFallDamage = false;
                    }
                    break;

                case PlayerState.IsActive:
                    ShowPlayerHud(true);
                    PlayerMovement();
                    ToDoIfMoved();
                    WeaponDirection();
                    WeaponForce();
                    UseWeapon();
                    break;

                case PlayerState.IsUsingUtility:
                    break;

                case PlayerState.IsFiring:
                    WeaponController.IsCurrentlyFiring();
                    break;

                case PlayerState.IsFiringWithTrigger:
                    WeaponController.IsCurrentlyFiring(true);
                    PlayerMovement();
                    ToDoIfMoved();
                    WeaponDirection();
                    UseWeapon(false);
                    break;

                case PlayerState.IsEndingTurn:
                    this._isFirstIteration = true;
                    ToDoIfMoved();
                    if(this.AllowMovement)
                        PlayerMovement();
                    break;

                case PlayerState.IsDone:
                    break;

                case PlayerState.IsDead:
                    this.PlayerBeenHit = false;
                    break;
            }
        }

        public void ChangeState(PlayerState state, bool displayHud = false, bool internalStateChange = false)
        {
            ShowPlayerHud(displayHud, state);
            this.AllowMovement = false;
            this.State = state;
            this.PlayersController.ChangePlayerState(state, this, internalStateChange);
        }

        private void UseWeapon(bool startFire = true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!startFire)
                    WeaponController.StopFire();
                else
                    WeaponController.Fire();
            }
        }

        public void ShowPlayerHud(bool enable, PlayerState state = PlayerState.IsActive)
        {
            enable = WeaponController.WeaponInventory.CurrentWeaponInstance.DisplayHud && enable;
            this.PowerStatsController.IsVisible = enable;

            if(state != PlayerState.IsActive && state != PlayerState.IsUsingUtility)
                WeaponController.SetTrajectory(enable);
        }

        private void WeaponDirection()
        {
            if (Math.Abs(Input.GetAxis("Vertical")) > 0)
            {
                var direction = Input.GetAxis("Vertical") > 0 ? Direction.Up : Direction.Down;
                WeaponController.ChangeDirection(direction);
                WeaponController.InitializeShot();
            }
        }

        private void WeaponForce()
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                var direction = Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.KeypadPlus) ? Direction.Up : Direction.Down;
                WeaponController.ChangePower(direction);
                WeaponController.InitializeShot();
            }
        }

        private void PlayerMovement()
        {
            transform.position += new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * this.Stats.MovementSpeed, 0, 0);

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                if (IsGrounded())
                {
                    this.RigidBody.velocity = Vector3.zero;
                    this.RigidBody.AddForce(new Vector3(0, this.Stats.JumpForce));
                }  
        }


        private float HandleHits()
        {
            var hit = this.Hits.FirstOrDefault();
            if (hit == null)
                return 0;

            var hitOwner = hit.Sender.Owner;
            float totalHealthLoss = 0;

            foreach (var obj in this.Hits)
                totalHealthLoss += obj.Sender.CalculateDamage(obj.TargetPosition);
                

            if (totalHealthLoss > 0)
            {
                hitOwner.Score.Money = hitOwner.Id != this.Id ? hitOwner.Score.Money + (((int)totalHealthLoss) * 10) : hitOwner.Score.Money - (((int)totalHealthLoss) * 10) >= 0 ? hitOwner.Score.Points - (((int)totalHealthLoss) * 10) : 0;
                hitOwner.Score.Points = hitOwner.Id != this.Id ? hitOwner.Score.Points + (((int)totalHealthLoss) * 15) : hitOwner.Score.Points - (((int)totalHealthLoss) * 15) >= 0 ? hitOwner.Score.Points - (((int)totalHealthLoss) * 15) : 0;
            }
            return totalHealthLoss;
        }

        private void HandleHitKill()
        {
            var hit = this.Hits.FirstOrDefault();
            if (hit == null)
                return;
            
            var hitOwner = hit.Sender.Owner;
            if (hitOwner.Id != this.Id)
            {
                hitOwner.Score.Money += 1500;
                hitOwner.Score.Points += 2250;
                hitOwner.Score.Kills += 1;
            }
        }

        private void RemoveHealth()
        {
            if (IsGroundedLong() && this.RigidBody.velocity == Vector2.zero)
            {
                this.ActionToRun = null;

                if(!this.PlayerbarController.IsVisible)
                    SetPlayerBar(true);

                PlayersController.CameraBehaviour.SetTarget(transform.position);
                StartCoroutine(Util.WaitWithDelegateContinueWithDelegate(1, () =>
                {
                    var totalHealthLoss = HandleHits();
                    this.Health = this.Health - (totalHealthLoss + this.FallHealthLoss) < 1 ? 0 : this.Health - (totalHealthLoss + this.FallHealthLoss);
                    this.FallHealthLoss = 0;
                }, 1, () =>
                {
                    if (this.Health < 1)
                    {
                        HandleHitKill();
                        KillPlayer();
                        return;
                    }
                    this.Hits.Clear();
                    ChangeState(PlayerState.HasTriggeredHealthLoss);
                    this._isFirstIteration = true;
                }));
            }
        }

        public bool FallDamageCalculationComplete()
        {
            Debug.Log(this.FallHealthLoss + " and " + this.HasCalculatedFallDamage + " and " + this.RigidBody.velocity + " and " + IsGroundedLong());
            bool check1 = (this.FallHealthLoss > 0 && this.HasCalculatedFallDamage);
            bool check2 = this.FallHealthLoss < 1;

            if (check1 || check2)
            {
                if (this.RigidBody.velocity == Vector2.zero)
                {
                    return true;
                }
            }
            return false;
        }

        private void CalculateFallDamage()
        {
            if (Vector3.Magnitude(this.RigidBody.velocity) > (6 * this.Stats.FallFactor))
            {
                if (!this.HasCalculatedFallDamage && !IsGroundedLong())
                {
                    this.IsFalling = true;
                    var healthLoss = ((Mathf.CeilToInt(Vector3.Magnitude(this.RigidBody.velocity)) - 5)*3 > 25 ? 25 : (Mathf.CeilToInt(Vector3.Magnitude(this.RigidBody.velocity)) - 5)*3);
                    if (this.FallHealthLoss < healthLoss)
                        this.FallHealthLoss = (float)healthLoss*(2-this.Stats.FallFactor);
                }
            }

            if (this.IsFalling && IsGroundedLong())
            {
                this.IsFalling = false;
                this.HasCalculatedFallDamage = true;
            }
        }

        private void KillPlayer()
        {
            WeaponController.StopFire(true);
            ChangeState(PlayerState.IsDead);
        }

        private void IsFirstIteration(Action toDo)
        {
            if (this._isFirstIteration)
            {
                toDo();
                this._isFirstIteration = false;
            }
        }

        public void SetMovementAllowance(bool enable)
        {
            this.RigidBody.mass = enable ? 1 : float.MaxValue;
            this.RigidBody.isKinematic = true;
            this.RigidBody.isKinematic = false;
        }

        public void SetPlayerBar(bool enable)
        {
            this.PlayerbarController.IsVisible = enable;
        }

        private bool IsMoving()
        {
            bool isMoving = transform.position != this._lastPosition;
            this._lastPosition = transform.position;
            return isMoving;
        }

        private void ToDoIfMoved()
        {
            if (IsMoving())
            {
                if (Util.OutOfBounds(transform.position))
                {
                    HandleHitKill();
                    KillPlayer();
                    return;
                }

                CalculateFallDamage();

                if (this.State != PlayerState.Idle && this.State != PlayerState.HasLostHealth)
                {
                    if(this.State != PlayerState.IsEndingTurn)
                        WeaponController.InitializeShot();


                    if (this.HasCalculatedFallDamage && IsGroundedLong() && !this.PlayerBeenHit)
                    {
                        this.ChangeState(PlayerState.HasLostHealth);
                    }
                }
            }
        }

        public bool IsGrounded()
        {
            if (Physics2D.Raycast(transform.position, Vector3.down, transform.localScale.y * 1.5f) || Physics2D.Raycast(new Vector2(transform.position.x - (transform.localScale.x / 2), transform.position.y), Vector3.down, transform.localScale.y * 1.5f) || Physics2D.Raycast(new Vector2(transform.position.x + (transform.localScale.x / 2), transform.position.y), Vector3.down, transform.localScale.y * 1.5f))
                return true;
            return false;
        }

        public bool IsGroundedLong()
        {
            if (Physics2D.Raycast(transform.position, Vector3.down, transform.localScale.y * 5f) || Physics2D.Raycast(new Vector2(transform.position.x - (transform.localScale.x / 1.5f), transform.position.y), Vector3.down, transform.localScale.y * 5f) || Physics2D.Raycast(new Vector2(transform.position.x + (transform.localScale.x / 1.5f), transform.position.y), Vector3.down, transform.localScale.y * 5f))
                return true;
            return false;
        }

        public void HasBeenHit(Hit hit)
        {
            if (this.State != PlayerState.IsDead)
            {
                hit.TargetPosition = transform.position;
                this.Hits.Add(hit);
            }  

            hit.Sender.AddPushbackForce(this.RigidBody);
            this.PlayerBeenHit = true;

        }

        public void ResetTurn()
        {
            if (this.HasUsedUtility)
            {
                this.Stats = new Stats(OriginalStats);
                this.HasUsedUtility = false;
            }
            this.WeaponController.WeaponInventory.ChangeCurrentWeapon(this.WeaponController.WeaponInventory.LastUsedWeaponType);
        }

        public void SetActive(bool enable)
        {
            if (enable)
            {
                this.RigidBody.isKinematic = false;
                GetComponent<Renderer>().enabled = true;
                return;
            }

            this.State = PlayerState.Inactive;
            this.Health = this.MaxHealth;
            this.Stats = new Stats(OriginalStats);
            this.HasUsedUtility = false;
            this.PlayerBeenHit = false;
            this.Hits = new List<Hit>();
            this.FallHealthLoss = 0;
            this.HasCalculatedFallDamage = false;
            this.IsFalling = false;
            this.RigidBody.isKinematic = true;
            this.transform.position = new Vector3(-200, -200);
            GetComponent<Renderer>().enabled = false;
        }
    }
}