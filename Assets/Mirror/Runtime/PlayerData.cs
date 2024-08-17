using System;

[Serializable]
public class PlayerData
{
    public string Faction;
    public int Level;

    public int Mana;
    public int Strength;
    public int Agility;
    public int Intelligence;

    // Constructeur par défaut
    public PlayerData()
    {
        Faction = "Véda";
        Level = 1;

        Mana = 50;
        Strength = 10;
        Agility = 10;
        Intelligence = 10;
    }

    // Exemple de méthode pour augmenter le niveau
    public void LevelUp()
    {
        Level++;

        Mana += 5;
        Strength += 2;
        Agility += 2;
        Intelligence += 2;
    }

    // Vous pouvez ajouter d'autres méthodes pour gérer les statistiques du joueur
}
