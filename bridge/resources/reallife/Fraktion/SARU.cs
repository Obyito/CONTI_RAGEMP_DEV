﻿using GTANetworkAPI;
using reallife.Data;
using reallife.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace reallife.Fraktion
{
    class SARU : Script
    {
        public SARU()
        {
            //3D Textelemente
            NAPI.TextLabel.CreateTextLabel("Utilisez  ~r~/fcar~w~ pour faire apparaître une ambulance.", new Vector3(1193.662, -1487.571, 34.84266), 12, 1f, 4, new Color(255, 255, 255, 255));

            NAPI.TextLabel.CreateTextLabel("Utilisez ~r~/duty~w~  pour vous rendre en service.", new Vector3(1207.794, -1487.555, 34.84264), 12, 1f, 4, new Color(255, 255, 255, 255));

            NAPI.TextLabel.CreateTextLabel("Utilisez ~r~/fduty~w~ pour aller en service.", new Vector3(1194.831, -1477.961, 34.85954), 12, 1f, 4, new Color(255, 255, 255, 255));

            NAPI.TextLabel.CreateTextLabel("Utilisez ~r~/fcar~w~ pour faire apparaître un camion de pompiers.", new Vector3(1191.315, -1474.554, 34.85954), 12, 1f, 4, new Color(255, 255, 255, 255));

            //Checkpoint
            //NAPI.Checkpoint.CreateCheckpoint(0, new Vector3(452.2535, -980.0837, 27.3896), new Vector3(0, 1, 0), 1f, new Color(198, 40, 40, 200));
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void OnPlayerEnterSARUVehicle(Client client, Vehicle vehicle, sbyte seatID)
        {
            PlayerInfo playerInfo = PlayerHelper.GetPlayerStats(client);

            uint ambulance = NAPI.Util.GetHashKey("ambulance");
            uint firetruk = NAPI.Util.GetHashKey("firetruk");

            if (vehicle.Model == ambulance || vehicle.Model == firetruk)
            {
                if (client.VehicleSeat == -1)
                {
                    if (!FraktionSystem.HasRank(client, 2))
                    {
                        client.SendNotification("~r~Vous n'êtes pas autorisé à le faire!");
                        client.WarpOutOfVehicle();
                    }
                }
            }
        }
    }
}
