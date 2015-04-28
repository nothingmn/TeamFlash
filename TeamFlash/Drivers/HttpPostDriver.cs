using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TeamFlash.Drivers
{
    [Serializable]
    public class HttpPostDriver : IBuildLight
    {
        private string _url;

        public string Url
        {
            get
            {
                if (!string.IsNullOrEmpty(_url)) return _url;

                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var configFilePath = Path.Combine(appDataPath, @"TeamFlash\config.http.xml");
                Console.WriteLine(string.Format("Driver Configuration File:{0}", configFilePath));

                if (!System.IO.File.Exists(configFilePath))
                {
                    _url = "http://localhost?status=";
                    var serializer = new XmlSerializer(typeof (string));
                    using (var stream = File.Create(configFilePath))
                    {
                        serializer.Serialize(stream, _url);
                    }
                }
                else
                {
                    var serializer = new XmlSerializer(typeof (string));
                    using (var stream = File.OpenRead(configFilePath))
                    {
                        _url = (string) serializer.Deserialize(stream);
                    }

                }                
                return _url;
            }
            set { _url = value; }
        }
                    WebClient client;

        private void LoadUrl(string url)
        {
            Logger.Verbose("Loading url:{0}", url);
            if (client == null)
            {
                client = new WebClient();
            }

            try
            {
                Logger.Verbose("Invoking query '{0}'.", url);
                using (var stream = client.OpenRead(url))
                {
                    var count = 0;
                    while (stream.ReadByte() > 0)
                    {
                        count++;
                    }
                    Logger.WriteLine("Grabbed:{0}bytes", count);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }


        public void Success()
        {            
            LoadUrl(string.Format("{0}success", this.Url));
        }

        public void Warning()
        {
            LoadUrl(string.Format("{0}warning", this.Url));
        }

        public void Fail()
        {
            LoadUrl(string.Format("{0}fail", this.Url));
        }

        public void Off()
        {
            LoadUrl(string.Format("{0}off", this.Url));
        }
    }    
}
