using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using TerrariaApi.Server;
using TShockAPI;
using Terraria;
using Newtonsoft.Json;
using System.IO;

namespace yukigassen
{
    [ApiVersion(2, 1)]
    public class Yukigassen : TerrariaPlugin
    {
        private static readonly string CONFIG_PATH = Path.GetFullPath(Path.Combine(TShock.SavePath, "SnowConfig.json"));
        public Config Config { get; private set; }

        private static int timer = 0;
        private static bool isInGame = false;

        public static readonly List<Player> activePlayers = new List<Player>();

        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
        public override string Author
        {
            get { return "Ranga"; }
        }
        public override string Name
        {
            get { return "Yukigassen"; }
        }

        public override string Description
        {
            get { return "Yukigassen de asobou"; }
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameUpdate.Register(this, Update);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, Update);
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreetPlayer);
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
            }
            base.Dispose(disposing);
        }

        public Yukigassen(Main game)
            : base(game)
        {
            Order = 1;
        }

        private void OnInitialize(EventArgs e)
        {
            Commands.ChatCommands.Add(new Command(Permissions.godmodeother, Snow_on, "snon"));
            Commands.ChatCommands.Add(new Command(Permissions.godmodeother, Snow_off, "snoff"));
        }
        
        private static void OnGreetPlayer(GreetPlayerEventArgs args)
        {
            lock (activePlayers)

                activePlayers.Add(new Player(args.Who));
        }
        private static void OnLeave(LeaveEventArgs args)
        {
            lock (activePlayers)
                activePlayers.RemoveAll(plr => plr.Index == args.Who);
        }
        

        #region TogglePvP

        private void Snow_on(CommandArgs args)
        {
            foreach (var player in activePlayers)
            {
                player.InitPoint();
                isInGame = true;
                LoadFromConfig();
            }
            TSPlayer.All.SendInfoMessage(string.Format("雪合戦 開始!!", args.Player.Name));
        }
        private static void Snow_off(CommandArgs args)
        {
            foreach (var player in activePlayers)
            {
                isInGame = false;
            }
            TSPlayer.All.SendInfoMessage(string.Format("雪合戦 終了!!", args.Player.Name));

        }

        private void Update(EventArgs args)
        {   
            timer++;
            if (timer % 10 != 0 || !isInGame) return;

            switch (Config.Mode)
            {
                /*
                case 0:
                    timer++;

                    foreach (var player in activePlayers)
                    {
                        if (timer % 10 == 0)
                        {
                            player.isAddPoint = true;
                        }

                        if (player.isAddPoint && player.Point < 100) player.AddPoint(5);

                        if (player.Point == 100)
                        {
                            player.TSPlayer.SetBuff(1, 1000);
                            player.isAddPoint = false;
                        }
                    }
                    break;
                */
                case 0:

                    foreach (var player in activePlayers)
                    {
                        if (player.TPlayer.IsStandingStillForSpecialEffects)
                            player.isAddPoint = true;
                        else 
                            player.isAddPoint = false;

                        if ((player.isAddPoint && player.Point < 100)) player.AddPoint(Config.IncPoint); //player.Point+=2;
                        else if (player.Point <= 100 && player.Point > 0) player.RemovePoint(Config.DecPoint);  //player.Point-=5;

                        if (player.Point == 100)
                        {
                            player.TSPlayer.SetBuff(Config.BuffId, Config.BuffDuration);

                            player.isAddPoint = false;
                        }
                    }
                    break;

            }
            //                        player.TSPlayer.TPlayer.honeyWet
            //                        player.TSPlayer.TPlayer.KeyDoubleTap(1);

            SendText();
        }

        private static void SendText()
        {
            foreach (var player in activePlayers)
            {
                if (player != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(LineBreaks(4));
                    //string text = string.Format("[c/FFF014:"+timer+ player.PlayerName +":" + "dps = "+ TShock.Players[player.Index].TPlayer.moveSpeed  + ", getDPS = "+ player.MoveSpeed + "]" + "\r\n");
                    string text = string.Format("[c/FFF014:" + player.PlayerName + "]" + "\r\n");


                    text += string.Format(":  "+ player.Point);

                    sb.Append(text);
                    sb.Append(LineBreaks(50));

                    BitsByte flags = new();

                    // Status Textに黒い縁をつける.
                    flags[1] = true;
                    player.TSPlayer.SendData(PacketTypes.Status, sb.ToString(), number2: flags);
                }
            }
        }

        public void LoadFromConfig()
        {
            string path = CONFIG_PATH;


            if (File.Exists(path))
            {
                using (var sr = new StreamReader(path))
                {
                    Config = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
                }
            }
            else
            {
                Config = new Config();
                using (var sw = new StreamWriter(path))
                {
                    sw.Write(JsonConvert.SerializeObject(Config, Formatting.Indented));
                }
            }
        }

        private static string LineBreaks(int count)
        {
            string line = "";
            for (int i = 0; i < count; i++)
                line += "\r\n";
            return line;
        }
    }




        

        #endregion
    
}
