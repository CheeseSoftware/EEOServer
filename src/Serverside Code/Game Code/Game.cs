﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PlayerIO.GameLibrary;
using System.Drawing;
using Movement;

namespace MushroomsUnity3DExample
{
    public class Room
    {
        public void DrawBlock(Block block)
        {
            throw new NotImplementedException();
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public Block getBlock(int p1, int p2, int p3)
        {
            throw new NotImplementedException();
        }
    }
    public class Block
    {
        private Message message;

        public Block(Message message)
        {
            // TODO: Complete member initialization
            this.message = message;
        }

        public int BlockId { get; set; }

        internal bool isPortal()
        {
            throw new NotImplementedException();
        }

        public int blockId { get; set; }

        public int pt_target { get; set; }

        public int pt_rotation { get; set; }
    }

    [RoomType("Lobby176")]
    class LobbyCode : Game<Player> { public override void GotMessage(Player player, Message message) { throw new Exception(message.ToString()); } }

    [RoomType("Beta176")]
    class BetaCode : Game<Player> { public override void GotMessage(Player player, Message message) { throw new Exception(message.ToString()); } }

    [RoomType("LobbyGuest176")]
    class LobbyGuestCode : Game<Player> { public override void GotMessage(Player player, Message message) { throw new Exception(message.ToString()); } }

    [RoomType("Auth176")]
    class AuthCode : Game<Player> { public override void GotMessage(Player player, Message message) { throw new Exception(message.ToString()); } }

    [RoomType("QuickInviteHandler176")]
    class QuickInviteHandlerCode : Game<Player> { public override void GotMessage(Player player, Message message) { throw new Exception(message.ToString()); } }

    [RoomType("Tutorial176")]
    class TutorialCode : Game<Player> { public override void GotMessage(Player player, Message message) { throw new Exception(message.ToString()); } }

    [RoomType("Tracking176")]
    class TrackingCode : Game<Player> { public override void GotMessage(Player player, Message message) { throw new Exception(message.ToString()); } }

    [RoomType("Everybodyedits176")]
    class GameCode : Game<Player>
    {
        string owner = "ostkaka";
        Room room = new Room();

        public override void GameStarted()
        {
            Console.WriteLine("Game is started: " + RoomId);

            this.AddTimer(new Action(() => OnPlayerUpdate(room)), 100);

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
                                p.Send("add", player.Id, player.X, player.Y);
                                player.Send("add", p.Id, p.X, p.Y);
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


