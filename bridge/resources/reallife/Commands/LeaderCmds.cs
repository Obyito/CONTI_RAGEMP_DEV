using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using reallife.Data;
using reallife.Db;
using reallife.Player;

namespace reallife.Commands
{
    class LeaderCmds : Script
    {
        [Command("delfwarn")]
        public void CMD_DelFraktionWarn(Client client, Client player)
        {
            //Spieler Statistiken
            PlayerInfo leaderInfo = PlayerHelper.GetPlayerStats(client);
            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(player);

            //Abfrage ob man ein Leader ist
            if (!LeaderSystem.IsLeader(client))
            {
                client.SendNotification("~r~Vous n'êtes pas un leader!");
                return;
            }

            if (!LeaderSystem.Same(client, player))
            {
                client.SendNotification("~r~Vous n'êtes pas dans la même faction!");
                return;
            }

            if (client.Name == player.Name)
            {
                client.SendNotification("~r~Vous ne pouvez pas vous spécifier!");
                return;
            }

            if (playerInfo.fwarn == 0)
            {
                player.SendNotification("Le joueur n'a pas d'avertissement!");
                return;
            }

            playerInfo.fwarn -= 1;
            playerInfo.Update();

            player.SendChatMessage("[~g~Faction~w~]: Un avertissement a été supprimé!");
        }

        [Command("fwarn")]
        public void CMD_FraktionWarn(Client client, Client player)
        {
            //Spieler Statistiken
            PlayerInfo leaderInfo = PlayerHelper.GetPlayerStats(client);
            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(player);

            //Abfrage ob man ein Leader ist
            if (!LeaderSystem.IsLeader(client))
            {
                client.SendNotification("~r~Vous n'êtes pas un leader!");
                return;
            }

            if (!LeaderSystem.Same(client, player))
            {
                client.SendNotification("~r~Vous n'êtes pas dans la même faction!");
                return;
            }

            if (client.Name == player.Name)
            {
                client.SendNotification("~r~Vous ne pouvez pas vous spécifier!");
                return;
            }

            playerInfo.fwarn += 1;
            playerInfo.Update();

            player.SendChatMessage($"[~g~Fraktion~w~]: Vous avez reçu un avertissement. ~r~{playerInfo.fwarn}~w~ avertissements.");

            if (playerInfo.fwarn == 3)
            {
                player.SendChatMessage("[~g~Fraktion~w~]: Vous avez trop d'avertissements sur la raison pour laquelle vous avez été libéré de la faction!");
                player.SendNotification("~r~Vous avez été renvoyé du groupe!");
                playerInfo.fraktion = 0;
                playerInfo.last_location = new double[] { -1167.994, -700.4285, 21.89281 };

                playerInfo.Update();

                PlayerData.Respawn(player);
            }
        }

        [Command("invite")]
        public void CMD_invite(Client client, string player)
        {
            //Spieler Statistiken
            PlayerInfo leaderInfo = PlayerHelper.GetPlayerStats(client);

            //Abfrage ob man ein Leader ist
            if (!LeaderSystem.IsLeader(client))
            {
                client.SendNotification("~r~Vous n'êtes pas un leader!");
                return;
            }

            /*if (client.Name == target.Name)
            {
                client.SendNotification("~r~Du kannst dich nicht selber einladen!");
                return;
            }*/

            if (FraktionSystem.SetRank(player, leaderInfo.fleader))
            {
                client.SendNotification($"[~r~Server~w~] {player} a été invité au groupe.");
                PlayerData.Respawn(client);
                return;
            }
            else
            {
                client.SendNotification($"[~r~Server~w~] {player} ne peut pas être invité au groupe!");
                return;
            }

        }

        [Command("uninvite")]
        public void CMD_uninvite(Client client, Client player)
        {
            //Spieler Statistiken
            PlayerInfo leaderInfo = PlayerHelper.GetPlayerStats(client);
            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(player);

            //Abfrage ob man ein Leader ist
            if (!LeaderSystem.IsLeader(client))
            {
                client.SendNotification("~r~Vous n'êtes pas un leader!");
                return;
            }

            //Abfrage ob Leader und Spieler in der selben Fraktion sind
            if (!LeaderSystem.Same(client, player))
            {
                client.SendNotification("~r~Ce joueur n'est pas dans votre faction!");
                return;
            }

            /*if (client.Name == player.Name)
            {
                client.SendNotification("~r~Du kannst dich nicht selber entlassen!");
                return;
            }*/

            client.SendNotification("Le joueur a été expulsé de la faction avec succès!");
            player.SendNotification("~r~Vous avez été renvoyé du groupe!");
            playerInfo.fraktion = 0;
            playerInfo.last_location = new double[] { -1167.994, -700.4285, 21.89281 };

            playerInfo.Update();

            PlayerData.Respawn(player);
        }
    }
}
