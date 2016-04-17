using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extra;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public class WeaponInventory : MonoBehaviour
    {
        public List<InventoryItem> Inventory;
        public WeaponType CurrentWeapon;
        public WeaponType LastUsedWeaponType = WeaponType.Bomb;

        public IWeapon CurrentWeaponInstance
        {
            get
            {
                if (this._currentWeaponInstance != null)
                    return this._currentWeaponInstance;
                SetCurrentWeaponInstance();
                UpdateCurrentWeaponValues();
                return this._currentWeaponInstance;
            }
        }

        private IWeapon _currentWeaponInstance;
        public IWeapon LastUsedWeapon;


        private WeaponController WeaponController { get; set; }

        void Awake()
        {
            this.WeaponController = GetComponent<WeaponController>();
        }

        public void ChangeCurrentWeapon(WeaponType weapon)
        {
            if (!HasWeaponInInventory(weapon))
                return;

            if (weapon == this.CurrentWeapon)
                return;

            this.CurrentWeapon = weapon;
            SetCurrentWeaponInstance();
            UpdateCurrentWeaponValues();
        }

        public void AddWeaponToInventory(WeaponType weapon, int quantity)
        {
            var inventoryItem = GetInventoryItem(weapon);
            inventoryItem.Quantity = inventoryItem.Quantity + quantity;
        }

        public void UpdateWeaponQuantity()
        {
            var inventoryItem = GetInventoryItem(this.CurrentWeapon);
            inventoryItem.Quantity = inventoryItem.Quantity - 1 <= 0 ? 0 : inventoryItem.Quantity - 1;

            if(inventoryItem.Quantity <= 0 && !(this.CurrentWeaponInstance is IUtility))
                ChangeCurrentWeapon(Inventory.FirstOrDefault(x => x.Quantity > 0).Weapon);
        }

        public void AddToWeaponQuantity(WeaponType weapon)
        {
            var inventoryItem = GetInventoryItem(weapon);
            inventoryItem.Quantity += 1;
        }

        public IWeapon GetWeaponInstance(WeaponType weaponType, Vector3 position)
        {
            var weapon = Instantiate(Util.LoadWeapon(weaponType), position, Quaternion.identity) as GameObject;
            return (IWeapon)weapon.GetComponent(typeof(IWeapon));
        }

        public IWeapon GetWeaponInstance(Vector3 position)
        {
            return this.GetWeaponInstance(this.CurrentWeapon, position);
        }

        public void SetCurrentWeaponInstance()
        {
            this._currentWeaponInstance = this.GetWeaponInstance(new Vector3(float.MaxValue, float.MaxValue));
            RemoveCurrentWeaponInstance();
        }

        public void RemoveCurrentWeaponInstance()
        {
            Destroy(CurrentWeaponInstance.GameObject);
        }

        public int GetWeaponQuantity(WeaponType weapon)
        {
            if (!HasWeaponInInventory(weapon))
                return 0;
            return GetInventoryItem(weapon).Quantity;
        }

        public bool HasWeaponInInventory(WeaponType weapon)
        {
            var item = GetInventoryItem(weapon);
            if (item == null)
                return false;
            return GetInventoryItem(weapon).Quantity > 0;
        }

        private void UpdateCurrentWeaponValues()
        {
            if (this.CurrentWeaponInstance == null)
                return;

            if (!(this.CurrentWeaponInstance is IUtility))
            {
                this.LastUsedWeaponType = this.CurrentWeapon;
                if (!this.WeaponController.IsAngleAllowed(this.WeaponController.ShootingAngle))
                    this.WeaponController.ShootingAngle = this.CurrentWeaponInstance.Crosshair.Start;
                this.WeaponController.ShootingPower = this.CurrentWeaponInstance.Power.Start;
            }

            this.WeaponController.InitializeShot();
        }

        private InventoryItem GetInventoryItem(WeaponType weapon)
        {
            var item = this.Inventory.FirstOrDefault(x => x.Weapon == weapon);
            return item;
        }
    }
}
