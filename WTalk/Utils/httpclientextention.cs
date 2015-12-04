﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WTalk.Utils
{
    public static class HttpClientExtention
    {
        static string[] DEFAULT_HEADER = new string[] {"Authorization", "x-origin","x-goog-authuser"};

        //public static void ClearCustomHeaders(this HttpClient client)
        //{
        //    foreach (string header in DEFAULT_HEADER)
        //        if (client.DefaultRequestHeaders.Contains(header))
        //            client.DefaultRequestHeaders.Remove(header);
        //}


        public static HttpResponseMessage Execute(this HttpClient client, string url, Dictionary<string, string> queryString = null, Dictionary<string, string> postData = null)
        {
            if(!client.DefaultRequestHeaders.Contains("UserAgent"))
                client.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.132 Safari/537.36");

            StringBuilder query = new StringBuilder(url);

            if (queryString != null)
            {
                if (url.IndexOf("?") < 0)
                    query.Append("?");
                query.Append(string.Join("&",queryString.Select(c=>string.Format("{0}={1}", c.Key, Uri.EscapeUriString(c.Value))).ToArray()));                                
            }

            HttpResponseMessage message = null;
            if (postData != null)
                message = client.PostAsync(query.ToString(), new FormUrlEncodedContent(postData)).Result;
            else
                message = client.GetAsync(query.ToString()).Result;
            
            return message;
        }

        public static HttpResponseMessage PostJson(this HttpClient client, string apiKey, string endPoint, JArray body)
        {
            return client.execute(apiKey, endPoint, body, true);
        }

        public static HttpResponseMessage PostProtoJson(this HttpClient client, string apiKey, string endPoint, JArray body)
        {
            return client.execute(apiKey, endPoint, body, false);
        }

        static HttpResponseMessage execute(this HttpClient client, string apiKey, string endPoint, JArray body, bool useJson)
        {
            HttpResponseMessage message = null;
            string uri = string.Format("{0}{1}?key={2}&alt={3}",HangoutUri.CHAT_SERVER_URL,endPoint, Uri.EscapeUriString(apiKey), useJson ? "json" : "protojson");
            message = client.PostAsync(uri,new StringContent(body.ToString(),Encoding.UTF8,"application/json+protobuf")).Result;
            message.EnsureSuccessStatusCode();

            return message;
        }



        public static IEnumerable<Cookie> GetAllCookies(this CookieContainer c)
        {
            Hashtable k = (Hashtable)c.GetType().GetField("m_domainTable", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(c);
            foreach (DictionaryEntry element in k)
            {
                SortedList l = (SortedList)element.Value.GetType().GetField("m_list", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(element.Value);
                foreach (var e in l)
                {
                    var cl = (CookieCollection)((DictionaryEntry)e).Value;
                    foreach (Cookie fc in cl)
                    {
                        yield return fc;
                    }
                }
            }
        }

        public static Cookie GetCookie(this CookieContainer c, string domain, string key)
        {
            Hashtable k = (Hashtable)c.GetType().GetField("m_domainTable", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(c);
            if (!k.ContainsKey(domain))
                return null;
            DictionaryEntry element = new DictionaryEntry("",k[domain]);
            SortedList list = (SortedList)element.Value.GetType().GetField("m_list", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(element.Value);
            if(list.Count > 0)
            {
                foreach (var e in list)
                {
                    var cl = (CookieCollection)((DictionaryEntry)e).Value;
                    foreach (Cookie fc in cl)
                    {
                        if(fc.Name == key)
                        return fc;
                    }
                }
            }
                        
            return null;
        }
    }
}