﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Egoi
{

    public class EgoiApiRestImpl : EgoiApi
    {

        private static EgoiApiRestImpl instance;

        private static readonly string Url = "http://api.e-goi.com/v2/rest.php?type=json";

        private EgoiApiRestImpl()
        {
        }

        public static EgoiApiRestImpl getInstance()
        {
            if (instance == null)
                instance = new EgoiApiRestImpl();
            return instance;
        }

        public String buildUrl(String method, EgoiMap values) {
		    StringBuilder url = new StringBuilder(Url);
		    url.Append("&method=").Append(method);
		    foreach (String key in values.Keys)
			    url.Append("&functionOptions[").Append(key).Append("]=").Append(values[key]);
		    return url.ToString();
	    }

	    protected EgoiType walkMap(Dictionary<string, object> map) {
		    EgoiType r = null;
		    if(map.ContainsKey("key_0")) {
			    EgoiMapList mrl = new EgoiMapList();
			    List<String> keys = map.Keys.ToList<string>();
                keys.Sort();
			    foreach(String k in keys) {
				    if(!k.StartsWith("key_"))
					    continue;
				
				    Dictionary<string, object> v = map[k] as Dictionary<string, object>;
				    mrl.Add(walkValues(new EgoiMap(v)));
			    }
			    r = mrl;
		    } else {
			    r = walkValues(new EgoiMap(map));
		    }
			
		    return r;
	    }

	    private EgoiType walkArray(List<Dictionary<string, object>> list) {
		    EgoiMapList mrl = new EgoiMapList();
		    foreach(Dictionary<string, object> map in list)
			    mrl.Add(walkValues(new EgoiMap(map)));
		    return mrl;
	    }

        protected EgoiMap walkValues(EgoiMap map) {
            List<string> keys = map.Keys.ToList<string>();
		    foreach(string key in keys) {
			    object obj = map[key];
			    if (obj is Dictionary<string, object>) {
				    Dictionary<string, object> sm = obj as Dictionary<string, object>;
				    map[key] = walkMap(sm);
			    }
			
			    if(obj is object[]) {
				    List<Dictionary<string, object>> mapList = extractMapList((Object[]) obj);
				    map[key] = walkArray(mapList);
			    }
		    }
		    return map;
	    }

	    private List<Dictionary<string, object>> extractMapList(Object[] arr) {
		    List<Dictionary<string, object>> mapList = new List<Dictionary<string,object>>();
		    foreach(object o in arr) {
			    if (o is Dictionary<string, object>) {
                    Dictionary<string, object> m = o as Dictionary<string, object>;
				    mapList.Add(m);
			    }
		    }
		    return mapList;
	    }

        public string fetchResponse(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            try
            {
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                StringBuilder lines = new StringBuilder();
                string line;
                while ((line = reader.ReadLine()) != null)
                    lines.Append(line).Append("\n");
                return lines.ToString();
            }
            catch (WebException e)
            {
                throw new EgoiException("Error processing Rest request: " + e.Message);
            }
        }

        public Dictionary<string, object> processResult(string method, EgoiMap arguments)
        {
            string url = buildUrl(method, arguments);
            string json = fetchResponse(url);
            JavaScriptSerializer s = new JavaScriptSerializer();

            /**
             * IT's A TRAP! 
             */
            Dictionary<string, object> response = s.Deserialize<Dictionary<string, object>>(json);
            Dictionary<string, object> r2 = response["Egoi_Api"] as Dictionary<string, object>;
            Dictionary<string, object> map = r2[method] as Dictionary<string, object>;

            if (map.ContainsKey("response"))
                throw new EgoiException(EgoiException.idToMessage(map["response"] as string));

            // Status not used
            map.Remove("status");

            return map;
        }

        public EgoiMap decodeMapResult(String method, EgoiMap arguments)
        {
            Dictionary<string, object> result = processResult(method, arguments);
            return walkMap(result) as EgoiMap;
        }

        public EgoiMapList decodeMapListResult(String method, EgoiMap arguments)
        {
            Dictionary<string, object> result = processResult(method, arguments);
            return walkMap(result) as EgoiMapList;
        }


        ///////////////////////////////////////////////////////////////////////////////////////
        
        public EgoiMap addExtraField(EgoiMap arguments)
        {
            return decodeMapResult("addExtraField", arguments);
        }

        public EgoiMap addSubscriber(EgoiMap arguments)
        {
            return decodeMapResult("addSubscriber", arguments);
        }

        public EgoiMap addSubscriberBulk(EgoiMap arguments)
        {
            return decodeMapResult("addSubscriberBulk", arguments);
        }

        public EgoiMap checklogin(EgoiMap arguments)
        {
            return decodeMapResult("checklogin", arguments);
        }

        public EgoiMap createCampaignEmail(EgoiMap arguments)
        {
            return decodeMapResult("createCampaignEmail", arguments);
        }

        public EgoiMap createCampaignFAX(EgoiMap arguments)
        {
            return decodeMapResult("createCampaignFAX", arguments);
        }

        public EgoiMap createCampaignSMS(EgoiMap arguments)
        {
            return decodeMapResult("createCampaignSMS", arguments);
        }

        public EgoiMap createCampaignVoice(EgoiMap arguments)
        {
            return decodeMapResult("createCampaignVoice", arguments);
        }

        public EgoiMap createList(EgoiMap arguments)
        {
            return decodeMapResult("createList", arguments);
        }

        public EgoiMap createSegment(EgoiMap arguments)
        {
            return decodeMapResult("createSegment", arguments);
        }

        public EgoiMap deleteCampaign(EgoiMap arguments)
        {
            return decodeMapResult("deleteCampaign", arguments);
        }

        public EgoiMap deleteSegment(EgoiMap arguments)
        {
            return decodeMapResult("deleteSegment", arguments);
        }

        public EgoiMap editCampaignEmail(EgoiMap arguments)
        {
            return decodeMapResult("editCampaignEmail", arguments);
        }

        public EgoiMap editCampaignSMS(EgoiMap arguments)
        {
            return decodeMapResult("editCampaignSMS", arguments);
        }

        public EgoiMap editExtraField(EgoiMap arguments)
        {
            return decodeMapResult("editExtraField", arguments);
        }

        public EgoiMap editSubscriber(EgoiMap arguments)
        {
            return decodeMapResult("editSubscriber", arguments);
        }

        public EgoiMapList getCampaigns(EgoiMap arguments)
        {
            return decodeMapListResult("getCampaigns", arguments);
        }

        public EgoiMap getClientData(EgoiMap arguments)
        {
            return decodeMapResult("getClientData", arguments);
        }

        public EgoiMap getCredits(EgoiMap arguments)
        {
            return decodeMapResult("getCredits", arguments);
        }

        public EgoiMapList getLists(EgoiMap arguments)
        {
            return decodeMapListResult("getLists", arguments);
        }

        public EgoiMap getReport(EgoiMap arguments)
        {
            return decodeMapResult("getReport", arguments);
        }

        public EgoiMapList getSegments(EgoiMap arguments)
        {
            return decodeMapListResult("getSegments", arguments);
        }

        public EgoiMapList getSenders(EgoiMap arguments)
        {
            return decodeMapListResult("getSenders", arguments);
        }

        public EgoiMap getUserData(EgoiMap arguments)
        {
            return decodeMapResult("getUserData", arguments);
        }

        public EgoiMap removeSubscriber(EgoiMap arguments)
        {
            return decodeMapResult("removeSubscriber", arguments);
        }

        public EgoiMap sendCall(EgoiMap arguments)
        {
            return decodeMapResult("sendCall", arguments);
        }

        public EgoiMap sendEmail(EgoiMap arguments)
        {
            return decodeMapResult("sendEmail", arguments);
        }

        public EgoiMap sendFAX(EgoiMap arguments)
        {
            return decodeMapResult("sendFAX", arguments);
        }

        public EgoiMap sendSMS(EgoiMap arguments)
        {
            return decodeMapResult("sendSMS", arguments);
        }

        public EgoiMap subscriberData(EgoiMap arguments)
        {
            return decodeMapResult("subscriberData", arguments);
        }

        public EgoiMap updateList(EgoiMap arguments)
        {
            return decodeMapResult("updateList", arguments);
        }



    }
}
