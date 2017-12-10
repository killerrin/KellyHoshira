using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace KellyHoshira.Core
{
    public class SecretKeys
    {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string UserToken { get; set; }

        public SecretKeys() : this("", "", "") { }
        public SecretKeys(string clientID, string clientSecret, string userToken)
        {
            ClientID = clientID;
            ClientSecret = clientSecret;
            UserToken = userToken;
        }

        public static SecretKeys Load(string fileName)
        {
            List<string> results = new List<string>();

            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            using (StreamReader reader = new StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    //Debug.WriteLine(line);
                    // Parse out Empty Lines or Comments
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    else if (line[0] == '#')
                        continue;

                    // Add to the list
                    results.Add(line);
                }
            }

            SecretKeys keys = new SecretKeys(results[0], results[1], results[2]);
            Debug.WriteLine("Parsed Keys");
            Debug.WriteLine(string.Format("Client ID: {0}", keys.ClientID));
            Debug.WriteLine(string.Format("Client Secret: {0}", keys.ClientSecret));
            Debug.WriteLine(string.Format("UserToken: {0}", keys.UserToken));
            return keys;
        }
    }
}
