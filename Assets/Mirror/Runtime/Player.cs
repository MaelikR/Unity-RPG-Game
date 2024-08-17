using UnityEngine;
using Mirror;

namespace Mirror
{
	public class Player : NetworkBehaviour
	{
		public PlayerData playerData;

		void Start()
		{
			// Initialisation des donn�es du joueur
			playerData = new PlayerData();
		}


		// M�thode pour augmenter le niveau du joueur
		public void LevelUp()
		{
			playerData.LevelUp();
		}

		// Vous pouvez ajouter d'autres m�thodes pour g�rer les actions du joueur
	}
}
