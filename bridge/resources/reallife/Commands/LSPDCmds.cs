using GTANetworkAPI;
using reallife.Data;
using reallife.Db;
using reallife.Events;
using reallife.Fraktion;
using reallife.Player;
using reallife.Chat;

namespace reallife.Commands
{
    class LSPDCmds : Script
    {
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        [Command("akte")]
        public void CMD_Akte(Client client, Client player)
        {
            PlayerInfo clientInfo = PlayerHelper.GetPlayerStats(client);
            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(player);

            if (!FraktionSystem.HasRank(client, 1))
            {
                client.SendNotification("Vous avez été renvoyé du groupe!");
                return;
            }

            if (!client.HasData("onduty"))
            {
                client.SendNotification("~r~Vous êtes pas en service!");
                return;
            }

            double vermoegen = playerInfo.money + playerInfo.bank;

            if(client.Position.DistanceTo2D(new Vector3(442.9581, -975.1335, 30.68961)) <= 2)
            {
                client.SendNotification($"[~b~record~w~]: {player.Name}");
                client.SendNotification($"[~b~niveau de recherche~w~]: {playerInfo.wantedlevel}");
                client.SendNotification($"[~b~actif~w~]: {vermoegen}~g~$");
                client.SendNotification($"[~b~Faction~w~]: {PlayerInfo.WhichFrak(player)}");
            } else
            {
                client.SendNotification("Vous n'êtes pas près des fichiers.");
            }

        }

        [Command("takeweapons")]
        public void CMD_TakeWeapons(Client client, Client player)
        {
            PlayerInfo clientInfo = PlayerHelper.GetPlayerStats(client);
            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(player);

            if (!FraktionSystem.HasRank(client, 1))
            {
                client.SendNotification("Vous n'avez aucune autorisation!");
                return;
            }

            if (!player.HasData("onduty"))
            {
                client.SendNotification("~r~Vous n'êtes pas en service!");
                return;
            }

            if (client.Position.DistanceTo2D(player.Position) < 5)
            {
                player.RemoveAllWeapons();
                client.SendNotification($"[~b~LSPD~w~]: Les armes de { player.Name} ont été enlevés!");
                player.SendNotification($"[~b~LSPD~w~]: Vos armes viennent d'etre {client.Name} supprimé!");
            } else
            {
                client.SendNotification("~r~Le joueur n'est pas à portée de main!");
            }
        }

        [Command("einsperren")]
        public void CMD_Einsperren(Client client, Client player)
        {
            PlayerInfo clientInfo = PlayerHelper.GetPlayerStats(client);
            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(player);

            if (!FraktionSystem.HasRank(client, 1))
            {
                client.SendNotification("Vous n'avez aucune autorisation!");
                return;
            }

            if (!client.HasData("onduty"))
            {
                client.SendNotification("~r~Vous n'êtes pas en service!");
                return;
            }

            if (!player.HasData("cuff") == true)
            {
                client.SendNotification("Le joueur n'est pas menotté!");
                return;
            }

            if (playerInfo.wantedlevel == 0)
            {
                client.SendNotification("Ce joueur n'est pas recherché!");
                return;
            }

            if (client.Position.DistanceTo2D(new Vector3(461.9194, -989.1077, 24.91486)) <= 3)
            {
                if (client.Position.DistanceTo2D(player.Position) <= 5)
                {
                    playerInfo.jail = 1;
                    playerInfo.cuff = 0;

                    playerInfo.Update();

                    PlayerData.Respawn(player);

                    client.SendNotification($"[~b~LSPD~w~]: Vous avez enfermé ~y~{player.Name}~w~ dans la prion !");
                    player.SendNotification($"[~b~LSPD~w~]: {client.Name}~w~ Vous venez d'etre mis en prison");
                } else
                {
                    client.SendNotification("Ce joueur n'est pas dans votre région!");
                }

            } else
            {
                client.SendNotification("Vous ne pouvez pas enfermer la personne ici!");
            }

        }

