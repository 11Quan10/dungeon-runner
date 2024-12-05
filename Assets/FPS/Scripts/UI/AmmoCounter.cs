using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;

namespace Unity.FPS.UI
{
    public class AmmoCounter : MonoBehaviour
    {
        public int WeaponCounterIndex { get; set; }

        PlayerWeaponsManager m_PlayerWeaponsManager;
        WeaponController m_Weapon;

        void Awake()
        {
            EventManager.AddListener<AmmoPickupEvent>(OnAmmoPickup);
        }

        void OnAmmoPickup(AmmoPickupEvent evt)
        {
            if (evt.Weapon == m_Weapon)
            {
                // Update the ammo count in gameplay logic (if needed)
                Debug.Log($"Ammo picked up for {evt.Weapon.name}: {m_Weapon.GetCarriedPhysicalBullets()} bullets.");
            }
        }

        public void Initialize(WeaponController weapon, int weaponIndex)
        {
            m_Weapon = weapon;
            WeaponCounterIndex = weaponIndex;

            m_PlayerWeaponsManager = FindFirstObjectByType<PlayerWeaponsManager>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerWeaponsManager, AmmoCounter>(m_PlayerWeaponsManager, this);
        }

        void Update()
        {
            // Ensure the active weapon logic still works
            bool isActiveWeapon = m_Weapon == m_PlayerWeaponsManager.GetActiveWeapon();
            if (isActiveWeapon)
            {
                // Additional gameplay-only updates if needed
            }
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<AmmoPickupEvent>(OnAmmoPickup);
        }
    }
}
