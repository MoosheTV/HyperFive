# HyperFive
A FiveM resource framework written in C#.

This project is still under development and there's no public release yet, however, this will give structure to resources written in C#, and hopefully encourage people to explore the development of FiveM resources in C#.

____

## :warning: PLEASE READ BEFORE CONTINUING :warning:
This framework has been discontinued. The framework was never finished, and doesn't run without some serious tweaking. I am archiving this repository for a number of reasons:
- There is no point of having a 3rd party basic framework like this; the majority of the changes made have been submitted to the main FiveM repository.
- The SessionManager API is currently broken (I believe the issue is related to steam identifiers, but I've not confirmed this).
- I don't have the time to dedicate working on this.

This repository *does* serve as a learning experience, which is why I have archived it. This repository serves as a good foundation to other would-be frameworks which expand on the basic nature of this one, which may include special game modes (such as roleplay or something entirely different).

I may release a fully fledged framework some day, but *this* repository is not it.

____

## Basic Structure

### Client
The Client BaseScript is a singleton instance which contains controllers which can be used to manipulate the GTA world via FiveM. This is a unified approach to ensure that only one script is taking control over a certain value without interrupting another script's behavior.

A lot of the controllers are wrappers around FiveM's pre-existing external wrappers, however values are cached and controlled through one endpoint. For example, instead of having multiple scripts which interact with GTA through natives, this is one interface which interacts with GTA, and the scripts which use this framework interact with this interface.

A common example of conflict between two script behaviors would be the disabling/enabling of controls and components.

Through HyperFive, all you would need to do to disable/enable a HUD component is calling this method once:
```cs
Client.Game.HideComponent( HudComponent.Cash );
```

There is no need to attempt to disable the component each frame manually as this interface will automatically do it for you.

### Server
The Server BaseScript is structured similarly to the Client BaseScript: you have `ServerAccessor`s, which serve as interfaces for scripts to interact with instead of calling natives directly.


### Session Manager
Along with this, there is the `SessionManager`, which is a neat and easy way to keep track of players, as well as data and states throughout the means of `ProtectedData` and `SharedData`.

#### Data Shared Between All Sessions

Here is an example of assigning a value which is shared between all clients to a session:
```cs
var session = Server.Sessions.FromPlayer( serverId );
session.SetSharedData( "SpeedMultiplier", 1.75f );
```
this will automatically assign that value associated with that Session, as well as update all other Sessions to allow them to see that value client-side.

----

#### Data Shared Between Server And Session

Here is an example of assigning a value which we only want shared between one session and the server:
```cs
var session = Server.Sessions.FromPlayer( serverId );
session.SetProtectedData( "Bank.Cash", 65536 );
```
This will set the value where it can only be seen by the session which owns that protected data, and the server.

These values can only be edited server-side. This ensures the integrity of the data. If you need to have the client set a data value, you can do so with events:

#### Client-Side:
```cs
var playerPos = Client.Player.Position;
BaseScript.TriggerServerEvent( "Player.CollectPosition", playerPos.X, playerPos.Y, playerPos.Z );
```

#### Server-Side:
```cs
Server.RegisterEvent( "Player.CollectPosition", new Action<Player, float, float, float>( OnClientData ) );

...

private void OnClientData([FromSource] Player source, float x, float y, float z) {
  var session = Server.Sessions.FromPlayer( source );
  session?.SetProtectedData( "Position", new float [] {x, y, z} );
}
```

-----

### Support

Thanks for taking your time to check out this project. If you'd like to support more projects of mine in the future, go check out my [Patreon](https://patreon.com/mooshe) where I post exclusive GTA mods and early access tutorials.
