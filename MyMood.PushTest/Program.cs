using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using PushSharp;
using PushSharp.Apple;
using PushSharp.Windows;
using CsvHelper;

namespace MyMood.PushTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create our service	
			PushService push = new PushService();

			//Wire up the events
			push.Events.OnDeviceSubscriptionExpired += new PushSharp.Common.ChannelEvents.DeviceSubscriptionExpired(Events_OnDeviceSubscriptionExpired);
            push.Events.OnDeviceSubscriptionIdChanged += new PushSharp.Common.ChannelEvents.DeviceSubscriptionIdChanged(Events_OnDeviceSubscriptionIdChanged);
            push.Events.OnChannelException += new PushSharp.Common.ChannelEvents.ChannelExceptionDelegate(Events_OnChannelException);
            push.Events.OnNotificationSendFailure += new PushSharp.Common.ChannelEvents.NotificationSendFailureDelegate(Events_OnNotificationSendFailure);
            push.Events.OnNotificationSent += new PushSharp.Common.ChannelEvents.NotificationSentDelegate(Events_OnNotificationSent);
            push.Events.OnChannelCreated += new PushSharp.Common.ChannelEvents.ChannelCreatedDelegate(Events_OnChannelCreated);
            push.Events.OnChannelDestroyed += new PushSharp.Common.ChannelEvents.ChannelDestroyedDelegate(Events_OnChannelDestroyed);

            //Configure and start Apple APNS
            // IMPORTANT: Make sure you use the right Push certificate.  Apple allows you to generate one for connecting to Sandbox,
            //   and one for connecting to Production.  You must use the right one, to match the provisioning profile you build your
            //   app with!
            var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Resources/myMood-prod-push-cert.p12"));

            //IMPORTANT: If you are using a Development provisioning Profile, you must use the Sandbox push notification server 
            //  (so you would leave the first arg in the ctor of ApplePushChannelSettings as 'false')
            //  If you are using an AdHoc or AppStore provisioning profile, you must use the Production push notification server
            //  (so you would change the first arg in the ctor of ApplePushChannelSettings to 'true')
            push.StartApplePushService(new ApplePushChannelSettings(appleCert, "d1scov3r!"));

            //push.QueueNotification(NotificationFactory.Apple()
            //           .ForDeviceToken("14e9c79db41cf4aa205cb72e7d60cede573cfa1867f1427ddd7372ef19c29b3a")
            //           .WithAlert("Remember to send yourself a myMood report before handing back the iPad")
            //           .WithSound("default")
            //           );


            using (var csv = new CsvHelper.CsvReader(new StreamReader(typeof(Device).Assembly.GetManifestResourceStream(typeof(Device).Assembly.GetName().Name + ".devices.csv"))))
            {
                while (csv.Read())
                {
                    var device = csv.GetRecord<Device>();
                    //Fluent construction of an iOS notification
                    //IMPORTANT: For iOS you MUST MUST MUST use your own DeviceToken here that gets generated within your iOS app itself when the Application Delegate
                    //  for registered for remote notifications is called, and the device token is passed back to you
                    push.QueueNotification(NotificationFactory.Apple()
                        .ForDeviceToken(device.DeviceId)
                        .WithAlert("Remember to send yourself a myMood report before handing back the iPad!")
                        //.WithSound("default")
                        );
                }

            }


           
          

            

			//Stop and wait for the queues to drains
			push.StopAllServices(true);

			Console.WriteLine("Queue Finished, press return to exit...");
			Console.ReadLine();			
		}

        static void Events_OnDeviceSubscriptionIdChanged(PushSharp.Common.PlatformType platform, string oldDeviceInfo, string newDeviceInfo, PushSharp.Common.Notification notification)
		{
			//Currently this event will only ever happen for Android GCM
			Console.WriteLine("Device Registration Changed:  Old-> " + oldDeviceInfo + "  New-> " + newDeviceInfo);
		}

        static void Events_OnNotificationSent(PushSharp.Common.Notification notification)
		{
			Console.WriteLine("Sent: " + notification.Platform.ToString() + " -> " + notification.ToString());
		}

        static void Events_OnNotificationSendFailure(PushSharp.Common.Notification notification, Exception notificationFailureException)
		{
			Console.WriteLine("Failure: " + notification.Platform.ToString() + " -> " + notificationFailureException.Message + " -> " + notification.ToString());
		}

        static void Events_OnChannelException(Exception exception, PushSharp.Common.PlatformType platformType, PushSharp.Common.Notification notification)
		{
			Console.WriteLine("Channel Exception: " + platformType.ToString() + " -> " + exception.ToString());
		}

        static void Events_OnDeviceSubscriptionExpired(PushSharp.Common.PlatformType platform, string deviceInfo, PushSharp.Common.Notification notification)
		{
			Console.WriteLine("Device Subscription Expired: " + platform.ToString() + " -> " + deviceInfo);
		}

        static void Events_OnChannelDestroyed(PushSharp.Common.PlatformType platformType, int newChannelCount)
		{
			Console.WriteLine("Channel Destroyed for: " + platformType.ToString() + " Channel Count: " + newChannelCount);
		}

        static void Events_OnChannelCreated(PushSharp.Common.PlatformType platformType, int newChannelCount)
		{
			Console.WriteLine("Channel Created for: " + platformType.ToString() + " Channel Count: " + newChannelCount);
		}
        

    }

    public class Device
    {
        public string DeviceId { get; set; }
    }
}
