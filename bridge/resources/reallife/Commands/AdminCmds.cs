using System;
using GTANetworkAPI;
using reallife.Db;
using reallife.Player;
using reallife.Events;
using reallife.Data;

namespace reallife.Commands
{
    class AdminCmds : Script
    {
        [Command("delwarn")]
        public void CMD_DelWarn(Client client, Client player)
        {
            //Spieler Statistiken
            PlayerInfo leaderInfo = PlayerHelper.GetPlayerStats(client);
            Players playerInfo = PlayerHelper.GetPlayer(player);

            //Abfrage ob man ein Leader ist
            if (!AdminSystem.HasRank(client, 1))
            {
                client.SendNotification("~r~Tu n'es pas Admin.");
                return;
            }

            if (client.Name == player.Name)
            {
                client.SendNotification("~r~Vous ne pouvez pas vous spécifier vous même");
                return;
            }

            if (playerInfo.warn == 0)
            {
                player.SendNotification("Le joueur n'a pas d'avertissement");
                return;
            }

            playerInfo.warn -= 1;
            playerInfo.Update();

            player.SendChatMessage($"[~r~Server~w~]: un avertissement à été suppriméx. Il possède maintenant {playerInfo.warn} avertissement(s).");
        }

        [Command("warn")]
        public void CMD_Warn(Client client, Client player)
        {
            //Spieler Statistiken
            PlayerInfo leaderInfo = PlayerHelper.GetPlayerStats(client);
            Players playerInfo = PlayerHelper.GetPlayer(player);
            BanLog banLog = PlayerHelper.BanLogs(player);

            //Abfrage ob man ein Leader ist
            if (!AdminSystem.HasRank(client, 1))
            {
                client.SendNotification("~r~Tu n'es pas Admin");
                return;
            }

            if (client.Name == player.Name)
            {
                client.SendNotification("~r~Tu ne peut pas te warn toi même");
                return;
            }

            playerInfo.warn += 1;
            playerInfo.Update();

            player.SendChatMessage($"[~r~Server~w~]:Vous avez reçu un avertissement, vous avez maintenant ~r~{playerInfo.warn}~w~ avertissement(s).");

            if (playerInfo.warn == 3)
            {
                player.SendChatMessage("[~r~Server~w~]: Vous avez trop d'avertissements sur la raison pour laquelle vous avez été banni sur ce serveur!");

                playerInfo.ban = 1;
                playerInfo.Update();

                banLog = new BanLog();
                banLog.banned = player.Name;
                banLog.bannedby = client.Name;
                banLog.grund = "3_Verwarnungen";
                banLog.Upsert();

                player.Kick();
            }
        }

        [Command("respawn")]
        public void CMD_Respawn(Client client, string target)
        {
            Client player = NAPI.Player.GetPlayerFromName(target);

            if (!AdminSystem.HasRank(client, 1))
            {
                client.SendChatMessage("Vous n'êtes pas autorisé!");
                return;
            }

            if (player != null)
            {
                PlayerData.Respawn(player);
            } else
            {
                client.SendChatMessage("Joueur introuvable");
                return;
            }
        }

