using GTANetworkAPI;
using reallife.Data;
using reallife.Db;
using reallife.Events;
using reallife.Player;
using System;

namespace reallife.Commands
{
    public class PlayerMoney : Script
    {
        [Command("money")]
        public void CMD_Money(Client client)
        {
            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(client);

            if (playerInfo == null)
            {
                Console.WriteLine("Les statistiques des joueurs sont introuvables.");
                return;
            }

            client.SendNotification($"Votre solde est de: ~g~{playerInfo.money}$");
            client.SendNotification($"Votre solde bancaire est de: ~g~{playerInfo.bank}$");

            EventTriggers.Update_Money(client);
        }

        [Command("givemoney")]
        public void CMD_GiveMoney(Client client, Client player, double amount)
        {
            if (!client.HasData("ID"))
                return;

            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(client);
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(player);

            if (pInfo == null)
            {
                client.SendNotification($"Le joueur {player.Name} n'a pas pu être trouvé.");
                return;
            }


            PlayerInfo otherInfo = Database.GetById<PlayerInfo>(pInfo._id);

            if (otherInfo == null)
            {
                Console.WriteLine($"{pInfo.vorname} {pInfo.nachname} n'a pas de table PlayerInfo!");
                return;
            }

            bool result = playerInfo.SubMoney(amount);

            if (!result)
            {
                client.SendNotification("~r~Vous ne possédez pas cet argent!");
                return;
            }

            otherInfo.AddMoney(amount);

            Database.Update(otherInfo);
            Database.Update(playerInfo);

            client.SendNotification($"Vous avez donné au joueur { player.Name} ~g~{amount}$ ~w~avec succes!");
            player.SendNotification($"Vous avez reçu ~g~{amount}~w~ de {client.Name} !");
            EventTriggers.Update_Money(client);

            Client other_player = NAPI.Player.GetPlayerFromName(player.Name);

            if (other_player == null)
                return;

            EventTriggers.Update_Money(other_player);
        }

        [Command("burnmoney")]
        public void CMD_BurnMoney(Client client, double amount)
        {
            if (!client.HasData("ID"))
                return;

            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(client);

            if (playerInfo == null)
                return;

            bool result = playerInfo.SubMoney(amount);
            Database.Update(playerInfo);

            if (!result)
                return;

            client.SendNotification($"~g~tu as jeté {amount}$ !");
            EventTriggers.Update_Money(client);
        }
    }
}
