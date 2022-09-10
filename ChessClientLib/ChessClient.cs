using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessClientLib
{
    public class ChessClient
    {
        public string host { get; private set; }
        public string user { get; private set; }

        int CurrentGameID;

        public GameInfo GetCurrentGame()
        {
            //GameInfo game = new GameInfo(ParceJson(CallServer()));
            //CurrentGameID = game.GameID;
            return false; //game;                      
        }

        public ChessClient(string host, string user)
        {
            //Regex regex = new Regex();
            
            //Type type = new Type();
            this.host = host;
            this.user = user;
        }

        public GameInfo SendMove(string move)
        {
            string json = CallServer("/" + move);
            var list = ParceJson(json);
            GameInfo game = new GameInfo(list);
            return game;
        }

        private string CallServer(string param="")
        {
            WebRequest request = WebRequest.Create(host +"/"+ user + "/" + param);
            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        private NameValueCollection ParceJson (string json)
        {
            NameValueCollection list = new NameValueCollection();

            string pattern = @"""(\w+)\"":""?([^,""}]*)""?";
            foreach (Match m in Regex.Matches(json, pattern))
                if (m.Groups.Count == 3)
                    list[m.Groups[1].Value] = m.Groups[2].Value;

            return list;                       
        }
    }
}
