using System;
using System.Collections.Generic;
using Assets.Scripts.Extra;
using Assets.Scripts.Player;
using Assets.Scripts.Weapon;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class InventoryController : MonoBehaviour, IUiControl
    {
        public float AnimationSpeed;
        public Button ButtonPrefab;
        public GameObject WeaponsGrid;
        public GameObject UtilityGrid;
        public GameObject WeaponDescription;
        private WeaponDescriptionController _weaponDescriptionController;

        private List<PlayerBehaviour> _players = new List<PlayerBehaviour>();

        private CanvasGroup _canvasGroup;
        private Button[] _buttons;
        private Sprite[] _sprites;

        private float _alpha = 0.0f;
        private bool _playersLoaded;
        private bool _hasUpdated;
        

        void Start()
        {
            Initialize();
            GenerateButtons();
        }

        void Update()
        {
            if (!_playersLoaded)
                InitializePlayers();

            if (!IsActivePlayer())
                Global.IsInventoryOpen = false;

            if (Input.GetMouseButtonUp(1) && IsActivePlayer())
            {
                UpdateInventory();
                Global.IsInventoryOpen = !Global.IsInventoryOpen;
            }
            else
            {
                CheckWhileOpen();
            }
            FadeLogic();
        }

        private void Initialize()
        {
            _sprites = Resources.LoadAll<Sprite>("Sprites/Inventory/");
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = _alpha;
            _canvasGroup.interactable = false;
            _weaponDescriptionController = this.WeaponDescription.GetComponent<WeaponDescriptionController>();
        }

        private void InitializePlayers()
        {
            if (_playersLoaded)
                return;

            if (Global.PlayersInitialized)
            {
                var players = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in players)
                    _players.Add(player.GetComponent<PlayerBehaviour>());
                _playersLoaded = true;
            }
        }

        private void FadeLogic()
        {
            _canvasGroup.interactable = Global.IsInventoryOpen;
            _alpha = Global.IsInventoryOpen ? 1.0f : 0.0f;
            _canvasGroup.alpha = Mathf.Clamp(Mathf.Lerp(_canvasGroup.alpha, _alpha, AnimationSpeed), 0.0f, 1.0f);
        }

        private void GenerateButtons()
        {
            var values = Enum.GetValues(typeof(WeaponType));
            _buttons = new Button[values.Length];
            IterateItemEnum(0, values.Length - 9, this.WeaponsGrid);
            IterateItemEnum(values.Length - 9, values.Length, this.UtilityGrid);
            AssignButtonDelegates();
        }

        private void IterateItemEnum(int from, int to, GameObject parent)
        {
            for (int i = from; i < to; i++)
            {
                Button button = Instantiate(ButtonPrefab);
                button.image.sprite = _sprites[i];
                button.transform.SetParent(parent.transform, false);
                var rect = button.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;

                var weapon = Instantiate(Util.LoadWeapon((WeaponType)i), new Vector3(-100, -100), Quaternion.identity) as GameObject;

                var weaponName = weapon != null ? ((IWeapon) weapon.GetComponent(typeof (IWeapon))).WeaponName : "Unknown";
                Destroy(weapon);
                AddEventTrigger(button, rect, weaponName);

                _buttons[i] = button;
            }
        }

        private void AssignButtonDelegates()
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                int index = i;
                _buttons[i].onClick.AddListener(() => AssignWeaponFromInventory(index));
                _buttons[i].onClick.AddListener(() => UpdateInventory(index));
            }

        }

        public void UpdateButtonState(WeaponType weapon, bool isAvailable)
        {
            _buttons[(int)weapon].interactable = isAvailable;
        }

        public void UpdateButtonState(int weaponIndex, bool isAvailable, int amountAvailable)
        {
            _buttons[weaponIndex].interactable = isAvailable;
            _buttons[weaponIndex].GetComponentInChildren<Text>().text = amountAvailable.ToString();
        }

        public void AssignWeaponFromInventory(int weaponType)
        {
            if (!IsActivePlayer())
                return;

            Global.CurrentPlayer.WeaponController.WeaponInventory.ChangeCurrentWeapon((WeaponType) weaponType);
            Global.IsInventoryOpen = false;
        }

        private void UpdateInventory(int weaponType)
        {
            if (!IsActivePlayer())
                return;

            var weaponInventory = Global.CurrentPlayer.WeaponController.WeaponInventory;
            UpdateButtonState(weaponType, weaponInventory.HasWeaponInInventory((WeaponType)weaponType), weaponInventory.GetWeaponQuantity((WeaponType)weaponType));
        }

        private void UpdateInventory()
        {
            if (!IsActivePlayer())
                return;

            var weapons = Enum.GetValues(typeof(WeaponType));
            var weaponInventory = Global.CurrentPlayer.WeaponController.WeaponInventory;
            foreach (int i in weapons)
                UpdateButtonState(i, weaponInventory.HasWeaponInInventory((WeaponType)i), weaponInventory.GetWeaponQuantity((WeaponType)i));
        }

        private bool IsActivePlayer()
        {
            if (Global.CurrentPlayer == null)
                return false;

            return Global.CurrentPlayer.State == PlayerState.IsActive;
        }

        private void CheckWhileOpen()
        {
            if (Global.IsInventoryOpen)
            {
                if(!Global.CurrentPlayer.HasUsedUtility)
                    this._hasUpdated = false;

                if (IsActivePlayer() && Global.CurrentPlayer.HasUsedUtility && !this._hasUpdated)
                {
                    UpdateInventory();
                    this._hasUpdated = true;
                }
            }
        }

        private void AddEventTrigger(Button button, RectTransform rect, string text)
        {
            EventTrigger trigger = button.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((eventData) => { this.ShowWeaponDescription(button, rect, true, text); });
            trigger.triggers.Add(entry);

            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.eventID = EventTriggerType.PointerExit;
            entry2.callback.AddListener((eventData) => { this.ShowWeaponDescription(button, rect, false, text); });
            trigger.triggers.Add(entry2);
        }

        public void ShowWeaponDescription(Button button, RectTransform rect, bool enable, string text)
        {
            if (enable)
            {
                this._weaponDescriptionController.ChangeText(text);
                this.WeaponDescription.transform.position = new Vector3((button.transform.position.x - (button.transform.localScale.x/2)) - this.WeaponDescription.transform.localScale.x, rect.transform.position.y + rect.sizeDelta.y);
            }
            this._weaponDescriptionController.IsVisible(enable);
        }
    }
}
