using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using Hyperfive.Server.SharedModels;
using HyperFive.Util;
using Newtonsoft.Json;

namespace Hyperfive.Server.Player
{
	public class SessionManager : ServerAccessor
	{
		private readonly List<Session> _sessions = new List<Session>();

		/// <summary>
		/// A thread-safe, readonly collection of the current active Sessions.
		/// </summary>
		public IReadOnlyList<Session> SessionList => new List<Session>( _sessions );

		public SessionManager( Server server ) : base( server ) {
			server.RegisterEventHandler( "playerConnecting", new Action<CitizenFX.Core.Player>( OnSessionPreLoad ) );
			server.RegisterEventHandler( "playerDropped", new Action<CitizenFX.Core.Player, string>( OnPlayerDropped ) );
			server.RegisterEventHandler( "Session.Loaded", new Action<CitizenFX.Core.Player>( OnSessionLoaded ) );
		}

		/// <summary>
		/// Gets the session associated with this player.
		/// </summary>
		/// <param name="source">The player source.</param>
		/// <returns>The session associated with this player, or null, if there is no session associated.</returns>
		public Session FromPlayer( CitizenFX.Core.Player source ) {
			return _sessions.FirstOrDefault( s => int.TryParse( source.Handle, out var netId ) && s.NetId == netId );
		}

		private void OnPlayerDropped( [FromSource] CitizenFX.Core.Player player, string reason ) {
			try {
				var session = FromPlayer( player );
				if( session == null ) return;

				_sessions.Remove( session );

				Log.Info( $"Player {session.Name} (net:{session.NetId}) has disconnected. ({reason})" );
				BaseScript.TriggerClientEvent( "Session.Drop", session.NetId );
			}
			catch( Exception ex ) {
				Log.Error( ex );
			}
		}

		private void OnSessionLoaded( [FromSource] CitizenFX.Core.Player player ) {
			try {
				var session = FromPlayer( player );
				if( session == null ) return;

				foreach( var p in new PlayerList() ) {
					var data = new SessionDataModel {
						Name = session.Name,
						NetId = session.NetId,
						SharedData = session.SharedData
					};
					if( p.Handle == player.Handle ) {
						data.ProtectedData = session.ProtectedData;
					}
					p.TriggerEvent( "Session.Join", session.NetId, JsonConvert.SerializeObject( data ) );
				}
				Log.Verbose( $"Player {session.Name} has finished loading." );
			}
			catch( Exception ex ) {
				Log.Error( ex );
			}
		}

		private void OnSessionPreLoad( [FromSource] CitizenFX.Core.Player player ) {
			try {
				var session = new Session( player );
				if( Server.IsSteamRequired && session.SteamId64 == 0 ) {
					player.Drop( "Steam is required to play on this server." );
					return;
				}

				_sessions.Add( session );
				Log.Info( $"Player {session.Name} (net:{session.NetId}) has connected." );
			}
			catch( Exception ex ) {
				Log.Error( ex );
			}
		}
	}
}
