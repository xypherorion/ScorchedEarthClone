using System;
using System.Linq;
using Assets.Scripts.Extra;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class GameSettingsController : MonoBehaviour
    {
        public float AnimationSpeed;
        public InputField InputPrefab;
        public GameObject PlayerPrefab;
        public GameObject NamePanel;
        public Dropdown PlayersDropdown;
        public Dropdown GameModeDropdown;
        public Dropdown MapSizeDropdown;
        public Dropdown RoundTimeDropDown;
        public Dropdown MoneyDropdown;
        public Button ProceedButton;

        public Text GameModeInfo;

        private float _alpha;
        private CanvasGroup _cg;
        private int _formerPlayers = 0;
        private int _players = 0;
        private int _formerNpcs = 0;
        private int _npcs = 0;
        private InputField[] _names;
    

        private void Start ()
        {
            ProceedButton.interactable = false;
            _names = new InputField[10];
            _cg = GetComponent<CanvasGroup>();
            AssignDefaultValues();
        }
	
        private void Update ()
        {
            if (Mathf.Abs(_cg.alpha - _alpha) > 0.0001)
            {
                _cg.alpha = Mathf.Lerp(_cg.alpha, _alpha, AnimationSpeed);
            }
        }

        public void IsVisible(bool visible)
        {
            _alpha = visible ? 1.0f : 0.0f;
            _cg.interactable = visible;
            _cg.blocksRaycasts = visible;
        }

        public void Proceed()
        {
            InstantiatePlayers();
            LoadScene();
        }

        public void CanStartGame(string change)
        {
            ProceedButton.interactable = (NameListComplete() && (_players + _npcs) > 1);
        }

        public void UpdateNumberOfPlayers()
        {
            _formerPlayers = _players;
            _players = PlayersDropdown.value;
            UpdateNameList();
            CanStartGame("");
        }

        private int GetMoney(int value)
        {
            switch (value)
            {
                case 1:
                    return 250;
                case 2:
                    return 500;
                case 3:
                    return 1000;
                case 4:
                    return 2500;
                case 5:
                    return 5000;
                case 6:
                    return 10000;
                default:
                    return 0;
            }
        }

        public void ChangedMapSize()
        {
            switch (MapSizeDropdown.value)
            {
                case 0:
                    Global.TerrainSize = 1000;
                    break;
                case 1:
                    Global.TerrainSize = 2000;
                    break;
                case 2:
                    Global.TerrainSize = 3000;
                    break;
                default:
                    Global.TerrainSize = 1000;
                    break;
            }
        }

        public void ChangedGameMode()
        {
            string text = "";
            switch (GameModeDropdown.value)
            {
                case 0:
                    text = 
                        "Classic game mode gives you\na standard PvP experience,\nwith varying terrains as well as\na wide selection of weapons and powerups";
                    break;
                case 1:
                    text =
                        "Overkill gives you an\nexplosive PvP experince,\ncomplete with overpowered weapons\nand nasty surprises!";
                    break;
                case 2:
                    text =
                        "With Poisoned mode, no player will remain safe!\n The air you breathe is toxic\nand your health slowly decreases\nwhile time passes by!\nDeath is inevitable!";
                    break;
                default:
                    text = "An error occurred";
                    break;
            }

            GameModeInfo.text = text;
        }

        public void ChangedRoundTime()
        {
            switch (RoundTimeDropDown.value)
            {
                case 0:
                    Global.RoundTime = 30;
                    break;
                case 1:
                    Global.RoundTime = 45;
                    break;
                case 2:
                    Global.RoundTime = 60;
                    break;
                default:
                    Global.RoundTime = 30;
                    break;
            }
        }

        private void AssignDefaultValues()
        {
            Global.RoundTime = 30;
            Global.TerrainSize = 2000;
        }

        private bool NameListComplete()
        {
            return _names.Where(input => input != null).All(input => !String.IsNullOrEmpty(input.text));
        }

        private void UpdateNameList()
        {
            if (_players > _formerPlayers)
            {
                AddNameTags();
            }
            else if (_players < _formerPlayers)
            {
                RemoveNameTags();
            }
        }

        private void RemoveNameTags()
        {
            int diff = _formerPlayers - _players;
            for (int i = 0; i < diff; i++)
            {
                InputField input = _names[_players + i];
                _names[_players + i].transform.SetParent(null);
                _names[_players + i] = null;
            
                Destroy(input);
            }
        }

        private void AddNameTags()
        {
            int diff = _players - _formerPlayers;
            for (int i = 0; i < diff; i++)
            {
                InputField input = Instantiate(InputPrefab);
                input.onValueChanged.AddListener(CanStartGame);
                input.transform.SetParent(NamePanel.transform, false);
                input.GetComponent<RectTransform>().localScale = Vector3.one;
                _names[_formerPlayers + i] = input;
            }
        }

        private void InstantiatePlayers()
        {
            for (int i = 0; i < _players; i++)
            {
                var p = Instantiate(PlayerPrefab);
                var pb = p.GetComponent<PlayerBehaviour>();
                pb.Name = _names[i].text;
                pb.State = PlayerState.Inactive;
                pb.Score.Money = GetMoney(MoneyDropdown.value);
                DontDestroyOnLoad(p);
            }
        }

        private void LoadScene()
        {
            var scene = MoneyDropdown.value == 0 ? "MainScene" : "ShopScene";
            Global.ResetGame();
            SceneManager.LoadScene(scene);
        }
    }
}
