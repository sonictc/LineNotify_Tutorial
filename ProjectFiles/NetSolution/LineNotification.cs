#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.UI;
using FTOptix.Retentivity;
using FTOptix.NativeUI;
using FTOptix.Core;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
using FTOptix.CommunicationDriver;
using FTOptix.Modbus;
using System.Net.Http;
using System.Text;
using System.Net;
using System.IO;
using FTOptix.Alarm;
using FTOptix.DataLogger;
using FTOptix.Store;
using FTOptix.WebUI;
#endregion

public class LineNotification : BaseNetLogic
{
    public override void Start()
    {
       
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

 

    [ExportMethod]
    public void SendLineNotify(string alertMessage,string clearMessage, bool alarmActive , 
    int alertStickerPackageId,int alertStickerId, int clearStickerPackageId,int clearStickerId,bool useAlertSticker,
    bool useClearSticker, bool allowSendingAlertMsg,bool allowSendingClearMsg)
    {
       
    
        string alertPayload = string.Format("message={0}", Uri.EscapeDataString(alertMessage));
        string alertSticker = string.Format("&stickerPackageId={0}&stickerId={1}",alertStickerPackageId,alertStickerId);
        if(useAlertSticker){
        alertPayload = alertPayload+alertSticker;
        }

        string clearPayload = string.Format("message={0}", Uri.EscapeDataString(clearMessage));
        string clearSticker = string.Format("&stickerPackageId={0}&stickerId={1}",clearStickerPackageId,clearStickerId);
        if(useClearSticker){
        clearPayload = clearPayload+clearSticker;
        }

        if(alarmActive & allowSendingAlertMsg){
            byte[] data = Encoding.UTF8.GetBytes(alertPayload);
            HTTPRequest(data);
        } else if(allowSendingClearMsg) {
            byte[] data = Encoding.UTF8.GetBytes(clearPayload);
            HTTPRequest(data);
        }
    }

   public void HTTPRequest(byte[] data){
        
        string accessToken = Project.Current.GetVariable("NetLogic/LineAccessToken").Value;
        //string accessToken = "bXVKuoork7L8GkOsykDt6J5VkUSXic7mCNfscbSi7d3";
        string url = "https://notify-api.line.me/api/notify";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responseString = reader.ReadToEnd();
                Log.Info(responseString);
            }
        }
        catch (WebException ex)
        {
            Log.Error(ex.Message);
        }

    }
}

