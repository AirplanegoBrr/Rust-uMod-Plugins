using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Facepunch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using Oxide.Game.Rust.Libraries;
using Rust;
using UnityEngine;
using UnityEngine.Networking;

namespace Oxide.Plugins {
    [Info("AACustomPlugin", "AirplaneGobrr", "0.0.1")]
    [Description("Custom Plugin for DCR")]
    class AACustomPlugin : RustPlugin {
        //Playertabs dictionary
        private Dictionary<ulong, string> playerTabs = new Dictionary<ulong, string>();

        private static readonly string[] Admins = {
            "76561198850884567", // Airplane
            "76561199079910004", // Afro
            "76561199196808566", // Elite "Alt"
            "76561198969057778" // AirplaneAlt
        };

        //perms
        private const string permAdmin = "aacustomplugin.admin";
        private Boolean alwaysDay = false;

        void permStart(){
            permission.RegisterPermission(permAdmin, this);
        }

        object playerCounts() {
            var players = 0;
            var playerAdmins = 0;
            bool adminsOnline = false;
            foreach (BasePlayer player in BasePlayer.activePlayerList) {
                players++;
                if (Admins.Contains(player.UserIDString)) {
                    playerAdmins++;
                    adminsOnline = true;
                }
            }

            // { players = 1, playerAdmins = 1, adminsOnline = True }
            return players;
        }

        void brodcast(string message) {
            foreach (BasePlayer player in BasePlayer.activePlayerList) {
                SendReply(player, message);
                if (Admins.Contains(player.UserIDString)) {
                    player.SetPlayerFlag(BasePlayer.PlayerFlags.IsDeveloper,true);
                    Puts(player.displayName + " is a developer");
                }
            }
        }

