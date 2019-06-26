using GTANetworkAPI;
using reallife.Db;
using reallife.Player;

namespace reallife.Events
{
    class LoginRegisterEvent : Script
    {
        [RemoteEvent("OnPlayerCharacterAttempt")]
        public void OnPlayerCharacterAttempt(Client client, string vorname, string nachname)
        {
            int pInfo = client.GetData("ID");
            PlayerInfo playerInfo = Database.GetById<PlayerInfo>(pInfo);


            /*if(Database.GetData<PlayerInfo>("vorname", vorname, "nachname", nachname) != null)
            {
                //client.SendChatMessage("Der Vor/nachname ist in der Kombination schon vorhanden!");
                client.TriggerEvent("CharacterResult", 0);
                return;
            }*/

            playerInfo.vorname = vorname;
            playerInfo.nachname = nachname;


            Database.Upsert(playerInfo);

            NAPI.Player.SetPlayerName(client, playerInfo.vorname + "" + playerInfo.nachname);

            for (int i = 0; i < 99; i++) client.SendChatMessage("~w~");
            client.TriggerEvent("CharacterResult", 1);
        }

        [RemoteEvent("OnPlayerRegisterAttempt")]
        public void OnPlayerRegisterAttempt(Client client, string username, string password, string passwordre)
        {
            int adminrank = 0;
            string socialclub = client.SocialClubName;
            string vorname = "None";
            string nachname = "None";

            PlayerInfo pInfo = Database.GetData<PlayerInfo>("username", username);

            Players players = new Players(username, password, socialclub);
            PlayerInfo playerInfo = new PlayerInfo(adminrank, vorname, nachname);

            if (password != passwordre)
            {
                client.TriggerEvent("RegisterResult", 0);
                return;
            }

            if (Database.GetData<Players>("username", username) != null)
            {
                client.SendChatMessage("Ce nom est déjà pris.");
                client.TriggerEvent("RegisterResult", 0);
                return;
            }

            playerInfo.Upsert();
            players.Upsert();
            for (int i = 0; i < 99; i++) client.SendChatMessage("~w~");
            client.SendChatMessage("Création du personnage réalisé avec succès !");
            client.SendChatMessage("Vous pouvez vous connecter avec ces identifiants!");

            client.TriggerEvent("RegisterResult", 3);
        }

        [RemoteEvent("OnPlayerLoginAttempt")]
        public void OnPlayerLoginAttempt(Client client, string username, string password)
        {
            Players players = Database.GetData<Players>("username", username);
            PlayerVehicles pVeh = PlayerHelper.GetpVehiclesStats(client);

            if (players == null)
            {
                client.SendChatMessage("~r~Données introuvables !");
                client.TriggerEvent("LoginResult");
                return;
            }

            if (!players.CheckPassword(password))
            {
                client.SendChatMessage("~r~Les données n'ont pas été trouvés!");
                client.TriggerEvent("LoginResult", 0);
                return;
            }

            if (client.HasData("ID"))
            {
                client.SendChatMessage("Vous êtes connecté !");
                return;
            }
            client.SetData("ID", players._id);

            //LOGIN ENDE
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(client);
            Players playerInfo = PlayerHelper.GetPlayer(client);

            client.SetData("AdminRank", pInfo.adminrank);

            if (playerInfo.ban == 0)
            {
                Handler.FinishLogin(client);
                client.TriggerEvent("LoginResult", 1);

                    for (int i = 0; i < 99; i++) client.SendChatMessage("~w~");
                    //GUIDE START
                    if (pInfo.vorname == "None")
                    {
                        client.SendChatMessage("~r~SERVEUR: ~ w ~ Veuillez choisir un nom/prénom.");
                        NAPI.ClientEvent.TriggerClientEvent(client, "StartCharBrowser");
                        return;
                    }
                    else
                    {
                        client.SendChatMessage($"Bienvenue, {pInfo.vorname} {pInfo.nachname} sur ~g~Continental Five");
                        client.SendChatMessage("~r~SERVEUR: Ceci est une version développeur de Continental Five");
 
                        client.SendNotification($"~g~En tant que : {pInfo.vorname} {pInfo.nachname}");
                        return;
                    }
            }
            else
            {
                client.SendChatMessage("Ce compte à été suspendu");
                client.TriggerEvent("LoginResult", 0);
                return;
            }
        }

    }
}
