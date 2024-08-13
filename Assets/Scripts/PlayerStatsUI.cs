using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerStatsUI : NetworkBehaviour
{
	public GameObject playerStatsPanel; // Assignez le panneau des statistiques dans l'inspecteur

	public Text manaText;
	public Text strengthText;
	public Text agilityText;
	public Text intelligenceText;

	private Player player;

	void Start()
	{
		// Assurez-vous que le panneau des statistiques est désactivé au démarrage
		playerStatsPanel.SetActive(false);

		if (isLocalPlayer)
		{
			// Trouver le script Player sur le GameObject du joueur
			player = GetComponent<Player>();
		}
	}

	void Update()
	{
		if (isLocalPlayer && Input.GetKeyDown(KeyCode.K))
		{
			ToggleStatsPanel();
		}
	}

	void ToggleStatsPanel()
	{
		playerStatsPanel.SetActive(!playerStatsPanel.activeSelf);
		if (playerStatsPanel.activeSelf)
		{
			UpdateStats();
		}
	}

	void UpdateStats()
	{
		if (player != null)
		{
			manaText.text = $"Mana: {player.playerData.Mana}";
			strengthText.text = $"Strength: {player.playerData.Strength}";
			agilityText.text = $"Agility: {player.playerData.Agility}";
			intelligenceText.text = $"Intelligence: {player.playerData.Intelligence}";
		}
	}
}
