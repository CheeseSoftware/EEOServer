using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PlayerIO.GameLibrary;
using System.Drawing;

namespace MushroomsUnity3DExample
{
    class Room
    {
        internal void DrawBlock(Block block)
        {
            throw new NotImplementedException();
        }
    }
    class Block
    {
        private Message message;

        public Block(Message message)
        {
            // TODO: Complete member initialization
            this.message = message;
        }
    }


    [RoomType("EEO")]
    class GameCode : Game<Player>
    {
        string owner = "ostkaka";
        Room room = new Room();

        public override void GameStarted()
        {
            Console.WriteLine("Game is started: " + RoomId);
            base.GameStarted();
        }

        public override void GameClosed()
        {
            base.GameClosed();
        }

        public override void UserJoined(Player player)
        {
            player.Send("init", "f");

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
                        foreach (Player p in Players)
                        {
                            if (p.ConnectUserId != player.ConnectUserId)
                            {
                                p.Send("add", player.ConnectUserId, player.X, player.Y);
                                player.Send("add", p.ConnectUserId, p.X, p.Y);
                            }
                        }
                    }
                    return;

                case "init2":
                    {

                    }
                    return;

                case "botinit":
                    {

                    }
                    return;

                case "b":
                case "bc":
                case "bs":
                case "pt":
                case "lb":
                case "br":
                    if (player.HasAccess)
                    {
                        Block block = new Block(message);

                        room.DrawBlock(block);
                    }
                    /*if (!room.DrawBlock(block))
                    {
                        this.Broadcast(block.ToMessage());
                    }
                    else
                    {
                        player.Send(room.getBlock(block).ToMessage());
                        player.Send("error", block.ToMessage());
                        player.AssignWarning("b");
                    }*/
                    return;

                case "say":
                    return;

                case "rk":  // red key
                case "gk":  // green key
                case "bk":  // blue key
                    return;

                case "k":   // crown
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

                case "p":   // potion <int id>
                    return;

                case "access":  // <string code>
                    return;

                case "levelcomplete":
                    this.Broadcast(Message.Create("write", "World", player.Name + " completed the level!"));
                    return;

                case "m":   // <double x> <double y> <double speedX> <double speedY> <Integer ModifierY> <Integer Horizontal> <Integer Vertical> <Double GravityMultiplier> <Boolean SpaceDown>
                    return;

            }
        }

    }
}


