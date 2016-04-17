using System.Collections.Generic;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Extra
{
    public class Global
    {
        public static float InitializeGameProgress { get; set; }

        public static string InitializeGameProgressStatusText { get; set; }

        public static bool IsTerrainGenerated { get; set; }

        public static int TerrainSize { get; set; }

        public static Vector3 TerrainNullPoint { get; set; }

        public static Vector3 TerrainEndPoint { get; set; }

        public static decimal TerrainRatio { get; set; }

        public static bool PlayersInitialized { get; set; }

        public static bool PlayersReady { get; set; }

        public static Vector3 NextProjectileCollision { get; set; }

        public static bool TrackWeaponPosition { get; set; }

        public static bool TrackProjectilePosition { get; set; }

        public static bool IsGameOver { get; set; }

        public static bool IsGamePaused { get; set; }

        public static bool IsInventoryOpen { get; set; }

        public static int CurrentPlayerId { get; set; }

        public static PlayerBehaviour CurrentPlayer { get; set; }

        public static int RoundTime { get; set; }

        public static int CurrentRoundTime { get; set; }

        public static int TimerDelay = 3;

        public static List<PlayerBehaviour> PlayersHit = new List<PlayerBehaviour>();

        public static bool LOL { get; set; }

        public static void ResetGame()
        {
            InitializeGameProgress = 0;
            IsTerrainGenerated = false;
            PlayersInitialized = false;
            PlayersReady = false;
            IsGameOver = false;
            IsGamePaused = false;
        }
    }
}
