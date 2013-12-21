using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;
using System.Text.RegularExpressions;

namespace MushroomsUnity3DExample
{
    [RoomType("Lobby12")]
    class Lobby12 : Game<Player>
    {
        Regex regex = new Regex("^[a-zA-Z0-9]*$");

        public override void GameStarted()
        {
            PreloadPlayerObjects = true;
        }


        public override void GotMessage(Player player, Message message)
        {
            switch (message.Type)
            {
                case "setUsername":
                    {

                        string username = message.GetString(0);
                        if (username.Count() >= 3 && regex.IsMatch(username))
                        {
                            if (!player.PlayerObject.Contains("name")
                                /*|| player.PlayerObject.GetString("name") == null*/)
                            {
                                player.PlayerObject.Set("name", username);
                                player.PlayerObject.Save();
                                player.Send("username", username);
                            }
                            else
                            {
                                player.Send("error", "You already have a username!");
                            }
                        }
                        else
                        {
                            player.Send("error", "Invalid username!");
                        }
                    }
                    return;

                case "getShop":
                    {
                        Message m = Message.Create("getShop",
                            //(int)
                            65, //energy
                            20, //time to energy
                            75, // total(max) energy
                            60, //secounds between energy
                            7, //gems


                            "XMAS decorations",    // string id
                            100,                // int energy cost
                            "25",               // string cost per click
                            "0",                // string energy used
                            "5",                // string total gem cost
                            "3"                 //  string count
                            );

                        player.Send(m);
                    }
                    return;
            }
            throw new Exception(message.ToString());
        }
    }

    [RoomType("EE42")]
    class EE092 : Game<Player>
    {
        string owner = "ostkaka";
        Room room = new Room();

        List<string> modList = new List<string>(new string[] { "ostkaka", "gustav9797" });

        int[,] blocks = new int[200, 200];

        public override void GameStarted()
        {
            Console.WriteLine("Game is started: " + RoomId);

            int block = 0;
            int border = 9;

            if (this.RoomData.ContainsKey("editkey"))
            {
                string[] roomdata = this.RoomData["editkey"].Split('#');

                if (roomdata.Count() >= 2)
                {
                    int.TryParse(roomdata[1], out block);
                }
                if (roomdata.Count() >= 3)
                {
                    int.TryParse(roomdata[2], out border);

                    if (border < 9 || border > 15)
                        border = 9;
                }
            }

            for (int y = 0; y < 200; y++)
            {
                for (int x = 0; x < 200; x++)
                {
                    if (x == 0 || x == 199 || y == 0 || y == 199)
                        blocks[x, y] = (int)border;
                    else if (x <= 2 && y <= 2)
                        blocks[x, y] = 0;
                    else
                        blocks[x, y] = (int)block;
                }
            }

            //this.AddTimer(new Action(() => OnPlayerUpdate(room)), 100);

            base.GameStarted();
        }

        public override void GameClosed()
        {
            base.GameClosed();
        }

        public override void UserJoined(Player player)
        {
            /*object[] world = new object[200 * 200 + 2];

            world[0] = (int)0;
            world[1] = (int)0;

            for (int y = 0; y < 200; y++)
            {
                for (int x = 0; x < 200; x++)
                {
                    world[y * 200 + x + 2] = blocks[x, y];
                }
            }*/

            player.name = player.PlayerObject.GetString("name");
            player.IsMod = modList.Contains(player.name);

            if (this.RoomData.ContainsKey("editkey") && !player.IsMod && player.name != owner)
            {
                if (this.RoomData["editkey"].Split('#').First() == "")
                {
                    player.HasCode = true;
                    player.Send("access");
                }
            }
            else
            {
                player.HasCode = true;
                player.Send("access");
            }

            player.Send("init",
                "o", //"b" in rot13
                player.ConnectUserId,
                player.x, player.y, player.name,
                player.HasCode, (player.name == owner),
                200, 200);

            base.UserJoined(player);
        }

        public override void UserLeft(Player player)
        {
            Broadcast("left", player.ConnectUserId);

            base.UserLeft(player);
        }

