using GTANetworkAPI;
using reallife.Player;
using reallife.Db;
using reallife.Data;

namespace reallife.Events
{
    class SE : Script
    {
        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(Client client, DisconnectionType type, string reason)
        {
            Player.Handler.DisconnectFinish(client);
        }

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(Client player, Client killer, uint reason)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(player);

            if (killer == null)
            {
                NAPI.Chat.SendChatMessageToPlayer(player, $"Tu es mort.");
            }
            else
            {
                NAPI.Chat.SendChatMessageToPlayer(player, $"{killer.Name} vous à tuer !");
            }

            NAPI.ClientEvent.TriggerClientEvent(player, "DeathTrue");

            player.SetData("dead", true);

            NAPI.Task.Run(() =>
            {
                if (player.HasData("dead"))
                {
                    NAPI.Player.SpawnPlayer(player, pInfo.GetLastPlayerLocation());
                    player.SendNotification("Vous réaparaissez !");
                    NAPI.ClientEvent.TriggerClientEvent(player, "DeathFalse");
                    player.ResetData("dead");
                }
            }, delayTime: 120000);
        }
    }
}