        [Command("entlassen")]
        public void CMD_Entlassen(Client client, Client player)
        {
            PlayerInfo clientInfo = PlayerHelper.GetPlayerStats(client);
            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(player);

            if (!FraktionSystem.HasRank(client, 1))
            {
                client.SendNotification("Vous n'avez aucune autorisation!");
                return;
            }

            if (!client.HasData("onduty"))
            {
                client.SendNotification("~r~Vous n'êtes pas en service!");
                return;
            }

            if (client.Position.DistanceTo2D(new Vector3(461.9194, -989.1077, 24.91486)) <= 3)
            {
                    playerInfo.jail = 0;
                    playerInfo.jailtime = 0;
                    playerInfo.temp_location = null;
                    playerInfo.Update();

                    PlayerData.Respawn(player);
                    client.SendNotification($"[~b~LSPD~w~]: ~y~{player.Name}~w~ta sortis de prison");
                    player.SendNotification($"[~b~LSPD~w~]: {client.Name}~w~ vous a laissé en dehors de la prison!");
            }
            else
            {
                client.SendNotification("Vous ne pouvez pas renvoyer la personne ici!");
            }
        }

        [Command("cuff")] 
        public void CMD_Cuff(Client client, Client player)
        {
            PlayerInfo clientInfo = PlayerHelper.GetPlayerStats(client);
            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(player);

            if (!FraktionSystem.HasRank(client, 1))
            {
                client.SendNotification("Vous n'avez aucune autorisation!");
                return;
            }

            if (!client.HasData("onduty"))
            {
                client.SendNotification("~r~Vous n'êtes pas en service!");
                return;
            }

            if (player.HasData("cuff"))
            {
                client.SendNotification("Ce joueur a déjà des menottes!");
                return;
            }

            if (playerInfo.jail == 1)
            {
                client.SendNotification("Ce joueur ne peut pas être menotté en prison!");
                return;
            }

            if(player.Position.DistanceTo2D(client.Position) < 5)
            {
                NAPI.Player.PlayPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "mp_arresting", "idle");
                client.SendNotification("[~b~LSPD~w~]:~w~ Vous avez arrêté : " + player.Name);
                LSPD.cuff(player);
            } else
            {
                client.SendNotification("Le joueur n'est pas à portée de main!");
            }
        }

        [Command("uncuff")]
        public void CMD_UnCuff(Client client, Client player)
        {
            PlayerInfo clientInfo = PlayerHelper.GetPlayerStats(client);
            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(player);

            if (!FraktionSystem.HasRank(client, 1))
            {
                client.SendNotification("Vous n'avez aucune autorisation!");
                return;
            }

            if (!player.HasData("onduty"))
            {
                client.SendNotification("~r~Vous n'êtes pas en service!");
                return;
            }

            if (!player.HasData("cuff"))
            {
                client.SendNotification("[~b~LSPD~w~]: Ce joueur n'a pas de menottes!");
                return;
            }

            if (player.Position.DistanceTo2D(client.Position) < 5)
            {
                NAPI.Player.StopPlayerAnimation(player);
                client.SendNotification("[~b~LSPD~w~]:Vous avez libéré: " + player.Name);
                LSPD.uncuff(player);
            } else
            {
                client.SendNotification("Le joueur n'est pas à portée de main!");
            }
        }

        [Command("mp")]
        public void CMD_MegaPhone(Client client, string message)
        {

            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(client);

            if (!FraktionSystem.HasRank(client, 1))
            {
                client.SendNotification("Vous n'avez pas accès à cette commande!");
                return;
            }

            //Überprüfen ob ein Spieler innerhalb der Reichweite (25) ist.
            Client[] clients = NAPI.Pools.GetAllPlayers().FindAll(x => x.Position.DistanceTo2D(client.Position) <= 25).ToArray();

            for (int i = 0; i < clients.Length; i++)
            {
                if (!clients[i].Exists)
                    continue;

                clients[i].SendChatMessage($"[~b~Megaphone~w~] {client.Name}: {message}");
            }
        }

        [Command("removewanted")]
        public void CMD_RemoveWanted(Client client, Client player, int wanteds)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(player);

            if (!FraktionSystem.HasRank(client, 1))
            {
                client.SendNotification("~r~Vous n'appartenez pas à la LSPD!");
                return;
            }

            if (!client.HasData("onduty"))
            {
                client.SendNotification("~r~Vous n'êtes pas en service!");
                return;
            }

            if (player == null)
            {
                client.SendNotification("Le joueur n'a pas été trouvé!");
                return;
            }

