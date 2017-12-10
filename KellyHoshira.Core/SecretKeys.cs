using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

                    // Parse out Empty Lines or Comments
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    else if (line[0] == '#')
                        continue;

                    // Add to the list
                    results.Add(reader.ReadLine());
                }
            }

            SecretKeys keys = new SecretKeys(results[0], results[1], results[2]);
            return keys;
        }
    }
}
