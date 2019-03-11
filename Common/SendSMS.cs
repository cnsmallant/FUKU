using EFClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;


public class SendSMS
{
    D8MallEntities db = new D8MallEntities();
    public  string SendSms(string phone, string content, string code)
    {
        StringBuilder sms = new StringBuilder();
        sms.AppendFormat("name={0}", "18562516441");
        sms.AppendFormat("&pwd={0}", "55182B7F1DB7A0B2C0171383A86F");//登陆平台，管理中心--基本资料--接口密码（28位密文）；复制使用即可。
        sms.AppendFormat("&content={0}", content);
        sms.AppendFormat("&mobile={0}", phone);
        sms.AppendFormat("&sign={0}", "");// 公司的简称或产品的简称都可以
        sms.Append("&type=pt");
        string resp = PushToWeb("http://web.cr6868.com/asmx/smsservice.aspx", sms.ToString(), Encoding.UTF8);
        string[] msg = resp.Split(',');
        if (msg[0] == "0")
        {
            Sms s = new Sms();
            s.SendID = msg[1];
            s.SendToPhone = phone;
            s.SendStatus = "提交成功：SendID=" + msg[1];
            s.SendContent = content;
            s.SendDate = DateTime.Now;
            db.Sms.Add(s);
            db.SaveChanges();
            return code;

        }
        else
        {
            Sms s = new Sms();
            s.SendID = msg[1];
            s.SendToPhone = phone;
            s.SendStatus = "提交失败：SendID=" + msg[1];
            s.SendContent = content;
            s.SendDate = DateTime.Now;
            db.Sms.Add(s);
            db.SaveChanges();
            return "NO";
        }
    }
    private string PushToWeb(string weburl, string data, Encoding encode)
    {
        byte[] byteArray = encode.GetBytes(data);
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(weburl));
        webRequest.Method = "POST";
        webRequest.ContentType = "application/x-www-form-urlencoded";
        webRequest.ContentLength = byteArray.Length;
        Stream newStream = webRequest.GetRequestStream();
        newStream.Write(byteArray, 0, byteArray.Length);
        newStream.Close();

        //接收返回信息：
        HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
        StreamReader aspx = new StreamReader(response.GetResponseStream(), encode);
        return aspx.ReadToEnd();
    }

}