        [Command("tune")]
        public void CMD_tune (Client client, int color1, int color2, int motor, int window, int turbo, int spoiler, int wheel)
        {
            int client_id = client.GetData("ID");
            Vehicle personal_vehicle = client.GetData("PersonalVehicle");

            PlayerVehicles pVeh = PlayerHelper.GetpVehiclesStats(client);

            if (client.Position.DistanceTo2D(new Vector3(-1038.625, -2678.062, 13.25966)) <= 4)
            {
                if (!client.IsInVehicle)
                {
                    client.SendNotification("~r~Vous n'êtes dans aucun véhicule!");
                    return;
                }
                else if (client.Position.DistanceTo2D(personal_vehicle.Position) <= 0.1)
                {
                    if (pVeh._id == client_id)
                    {
                        Vehicle previous_vehicle = client.GetData("PersonalVehicle");
                        previous_vehicle.Delete();

                        Vector3 pVehSpawn = new Vector3(-1038.625, -2678.062, 13.25966);
                        uint pVehHash = NAPI.Util.GetHashKey(pVeh.carmodel);
                        Vehicle veh = NAPI.Vehicle.CreateVehicle(pVehHash, pVehSpawn, 149.258f, 0, 0);
                        NAPI.Vehicle.SetVehicleNumberPlate(veh, client.Name);
                        client.SetIntoVehicle(veh, -1);

                        //TUNES

                        pVeh.Color1 = color1; pVeh.Color2 = color2; pVeh.spoilers = spoiler; pVeh.fbumber = -1; pVeh.rbumber = -1; pVeh.sskirt = -1; pVeh.exhaust = -1; pVeh.frame = -1;
                        pVeh.grill = -1; pVeh.roof = -1; pVeh.motortuning = motor; pVeh.brakes = -1; pVeh.transmission = -1; pVeh.turbo = turbo; pVeh.fwheels = wheel; pVeh.bwheels = -1;
                        pVeh.window = window; pVeh.suspension = -1;

                        NAPI.Vehicle.SetVehiclePrimaryColor(veh, pVeh.Color1); NAPI.Vehicle.SetVehicleSecondaryColor(veh, pVeh.Color2); NAPI.Vehicle.SetVehicleMod(veh, 0, pVeh.spoilers);
                        NAPI.Vehicle.SetVehicleMod(veh, 1, pVeh.fbumber); NAPI.Vehicle.SetVehicleMod(veh, 2, pVeh.rbumber); NAPI.Vehicle.SetVehicleMod(veh, 3, pVeh.sskirt);
                        NAPI.Vehicle.SetVehicleMod(veh, 4, pVeh.exhaust); NAPI.Vehicle.SetVehicleMod(veh, 5, pVeh.frame); NAPI.Vehicle.SetVehicleMod(veh, 6, pVeh.grill); NAPI.Vehicle.SetVehicleMod(veh, 10, pVeh.roof);
                        NAPI.Vehicle.SetVehicleMod(veh, 11, pVeh.motortuning); NAPI.Vehicle.SetVehicleMod(veh, 12, pVeh.brakes); NAPI.Vehicle.SetVehicleMod(veh, 13, pVeh.transmission);
                        NAPI.Vehicle.SetVehicleMod(veh, 18, pVeh.turbo); NAPI.Vehicle.SetVehicleMod(veh, 23, pVeh.fwheels); NAPI.Vehicle.SetVehicleMod(veh, 24, pVeh.bwheels); //MOTORAD
                        NAPI.Vehicle.SetVehicleWindowTint(veh, pVeh.window); NAPI.Vehicle.SetVehicleMod(veh, 15, pVeh.suspension);

                        Database.Update(pVeh);
                        client.SetData("PersonalVehicle", veh);
                    }
                }
                else
                {
                    client.SendNotification("~r~Vous ne pouvez pas régler ce véhicule!");
                    return;
                }
            }
            else
            {
                client.SendNotification("Vous n'êtes pas au testeur");
                return;
            }
        }

        [Command("clearchat")]
        public void CMD_ClearChat(Client client)
        {
            if (!AdminSystem.HasRank(client, 2))
            {
                client.SendNotification("~r~Vous n'avez pas l'autorisation!");
                return;
            }

            for (int i = 0; i < 99; i++) NAPI.Chat.SendChatMessageToAll("~w~");
            NAPI.Chat.SendChatMessageToAll($"~r~[SERVER]: ~w~{client.Name} à nettoyé le chat!");

        }

