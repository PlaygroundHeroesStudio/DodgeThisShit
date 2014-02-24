using UnityEngine;

public class MainMenu : MonoBehaviour
{
	public Texture Background = null;

	public Texture Logo = null;

	Rect LogoRect = new Rect(Screen.width * 0.2f, Screen.height * 0.1f, Screen.width * 0.6f, Screen.height * 0.4f);

	Vector2 ButtonSize = new Vector2(Screen.width * 0.25f, Screen.height * 0.1f);

	void OnGUI()
	{
		if (Background != null)
			GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), Background);

		if (Logo != null)
			GUI.DrawTexture(LogoRect, Logo);

		int ButtonCount = 0;

		if (MenuButton("Play", ButtonCount++))
			Application.LoadLevel("InGame");
		
		if (MenuButton("Quit", ButtonCount++))
			Application.Quit();
	}

	bool MenuButton(string Text, int ButtonCount)
	{
		return GUI.Button(new Rect((Screen.width - ButtonSize.x) * 0.5f, LogoRect.y + LogoRect.height + 20 + (20 + ButtonSize.y) * ButtonCount, ButtonSize.x, ButtonSize.y), Text);
	}
}