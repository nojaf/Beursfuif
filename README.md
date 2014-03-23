# Beursfuif
=========

Beursfuif program, written in C#/XAML.

Created by [Nojaf](http://nojaf.com)

All rights reserved.

## Running from Visual Studio

Make sure you start Visual Studio (2012 or 2013) as Administrator. The server application needs to run with elevated permissions. This is because the server uses Owin Selfhost.

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

## Some remarks
- If you want to use the server in your local network, be sure to disable your Windows firewall.
- Browser support: IE10+, Chrome, Opera and Firefox.
- Start beamer.html to open the projection client.
- When opening the solution, you might get an error that one project wasn't found. The setup project has been excluded in the repository.

## Permission to use for a real party
I have no problem if anyone wants to use this application in real life. However, be a gentlemen and kindly ask for permission!