        [Command("makeadmin")]
        public void CMD_MakeAdmin(Client client, string playerName, int rank)
        {
            Client player = NAPI.Player.GetPlayerFromName(playerName);

            if (!AdminSystem.HasRank(client, 3))
            {
                client.SendNotification("~r~Vous n'avez pas l'autorisation!");
                return;
            }

            if (player != null)
            {
                if (AdminSystem.SetRank(client, playerName, rank))
                {
                    client.SendNotification("Le rang à était défini!");
                    return;
                }
                else
                {
                    client.SendNotification("Le rang n'a pas était défini!");
                    return;
                }
            } else
            {
                client.SendChatMessage("Le joueur n'a pas été trouvé!");
                return;
            }

        }

        [Command("makeleader")]
        public void CMD_MakeLeader(Client client, string player, int rank)
        {
            Client target = NAPI.Player.GetPlayerFromName(player);

            if (!AdminSystem.HasRank(client, 2))
            {
                client.SendNotification("~r~Vous n'avez pas l'autorisation!");
                return;
            }

            if (LeaderSystem.SetRank(player, rank))
            {
                client.SendNotification($"[~r~Server~w~] Joueur {player} était {LeaderSystem.GetSetLeaderText(target)}");
                PlayerData.Respawn(client);
                return;
            } else
            {
                client.SendNotification($"[~r~Server~w~] Joueur {player} ne peut pas être nommé chef!");
                return;
            }
        }

        [Command("kick")]
        public void CMD_Kick(Client client, Client player, string grund)
        {
            KickLog Kicklog = PlayerHelper.KickLogs(client);

            if (!AdminSystem.HasRank(client, 1))
            {
                client.SendNotification("~r~Vous n'avez pas l'autorisation!");
                return;
            }

                NAPI.Chat.SendChatMessageToAll($"[~r~SERVER~w~] Le joueur {player.Name} était à cause de: {grund}, ~y~coups de pied!");
                Kicklog = new KickLog();
                Kicklog.kicked = player.Name;
                Kicklog.kickedby = client.Name;
                Kicklog.grund = grund;
                Database.Upsert(Kicklog);
                player.Kick();
        }

        [Command("unban")]
        public void CMD_UnBan(Client client, Client player)
        {
            Players tarInfo = PlayerHelper.GetPlayer(player);

            if (!AdminSystem.HasRank(client, 2))
            {
                client.SendNotification("~r~Vous n'avez pas l'autorisation!");
                return;
            }

            if (tarInfo == null)
            {
                client.SendNotification("Le joueur n'a pas pu être trouvé dans la base de données.");
                return;
            }

            if (tarInfo.ban <= 0 && tarInfo.ban >= 2)
            {
                client.SendNotification("Le joueur n'est pas banni!");
                return;
            }

            if (tarInfo.ban == 1)
            {
                tarInfo.ban = 0;
                client.SendNotification("Le joueur a été deban avec succès!");
                tarInfo.Update();
            }
        }

        [Command("ban")]
        public void CMD_Ban(Client client, Client player, string grund)
        {
            Players tarInfo = PlayerHelper.GetPlayer(player);
            Players p = PlayerHelper.GetPlayer(player);
            BanLog Banlog = PlayerHelper.BanLogs(client);

            if (!AdminSystem.HasRank(client, 2))
            {
                client.SendNotification("~r~Vous n'avez pas l'autorisation!");
                return;
            }

            if (p.username == null)
            {
                client.SendNotification("Le joueur n'existe pas!");
            }

            tarInfo.ban = 1;
            Database.Update(tarInfo);
            tarInfo.Update();
            player.SendChatMessage("~r~Vous avez été banni!");
            NAPI.Chat.SendChatMessageToAll($"[~r~SERVER~w~]Le joueur {player.Name} à cause de: {grund}, ~r~ban");
            client.SendNotification($"Vous avez ban le joueur { player.Name} avec succès!");
            Banlog = new BanLog();
            Banlog.banned = player.Name;
            Banlog.bannedby = client.Name;
            Banlog.grund = grund;
            Database.Upsert(Banlog);
            player.Kick();
        }

