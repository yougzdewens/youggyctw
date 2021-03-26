﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace YouggyTw
{
    /// <summary>
    /// Enumeration of the supported HTTP verbs supported by the <see cref="Twitterizer.Core.CommandPerformer"/>
    /// </summary>
    public enum HTTPVerb
    {
        /// <summary>
        /// The HTTP GET method is used to retrieve data.
        /// </summary>
        GET,

        /// <summary>
        /// The HTTP POST method is used to transmit data.
        /// </summary>
        POST,

        /// <summary>
        /// The HTTP DELETE method is used to indicate that a resource should be deleted.
        /// </summary>
        DELETE
    }

    /// <summary>
    /// The Web Request Builder class.
    /// </summary>
    public sealed class WebRequestBuilder
    {
        /// <summary>
        /// Holds file data form performing multipart form posts.
        /// </summary>
        private byte[] formData;

        /// <summary>
        /// The HTTP Authorization realm.
        /// </summary>
        public const string Realm = "Twitter API";

        /// <summary>
        /// Gets or sets the request URI.
        /// </summary>
        /// <value>The request URI.</value>
        public Uri RequestUri { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
		public Dictionary<string, object> Parameters { get; private set; }

        /// <summary>
        /// Gets or sets the verb.
        /// </summary>
        /// <value>The verb.</value>
        public HTTPVerb Verb { get; set; }

        /// <summary>
        /// Gets or sets the oauth tokens.
        /// </summary>
        /// <value>The tokens.</value>
        public OAuthTokens Tokens { private get; set; }

        /// <summary>
        /// Gets or sets the UserAgent.
        /// </summary>
        /// <value>The User Agent.</value>
        private readonly string userAgent;

        /// <summary>
        /// Gets or sets the Basic Auth Credentials.
        /// </summary>
        /// <value>The Basic Auth Credentials.</value>
        private readonly NetworkCredential networkCredentials;

        public byte[] DataInByte { get; set; }

        /// <summary>
        /// Gets or sets the Multipart config
        /// </summary>
        /// <value>Multipart</value>
        public bool Multipart { get; set; }

        /// <summary>
        /// Gets or sets whether to use accept compression on this request
        /// </summary>
        /// <value>UseCompression</value>
        public bool UseCompression { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request will be signed with an OAuth authorization header.
        /// </summary>
        /// <value><c>true</c> if [use O auth]; otherwise, <c>false</c>.</value>
        public bool UseOAuth { get; private set; }

        /// <summary>
        /// OAuth Parameters key names to include in the Authorization header.
        /// </summary>
        private static readonly string[] OAuthParametersToIncludeInHeader = new[]
                                                          {
                                                              "oauth_version",
                                                              "oauth_nonce",
                                                              "oauth_timestamp",
                                                              "oauth_signature_method",
                                                              "oauth_consumer_key",
                                                              "oauth_token",
                                                              "oauth_verifier"
                                                              // Leave signature omitted from the list, it is added manually
                                                              // "oauth_signature",
                                                          };

        /// <summary>
        /// Parameters that may appear in the list, but should never be included in the header or the request.
        /// </summary>
        private static readonly string[] SecretParameters = new[]
                                                                {
                                                                    "oauth_consumer_secret",
                                                                    "oauth_token_secret",
                                                                    "oauth_signature"
                                                                };

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequestBuilder"/> class.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="verb">The http verb.</param>
        /// <param name="userAgent">The http user agent.</param>
        /// <param name="networkCredentials">The network credentials.</param>
        /// <remarks></remarks>
        public WebRequestBuilder(Uri requestUri, HTTPVerb verb, String userAgent, NetworkCredential networkCredentials)
        {
            if (requestUri == null)
                throw new ArgumentNullException("requestUri");

            this.RequestUri = requestUri;
            this.Verb = verb;
            this.userAgent = userAgent;
            this.UseOAuth = false;
            if (networkCredentials != null)
                this.networkCredentials = networkCredentials;

            this.Parameters = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(this.RequestUri.Query)) return;

            foreach (Match item in Regex.Matches(this.RequestUri.Query, @"(?<key>[^&?=]+)=(?<value>[^&?=]+)"))
            {
                this.Parameters.Add(item.Groups["key"].Value, item.Groups["value"].Value);
            }

            this.RequestUri = new Uri(this.RequestUri.AbsoluteUri.Replace(this.RequestUri.Query, ""));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequestBuilder"/> class.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="verb">The verb.</param>
        /// <param name="tokens">The tokens.</param>
        /// <param name="userAgent">The user agent.</param>
        public WebRequestBuilder(Uri requestUri, HTTPVerb verb, OAuthTokens tokens, string userAgent = "")
            : this(requestUri, verb, userAgent, null)
        {
            this.Tokens = tokens;

            if (tokens != null)
            {
                if (string.IsNullOrEmpty(this.Tokens.ConsumerKey) || string.IsNullOrEmpty(this.Tokens.ConsumerSecret))
                {
                    throw new ArgumentException("Consumer key and secret are required for OAuth requests.");
                }

                if (string.IsNullOrEmpty(this.Tokens.AccessToken) ^ string.IsNullOrEmpty(this.Tokens.AccessTokenSecret))
                {
                    throw new ArgumentException("The access token is invalid. You must specify the key AND secret values.");
                }

                this.UseOAuth = true;
            }
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <returns></returns>
        public HttpWebResponse ExecuteRequest()
        {
            HttpWebRequest request = PrepareRequest();

            return (HttpWebResponse)request.GetResponse();
        }

        /// <summary>
        /// Prepares the request. It is not nessisary to call this method unless additional configuration is required.
        /// </summary>
        /// <returns>A <see cref="HttpWebRequest"/> object fully configured and ready for execution.</returns>
        public HttpWebRequest PrepareRequest()
        {
            SetupOAuth();

            formData = null;
            string contentType = string.Empty;

            if (!Multipart)
            {   //We don't add the parameters to the query if we are multipart-ing
                AddQueryStringParametersToUri();
            }
            else
            {
                string dataBoundary = "--------------------r4nd0m";
                contentType = "multipart/form-data; boundary=" + dataBoundary;

                formData = GetMultipartFormData(Parameters, dataBoundary);

                this.Verb = HTTPVerb.POST;
            }

            HttpWebRequest request;

            request = (HttpWebRequest)WebRequest.Create(this.RequestUri);

            if (this.UseCompression == true)
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            else
                request.AutomaticDecompression = DecompressionMethods.None;

            if (!this.UseOAuth && this.networkCredentials != null)
            {
                request.Credentials = this.networkCredentials;
                request.UseDefaultCredentials = false;
            }
            else
            {
                request.UseDefaultCredentials = true;
            }

            request.Method = this.Verb.ToString();

            request.ContentLength = Multipart ? ((formData != null) ? formData.Length : 0) : 0;

            if (DataInByte != null)
            {
                request.ContentLength = DataInByte.Length;
            }

            request.UserAgent = (string.IsNullOrEmpty(userAgent)) ? string.Format(CultureInfo.InvariantCulture, "Twitterizer/{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version) : userAgent;

            //request.ServicePoint.Expect100Continue = false;

            if (this.UseOAuth)
            {
                request.Headers.Add("Authorization", GenerateAuthorizationHeader());
            }

            if (Multipart)
            {   //Parameters are not added to the query string, post them in the request body instead

                request.ContentType = contentType;

                using (Stream requestStream = request.GetRequestStream())
                {
                    if (formData != null)
                    {
                        requestStream.Write(formData, 0, formData.Length);
                    }
                }
            }
            else
            {
                if (DataInByte != null)
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(DataInByte, 0, DataInByte.Length);
                    }
                }
            }

            return request;
        }

        /// <summary>
        /// Adds the parameters to request uri.
        /// </summary>
        private void AddQueryStringParametersToUri()
        {
            StringBuilder requestParametersBuilder = new StringBuilder(this.RequestUri.AbsoluteUri);
            requestParametersBuilder.Append(this.RequestUri.Query.Length == 0 ? "?" : "&");


            Dictionary<string, object> fieldsToInclude = new Dictionary<string, object>(this.Parameters.Where(p => !OAuthParametersToIncludeInHeader.Contains(p.Key) &&
                                         !SecretParameters.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value));

            foreach (KeyValuePair<string, object> item in fieldsToInclude)
            {
                if (item.Value is string)
                    requestParametersBuilder.AppendFormat("{0}={1}&", item.Key, UrlEncode((string)item.Value));
            }

            if (requestParametersBuilder.Length == 0)
                return;

            requestParametersBuilder.Remove(requestParametersBuilder.Length - 1, 1);

            this.RequestUri = new Uri(requestParametersBuilder.ToString());
        }

        private byte[] GetMultipartFormData(Dictionary<string, object> param, string boundary)
        {
            Stream formDataStream = new MemoryStream();
            Encoding encoding = Encoding.UTF8;

            Dictionary<string, object> fieldsToInclude = new Dictionary<string, object>(param.Where(p => !OAuthParametersToIncludeInHeader.Contains(p.Key) &&
                             !SecretParameters.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value));

            foreach (KeyValuePair<string, object> kvp in fieldsToInclude)
            {
                if (kvp.Value.GetType() == typeof(byte[]))
                {	//assume this to be a byte stream
                    byte[] data = (byte[])kvp.Value;

                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: application/octet-stream\r\n\r\n",
                        boundary,
                        kvp.Key,
                        kvp.Key);

                    byte[] headerBytes = encoding.GetBytes(header);

                    formDataStream.Write(headerBytes, 0, headerBytes.Length);
                    formDataStream.Write(data, 0, data.Length);
                }
                else
                {   //this is normal text data
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n",
                        boundary,
                        kvp.Key,
                        kvp.Value);

                    byte[] headerBytes = encoding.GetBytes(header);

                    formDataStream.Write(headerBytes, 0, headerBytes.Length);
                }
            }

            string footer = string.Format("\r\n--{0}--\r\n", boundary);
            formDataStream.Write(encoding.GetBytes(footer), 0, footer.Length);
            formDataStream.Position = 0;
            byte[] returndata = new byte[formDataStream.Length];

            formDataStream.Read(returndata, 0, returndata.Length);
            formDataStream.Close();

            return returndata;
        }

        #region OAuth Helper Methods
        /// <summary>
        /// Sets up the OAuth request details.
        /// </summary>
        private void SetupOAuth()
        {
            // We only sign oauth requests
            if (!this.UseOAuth)
            {
                return;
            }

            // Add the OAuth parameters
            this.Parameters.Add("oauth_version", "1.0");
            this.Parameters.Add("oauth_nonce", GenerateNonce());
            this.Parameters.Add("oauth_timestamp", GenerateTimeStamp());
            this.Parameters.Add("oauth_signature_method", "HMAC-SHA1");
            this.Parameters.Add("oauth_consumer_key", this.Tokens.ConsumerKey);
            this.Parameters.Add("oauth_consumer_secret", this.Tokens.ConsumerSecret);

            if (!string.IsNullOrEmpty(this.Tokens.AccessToken))
            {
                this.Parameters.Add("oauth_token", this.Tokens.AccessToken);
            }

            if (!string.IsNullOrEmpty(this.Tokens.AccessTokenSecret))
            {
                this.Parameters.Add("oauth_token_secret", this.Tokens.AccessTokenSecret);
            }

            string signature = GenerateSignature();

            // Add the signature to the oauth parameters
            this.Parameters.Add("oauth_signature", signature);
        }

        /// <summary>
        /// Generates the signature.
        /// </summary>
        /// <returns></returns>
        public string GenerateSignature()
        {
            IEnumerable<KeyValuePair<string, object>> nonSecretParameters;

            if (Multipart)
            {
                nonSecretParameters = (from p in this.Parameters
                                       where (!SecretParameters.Contains(p.Key) && p.Key.StartsWith("oauth_"))
                                       select p);
            }
            else
            {
                nonSecretParameters = (from p in this.Parameters
                                       where (!SecretParameters.Contains(p.Key))
                                       select p);
            }

            Uri urlForSigning = this.RequestUri;

            // Create the base string. This is the string that will be hashed for the signature.
            string signatureBaseString = string.Format(
                CultureInfo.InvariantCulture,
                "{0}&{1}&{2}",
                this.Verb.ToString().ToUpper(CultureInfo.InvariantCulture),
                UrlEncode(NormalizeUrl(urlForSigning)),
                UrlEncode(nonSecretParameters));

            // Create our hash key (you might say this is a password)
            string key = string.Format(
                CultureInfo.InvariantCulture,
                "{0}&{1}",
                UrlEncode(this.Tokens.ConsumerSecret),
                UrlEncode(this.Tokens.AccessTokenSecret));


            // Generate the hash
            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(key));
            byte[] signatureBytes = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(signatureBaseString));
            return Convert.ToBase64String(signatureBytes);
        }

        /// <summary>
        /// Generate the timestamp for the signature        
        /// </summary>
        /// <returns>A timestamp value in a string.</returns>
        public static string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds, CultureInfo.CurrentCulture).ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns>A random number between 123400 and 9999999 in a string.</returns>
        public static string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            return new Random()
                .Next(123400, int.MaxValue)
                .ToString("X", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Normalizes the URL.
        /// </summary>
        /// <param name="url">The URL to normalize.</param>
        /// <returns>The normalized url string.</returns>
        public static string NormalizeUrl(Uri url)
        {
            string normalizedUrl = string.Format(CultureInfo.InvariantCulture, "{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }

            normalizedUrl += url.AbsolutePath;
            return normalizedUrl;
        }

        /// <summary>
        /// Encodes a value for inclusion in a URL querystring.
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        public static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            value = Uri.EscapeDataString(value);

            // UrlEncode escapes with lowercase characters (e.g. %2f) but oAuth needs %2F
            value = Regex.Replace(value, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());

            // these characters are not escaped by UrlEncode() but needed to be escaped
            value = value
                .Replace("(", "%28")
                .Replace(")", "%29")
                .Replace("$", "%24")
                .Replace("!", "%21")
                .Replace("*", "%2A")
                .Replace("'", "%27");

            // these characters are escaped by UrlEncode() but will fail if unescaped!
            value = value.Replace("%7E", "~");

            return value;
        }

        /// <summary>
        /// Encodes a series of key/value pairs for inclusion in a URL querystring.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A string of all the <paramref name="parameters"/> keys and value pairs with the values encoded.</returns>
		private static string UrlEncode(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            StringBuilder parameterString = new StringBuilder();

            var paramsSorted = from p in parameters
                               orderby p.Key, p.Value
                               select p;

            foreach (var item in paramsSorted)
            {
                if (item.Value is string)
                {
                    if (parameterString.Length > 0)
                    {
                        parameterString.Append("&");
                    }

                    parameterString.Append(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}={1}",
                            UrlEncode(item.Key),
                            UrlEncode((string)item.Value)));
                }
            }

            return UrlEncode(parameterString.ToString());
        }

        /// <summary>
        /// Generates the authorization header.
        /// </summary>
        /// <returns>The string value of the HTTP header to be included for OAuth requests.</returns>
        public string GenerateAuthorizationHeader()
        {
            StringBuilder authHeaderBuilder = new StringBuilder();
            authHeaderBuilder.AppendFormat("OAuth realm=\"{0}\"", Realm);

            var sortedParameters = from p in this.Parameters
                                   where OAuthParametersToIncludeInHeader.Contains(p.Key)
                                   orderby p.Key, UrlEncode((p.Value is string) ? (string)p.Value : string.Empty)
                                   select p;

            foreach (var item in sortedParameters)
            {
                authHeaderBuilder.AppendFormat(
                    ",{0}=\"{1}\"",
                    UrlEncode(item.Key),
                    UrlEncode(item.Value as string));
            }

            authHeaderBuilder.AppendFormat(",oauth_signature=\"{0}\"", UrlEncode(this.Parameters["oauth_signature"] as string));

            return authHeaderBuilder.ToString();
        }
        #endregion

    }
}
