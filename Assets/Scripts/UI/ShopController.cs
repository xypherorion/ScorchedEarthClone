using System;
using System.Collections.Generic;
using Assets.Scripts.Extra;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.UI;
using Assets.Scripts.Weapon;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Assets.Scripts.UI
{
    public class ShopController : MonoBehaviour
    {
        public Button ItemButtonPrefab;
        public Text NameText;
        public Text MoneyText;
        public GameObject WeaponPanel;
        public GameObject PowerupPanel;
        public GameObject DetailsPanel;
        public Image DetailsImage;
        public Button PreviousButton;
        public Button NextButton;
        public Button BuyButton;
        public Button MainMenuButton;
        public GameObject ConfirmPanel;

        private PlayerBehaviour[] _players;
        private PlayerBehaviour _currentPlayer;
        private int _currentPlayerId = 0;
        private List<Item> _items;
        private Item _currentItem;
        private Sprite[] _sprites;
        private MainMenuController _mmc;


        void Awake ()
        {
            InitVars();
            GetPlayers();
            SetCredentials();
            GenerateItems();
            DisplayDefaultDetails();
            SetNavigationButtons();
            UpdateButtons();
        }

        public void ReturnToMainMenu()
        {
            _mmc.IsVisible(true);
        }

        public void ReturnToGame()
        {
            _mmc.IsVisible(false);
        }

        public void ActualReturnToMainMenu()
        {
            foreach(var player in this._players)
                Destroy(player.gameObject);
            SceneManager.LoadScene("MainMenuScene");
        }

        public void Buy()
        {
            _currentPlayer.Score.Money -= _currentItem.Weapon.WeaponPrice;
            _currentPlayer.WeaponController.WeaponInventory.AddToWeaponQuantity(_currentItem.Type);
            UpdateButton(_currentItem);
            UpdateMoney();
            BuyButton.interactable = _currentItem.Weapon.WeaponPrice <= _currentPlayer.Score.Money;
        }

        private void GetPlayers()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            _players = new PlayerBehaviour[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                _players[i] = players[i].GetComponent<PlayerBehaviour>();
            }

            _currentPlayer = _players[_currentPlayerId];
            _currentPlayerId = 0;
        }

        private void SetCredentials()
        {
            NameText.text =  GetBanter() + _currentPlayer.Name + "!";
            UpdateMoney();
        }

        private void UpdateMoney()
        {
            MoneyText.text = "$" + _currentPlayer.Score.Money;
        }

        private string GetBanter()
        {
            string[] banter = new[]
            {
                "Time to stock up, ",
                "Time to spend some cash, ",
                "Need ammo? Look no further, ",
                "Get your guns, ",
            };

            return banter[Random.Range(0, banter.Length)];
        }

        private void GenerateItems()
        {
            int itemCount = Enum.GetValues(typeof(WeaponType)).Length;
            
            for (int i = 0; i < itemCount; i++)
            {
                var itemGameObject = Instantiate(Util.LoadWeapon((WeaponType)i), new Vector3(0, 0), Quaternion.identity) as GameObject;
                var weapon = (IWeapon)itemGameObject.GetComponent(typeof(IWeapon));
                bool utility = weapon is IUtility || weapon is ITool;
                GenerateButton((WeaponType)i, weapon, utility, _sprites[i]);
                Destroy(itemGameObject.gameObject);
            }
        }



        private void GenerateButton(WeaponType type, IWeapon weapon, bool isUtility, Sprite sprite)
        {
            var panel = isUtility ? PowerupPanel : WeaponPanel;

            Button b = Instantiate(ItemButtonPrefab);
            Text[] t = b.GetComponentsInChildren<Text>();
            t[0].text = weapon.WeaponName;
            t[1].text = _currentPlayer.WeaponController.WeaponInventory.GetWeaponQuantity(type).ToString();
            t[2].text = "$" + weapon.WeaponPrice;

            b.transform.SetParent(panel.transform);
            b.GetComponent<RectTransform>().localScale = Vector3.one;

            Item item = new Item(type, weapon, b, sprite);

            b.onClick.AddListener(()=>DisplayDetails(item));
            _items.Add(item);
            
        }

        private void UpdateButton(Item item)
        {
            Text[] t = item.Button.GetComponentsInChildren<Text>();
            t[1].text = _currentPlayer.WeaponController.WeaponInventory.GetWeaponQuantity(item.Type).ToString();
        }

        private void UpdateButtons()
        {
            foreach (Item i in _items)
            {
                UpdateButton(i);
            }
        }

        private void DisplayDefaultDetails()
        {
            _currentItem = _items[0];
            DisplayDetails(_currentItem);
            BuyButton.interactable = _items[0].Weapon.WeaponPrice <= _currentPlayer.Score.Money;
        }

        private void DisplayDetails(Item item)
        {
            _currentItem = item;

            string damageMessage = "N/A";
            if (!item.IsUtility)
            {
                if (item.Weapon is IShooter)
                {
                    var i = item.Weapon as IShooter;
                    damageMessage = "Damage: " + i.Damage;
                }
                else
                {
                    var i = item.Weapon as IMelee;
                    damageMessage = "Damage: " + i.Damage;
                }
            }
            
            DetailsImage.sprite = item.Sprite;

            Text[] t = DetailsPanel.GetComponentsInChildren<Text>();
            t[1].text = item.Weapon.WeaponName;
            t[2].text = damageMessage;
            t[3].text = "$" + item.Weapon.WeaponPrice;
            t[4].text = item.Weapon.WeaponDescription.Replace("\\n", "\n");

            BuyButton.interactable = item.Weapon.WeaponPrice<=_currentPlayer.Score.Money;
        }

        public void NextPlayer()
        {
            if (_currentPlayer == _players[_players.Length-1])
            {
                PreservePlayers();
                SceneManager.LoadScene("MainScene");
            }
            else
            {
                _currentPlayerId++;
                _currentPlayer = _players[_currentPlayerId];
                SetNavigationButtons();
                SetCredentials();
                UpdateButtons();
                if(_currentItem!=null) BuyButton.interactable = _currentItem.Weapon.WeaponPrice <= _currentPlayer.Score.Money;
            }
        }

        public void PreviousPlayer()
        {
                _currentPlayerId--;
                _currentPlayer = _players[_currentPlayerId];
                SetNavigationButtons();
                SetCredentials();
                UpdateButtons();
                if (_currentItem != null) BuyButton.interactable = _currentItem.Weapon.WeaponPrice <= _currentPlayer.Score.Money;
        }

        private void SetNavigationButtons()
        {
            PreviousButton.interactable = _currentPlayerId > 0;
            NextButton.GetComponentInChildren<Text>().text = _currentPlayerId == _players.Length - 1 ? "FIGHT!" : "NEXT";
        }

        private void PreservePlayers()
        {
            foreach (PlayerBehaviour p in _players)
            {
                p.SetActive(true);
                DontDestroyOnLoad(p.gameObject);
            }
            Global.ResetGame();
        }

        private void InitVars()
        {
            _sprites = Resources.LoadAll<Sprite>("Sprites/Inventory/");
            _items = new List<Item>();
            _mmc = ConfirmPanel.GetComponent<MainMenuController>();
        }

    }

    
}
