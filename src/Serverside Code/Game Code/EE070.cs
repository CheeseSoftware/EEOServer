using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace MushroomsUnity3DExample
{
    [RoomType("FlixelWalkerFX3")]
    class EE070 : Game<Player>
    {
        string owner = "ostkaka";
        Room room = new Room();

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
            object[] messageData = new object[200 * 200 + 2];

            if (this.RoomData.ContainsKey("editkey"))
            {
                if (this.RoomData["editkey"].Split('#').First() == "")
                {
                    player.HasCode = true;
                }
            }
            else
            {
                player.HasCode = true;
            }

            messageData[0] = player.Id;
            messageData[1] = player.HasCode;

            //world data
            for (int y = 0; y < 200; y++)
            {
                for (int x = 0; x < 200; x++)
                {
                    messageData[y * 200 + x + 2] = blocks[x, y];
                }
            }

            player.Send("init", messageData);
            base.UserJoined(player);
        }

        public override void UserLeft(Player player)
        {
            Broadcast("left", player.Id);

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
                        foreach (Player p in Players)
                        {
                            if (p.Id != player.Id)
                            {
                                p.Send("add",
                                    player.Id,
                                    (int)0,     // faceid
                                    (double)16.0,     // x
                                    (double)16.0);    // y

                                player.Send("add",
                                    p.Id,
                                    (int)p.Face,     // faceid
                                    (double)p.x,     // x
                                    (double)p.y);    // y
                            }
                        }
                    }
                    return;

                case "init2":
                    return;

                case "face":
                    player.Face = message.GetInt(0);
                    Broadcast("face", player.Id, message.GetInt(0));
                    return;

                case "u":   //movement
                    Broadcast("u",
                        player.Id,
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

                case "c": //block
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
                    Broadcast("k", player.Id);
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

                case "m":   // <double x> <double y> <double speedX> <double speedY> <Integer ModifierY> <Integer Horizontal> <Integer Vertical> <Double GravityMultiplier> <Boolean SpaceDown>
                    return;

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