            if (client.Name == player.Name)
            {
                client.SendNotification("~r~Vous ne pouvez pas vous spécifier!");
                return;
            }

            if (pInfo.wantedlevel - wanteds <= 0)
            {
                client.SendChatMessage("Vous ne pouvez pas donner au joueur pas moins de 0 Wanted!");
                return;
            }

            pInfo.wantedlevel -= wanteds;
            client.SendNotification($"[~b~LSPD~w~]: Vous avez enlevé un recherche à ~r~{player.Name}~w~ il à ~r~{wanteds} recherche~w~!");
            player.SendChatMessage($"[~b~LSPD~w~]: ~b~{client.Name}~w~ a vous ~r~{wanteds} recherche~w~ abgezogen!");
            pInfo.Update();
            EventTriggers.Update_Wanteds(player);
        }

        [Command("delakte")]
        public void CMD_DelAkte(Client client, Client player)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(player);

            if (!FraktionSystem.HasRank(client, 1))
            {
                client.SendNotification("~r~Vous n'appartenez pas à la LSPD!");
                return;
            }

            if (!client.HasData("onduty"))
            {
                client.SendNotification("~r~Vous n'êtes pas de service!");
                return;
            }

            if (player == null)
            {
                client.SendNotification("Le joueur n'a pas été trouvé!");
                return;
            }

            if (client.Name == player.Name)
            {
                client.SendNotification("~r~Vous ne pouvez pas vous spécifier!");
                return;
            }

            if (pInfo.wantedlevel > 4)
            {
                client.SendNotification("Ce joueur a trop de souhaits!");
                return;
            }

            pInfo.wantedlevel = 0;
            client.SendNotification($"[~b~LSPD~w~]: Vous avez supprimé le dossier de { player.Name} ~r~~w~!");
            player.SendChatMessage($"[~b~LSPD~w~]: ~b~{client.Name}~w~ à supprimer ton dossier ~r~~w~!");
            pInfo.Update();
            EventTriggers.Update_Wanteds(player);
        }

        [Command("setwanted")]
        public void CMD_SetWanted(Client client, Client player, int wanteds)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(player);

            if (!FraktionSystem.HasRank(client, 1))
            {
                client.SendNotification("~r~Vous n'appartenez pas à la LSPD!");
                return;
            }

            if (!client.HasData("onduty"))
            {
                client.SendNotification("~r~Vous n'êtes pas en service!");
                return;
            }

            if (player == null)
            {
                client.SendNotification("Le joueur n'a pas été trouvé!");
                return;
            }

            if (client.Name == player.Name)
            {
                client.SendNotification("~r~Vous ne pouvez pas vous spécifier!");
                return;
            }

            if (wanteds > 6)
            {
                client.SendChatMessage("Vous ne pouvez pas distribuer plus de 6 Wanted!");
                return;
            }

            pInfo.wantedlevel += wanteds;
            client.SendNotification($"[~b~LSPD~w~]: Le joueur ~r~{player.Name}~w~ vous à donné  ~r~{wanteds} recherche~w~ !");
            player.SendChatMessage($"[~b~LSPD~w~]: ~b~{client.Name}~w~ est recherché ~r~{wanteds} ~w~ !");
            pInfo.Update();
            EventTriggers.Update_Wanteds(player);
        }

        /*[Command("lspdcar")]
        public void CMD_LSPDCar(Client client)
        {

            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(client);

            if (!FraktionSystem.HasRank(client, 1))
            {
                client.SendNotification("~r~Du gehörst nicht zur LSPD!");
                return;
            }

            if (client.Position.DistanceTo2D(new Vector3(458.2729, -1008.082, 28.28012)) <= 5)
            {
                if (client.HasData("FrakVehicle"))
                {
                    client.SendNotification("~g~[POLICE]:~w~ Du besitzt bereits ein LSPD Fahrzeug!");
                    return;
                }

                if (client.HasData("onduty"))
                {
                    uint hash = NAPI.Util.GetHashKey("police");
                    Vehicle veh = NAPI.Vehicle.CreateVehicle(hash, new Vector3(447.323150634766, -996.606872558594, 25.3755207061768), 179.479125976563f, 111, 0);
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