        public override bool AllowUserJoin(Player player)
        {
            return base.AllowUserJoin(player);
        }

        public override void GotMessage(Player player, Message message)
        {
            if (player.IsMod)
            {

            }
            if (player.Name == owner || player.IsMod)
            {

            }


            switch (message.Type)
            {
                case "init":
                    {
                        /*foreach (Player p in Players)
                        {
                            if (p.ConnectUserId != player.ConnectUserId)
                            {
                                p.Send("add", player.ConnectUserId, player.X, player.Y);
                                player.Send("add", p.ConnectUserId, p.X, p.Y);
                            }
                        }*/

                        player.Send("info", "something room", owner);

                        foreach (Player p in Players)
                        {
                            if (p.ConnectUserId != player.ConnectUserId)
                            {
                                p.Send("add",
                                    player.ConnectUserId,
                                    player.name,
                                    (int)0,     // faceid
                                    (double)16.0,     // x
                                    (double)16.0,
                                    player.isgod,
                                    player.ismod,
                                    0);    // y

                                player.Send("add",
                                    p.ConnectUserId,
                                    p.name,
                                    (int)p.Face,     // faceid
                                    (double)p.x,     // x
                                    (double)p.y,
                                    p.isgod,
                                    p.ismod,
                                    0);    // y
                            }
                        }


                    }
                    return;

                case "init2":
                    return;

                case "face":
                    player.Face = message.GetInt(0);
                    Broadcast("face", player.ConnectUserId, message.GetInt(0));
                    return;

                case "m":   //movement
                    Broadcast("u",
                        player.ConnectUserId,
                        message.GetDouble(0),    //x
                        message.GetDouble(1),    //y
                        message.GetDouble(2),    //xspeed
                        message.GetDouble(3),    //yspeed
                        message.GetDouble(4),    //modifierx
                        message.GetDouble(5),    //modifiery
                        message.GetDouble(6),    //mx
                        message.GetDouble(7));  //my

                    player.x = message.GetDouble(0);
                    player.y = message.GetDouble(1);

                    return;

                case "b": //block
                    {
                        if (player.HasCode)
                        {
                            int x = message.GetInt(0);
                            int y = message.GetInt(1);
                            int blockId = message.GetInt(2);



                            if ((x >= 0 && y >= 0 && x <= 199 && y <= 199)
                                && ((x != 0 && x != 199 && y != 0 && y != 199) || (blockId >= 9 && blockId <= 15)))
                            {
                                blocks[x, y] = blockId;
                                Broadcast("c", x, y, blockId);
                            }
                        }
                    }
                    return;

                case "red":
                case "green":
                case "blue":
                case "hide":
                    Broadcast(message);
                    return;

                case "say":
                    return;

                case "rk":  // red key
                case "gk":  // green key
                case "bk":  // blue key
                    return;

                case "k":   // crown
                    Broadcast("k", player.ConnectUserId);
                    return;

                case "f":    // face <int id>
                    return;

                case "clear":
                    return;

                case "save":
                    return;

                case "allowpotions":    // <bool>
                    return;

                case "god":     // <bool>
                    return;

                case "name":    // <string title>
                    return;

                case "autosay": // <int>
                    return;

                //case "p":   // potion <int id>
                //    return;

                case "access":  // <string code>
                    if (this.RoomData.ContainsKey("editkey"))
                    {
                        if (this.RoomData["editkey"].Split('#').First() == message.GetString(0))
                        {
                            player.HasCode = true;
                            player.Send("access");
                        }
                    }
                    return;

                case "levelcomplete":
                    this.Broadcast(Message.Create("write", "World", player.Name + " completed the level!"));
                    return;

                //case "m":   // <double x> <double y> <double speedX> <double speedY> <Integer ModifierY> <Integer Horizontal> <Integer Vertical> <Double GravityMultiplier> <Boolean SpaceDown>
                //    return;

            }

            throw new Exception(message.ToString());
        }

        private void OnPlayerUpdate(Room room)
        {
            foreach (Movement.PhysicsPlayer p in Players)
            {
                p.tick(room);
            }
        }
    }
}
