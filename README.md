<h2><b>Weather By Text</b></h2>

This project uses the Gmail API to read a specific Gmail address that i have set up for this, it reads all incoming email massages that are Unread and when it gets a massage with only a 1 it automatically reply's with the weather for Today if you send only the number 2 it will reply with the weather for today and tomorrow if you send 3 it will give you 3 day forecast.
The program parse the weather for 5 days so you can easily add more then 3 days up to 5. 
You can also easily adjust what need to be send in order to get a reply.

After the reply with the weather is send, it marks the email as read.

<h4><b>What you need</b></h4>
You must have a Gmail Account set up to use with Gmail API and you need to place the "client_secret.jason" file in the root of your project. You can refare to this link for how to set up Gmail API https://developers.google.com/gmail/api/guides/.
And that's all you need.

<h4><b>To run </b></h4>
After you have set up Gmail API and you have the "client_secret.jason" file in the root of your project "/yourProject/bin/debug", You need to change the property of the file to Always Copy. 


<br />
For sending the reply email i used SMTP Gmail Client. So in the GetWeather.cs file where it say's in the code YourGmailAddress replace with your email address and YourPassword replace with your Gmail Password and Name replace with your gmail name.


I used a physical path to save the weather in a file so you need to change it to your path.

Then you can Build it end Run it.

<h4><b>Additional Info</b></h4>
<ul>
<li>It uses Gmail API to read Inbox Unread and to mark as read after reply.
<li>It uses SMTP Client to reply.</li>
<li>It parse the weather every 20 minutes.</li>
<li>It reads Gmail Inbox every 2 Seconds.</li>
<li>It writes to a log file every time it parse the weather and every time it sends the weather and when there is a error.</li>
<li>It has an additional Program for when ever it fails it will just restart it.</li>
<li><b>It does not open a console window.</b></li>
</ul>
