using GTANetworkAPI;
using reallife.Player;
using reallife.Db;
using System;
using System.Collections.Generic;
using System.Text;
using reallife.Events;

namespace reallife.Bank
{
    public class TriggerBank : Script
    {
        //-------------------[Bankkonto Change Pin]-------------------//
        [RemoteEvent("test")]
        public void test(Client client, int handgeld)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(client);
            handgeld = 1000;
        }
        [RemoteEvent("OnPlayerPinChange")]
        public void OnPlayerPinChange(Client client, int opin, int npin, int npinre)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(client);

            if (pInfo.bkontopin != opin)
            {
                client.SendChatMessage("[~g~Bank~w~] Le code pin est faux");
                client.TriggerEvent("bChangepinResult", 0);
                return;
            }
            else if (npin != npinre)
            {
                client.SendChatMessage("[~g~Bank~w~] Le nouveau code pin ne correspond pas !");
                client.TriggerEvent("bChangepinResult");
                return;
            }
            else
            {
                client.SendChatMessage($"[~g~Bank~w~] Vous avez changez votre code pin avec succès {npin}");
                pInfo.bkontopin = npin;
                client.TriggerEvent("bChangepinResult", 1);
                Database.Update(pInfo);
            }
        }
        //-------------------[Bankkonto Login]-------------------//
        [RemoteEvent("OnplayerbKontoLogin")]
        public void OnplayerbKontoLogin(Client client, int pin)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(client);

            if (pInfo.bkontopin != pin)
            {
                client.SendChatMessage("[~g~Bank~w~] Mauvais code, veuillez réessayez");
                client.TriggerEvent("bKontoLoginResult", 0);
                return;
            }
            else
            {
                client.SendChatMessage("[~g~Bank~w~] Vous vous êtes connecté avec succès!");
                NAPI.ClientEvent.TriggerClientEvent(client, "StartBankBrowser");
                client.TriggerEvent("bKontoLoginResult", 1);
            }
        }
        //-------------------[Bankkonto Erstellung]-------------------//
        [RemoteEvent("OnPlayerbKonto")]
        public void OnPlayerbKonto(Client client, int pin, int repin)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(client);

            if (pin != repin)
            {
                client.SendChatMessage("Votre code pin est faux");
                client.TriggerEvent("bKontoResult", 0);
                return;
            }
            else
            {
                client.SendChatMessage($"[~g~Bank~w~] Votre pin est {pin}");
                client.SendChatMessage($"[~g~Bank~w~] Vous pouvez toujours changer votre PIN dans une banque!");
                pInfo.bkontopin += pin;
                pInfo.bkonto += 1;

                client.TriggerEvent("bKontoResult", 1);
                Database.Update(pInfo);
            }
        }
        //-------------------[Bankkonto Einzahlung/Auszahlung/Überweisung]-------------------//
        [RemoteEvent("OnPlayerUberweisungAttempt")]
        public void OnPlayerUberweisungAttempt(Client client, string name, int summe)
        {
            if (!client.HasData("ID"))
                return;

            Client player = NAPI.Pools.GetAllPlayers().Find(x => x.Name == name);

            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(client);
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(player);
            PlayerInfo otherInfo = Database.GetById<PlayerInfo>(pInfo._id);

            string spielername = pInfo.vorname + pInfo.nachname;

            /*if (name)
            {
                client.SendChatMessage("[~g~BANK~w~] Diese Person existiert nicht!");
                client.TriggerEvent("BankResult", 0);
                return;
            }*/

            if (spielername == null)
            {
                client.SendChatMessage("[~g~BANK~w~] Cette personne n'existe pas");
                client.TriggerEvent("BankResult", 0);
                return;
            }

            if (summe <= 0)
            {
                client.SendChatMessage("[~g~BANK~w~] Votre montant est trop petit!");
                client.TriggerEvent("BankResult", 0);
                return;
            }
            else if (playerInfo.bank < summe)
            {
                client.SendChatMessage("[~g~BANK~w~] Votre crédit ne suffit pas!");
                client.TriggerEvent("BankResult", 0);
                return;
            }
            if (client.Name == player.Name)
            {
                client.SendChatMessage($"[~g~BANK~w~] Vous ne pouvez pas transférer de l'argent vous-même!");
                client.TriggerEvent("BankResult", 0);
                return;
            }
            else
            {
                client.SendChatMessage($"[~g~BANK~w~] Tu as payé  ~g~{summe}$~w~ à {otherInfo.vorname}{otherInfo.nachname} ");
                player.SendChatMessage($"[~g~BANK~w~]  {playerInfo.vorname}{playerInfo.nachname} t'as payé ~g~{summe}$~w~ ");

                playerInfo.bank -= summe;
                otherInfo.bank += summe;

                Database.Update(playerInfo);
                EventTriggers.Update_Bank(client);

                Database.Update(otherInfo);
                EventTriggers.Update_Bank(player);
            }
        }
        [RemoteEvent("OnPlayerAuszahlung")]
        public void OnPlayerAuszahlung(Client client, int summe)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(client);

            if (pInfo.bank < summe)
            {
                client.SendChatMessage("~r~Vous n'avez pas assez d'argent à la banque!");
                client.TriggerEvent("BankResult", 0);
                return;
            }
            else
            {
                pInfo.bank -= summe;
                pInfo.money += summe;
                client.SendChatMessage($"~w~Tu as~g~${summe} ~w~suite à la levé de votre compte");
                client.SendChatMessage($"~w~Nouveau solde du compte  ~g~${pInfo.bank}~w~ | trésorerie:: ~g~${pInfo.money}");
                client.TriggerEvent("BankResult", 1);
                Database.Upsert(pInfo);

                EventTriggers.Update_Money(client);
                EventTriggers.Update_Bank(client);
            }
        }
        [RemoteEvent("OnPlayerEinzahlung")]
        public void OnPlayerEinzahlung(Client client, int summe)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(client);

            if (pInfo.money < summe)
            {
                client.SendChatMessage("~r~Vous n'avez pas assez d'argent!");
                client.TriggerEvent("BankResult", 0);
                return;
            }
            else
            {
                pInfo.money -= summe;
                pInfo.bank += summe;
                client.SendChatMessage($"~w~Vous avez déposé ~g~${summe} ~w~sur votre compte");
                client.SendChatMessage($"~w~Nouveau solde du compte ~g~${pInfo.bank}~w~ | trésorerie: ~g~${pInfo.money}");
                client.TriggerEvent("BankResult", 1);
                Database.Upsert(pInfo);

                EventTriggers.Update_Money(client);
                EventTriggers.Update_Bank(client);
            }
        }
    }
}
