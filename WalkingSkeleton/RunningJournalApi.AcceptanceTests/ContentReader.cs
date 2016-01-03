using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Xml.Linq;

namespace RunningJournalApi.AcceptanceTests
{
    /// <summary>
    /// Code from https://github.com/ploeh/RunningJournalApi/blob/master/RunningJournalApi.AcceptanceTests/ContentReader.cs
    /// </summary>
    public static class ContentReader
    {
        public static Task<dynamic> ReadAsJsonAsync(this HttpContent content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            return content.ReadAsStringAsync().ContinueWith(t =>
                JsonConvert.DeserializeObject(t.Result));
        }

        public static Task<XDocument> ReadAsXmlAsync(this HttpContent content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            return content.ReadAsStringAsync().ContinueWith(t =>
                XDocument.Parse(t.Result));
        }
    }
}