        [Command("tempban")]
        public void CMD_TempBan(Client player)
        {
            //Befehl hier einfügen
            //Spieler wird vom Server Zeitlich ausgeschlossen
        }

        [Command("settime")]
        public void CMD_SetTime(Client client, int hours, int minutes, int seconds)
        {
            if (!AdminSystem.HasRank(client, 2))
            {
                client.SendNotification("~r~Vous n'êtes pas autorisé à le faire!");
                return;
            }

            NAPI.World.SetTime(hours, minutes, seconds);
        }

        [Command("setweather")]
        public void CMD_SetWeather(Client client, string weather)
        {
            if (!AdminSystem.HasRank(client, 2))
            {
                client.SendNotification("~rVous n'êtes pas autorisé à le faire!");
                return;
            }

            NAPI.World.SetWeather(weather);
        }

        [Command("gethere")]
        public void CMD_GetHere(Client client, Client player)
        {
            if (!AdminSystem.HasRank(client, 1))
            {
                client.SendNotification("~r~Vous n'avez aucune autorisation!");
                return;
            }

            player.Position = client.Position;
        }

        [Command("goto")]
        public void CMD_GoTo(Client client, Client player)
        {
            if (!AdminSystem.HasRank(client, 1))
            {
                client.SendNotification("~r~Vous n'avez aucune autorisation!");
                return;
            }

            client.Position = player.Position;
        }

        [Command("tp")]
        public void CMD_Teleport(Client client, Client player1, Client player2)
        {

            if (!AdminSystem.HasRank(client, 1))
            {
                client.SendNotification("~r~Vous n'avez aucune autorisation!");
                return;
            }

            player1.Position = player2.Position;
        }

        [Command("getweapon")]
        public void CMD_GetWeapon(Client client, WeaponHash hash)
        {
            if (!AdminSystem.HasRank(client, 2))
            {
                client.SendNotification("~r~Vous n'avez aucune autorisation!");
                return;
            }

                client.GiveWeapon(hash, 999);
        }

        [Command("sethealth")]
        public void CMD_SetHealth(Client client, int wert)
        {

            if (!AdminSystem.HasRank(client, 3))
            {
                client.SendNotification("~r~Vous n'avez aucune autorisation!");
                return;
            }

                client.Health = wert;
        }

        [Command("ac")]
        public void CMD_AC(Client client, string message)
        {
            if (!AdminSystem.HasRank(client, 1))
            {
                client.SendNotification("~r~Vous n'avez aucune autorisation!");
                return;
            }

                NAPI.Chat.SendChatMessageToAll($"[~r~AC~w~] {client.Name} dit: {message}");
        }

        [Command("setmoney")]
        public void CMD_setmoney(Client client, Client player, double amount)
        {
            if (!client.HasData("ID"))
                return;

            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(client);
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(player);

            if (!AdminSystem.HasRank(client, 2))
            {
                client.SendNotification("~r~Vous n'avez aucune autorisation!");
                return;
            }

            client.SendNotification($"Vous avez donné à ~y~{player.Name}~w~  ~g~{amount}$~w~ $!");
            player.SendNotification($"Vous avez obtenu ~g~{amount}$~w~ ");

            pInfo.AddMoney(amount);

            Database.Update(pInfo);
            EventTriggers.Update_Money(player);
        }
        [Command("getpos")]
        public void GetPosition(Client client, string message)
        {

            if (!AdminSystem.HasRank(client, 1))
            {
                client.SendNotification("~r~Vous n'avez aucune autorisation!");
                return;
            }

                Vector3 PlayerPos = NAPI.Entity.GetEntityPosition(client);
                Vector3 rPlayerPos = NAPI.Entity.GetEntityRotation(client);
                NAPI.Chat.SendChatMessageToPlayer(client, $"X:  {PlayerPos.X} Y: {PlayerPos.Y} Z: {PlayerPos.Z} | {message} |");
                NAPI.Chat.SendChatMessageToPlayer(client, $"rX: {rPlayerPos.X} rY: {rPlayerPos.Y} rZ {rPlayerPos.Z}");
                Console.WriteLine($"X: {PlayerPos.X} Y: {PlayerPos.Y} Z: {PlayerPos.Z}  | rX: {rPlayerPos.X} rY: {rPlayerPos.Y} rZ {rPlayerPos.Z} | {message} |");
        }