        void discordSendMessage(string message) {
            string webhookURL =
                "https://discordapp.com/api/webhooks/978196419099914301/THMVpMMXXij4Sb5hHoL8C8bNG40YEpa9uDgNs7IT5JwHdg_XGBagaM_jiO5K_FRKoMhR";

            //get the time and put it before message
            string time = DateTime.Now.ToString("HH:mm:ss");
            string finalMessage = "`" + time + "` " + message;

            //build the json object
            var json = new { username = "CustomPlugin", content = finalMessage };

            //post the json object to the webhook with UnityWebRequest
            UnityWebRequest www = UnityWebRequest.Post(webhookURL, "");
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json)));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SendWebRequest();
        }

        void repeater() {
            Puts("Repeater");
            foreach (BasePlayer player in BasePlayer.activePlayerList)
            {
                if (Admins.Contains(player.UserIDString))
                {
                    player.SetPlayerFlag(BasePlayer.PlayerFlags.IsDeveloper, true);
                }
            }

            //Random quote thing
            var number = UnityEngine.Random.Range(1, 3);
            Puts(number.ToString());

            switch (number)
            {
                case 1:
                    brodcast("Thanks for playing!");
                    break;
                case 2:
                    var online = playerCounts();
                    brodcast("There are " + online.ToString() + " players online!");
                    break;
            }
        }

        void everyMinute(){

        }

        void Init()
        {
            Puts("Init works!");
            discordSendMessage("`Plugin Loaded!` `Waiting for server to start...`");
            repeater();
            timer.Repeat(15 * 60, 0, repeater);
            timer.Repeat(60, 0, everyMinute);
            permStart();
        }

        void Unload()
        {
            foreach (var playerTab in playerTabs)
            {
                //Puts(playerTab.Key.ToString()); //plr id
                //Puts(playerTab.Value.ToString()); //UI name?
                var player = BasePlayer.activePlayerList.FirstOrDefault(f => f.userID == playerTab.Key);
                CuiHelper.DestroyUi(player, playerTab.Value);
            }
            playerTabs.Clear();
        }

        void OnServerInitialized(bool initial)
        {
            Puts("OnServerInitialized works!");
            discordSendMessage($"`Server has started!` `Waiting for players to connect...`\n`Server has already started before:` `{!initial}`");
            permStart();
        }

        //player tracker
        void OnPlayerConnected(BasePlayer player)
        {
            Puts("OnPlayerConnected works!");
            discordSendMessage($"`Player {player.displayName} has connected!`");
        }

        void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            Puts("OnPlayerDisconnected works!");
            discordSendMessage($"`Player {player.displayName} has disconnected! Reason: {reason} `");
        }

        object OnPlayerViolation(BasePlayer player, AntiHackType type, float amount)
        {
            Puts("OnPlayerViolation works!");
            //discordSendMessage("`Player " + player.displayName + " has been detected hacking! Type: " + type + " Amount: " + amount + "`");
            return null;
        }

        void OnPlayerReported(BasePlayer reporter, string targetName, string targetId, string subject, string message, string type)
        {
            Puts($"{reporter.displayName} reported {targetName} for {subject}.");
        }

        void OnUserConnected(IPlayer player)
        {
            Puts($"{player.Name} ({player.Id}) connected from {player.Address}");

            if (player.IsAdmin)
            {
                Puts($"{player.Name} ({player.Id}) is admin");
            }

            Puts($"{player.Name} is {(player.IsBanned ? "banned" : "not banned")}");

            brodcast($"Welcome {player.Name}!");
            discordSendMessage("`Player " + player.Name + " has connected!`");
        }

        void OnUserDisconnected(IPlayer player)
        {
            Puts($"{player.Name} ({player.Id}) disconnected");
            discordSendMessage("`Player " + player.Name + " has disconnected!`");
        }

        //Rcon Warnings
        object OnRconConnection(IPAddress ip)
        {
            Puts("OnRconConnection works!");
            discordSendMessage(":warning: :warning: `Rcon has connected from ` `" + ip.ToString() + "`");
            return null;
        }

        void OnRconCommand(IPAddress ip, string command, string[] args)
        {
            Puts("OnRconCommand works!");
            discordSendMessage(":warning: :warning: `Rcon has sent a command from` `" + ip.ToString() + "` `" + command + "` `" + string.Join(" ", args) + "`");
        }

        //End Rcon
        void gui1(BasePlayer player)
        {
            var elements = new CuiElementContainer();

            var mainName =
                elements
                    .Add(new CuiPanel
                    {
                        Image = { Color = "0.1 0.1 0.1 1" },
                        RectTransform =
                            {
                                AnchorMin = "0.3 0.025",
                                AnchorMax = "0.35 0.11"
                            },
                        CursorEnabled = true
                    },
                    "Hud",
                    "test");

            var closeButton =
                new CuiButton
                {
                    Button = { Close = mainName, Color = "0.8 0.8 0.8 0.2" },
                    RectTransform =
                        { AnchorMin = "0.86 0.92", AnchorMax = "0.97 0.98" },
                    Text =
                        {
                            Text = "X",
                            FontSize = 5,
                            //color white
                            Align = TextAnchor.MiddleCenter
                        }
                };

            var text =
                new CuiLabel
                {
                    Text =
                        {
                            Text = "Hello World!",
                            FontSize = 22,
                            Align = TextAnchor.MiddleCenter
                            //color white
                        },
                    RectTransform =
                        { AnchorMin = "0.1 0.1", AnchorMax = "0.9 0.9" }
                };

            elements.Add(closeButton, mainName);
            elements.Add(text, mainName);
            Puts(elements.ToString());
            CuiHelper.AddUi(player, elements);
            playerTabs.Add(player.userID, mainName);
        }

        void gui2(BasePlayer player)
        {
            var elements = new CuiElementContainer();

            var mainName =
                elements
                    .Add(new CuiPanel
                    {
                        Image = { Color = "0.1 0.1 0.1 1" },
                        RectTransform =
                            { AnchorMax = "0.8 0.9", AnchorMin = "0.20 0.15" },
                        CursorEnabled = true
                    },
                    "Hud",
                    "test");

            Puts(mainName.ToString());

            var closeButton =
                new CuiButton
                {
                    RectTransform =
                        {
                            //AnchorMin = "0.86 0.92",
                            //AnchorMax = "0.97 0.98"
                            AnchorMax = "0.95 0.95",
                            AnchorMin = "0.85 0.85"
                        },
                    Button =
                        {
                            //Close = mainName,
                            Command = "closeGUI",
                            Color = "0.8 0.8 0.8 0.2"
                        },
                    Text =
                        {
                            Text = "X",
                            FontSize = 15,
                            //color white
                            Align = TextAnchor.MiddleCenter
                        }
                };

            var text =
                new CuiLabel
                {
                    Text =
                        {
                            Text = "Welcome to the server!",
                            FontSize = 22,
                            Align = TextAnchor.MiddleCenter
                            //color white
                        },
                    RectTransform =
                        {
                            //1 1.8
                            AnchorMax = "0.7 1",
                            AnchorMin = "0.3 0.8"
                        }

                    //set blackground color to red
                };

            var outline =
                new CuiOutlineComponent
                {
                    Color = "0.1 0.1 0.1 1",
                    Distance = "1"
                };

            elements.Add(closeButton, mainName);
            elements.Add(text, mainName);

            // elements.Add(outline, mainName);
            Puts(elements.ToString());
            CuiHelper.AddUi(player, elements);
            playerTabs.Add(player.userID, mainName);
        }

        //Commands
        [ChatCommand("test")]
        void cmdChatTest(BasePlayer player, string command, string[] args)
        {
            //gui1(player);
            //gui2(player);
            player.ChatMessage("<size=18>Chat Commands</size>\n<color=#fc5a03>/{0} </color>Start or stop placing a pipe");
        }

        [ChatCommand("day")]
        void cmdDay(BasePlayer player, string command)
        {
            if (permission.UserHasPermission(player.UserIDString, permAdmin)){
                covalence.Server.Command("env.time 9");
            } else {
                player.ChatMessage("You do not have permission to use this command!");
            }

        }

        [ChatCommand("toggleAdmin")]
        void cmdToggleAdmin(BasePlayer player, string command) {
            if (Admins.Contains(player.UserIDString)) {
                bool UserHasGroup = permission.UserHasGroup(player.UserIDString, "admin");
                if (UserHasGroup) {
                    permission.RemoveUserGroup(player.UserIDString, "admin");
                    player.ChatMessage("Admin is now off!");
                } else {
                    permission.AddUserGroup(player.UserIDString, "admin");
                    player.ChatMessage("Admin is now on!");
                }
            } else {
                player.ChatMessage("You do not have permission to use this command!");
            }
        }

        [ChatCommand("online")]
        void cmdOnline(BasePlayer player){
            var players = playerCounts();
            player.ChatMessage("Players online: " + players);
        }

        //console command to close GUI
        [ConsoleCommand("closeGUI")]
        void cmdConsoleCloseGUI(ConsoleSystem.Arg arg)
        {
            Puts("cmdConsoleCloseGUI works!");
            var player = arg.Player();
            if (player == null)
            {
                Puts("Console command can only be used by a player!");
                return;
            }
            if (playerTabs.ContainsKey(player.userID))
            {
                CuiHelper.DestroyUi(player, playerTabs[player.userID]);
                playerTabs.Remove(player.userID);
                Puts("Player " + player.displayName + " has closed the GUI!");
            }
        }
    
    }
}
