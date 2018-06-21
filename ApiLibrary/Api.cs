using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApiLibrary
{
    public class Api
    {
        private string responseFromServer;

        public Api(string url)
        {
            setResponse(url);
        }

        public void setResponse(string url)
        {
            // Create a request for the URL.
            WebRequest request = WebRequest.Create(url);

            // If required by the server, set the credentials.  
            request.Credentials = CredentialCache.DefaultCredentials;

            // Get the response.  
            WebResponse response = request.GetResponse();

            // Display the status.  
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            // Get the stream containing content returned by the server.  
            Stream dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);

            // Read the content.  
            responseFromServer = reader.ReadToEnd();

            // Display the content.  
            //WriteLine(responseFromServer);

            // Clean up the streams and the response.  
            reader.Close();
            response.Close();
        }

        public string getResponse()
        {
            return responseFromServer;
        }
    }
}