        [Command("testv")]
        public void CMD_TestVehicle(Client client, string fahrzeug_modell)
        {
            uint hash = NAPI.Util.GetHashKey(fahrzeug_modell);
            Vehicle veh = NAPI.Vehicle.CreateVehicle(hash, client.Position, client.Rotation.Z, 0, 0);
            NAPI.Vehicle.SetVehicleNumberPlate(veh, client.Name);
            client.SetIntoVehicle(veh, -1);
        }

        [Command("veh")]
        public void CMD_CreateVeh(Client client, string fahrzeug_modell)
        {
            int client_id = client.GetData("ID");

            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(client);
            PlayerVehicles pVeh = PlayerHelper.GetpVehiclesStats(client);

            if (!AdminSystem.HasRank(client, 2))
            {
                client.SendNotification("~r~Vous n'avez aucune autorisation!");
                return;
            }

                if (pVeh == null)
                {
                    uint hash = NAPI.Util.GetHashKey(fahrzeug_modell);
                    Vehicle veh = NAPI.Vehicle.CreateVehicle(hash, client.Position, client.Rotation.Z, 0, 0);
                    NAPI.Vehicle.SetVehicleNumberPlate(veh, client.Name);
                    client.SetIntoVehicle(veh, -1);

                    veh.Locked = true;
                    pVeh = new PlayerVehicles();
                    pVeh._id = client_id;
                    pVeh.carslot += 1;
                    pVeh.carmodel = fahrzeug_modell;
                    pVeh.last_location = new double[] { veh.Position.X, veh.Position.Y, veh.Position.Z };
                    pVeh.last_rotation = veh.Rotation.Z;

                    //TUNES
                    pVeh.Color1 = 0; pVeh.Color2 = 0; pVeh.spoilers = -1;pVeh.fbumber = -1; pVeh.rbumber = -1;pVeh.sskirt = -1;pVeh.exhaust = -1;pVeh.frame = -1;
                    pVeh.grill = -1; pVeh.roof = -1;pVeh.motortuning = -1; pVeh.brakes = -1;pVeh.transmission = -1; pVeh.turbo = -1; pVeh.fwheels = -1; pVeh.bwheels = -1;
                    pVeh.window = -1;

                    NAPI.Vehicle.SetVehiclePrimaryColor(veh, pVeh.Color1); NAPI.Vehicle.SetVehicleSecondaryColor(veh, pVeh.Color2); NAPI.Vehicle.SetVehicleMod(veh, 0, pVeh.spoilers);
                    NAPI.Vehicle.SetVehicleMod(veh, 1, pVeh.fbumber); NAPI.Vehicle.SetVehicleMod(veh, 2, pVeh.rbumber); NAPI.Vehicle.SetVehicleMod(veh, 3, pVeh.sskirt);
                    NAPI.Vehicle.SetVehicleMod(veh, 4, pVeh.exhaust);NAPI.Vehicle.SetVehicleMod(veh, 5, pVeh.frame); NAPI.Vehicle.SetVehicleMod(veh, 6, pVeh.grill);NAPI.Vehicle.SetVehicleMod(veh, 10, pVeh.roof);
                    NAPI.Vehicle.SetVehicleMod(veh, 11, pVeh.motortuning);NAPI.Vehicle.SetVehicleMod(veh, 12, pVeh.brakes); NAPI.Vehicle.SetVehicleMod(veh, 13, pVeh.transmission);
                    NAPI.Vehicle.SetVehicleMod(veh, 18, pVeh.turbo);NAPI.Vehicle.SetVehicleMod(veh, 23, pVeh.fwheels); NAPI.Vehicle.SetVehicleMod(veh, 24, pVeh.bwheels); //MOTORAD
                    NAPI.Vehicle.SetVehicleWindowTint(veh, pVeh.window);

                    Database.Upsert(pVeh);
                    client.SetData("PersonalVehicle", veh);
                    veh.SetData("ID", client_id);
                }
                else if (pVeh._id == client_id)
                {
                    Vehicle previous_vehicle = client.GetData("PersonalVehicle");
                    previous_vehicle.Delete();

                    uint hash = NAPI.Util.GetHashKey(fahrzeug_modell);
                    Vehicle veh = NAPI.Vehicle.CreateVehicle(hash, client.Position, client.Rotation.Z, 0, 0);
                    NAPI.Vehicle.SetVehicleNumberPlate(veh, client.Name);
                    client.SetIntoVehicle(veh, -1);

                    veh.Locked = true;
                    pVeh = new PlayerVehicles();
                    pVeh._id = client_id;
                    pVeh.carslot += 1;
                    pVeh.carmodel = fahrzeug_modell;
                    pVeh.last_location = new double[] { veh.Position.X, veh.Position.Y, veh.Position.Z };
                    pVeh.last_rotation = veh.Rotation.Z;

                    //TUNES
                    pVeh.Color1 = 0; pVeh.Color2 = 0; pVeh.spoilers = -1; pVeh.fbumber = -1; pVeh.rbumber = -1; pVeh.sskirt = -1; pVeh.exhaust = -1; pVeh.frame = -1;
                    pVeh.grill = -1; pVeh.roof = -1; pVeh.motortuning = -1; pVeh.brakes = -1; pVeh.transmission = -1; pVeh.turbo = -1; pVeh.fwheels = -1; pVeh.bwheels = -1;
                    pVeh.window = -1; pVeh.suspension = -1;

                    NAPI.Vehicle.SetVehiclePrimaryColor(veh, pVeh.Color1); NAPI.Vehicle.SetVehicleSecondaryColor(veh, pVeh.Color2); NAPI.Vehicle.SetVehicleMod(veh, 0, pVeh.spoilers);
                    NAPI.Vehicle.SetVehicleMod(veh, 1, pVeh.fbumber); NAPI.Vehicle.SetVehicleMod(veh, 2, pVeh.rbumber); NAPI.Vehicle.SetVehicleMod(veh, 3, pVeh.sskirt);
                    NAPI.Vehicle.SetVehicleMod(veh, 4, pVeh.exhaust); NAPI.Vehicle.SetVehicleMod(veh, 5, pVeh.frame); NAPI.Vehicle.SetVehicleMod(veh, 6, pVeh.grill); NAPI.Vehicle.SetVehicleMod(veh, 10, pVeh.roof);
                    NAPI.Vehicle.SetVehicleMod(veh, 11, pVeh.motortuning); NAPI.Vehicle.SetVehicleMod(veh, 12, pVeh.brakes); NAPI.Vehicle.SetVehicleMod(veh, 13, pVeh.transmission);
                    NAPI.Vehicle.SetVehicleMod(veh, 18, pVeh.turbo); NAPI.Vehicle.SetVehicleMod(veh, 23, pVeh.fwheels); NAPI.Vehicle.SetVehicleMod(veh, 24, pVeh.bwheels); //MOTORAD
                    NAPI.Vehicle.SetVehicleWindowTint(veh, pVeh.window); NAPI.Vehicle.SetVehicleMod(veh, 15, pVeh.suspension);

                    Database.Update(pVeh);

                    client.SetData("PersonalVehicle", veh);
                    veh.SetData("ID", client_id);
                }

                return;
        }
    }
}