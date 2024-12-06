using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class InGameMenuManager : MonoBehaviour
    {
        [Tooltip("Root GameObject of the menu used to toggle its activation")]
        public GameObject MenuRoot;

        [Tooltip("Master volume when menu is open")] [Range(0.001f, 1f)]
        public float VolumeWhenMenuOpen = 0.5f;

        [Tooltip("Slider component for look sensitivity")]
        public Slider LookSensitivitySlider;

        [Tooltip("Toggle component for invincibility")]
        public Toggle InvincibilityToggle;

        [Tooltip("GameObject for the controls")]
        public GameObject ControlImage;

        PlayerInputHandler m_PlayerInputsHandler;
        Health m_PlayerHealth;

        void Start()
        {
            m_PlayerInputsHandler = FindObjectOfType<PlayerInputHandler>();
            if (!m_PlayerInputsHandler)
            {
                Debug.LogError("PlayerInputHandler not found.");
                return;
            }

            m_PlayerHealth = m_PlayerInputsHandler.GetComponent<Health>();
            if (!m_PlayerHealth)
            {
                Debug.LogError("Health component not found on PlayerInputHandler.");
                return;
            }
        }

        void Update()
        {
        }


        void SetPauseMenuActivation(bool active)
        {
            MenuRoot.SetActive(active);

            if (MenuRoot.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
                AudioListener.volume = VolumeWhenMenuOpen;
                EventSystem.current?.SetSelectedGameObject(null);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
                AudioListener.volume = 1f;
            }
        }

        void OnMouseSensitivityChanged(float newValue)
        {
            m_PlayerInputsHandler.LookSensitivity = newValue;
        }

        void OnInvincibilityChanged(bool newValue)
        {
            m_PlayerHealth.Invincible = newValue;
        }

        public void OnShowControlButtonClicked(bool show)
        {
            ControlImage.SetActive(show);
        }
    }
}
