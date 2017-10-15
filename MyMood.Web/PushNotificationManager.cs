using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Discover.DomainModel;
using MyMood.Domain;
using PushSharp;
using System.IO;
using PushSharp.Apple;
using Discover.Logging;
using System.Security.Cryptography.X509Certificates;

namespace MyMood.Web
{
    public class PushNotificationManager
    {
        IDomainDataContext db;
        ILogger logger;

        public PushNotificationManager(IDomainDataContext db, ILogger logger)
        {
            this.db = db;
            this.logger = logger;
        }

        public void CheckAndSendNotifications()
        {

            MoodServer server = this.db.Get<MoodServer>().Where(s => s.Name.Equals(Configuration.WebConfiguration.ServerName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (server != null && server.CanPushClientNotifcations)
            {
                //todo check if server can send push
                var now = DateTime.UtcNow;
                var notifications = this.db.Get<PushNotification>().Where(p => p.Sent == false && p.SendDate <= now).OrderBy(p => p.SendDate);


                if (notifications.Any())
                {
                    //Create our service	
                    PushService push = new PushService();

                    //Wire up the events
                    push.Events.OnNotificationSendFailure += new PushSharp.Common.ChannelEvents.NotificationSendFailureDelegate(Events_OnNotificationSendFailure);
                    push.Events.OnNotificationSent += new PushSharp.Common.ChannelEvents.NotificationSentDelegate(Events_OnNotificationSent);


                    //Configure and start Apple APNS
                    // IMPORTANT: Make sure you use the right Push certificate.  Apple allows you to generate one for connecting to Sandbox,
                    //   and one for connecting to Production.  You must use the right one, to match the provisioning profile you build your
                    //   app with!
                    //var appleCert = File.ReadAllBytes(Configuration.WebConfiguration.APNSCertificatePath);


                    var appleCert = new X509Certificate2(File.ReadAllBytes(Configuration.WebConfiguration.APNSCertificatePath), "d1scov3r!", X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                    //IMPORTANT: If you are using a Development provisioning Profile, you must use the Sandbox push notification server 
                    //  (so you would leave the first arg in the ctor of ApplePushChannelSettings as 'false')
                    //  If you are using an AdHoc or AppStore provisioning profile, you must use the Production push notification server
                    //  (so you would change the first arg in the ctor of ApplePushChannelSettings to 'true')
                    //push.StartApplePushService(new ApplePushChannelSettings(appleCert, "d1scov3r!"));
                    push.StartApplePushService(new ApplePushChannelSettings(appleCert));
                    foreach (var notification in notifications)
                    {
                        this.logger.Info(this.GetType(), string.Format("Sending APNS notification - {0}", notification.Message));
                        //set sent first so doesn't repeat if goes wrong
                        notification.Sent = true;
                       

                        var recipients = this.db.Get<Responder>().Where(r => r.Event.Id == notification.Event.Id);
                        foreach (var recipient in recipients)
                        {

                            //Fluent construction of an iOS notification
                            //IMPORTANT: For iOS you MUST MUST MUST use your own DeviceToken here that gets generated within your iOS app itself when the Application Delegate
                            //  for registered for remote notifications is called, and the device token is passed back to you
                            push.QueueNotification(NotificationFactory.Apple()
                                .ForDeviceToken(recipient.DeviceId)
                                .WithAlert(notification.Message)
                                .WithSound(notification.PlaySound ? "default" : "")
                                );
                        }


                    }
                    this.db.SaveChanges();
                    //Stop and wait for the queues to drains
                    push.StopAllServices(true);
                }
            }
        }

        static void Events_OnNotificationSent(PushSharp.Common.Notification notification)
        {
            Console.WriteLine("Sent: " + notification.Platform.ToString() + " -> " + notification.ToString());
        }

        static void Events_OnNotificationSendFailure(PushSharp.Common.Notification notification, Exception notificationFailureException)
        {
            Console.WriteLine("Failure: " + notification.Platform.ToString() + " -> " + notificationFailureException.Message + " -> " + notification.ToString());
        }

    }
}