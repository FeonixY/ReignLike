using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : SingletonMonoBehaviour<InputManager>
{
    [HideInInspector] public bool IsRestartPressed;
    [HideInInspector] public bool IsExitPressed;

    public TMP_Text InputHint;

    private PlayerInput PlayerInput;

    private InputAction restartAction;
    private InputAction exitAction;

    protected override void Awake()
    {
        base.Awake();

        PlayerInput = GetComponent<PlayerInput>();

        restartAction = PlayerInput.actions["Restart"];
        exitAction = PlayerInput.actions["Exit"];

        if (InputHint != null)
        {
            InputHint.text =
                $"[{restartAction.bindings[0].ToDisplayString()}] {restartAction.name}\n" +
                $"[{exitAction.bindings[0].ToDisplayString()}] {exitAction.name}";
        }
    }

    private void Update()
    {
        IsRestartPressed = restartAction.WasPressedThisFrame();
        IsExitPressed = exitAction.WasPressedThisFrame();

        if (IsRestartPressed)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (IsExitPressed)
        {
            Application.Quit();
        }
    }
}
