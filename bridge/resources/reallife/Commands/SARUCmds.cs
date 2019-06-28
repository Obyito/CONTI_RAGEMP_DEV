using GTANetworkAPI;
using reallife.Data;
using reallife.Events;
using reallife.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace reallife.Commands
{
    class SARUCmds : Script
    {
        [Command("revive")]
        public void CMD_Revive(Client client, Client player)
        {
            PlayerInfo cInfo = PlayerHelper.GetPlayerStats(client);
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(player);

            if (!FraktionSystem.HasRank(client, 2))
            {
                client.SendNotification("~r~Vous n'appartenez pas à la SARU!");
                return;
            }

            if (!client.HasData("onduty"))
            {
                client.SendNotification("~r~Vous n'êtes pas en service!");
                return;
            }

            if (client.Position.DistanceTo2D(player.Position) <= 3)
            {
                if (!player.HasData("dead"))
                {
                    client.SendNotification("Ce joueur n'est pas mort!");
                    return;
                }

                NAPI.Player.SpawnPlayer(player, pInfo.GetLastPlayerLocation());
                player.SendNotification("Vous avez été réapparu!");
                NAPI.ClientEvent.TriggerClientEvent(player, "DeathFalse");
                player.ResetData("dead");

                cInfo.money += 100;
                cInfo.Update();
                client.SendNotification("[~r~SARU~w~]: Vous avez 100~g~$~w~.");

                if (pInfo.money > 100)
                {
                    pInfo.money -= 100;
                    pInfo.Update();
                    player.SendNotification("[~r~SARU~w~]: Vous avez payé 100~g~$~w~ pour le traitement..");
                }
                else if (pInfo.bank > 100)
                {
                    pInfo.bank -= 100;
                    pInfo.Update();
                    player.SendNotification("[~r~SARU~w~]: Vous avez payé 100~g~$~w~ pour le traitement.");
                } else
                {
                    client.SendNotification("[~y~EasterEgg~w~]: Tu es sacrément pauvre! Va travailler!");
                }

                EventTriggers.Update_Money(client);
                EventTriggers.Update_Bank(client);

                EventTriggers.Update_Money(player);
                EventTriggers.Update_Bank(player);

            } else
            {
                client.SendNotification("Vous n'êtes pas près de cette personne!");
            }
        }

        [Command("fduty")]
        public void CMD_fduty(Client client)
        {
            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(client);

            if (client.Position.DistanceTo2D(new Vector3(1194.831, -1477.961, 34.85954)) <= 0.8)
            {
                if (FraktionSystem.HasRank(client, 2))
                {
                    if (client.HasData("fonduty") || client.HasData("onduty"))
                    {
                        PlayerData.OffDuty(client);
                    }
                    else
                    {
                        client.SetClothes(8, 129, 0);
                        client.SetClothes(11, 95, 0);
                        client.SetClothes(4, 13, 0);
                        client.SetClothes(6, 10, 0);
                        client.SetClothes(0, 124, 0);
                        client.SetClothes(5, 45, 0);

                        client.SendNotification("Vous êtes maintenant en service!");

                        client.SetData("fonduty", 1);
                    }
                }
                else
                {
                    client.SendNotification("~r~Vous n'appartenez pas à la SARU!");
                }
            } else
            {
                client.SendNotification("Vous n'êtes pas près d'un devoir!");
            }
        }

        [Command("heal")]
        public void CMD_heal(Client client, Client player)
        {
            if (!FraktionSystem.HasRank(client, 2))
            {
                client.SendNotification("~r~Vous n'appartenez pas à la SARU!");
                return;
            }

            if (!client.HasData("onduty"))
            {
                client.SendNotification("~r~Vous n'êtes pas en service!");
                return;
            }

            if (client.Name == player.Name)
            {
                client.SendNotification("~r~Vous ne pouvez pas vous spécifier!");
                return;
            }

            if (client.Position.DistanceTo2D(player.Position) <= 3)
            {
                player.Health = 100;
                client.SendNotification("~g~Joueur guéri avec succès!");
                player.SendNotification($"~g~Vous avez été guéri par  { client.Name} ");
            } else
            {
                client.SendNotification("Le joueur n'est pas dans votre région!");
            }
        }

        /*[Command("sarucar")]
        public void CMD_saruCar(Client client)
        {

            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(client);

            if (!FraktionSystem.HasRank(client, 2))
            {
                client.SendNotification("~r~Du gehörst nicht zur SARU!");
                return;
            }

            if (client.Position.DistanceTo2D(new Vector3(1193.662, -1487.571, 34.84266)) <= 5)
            {
                if (client.HasData("FrakVehicle"))
                {
                    client.SendNotification("Du besitzt bereits ein SARU Fahrzeug!");
                    return;
                }

                if (client.HasData("onduty"))
                {
                    uint hash = NAPI.Util.GetHashKey("ambulance");
                    Vehicle veh = NAPI.Vehicle.CreateVehicle(hash, new Vector3(1200.68286132813, -1489.611328125, 34.5351028442383), 179.496887207031f, 0, 0);
                    NAPI.Vehicle.SetVehicleNumberPlate(veh, client.Name);
                    client.SetIntoVehicle(veh, -1);

                    client.SendChatMessage("Nutze ~b~/lock~w~ zum aufschließen oder abschließen!");

                    client.SetData("FrakVehicle", veh);
                }
                else
                {
                    client.SendNotification("~r~Du musst dafür im Dienst sein!");
                }
            }
            else if (client.Position.DistanceTo2D(new Vector3(1191.315, -1474.554, 34.85954)) <= 5)
            {
                if (client.HasData("FrakVehicle"))
                {
                    client.SendNotification("Du besitzt bereits ein SARU Fahrzeug!");
                    return;
                }

                if (client.HasData("fonduty"))
                {
                    uint hash = NAPI.Util.GetHashKey("firetruk");
                    Vehicle veh = NAPI.Vehicle.CreateVehicle(hash, new Vector3(1200.7568359375, -1468.86804199219, 34.9273796081543), 0.40966796875f, 150, 150);
                    NAPI.Vehicle.SetVehicleNumberPlate(veh, client.Name);
                    client.SetIntoVehicle(veh, -1);

                    client.SendChatMessage("Nutze ~b~/lock~w~ zum aufschließen oder abschließen!");

                    client.SetData("FrakVehicle", veh);
                }
                else
                {
                    client.SendNotification("~r~Du musst dafür im Dienst sein!");
                }
            }
            else
            {
                client.SendNotification("Du bist nicht in Reichweite!");
            }
        }*/
    }
}
