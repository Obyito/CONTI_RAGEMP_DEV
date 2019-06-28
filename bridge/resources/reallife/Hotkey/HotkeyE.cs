using GTANetworkAPI;
using reallife.Player;
using reallife.Db;
using reallife.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace reallife.Hotkey
{
    public class HotkeyE : Script
    {
        bool isMenuOpen = false;


        [RemoteEvent("testanzeige")]
        public void testanzeige(Client client)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(client);

            double[] money = new[] {pInfo.money};

            var dataInJson = NAPI.Util.ToJson(money);

            client.TriggerEvent("showgeldanzeige", NAPI.Util.ToJson(money));
        }
        [RemoteEvent("OpenMosMenu")]
        public void OpenMosMenu(Client client)
        {
            client.TriggerEvent("StartAutohausBrowser");
        }
        [RemoteEvent("OpenATMcode")]
        public void OpenATMcode(Client client)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(client);

            if (pInfo.bkonto == 0)
            {
                client.SendChatMessage("[~g~BANK~w~] Du besitzt kein Bankkonto!");
                return;
            }
            else
            {
                //client.SendChatMessage("PIN OPEN");
                //NAPI.ClientEvent.TriggerClientEvent(client, "StartbKontoLoginBrowser");
                if (isMenuOpen == false)
                {
                    client.TriggerEvent("StartbKontoLoginBrowser");
                    isMenuOpen = true;
                }
                else
                {
                    client.TriggerEvent("CloseMenu");
                    isMenuOpen = false;
                }
            }
        }
        [RemoteEvent("bKontoErst")]
        public void bKontoErst(Client client)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(client);
            if (pInfo.bkonto == 1)
            {
                client.SendChatMessage("[~g~BANK~w~] Vous possédez déjà un compte bancaire!");
                // client.TriggerEvent("bKontoResult", 0);
                return;
            }
            else
            {
                if (isMenuOpen == false)
                {

    
                    //client.SendChatMessage("PIN OPEN");
                    NAPI.ClientEvent.TriggerClientEvent(client, "StartbKontoBrowser");
                    isMenuOpen = true;
                }
                else
                {
                    NAPI.ClientEvent.TriggerClientEvent(client, "ClosePinAuswahl");
                    isMenuOpen = false;
                }
            }
        }
        [RemoteEvent("bKontob")]
        public void bKontob(Client client)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(client);
            if (pInfo.bkonto == 0)
            {
                client.SendChatMessage("[~g~Bank~w~] Vous n'avez pas de compte bancaire!");
                return;
            }
            else
            {
                if (isMenuOpen == false)
                {
                    NAPI.ClientEvent.TriggerClientEvent(client, "StartPinChangeBrowser");
                    isMenuOpen = true;
                }
                else
                {
                    NAPI.ClientEvent.TriggerClientEvent(client, "ClosePinChaneAuswahl");
                    isMenuOpen = false;
                }
            }
        }


        [RemoteEvent("HotRent")]
        public void HotRent(Client client)
        {
          

            if (isMenuOpen == false)
            {
                NAPI.ClientEvent.TriggerClientEvent(client, "OpenRentBrowser");
                isMenuOpen = true;
            }
            else
            {
                NAPI.ClientEvent.TriggerClientEvent(client, "RentVehicleClose");
                isMenuOpen = false;
            }


        }
        [RemoteEvent("RentSpawnCarRoller1")]
        public void RentSpawnCarRoller(Client client)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(client);

            if (client.HasData("RentVehicle"))
            {
                client.SendNotification("~y~Vous avez déjà loué un véhicule !");
                client.SendChatMessage("~r~Avertissement:~w~ Faite /unrent pour résilier la location !");
                return;
            }

            uint rveh = NAPI.Util.GetHashKey("faggio2");

            Vehicle veh = NAPI.Vehicle.CreateVehicle(rveh, new Vector3(-1151.06201171875, -716.578186035156, 20.6585292816162), 311.515930175781f, 0, 0);
            NAPI.Vehicle.SetVehicleNumberPlate(veh, client.Name);
            client.SetIntoVehicle(veh, -1);
            veh.Locked = true;

            client.SendChatMessage("~r~Avertissement:~w~ /lock - Pour bloquer/débloquer votre véhicule");
            client.SendChatMessage("~r~Avertissement:~w~/motor - Pour démarrer le moteur");

            pInfo.SubMoney(150);
            Database.Update(pInfo);

            EventTriggers.Update_Money(client);

            client.SendNotification("Vous avez payez ~g~150$~w~ pour la location.");

            client.SetData("RentVehicle", veh);
            isMenuOpen = false;
        }
        [RemoteEvent("RentSpawnCarRoller2")]
        public void RentSpawnCarRoller2(Client client)
        {
            PlayerInfo pInfo = PlayerHelper.GetPlayerStats(client);

            if (client.HasData("RentVehicle"))
            {
                client.SendNotification("~y~Vous avez déjà loué un véhicule !");
                client.SendChatMessage("~r~Avertissement:~w~ Faite /unrent pour résilier la location !");
                return;
            }

            uint rveh = NAPI.Util.GetHashKey("blista2");

            Vehicle veh = NAPI.Vehicle.CreateVehicle(rveh, new Vector3(-1151.06201171875, -716.578186035156, 20.6585292816162), 311.515930175781f, 0, 0);
            NAPI.Vehicle.SetVehicleNumberPlate(veh, client.Name);
            client.SetIntoVehicle(veh, -1);
            veh.Locked = true;

            client.SendChatMessage("~r~Avertissement::~w~ /lock - Pour bloquer/débloquer votre véhicule");
            client.SendChatMessage("~r~Avertissement:~w~ /motor - Pour démarrer le moteur");

            pInfo.SubMoney(150);
            Database.Update(pInfo);

            EventTriggers.Update_Money(client);

            client.SendNotification("Vous avez payez ~g~150$~w~ pour la location.");

            client.SetData("RentVehicle", veh);
            isMenuOpen = false;
        }
    }
}
