// GENERATED AUTOMATICALLY FROM 'Assets/Input/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerInput : IInputActionCollection, IDisposable
{
    private InputActionAsset asset;
    public PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""Character"",
            ""id"": ""7aa3be75-3ffe-4b6d-87bc-796e3f208b00"",
            ""actions"": [
                {
                    ""name"": ""HorizontalMovement"",
                    ""type"": ""Value"",
                    ""id"": ""f1edd1db-9317-4b47-b127-2bf023282d6b"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""d3d72f7f-32a4-42df-8f7d-b38de9d4a298"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""446cbefd-e404-4aeb-ad68-ccf289db140f"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Input"",
                    ""id"": ""4c55f657-4957-452f-9dc7-15abe4c9d987"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""24aff082-b083-4579-9c59-591498abdaa4"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""HorizontalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""ebe32eef-a681-43f3-8fb7-2ece478ed7b2"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""HorizontalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": []
        }
    ]
}");
        // Character
        m_Character = asset.FindActionMap("Character", throwIfNotFound: true);
        m_Character_HorizontalMovement = m_Character.FindAction("HorizontalMovement", throwIfNotFound: true);
        m_Character_Jump = m_Character.FindAction("Jump", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Character
    private readonly InputActionMap m_Character;
    private ICharacterActions m_CharacterActionsCallbackInterface;
    private readonly InputAction m_Character_HorizontalMovement;
    private readonly InputAction m_Character_Jump;
    public struct CharacterActions
    {
        private PlayerInput m_Wrapper;
        public CharacterActions(PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @HorizontalMovement => m_Wrapper.m_Character_HorizontalMovement;
        public InputAction @Jump => m_Wrapper.m_Character_Jump;
        public InputActionMap Get() { return m_Wrapper.m_Character; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CharacterActions set) { return set.Get(); }
        public void SetCallbacks(ICharacterActions instance)
        {
            if (m_Wrapper.m_CharacterActionsCallbackInterface != null)
            {
                HorizontalMovement.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnHorizontalMovement;
                HorizontalMovement.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnHorizontalMovement;
                HorizontalMovement.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnHorizontalMovement;
                Jump.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnJump;
                Jump.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnJump;
                Jump.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnJump;
            }
            m_Wrapper.m_CharacterActionsCallbackInterface = instance;
            if (instance != null)
            {
                HorizontalMovement.started += instance.OnHorizontalMovement;
                HorizontalMovement.performed += instance.OnHorizontalMovement;
                HorizontalMovement.canceled += instance.OnHorizontalMovement;
                Jump.started += instance.OnJump;
                Jump.performed += instance.OnJump;
                Jump.canceled += instance.OnJump;
            }
        }
    }
    public CharacterActions @Character => new CharacterActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface ICharacterActions
    {
        void OnHorizontalMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
    }
}
