﻿# CloudFivePush

Dead simple push notifications. This Nuget package is a simple wrapper around the Cloud Five push notification API. 
With it (and an account on http://cloudfiveapp.com), you can painlessly schedule push notifications to android and ios devices 
from your server.

## Installation

Download the Nuget package at https://www.nuget.org/packages/CloudFivePush

## Usage

Add your API key to your web.config or app.config file (OPTIONAL - if specified here, you can avoid sending your API key to the Notification constructor)

	<configuration>
		<appSettings>
			<add key="CloudFivePushAPIKey" value="YOUR_API_KEY" />
		</appSettings>
	</configuration>

Instantiate the object and send notifications to all devices or a list of devices

	// send a broadcast with API key specified in web.config
	new CloudFivePush.Notification().Broadcast("Your broadcast message here!");

	// instantiate a Notification object with an inline API key and send a broadcast
	CloudFivePush.Notification notification = new CloudFivePush.Notification("YOUR_API_KEY");
	notification.Broadcast("Your broadcast message here!");

	// instantiate a Notification object with an inline API key and send a notification to a specific DEVICE_ID
	CloudFivePush.Notification notification = new CloudFivePush.Notification("YOUR_API_KEY");
	notification.Notify("Your Notification Message", "DEVICE_ID");

Instance methods

	public void Broadcast(string alert)
	public void Notify(string alert, string userIdentifier, DateTime? scheduledAt = null, IDictionary<String, Object> data = null)
	public void Notify(string alert, List<String> userIdentifiers, DateTime? scheduledAt = null, IDictionary<String, Object> data = null)

Optional additional parameters

	notification.Message = "Additional info"   // an additional message that accompanies your alert
	notification.Data = { "key": "value" }     // a JSON dictionary of data to send to the target device(s)
	notification.Badge = 1                     // update the icon badge

See Cloud Five documentation for more details

## iOS Specific features

By default, all iOS notifications will be sent through the production APNs. To use the development service instead:

	notification.APSEnvironment = "development";

You can also set the content-available flag if you have other plugins that rely on it:

	notification.ContentAvailable = 1;

## Contributing

1. Fork it (https://github.com/cloudfive/pushapi-net/fork)
2. Create your feature branch (git checkout -b my-new-feature)
3. Commit your changes (git commit -am 'Add some feature')
4. Push to the branch (git push origin my-new-feature)
5. Create new Pull Request