using System; //!!
using System.Text; //!!
using Newtonsoft.Json; //!!
using UnityEngine.Networking; //!!
//!! = required


namespace Oxide.Plugins
{
    [Info("DiscordWebhook", "AirplaneGobrr", "0.0.1")]
    [Description("Discord webhook example")]
    class DiscordWebhook : RustPlugin
    {
        void discordSendMessage(string message)
        {
            string webhookURL = "https://support.discord.com/hc/en-us/articles/228383668";

            //get the time and put it before message
            string time = DateTime.Now.ToString("HH:mm:ss");
            string finalMessage = "`" + time + "` " + message;

            //build the json object
            var json =
                new { username = "DiscordWebhook", content = finalMessage };

            //post the json object to the webhook with UnityWebRequest
            UnityWebRequest www = UnityWebRequest.Post(webhookURL, "");
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json)));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SendWebRequest();
        }

        void Init()
        {
            Puts("Init works!");
            discordSendMessage("`Plugin Loaded!`");
        }
    }
}