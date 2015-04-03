using System;
using System.Collections.Generic;
using System.Configuration;
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

		/// <summary>
		/// Initialize the Notification object, "CloudFivePushAPIKey" entry required in your web.config
		/// </summary>
		public Notification()
		{
			string apiKey = ConfigurationManager.AppSettings["CloudFivePushAPIKey"].ToString();
			if (string.IsNullOrEmpty(apiKey))
			{
				throw new Exception("API key is not defined. \"CloudFivePushAPIKey\" not found in web.config app settings.");
			}

			this.APIKey = apiKey;
		}

		/// <summary>
		/// Initialize the Notification object and provide your API key
		/// </summary>
		/// <param name="apiKey">Your API key provided from cloudfiveapp.com</param>
		public Notification(string apiKey)
		{
			this.APIKey = apiKey;
		}

		/// <summary>
		/// Send a message to all registered devices
		/// </summary>
		/// <param name="alert">The message you want to send to all devices</param>
		public void Broadcast(string alert)
		{
			this.Audience = "broadcast";
			this.Alert = alert;
			this.SendReqeust();
		}

		/// <summary>
		/// Send a notification to an individual device
		/// </summary>
		/// <param name="alert">The message you want to send to the specified device</param>
		/// <param name="userIdentifier">The unique key you specified in the app</param>
		/// <param name="scheduledAt">The optional date/time you want the notification sent - if not supplied, the notification will be sent immediately</param>
		/// <param name="data">An optional set of key/value pairs to send down to the device</param>
		public void Notify(string alert, string userIdentifier, DateTime? scheduledAt = null, IDictionary<String, Object> data = null)
		{
			this.Notify(alert, new List<String>() { userIdentifier }, scheduledAt, data);
		}

		/// <summary>
		/// Send a notification to a list of devices
		/// </summary>
		/// <param name="alert">The message you want to send to the specified devices</param>
		/// <param name="userIdentifiers">A list of unique keys you specified in the app</param>
		/// <param name="scheduledAt">The optional date/time you want the notification sent - if not supplied, the notification will be sent immediately</param>
		/// <param name="data">An optional set of key/value pairs to send down to the device</param>
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

			try
			{
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
			catch (Exception ex)
			{
				throw new Exception("Error occurred while building JSON payload: " + ex.Message);
			}
		}

		private void SendReqeust()
		{
			BuildParameters();

			string url = "https://www.cloudfiveapp.com/push/notify";
			string data = BuildParameters();

			try
			{
				using (WebClient wc = new WebClient())
				{
					wc.Headers[HttpRequestHeader.ContentType] = "application/json";
					string HtmlResult = wc.UploadString(url, data);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error occurred while attempting to send the request: " + ex.Message);
			}
		}
	}
}