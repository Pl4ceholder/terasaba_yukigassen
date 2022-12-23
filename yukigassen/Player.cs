using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;
using TerrariaApi;

using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI.DB;
using TShockAPI.Hooks;
using TShockAPI.Net;
using IL.Terraria;
using On.Terraria;

namespace yukigassen
{
    public class Player
    {
        public string PvPType = "";
        public int Index;
        public bool isInGame = false;
        public bool isAddPoint = false;
        public TSPlayer TSPlayer { get { return TShock.Players[Index]; } }

        public Terraria.Player TPlayer { get { return TShock.Players[Index].TPlayer; } }
        public string PlayerName { get { return Terraria.Main.player[Index].name; } }

        public float MoveSpeed { get { return Terraria.Main.player[Index].moveSpeed; } }

        public int Point = 0;

        public Player(int index)
        {
            Index = index;
        }
        public void InitPoint()
        {
            Point = 0;
        }

        public void AddPoint(int point)
        {
            Point = Math.Min(100, Point + point);
        }

        public void RemovePoint(int point)
        {
            Point = Math.Max(0, Point - point);
        }
    }
}
