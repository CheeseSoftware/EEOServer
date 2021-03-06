﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using PlayerIO.GameLibrary;
using System.Text.RegularExpressions;

namespace MushroomsUnity3DExample
{
    /*[RoomType("Lobby")]
    class Lobby : Game<Player>
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
                case "setusername":
                    {

                        string username = message.GetString(0);
                        if (username.Count() >= 3 && regex.IsMatch(username))
                        {
                            if (!player.PlayerObject.Contains("name")
                                /*|| player.PlayerObject.GetString("name") == null* /)
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
    }*/

    [RoomType("FlixelWalkerFX3")]
    class EE070 : Game<Player>
    {
        string owner = "";
        Room room = new Room();

        int[,] blocks = new int[200, 200];

        string editCode = "";
        int fillBlock = 0;
        int borderBlock = 9;
        int width = 200;
        int height = 200;

        Dictionary<string, KeyValuePair<DateTime, Action>> timers = new Dictionary<string, KeyValuePair<DateTime, Action>>();

        private void HandleCodeArguemnts()
        {
            //all arguments
            string[] arguments;

            if (this.RoomData.ContainsKey("editkey"))
            {
                arguments = this.RoomData["editkey"].Split('/');

                this.editCode = arguments[0];

                foreach (string argument in arguments)
                {
                    if (argument == arguments[0])
                        continue;

                    //args = 

                    HandleCommand(null, argument.Split(' '));
                }
            }
        }

        private void HandleCommand(Player player, string[] args)
        {
            switch (args[0])
            {
                case "fill":
                    if (args.Count() >= 2)
                    {
                        int.TryParse(args[1], out this.fillBlock);
                    }
                    break;

                case "border":
                    if (args.Count() >= 2)
                    {
                        int.TryParse(args[1], out this.borderBlock);

                        if (this.borderBlock < 9 || this.borderBlock > 15)
                            this.borderBlock = 9;
                    }
                    break;
            }
        }

        public override void GameStarted()
        {
            Console.WriteLine("Game is started: " + RoomId);

            Exception exception = null;
            try
            {
                HandleCodeArguemnts();
            }
            catch (Exception e)
            {
                exception = e;
            }

            for (int y = 0; y < 200; y++)
            {
                for (int x = 0; x < 200; x++)
                {
                    if (x == 0 || x == 199 || y == 0 || y == 199)
                        blocks[x, y] = (int)this.borderBlock;
                    else if (x <= 2 && y <= 2)
                        blocks[x, y] = 0;
                    else
                        blocks[x, y] = (int)this.fillBlock;
                }
            }

            //this.AddTimer(new Action(() => OnPlayerUpdate(room)), 100);
            this.AddTimer(new Action(() => OnTimerUpdate()), 100);

            base.GameStarted();

            if (exception != null)
                throw exception;
        }

        public override void GameClosed()
        {
            base.GameClosed();
        }

        public override void UserJoined(Player player)
        {
            if (owner == "")
            {
                owner = player.ConnectUserId;
                player.HasCode = true;
            }

            object[] messageData = new object[200 * 200 + 2];

            if (!player.HasCode)
            {
                if (this.RoomData.ContainsKey("editkey"))
                {
                    if (this.RoomData["editkey"].Split('/').First() == "")
                    {
                        player.HasCode = true;
                    }
                }
                else
                {
                    player.HasCode = true;
                }
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
                    if (player.Face != message.GetInt(0))
                    {
                        player.Face = message.GetInt(0);
                        Broadcast("face", player.Id, message.GetInt(0));
                    }
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
                                && ((x != 0 && x != 199 && y != 0 && y != 199) || (blockId >= 9 && blockId <= 15))
                                && blockId != blocks[x, y])
                            {
                                blocks[x, y] = blockId;
                                Broadcast("c", x, y, blockId);
                            }
                        }
                    }
                    return;

                case "hide":
                    if (message.Count >= 2)
                    {
                        string color = message.GetString(1);

                        Broadcast("hide", color);

                        lock (timers)
                        {
                            if (timers.ContainsKey(color))
                                timers.Remove(color);

                            timers.Add(color, new KeyValuePair<DateTime, Action>(DateTime.Now.AddSeconds(5), new Action(() =>
                            {
                                Broadcast("show", color);
                            })));
                        }
                    }
                    return;

                case "say":
                    return;

                case "k":   // crown
                    if (!player.hascrown)
                    {
                        player.hascrown = true;
                        Broadcast("k", player.Id);
                        foreach (Player p in Players)
                        {
                            if (p.Id != player.Id)
                            {
                                p.hascrown = false;
                            }
                        }
                    }
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
                    if (!player.HasCode)
                    {
                        if (this.editCode == message.GetString(0))
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

        public void OnTimerUpdate()
        {
            List<Action> tasks = new List<Action>();

            lock (timers)
            {
                List<string> keys = new List<string>(timers.Keys);

                foreach (var k in keys)
                {
                    if (DateTime.Compare(timers[k].Key, DateTime.Now) >= 0)
                    {
                        tasks.Add(timers[k].Value);
                        timers.Remove(k);
                    }
                }
            }

            foreach (var t in tasks)
                t();
        }
    }
}
