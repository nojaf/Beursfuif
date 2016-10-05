# Beursfuif

Beursfuif program, written in C#/XAML.

Created by [nojaf](http://nojaf.com)

All rights reserved.

## Running from Visual Studio

Execute the netsh commands first. More info [below](#server-and-client)

## Screenshots

> Splash screen

![Splash screen](http://i.imgur.com/y2GQsQK.png)

> Enter drinks with images and prices.

![Drinks](http://i.imgur.com/IthI3FJ.jpg)

> Set up the interval, the prices always change at the beginning of a new interval.

![Intervals](http://i.imgur.com/tEELPqS.jpg)

> Signalr server is running and html5 clients can connect

![Connected clients](http://i.imgur.com/pS2XIzs.jpg)


> Start the client application in a modern browser, hit F11 and you won't ever notice your in a browser

![Client view](http://i.imgur.com/ODp332O.jpg)


> Real time updates in nice graph
![Stats](http://i.imgur.com/aAp8WBs.jpg)

## Under the hood

The server uses Xaml, Mvvm light, Signalr (Owin Selfhost)

The client is built with AngularJs and Typescript

## Download the latest release

Binaries have been attached to the latest release (2.6.0).
You should be able to run the application with these files.

### 
- Extract the zip
- Open a command promt as administrator.

```cmd
C:\Windows\System32>rem "Find your username"
C:\Windows\System32>whoami
your-domain\your-users

C:\Windows\System32>rem "Register urls with port"
C:\Windows\System32>netsh http add urlacl url=http://+:5678/ user=your-domain\your-users
```

- Open Beursfuif.Server.exe
- Configure drinks and interval
- Start party

### Server and Client

- Open a command promt as administrator.

```cmd
C:\Windows\System32>rem "Add an incoming firewall rule"
C:\Windows\System32>netsh advfirewall firewall add rule name="Open Port 5678" dir=in action=allow protocol=TCP localport=5678

C:\Windows\System32>rem "Add an outgoing firewall rule"
C:\Windows\System32>netsh advfirewall firewall add rule name="Open Port 5678" dir=out action=allow protocol=TCP localport=5678
```

- Browse to the urls displayed in the Settings pane on the server.

## Remarks
- Browser support: IE10+, Chrome, Opera and Firefox.
- Start beamer.html to open the projection client.

## Permission to use for a real party
I have no problem if anyone wants to use this application in real life. However, be a gentlemen and kindly ask for permission!