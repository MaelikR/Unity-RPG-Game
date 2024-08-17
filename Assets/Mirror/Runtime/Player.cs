using UnityEngine;
using Mirror;

namespace Mirror
{
	public class Player : NetworkBehaviour
	{
		public PlayerData playerData;

		void Start()
		{
			// Initialisation des données du joueur
			playerData = new PlayerData();
		}


		// Méthode pour augmenter le niveau du joueur
		public void LevelUp()
		{
			playerData.LevelUp();
		}

		// Vous pouvez ajouter d'autres méthodes pour gérer les actions du joueur
	}
}
