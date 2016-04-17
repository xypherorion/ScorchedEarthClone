using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extra;
using Assets.Scripts.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Player
{
    public class PlayersController : MonoBehaviour
    {
        public CameraBehaviour CameraBehaviour;
        public UiController UiController;
        private List<PlayerBehaviour> _players = new List<PlayerBehaviour>();
        private List<PlayerBehaviour> _playersToTrigger = new List<PlayerBehaviour>();
        private PlayerBehaviour _currentActivePlayer;

        private Action _actionToRun;
        private IEnumerator _timingCoroutine;
        private bool _initializingPlayers;
        private bool _initializingPlayersComplete;
        private bool _gameOver;
        private bool _gamePaused;


        void Start()
        {
            Initialize();
        }

        void Update()
        {
            if (Global.IsGameOver)
            {
                if (this._gameOver)
                    return;

                this._gameOver = true;
                this.CameraBehaviour.Locked = true;
                DisplayMessage(_currentActivePlayer.Name + " has won the game.", overwrite: true);
                StartCoroutine(Util.WaitWithDelegate(3, () =>
                {
                    UiController.DisplayHighscore(this._players);
                }));
                return;
            }

            if (!Global.IsTerrainGenerated)
                return;

            if (!Global.PlayersInitialized && !this._initializingPlayers)
                InitializePlayers();

            if (Global.PlayersInitialized && !Global.PlayersReady)
                SetPlayersReady();

            PauseGame();
            RunAction();
        }

        private void Initialize()
        {
            this.CameraBehaviour = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraBehaviour>();
            this.UiController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UiController>();
            StartCoroutine(this.UiController.DisplayWhenReady(() => this.UiController.DisplayUi(false)));
        }

        private void PauseGame()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Global.IsGamePaused)
                {
                    Time.timeScale = 1;
                    Global.IsGamePaused = false;
                    this._gamePaused = false;
                    this.CameraBehaviour.Locked = false;
                    this.UiController.ConfirmPanelController.IsVisible(false);
                    return;
                }

                if (!this._players.Exists(x => x.State == PlayerState.IsActive))
                {
                    DisplayMessage("You can't pause the game right now.", 1, overwrite: true);
                    return;
                }

                Global.IsGamePaused = true;
                this._gamePaused = true;
                this.CameraBehaviour.Locked = true;
                Time.timeScale = 0;
                this.UiController.ConfirmPanelController.IsVisible(true);
            }
            else if (Global.IsGamePaused != this._gamePaused)
            {
                Time.timeScale = Global.IsGamePaused ? 0 : 1;
                this._gamePaused = Global.IsGamePaused;
                this.CameraBehaviour.Locked = Global.IsGamePaused;
            }
        }

        private void RunAction()
        {
            if (this._actionToRun != null)
                _actionToRun();
        }

        public void ChangePlayerState(PlayerState state, PlayerBehaviour player, bool internalStateChange = false)
        {
            this._actionToRun = null;

            
            if (!internalStateChange)
            {
                if (this._timingCoroutine != null)
                {
                    StopCoroutine(this._timingCoroutine);
                }
                    
            }

            switch (state)
            {
                case PlayerState.IsActive:
                    if (!internalStateChange)
                    {
                        SetTimerCoroutine(Global.CurrentRoundTime, () =>
                        {
                            if (player.State == PlayerState.IsActive || player.State == PlayerState.IsUsingUtility)
                                player.ChangeState(PlayerState.IsDone);
                        });
                    }
                    break;

                case PlayerState.IsUsingUtility:
                    if (player.HasUsedUtility)
                        DisplayMessage("You have already used a utility in this round.", 2);
                    player.ChangeState(PlayerState.IsActive, true, true);
                    break;

                case PlayerState.IsFiring:
                    CameraBehaviour.Locked = true;
                    UiController.TimerController.Freeze = true;

                    foreach (var obj in this._players.Where(x => x.Id != player.Id))
                        obj.SetMovementAllowance(true);

                    this._actionToRun = () =>
                    {
                        if (Global.TrackWeaponPosition)
                            CameraBehaviour.SetTarget(Global.NextProjectileCollision);
                        else if (Global.TrackProjectilePosition && player.WeaponController.ProjectileToTrack != null)
                            CameraBehaviour.SetTarget(player.WeaponController.ProjectileToTrack.transform.position);
                    };
                    break;

                case PlayerState.IsFiringWithTrigger:
                    UiController.TimerController.Freeze = true;

                    foreach (var obj in this._players.Where(x => x.Id != player.Id))
                        obj.SetMovementAllowance(true);
                    break;

                case PlayerState.IsEndingTurn:
                    InitiatePlayerHits(player);
                    break;

                case PlayerState.HasTriggeredHealthLoss:
                    this._playersToTrigger.Remove(player);
                    player.PlayerBeenHit = false;

                    if (player.Id != _currentActivePlayer.Id)
                        player.ChangeState(PlayerState.Idle);

                    if (this._playersToTrigger.Count > 0)
                        this._playersToTrigger.First().ChangeState(PlayerState.HasLostHealth);
                    else
                    {
                        if (player.Id == _currentActivePlayer.Id)
                        {
                            player.ChangeState(PlayerState.IsDone);
                            break;
                        }

                        EndActivePlayerTurn(_currentActivePlayer);
                    }
                    break;

                case PlayerState.IsDone:
                    NextRound();
                    break;

                case PlayerState.IsDead:
                    DisplayMessage(player.Name + " has been killed.", 2);

                    if (_players.Count(x => x.State == PlayerState.IsDead) >= (this._players.Count) - 1)
                    {
                        NextRound();
                        Global.IsGameOver = true;
                        break;
                    }

                    if (this._playersToTrigger.Contains(player))
                    {
                        this._playersToTrigger.Remove(player);
                        player.PlayerBeenHit = false;
                        player.SetMovementAllowance(false);

                        if (this._playersToTrigger.Count > 0)
                            this._playersToTrigger.First().ChangeState(PlayerState.HasLostHealth);
                        else
                        {
                            if (player.Id == _currentActivePlayer.Id)
                            {
                                NextRound();
                                break;
                            }

                            EndActivePlayerTurn(_currentActivePlayer);
                        }
                    }
                        
                    if (player.Id == this._currentActivePlayer.Id)
                    {
                        player.PlayerBeenHit = false;
                        player.SetMovementAllowance(false);
                        NextRound();
                    }
                    break;
            }
        }

        private void InitiatePlayerHits(PlayerBehaviour player)
        {
            StartCoroutine(Util.WaitWithDelegate(1, () =>
            {
                var players = Global.PlayersHit.Count > 0 ? Global.PlayersHit.Count(x => x.State != PlayerState.IsDead) : 0;
                var playersHit = _players.Where(x => x.PlayerBeenHit && x.State != PlayerState.IsDead).ToArray();
                if (playersHit.Count() != players || playersHit.Any(x => !x.FallDamageCalculationComplete()))
                {
                    Debug.Log(playersHit.Count() + " and " + players);
                    InitiatePlayerHits(player);
                    return;
                }
                SetPlayerHits(player);
            }));
        }

        private void SetPlayerHits(PlayerBehaviour player)
        {
            foreach (var obj in this._players.Where(x => x.Id != player.Id))
            {
                if (!obj.PlayerBeenHit || obj.State == PlayerState.IsDead)
                    continue;

                this._playersToTrigger.Add(obj);
            }


            if (player.PlayerBeenHit)
                this._playersToTrigger.Add(player);

            if (this._playersToTrigger.Count > 0)
                this._playersToTrigger.First().ChangeState(PlayerState.HasLostHealth);
            else
            {
                StartCoroutine(Util.WaitWithDelegate(1, () =>
                {
                    EndActivePlayerTurn(player);
                }));
            }
        }

        private void EndActivePlayerTurn(PlayerBehaviour player)
        {
            CameraBehaviour.SetTarget(player.transform.position);

            foreach (var obj in this._players.Where(x => x.Id != player.Id))
                obj.SetMovementAllowance(false);

            CameraLock();
            player.AllowMovement = true;

            DisplayMessage("Your turn is ending in 3 seconds.", 2);
            SetRoundTime(4);
            SetTimerCoroutine(4, () =>
            {
                if (player.State == PlayerState.IsEndingTurn)
                    player.ChangeState(PlayerState.IsDone);
            });
        }

        private void NextRound(int nextPlayer = 0)
        {
            Global.IsInventoryOpen = false;
            SetRoundTime(Global.RoundTime);
            SetNextActivePlayer(nextPlayer);
        }

        private void SetNextActivePlayer(int currentPlayerId = 0)
        {
            CameraLock();
            this._playersToTrigger = new List<PlayerBehaviour>();
            Global.PlayersHit = new List<PlayerBehaviour>();
            currentPlayerId = currentPlayerId > 0 ? currentPlayerId : this._currentActivePlayer.Id;
            var rearrangeList = this._players.Where(x => x.Id > currentPlayerId).OrderBy(x => x.Id).ToList();
            rearrangeList.AddRange(this._players.Where(x => x.Id <= currentPlayerId).OrderBy(x => x.Id));

            bool firstIteration = true;
            foreach (PlayerBehaviour player in rearrangeList)
            {
                if (player.State == PlayerState.IsDead)
                    continue;

                if (firstIteration)
                {
                    if(!Global.IsGameOver)
                        DisplayMessage("Your turn, " + player.Name + "!", 2);

                    this._currentActivePlayer = player;
                    Global.CurrentPlayerId = player.Id;
                    Global.CurrentPlayer = player;
                    player.ChangeState(PlayerState.IsActive);
                    player.SetMovementAllowance(true);
                    player.SetPlayerBar(false);

                    CameraBehaviour.SetObjectToTrack(player.gameObject);
                    CameraBehaviour.SetTarget(player.gameObject.transform.position);

                    firstIteration = false;
                }
                else
                {
                    player.State = PlayerState.Idle;
                    player.SetMovementAllowance(false);
                    player.SetPlayerBar(true);
                }

                player.PlayerBeenHit = false;
            }
        }

        private void SetRoundTime(int seconds)
        {
            Global.CurrentRoundTime = seconds;
            UiController.TimerController.Freeze = false;
        }

        private void SetTimerCoroutine(int maxTime, Action toDo)
        {
            this._timingCoroutine = Util.WaitWithTicksEndByDelegate(1f, () =>
            {
                Global.CurrentRoundTime = Global.CurrentRoundTime - 1;
                if (Global.CurrentRoundTime < 1)
                {
                    toDo();
                    return true;
                }
                return false;
            }, maxTime);
            StartCoroutine(this._timingCoroutine);
        }

        private void StartGame()
        {
            StartCoroutine(this.UiController.DisplayWhenReady(() => this.UiController.DisplayUi(true), () => NextRound(Random.Range(1, this._players.Count))));
        }

        private void SetPlayersReady()
        {
            if (!Global.PlayersInitialized || Global.PlayersReady)
                return;

            if (!this._initializingPlayersComplete && this._players.Any(player => !player.IsGroundedLong()))
                return;

            if (this._players.Any(player => player.RigidBody.velocity != Vector2.zero))
                return;

            Global.PlayersReady = true;
            StartGame();
        }

        private void InitializePlayers()
        {
            if (Global.PlayersInitialized || this._initializingPlayers)
                return;

            this._initializingPlayers = true;
            Global.InitializeGameProgressStatusText = "Initializing players.";
            var initialProgress = (1 - Global.InitializeGameProgress);
            var players = GameObject.FindGameObjectsWithTag("Player");
            var totalPlayers = players.Count();
            var currentPlayers = new List<GameObject>();

            int i = 1;
            foreach (var player in players)
            {
                SetPlayerPosition(player, currentPlayers, 2f);
                var playerBehaviour = player.GetComponent<PlayerBehaviour>();
                playerBehaviour.Id = i;
                playerBehaviour.PlayersController = this;

                currentPlayers.Add(player);
                this._players.Add(playerBehaviour);
                Util.AssignObjectParent("Players", player);
                Global.InitializeGameProgress = Global.InitializeGameProgress + (initialProgress / totalPlayers);
                i++;
            }

            StartCoroutine(Util.WaitWithDelegate(1.5f, () =>
            {
                this._initializingPlayersComplete = true;
            }));
            Global.PlayersInitialized = true;
        }

        private void SetPlayerPosition(GameObject player, List<GameObject> players, float minDistance)
        {
            float x = 0;
            var count = 1;

            while (count > 0)
            {
                x = Random.Range(2.0f + Global.TerrainNullPoint.x, Global.TerrainEndPoint.x - 2.0f);
                count = players.Count(g => g.transform.position.x < x + minDistance && g.transform.position.x > x - minDistance);
            }


            var collisions = true;
            var position = new Vector3(x, 1);
            while (collisions)
            {
                List<Vector3> directions = new List<Vector3>() { Vector3.down, Vector3.up, Vector3.left, Vector3.right };
                foreach (var direction in directions)
                {
                    var size = direction.y > 0 || direction.y < 0 ? player.transform.localScale.y : player.transform.localScale.x;
                    if (Physics2D.Raycast(position, direction, size * 3f))
                    {
                        position.y += 2;
                        collisions = true;
                        break;
                    }
                    collisions = false;
                }
            }
            player.transform.position = position;
        }

        private void CameraLock(bool enable = false)
        {
            if (Global.IsGameOver)
                return;

            CameraBehaviour.Locked = enable;
        }

        public void DisplayMessage(string message, int seconds = 0, bool overwrite = false)
        {
            UiController.MessageController.DisplayMessage(message, seconds, overwrite);
        }
    }
}
