using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Dynamic;

namespace CloudFivePush
{
	public class Notification
	{
		public string APIKey { get; set; }
		public string Alert { get; set; }
		private string Audience { get; set; }
		public List<String> UserIdentifiers { get; set; }
		public DateTime? ScheduledAt { get; set; }
		public string Message { get; set; }
		public IDictionary<String, Object> Data { get; set; }
		public int Badge { get; set; }
		public string APSEnvironment { get; set; }
		public bool ContentAvailable { get; set; }

		public Notification(string apiKey)
		{
			this.APIKey = apiKey;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alert"></param>
		public void Broadcast(string alert)
		{
			this.Audience = "broadcast";
			this.Alert = alert;
			this.SendReqeust();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alert"></param>
		/// <param name="userIdentifier"></param>
		/// <param name="scheduledAt"></param>
		/// <param name="data"></param>
		public void Notify(string alert, string userIdentifier, DateTime? scheduledAt = null, IDictionary<String, Object> data = null)
		{
			this.Notify(alert, new List<String>() { userIdentifier }, scheduledAt, data);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alert"></param>
		/// <param name="userIdentifiers"></param>
		/// <param name="scheduledAt"></param>
		/// <param name="data"></param>
		public void Notify(string alert, List<String> userIdentifiers, DateTime? scheduledAt = null, IDictionary<String, Object> data = null)
		{
			this.Alert = alert;
			this.UserIdentifiers = userIdentifiers;
			this.ScheduledAt = scheduledAt;
			this.Data = data;
			this.SendReqeust();
		}

		private string BuildParameters()
		{
			dynamic data = new ExpandoObject();

			data.api_key = this.APIKey;
			if (this.Audience == "broadcast")
			{
				data.audience = "broadcast";
			}
			else
			{
				data.user_identifiers = this.UserIdentifiers;
			}
			data.alert = this.Alert;
			if (!String.IsNullOrEmpty(this.Message))
			{
				data.message = this.Message;
			}
			if (this.ScheduledAt != null)
			{
				data.when = this.ScheduledAt;
			}
			if (this.Data != null)
			{
				data.data = this.Data;
			}
			if (this.Badge > 0)
			{
				data.badge = this.Badge;
			}
			if (!String.IsNullOrEmpty(this.APSEnvironment))
			{
				data.aps_environment = this.APSEnvironment;
			}
			if (this.ContentAvailable)
			{
				data.content_available = this.ContentAvailable;
			}

			JavaScriptSerializer serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new JavaScriptConverter[] { new ExpandoJSONContracto() });
			return serializer.Serialize(data);
		}

		private void SendReqeust()
		{
			BuildParameters();

			string url = "https://www.cloudfiveapp.com/push/notify";
			string data = BuildParameters();

			using (WebClient wc = new WebClient())
			{
				wc.Headers[HttpRequestHeader.ContentType] = "application/json";
				string HtmlResult = wc.UploadString(url, data);
			}
		}
	}
}
