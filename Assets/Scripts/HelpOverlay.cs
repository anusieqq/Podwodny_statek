using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Pokazuje prostą pomoc ekranową. Okno jest widoczne po uruchomieniu gry,
/// a klawisz H przełącza jego widoczność.
/// </summary>
public class HelpOverlay : MonoBehaviour
{
    private bool isVisible = true;
    private GUIStyle helpTextStyle;

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.hKey.wasPressedThisFrame)
        {
            isVisible = !isVisible;
        }
    }

    private void OnGUI()
    {
        if (!isVisible)
        {
            return;
        }

        if (helpTextStyle == null)
        {
            helpTextStyle = new GUIStyle(GUI.skin.label);
            helpTextStyle.fontSize = 16;
            helpTextStyle.wordWrap = true;
            helpTextStyle.normal.textColor = Color.white;
        }

        float panelWidth = Mathf.Min(440f, Screen.width - 40f);
        float panelHeight = Mathf.Min(280f, Screen.height - 40f);
        Rect panel = new Rect(20f, 20f, panelWidth, panelHeight);

        // GUI.Box rysuje tło i nagłówek okienka, a GUI.Label jego opis.
        GUI.Box(panel, "HELP — Sterowanie");

        string helpText =
            "W / S  — pochylenie: dziób w górę / w dół\n" +
            "A / D  — kurs: skręt w lewo / w prawo\n" +
            "Q / E  — przechylenie: w lewo / w prawo\n\n" +
            "Spacja  — ciąg do przodu\n" +
            "Shift   — ruch wstecz\n\n" +
            "H  — zamknij / otwórz to okno";

        Rect textRect = new Rect(
            panel.x + 18f,
            panel.y + 42f,
            panel.width - 36f,
            panel.height - 56f);
        GUI.Label(textRect, helpText, helpTextStyle);
    }
}
