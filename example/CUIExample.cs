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
    [Info("AAcuiTest", "AirplaneGobrr", "0.0.1")]
    [Description("Testing plugin")]
    class AACustomPlugin: RustPlugin {
        private Dictionary < ulong, string > playerTabs = new Dictionary < ulong, string > ();

        void gui(BasePlayer player) {
            var elements = new CuiElementContainer();

            var mainName = elements.Add(new CuiPanel {
                Image = { Color = "0.1 0.1 0.1 1" },
                RectTransform = {
                    AnchorMax = "0.8 0.9",
                    AnchorMin = "0.20 0.15"
                },
                CursorEnabled = true
            }, "Hud", "test");

            var closeButton = new CuiButton {
                RectTransform = {
                    AnchorMax = "0.95 0.95",
                    AnchorMin = "0.85 0.85"
                },
                Button = {
                    Close = mainName,
                    Color = "0.8 0.8 0.8 0.2"
                },
                Text = {
                    Text = "X",
                    FontSize = 15,
                    Align = TextAnchor.MiddleCenter
                }
            };

            var text = new CuiLabel {
                Text = {
                    Text = "Welcome to the server!",
                    FontSize = 22,
                    Align = TextAnchor.MiddleCenter
                },
                RectTransform = {
                    AnchorMax = "0.7 1",
                    AnchorMin = "0.3 0.8"
                }
            };

            var outline = new CuiOutlineComponent {
                Color = "0.1 0.1 0.1 1",
                Distance = "1"
            };

            elements.Add(closeButton, mainName);
            elements.Add(text, mainName);

            CuiHelper.AddUi(player, elements);
            playerTabs.Add(player.userID, mainName);
        }

        [ChatCommand("test")]
        void cmdChatTest(BasePlayer player)
        {
            gui(player);
        }
    }
}