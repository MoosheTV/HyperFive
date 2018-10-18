using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using Newtonsoft.Json;

namespace Hyperfive.Server.Player
{
	public class Session
	{
		public int NetId { get; }
		public CitizenFX.Core.Player Player { get; }

		/// <summary>
		/// The HEX value of the Steam Identifier, or String.Empty if no steam identifier.
		/// </summary>
		public string SteamIdentifier { get; }

		/// <summary>
		/// The 64-bit identifier of the player, or 0 if no steam identifier.
		/// </summary>
		public ulong SteamId64 { get; }

		/// <summary>
		/// The license ID associated with their copy of Grand Theft Auto.
		/// </summary>
		public string LicenseId { get; }

		private string _name;
		/// <summary>
		/// The display name of the user, which by default is the FiveM Player Name.
		/// This can be updated, unlike the FiveM Player Name. This is generally used
		/// internally for commands and messages.
		/// </summary>
		public string Name
		{
			get => _name;
			set {
				_name = value;
				BaseScript.TriggerClientEvent( "Session.UpdateName", NetId, value );
			}
		}

		/// <summary>
		/// The UTC timestamp of when this session was created.
		/// </summary>
		public readonly DateTime TimeJoined = DateTime.UtcNow;

		protected internal readonly Dictionary<string, dynamic> SharedData = new Dictionary<string, dynamic>();
		protected internal readonly Dictionary<string, dynamic> ProtectedData = new Dictionary<string, dynamic>();

		public Session( CitizenFX.Core.Player player, Dictionary<string, dynamic> sharedData = null, Dictionary<string, dynamic> protectedData = null ) {
			NetId = int.Parse( player.Handle );
			Player = player;
			var hasSteam = player.Identifiers.Any( p => p.StartsWith( "steam:" ) );
			SteamIdentifier = hasSteam ? player.Identifiers["steam"].Replace( "steam:", "" ) : "";
			SteamId64 = hasSteam ? Convert.ToUInt64( SteamIdentifier, 16 ) : 0;

			LicenseId = player.Identifiers["license"].Replace( "license:", "" );

			if( sharedData != null ) {
				foreach( var kvp in sharedData ) {
					SharedData.Add( kvp.Key, kvp.Value );
				}
			}
			if( protectedData != null ) {
				foreach( var kvp in protectedData ) {
					ProtectedData.Add( kvp.Key, kvp.Value );
				}
			}

			_name = Player.Name;
		}

		public void SetProtectedData( string key, dynamic value ) {
			SetProtectedData( new Dictionary<string, dynamic> { { key, value } } );
		}

		public void SetProtectedData( Dictionary<string, dynamic> pairs ) {
			foreach( var kvp in pairs ) {
				ProtectedData[kvp.Key] = kvp.Value;
			}

			TriggerEvent( "Session.UpdateProtectedData", NetId, JsonConvert.SerializeObject( pairs ) );
		}

		/// <summary>
		/// Gets the protected data value, or defaultValue if none exists. This data is
		/// only available between the server and this session's client.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public dynamic GetProtectedData( string key, dynamic defaultValue = null ) {
			if( ProtectedData.ContainsKey( key ) ) {
				return ProtectedData[key];
			}

			if( defaultValue != null ) {
				SetProtectedData( key, defaultValue );
			}

			return defaultValue;
		}

		/// <summary>
		/// Sets the value to the data which is shared between all clients. Keep in mind that
		/// these values should never contain sensitive details.
		/// </summary>
		/// <param name="key">The key value</param>
		/// <param name="value">The value to pair to the key. Intended only for native data types.</param>
		public void SetSharedData( string key, dynamic value ) {
			SetSharedData( new Dictionary<string, dynamic> { { key, value } } );
		}

		/// <summary>
		/// Adds the given values to the data dictionary which is shared between all clients.
		/// Keep in mind that these values should never contains sensitive details.
		/// </summary>
		/// <param name="pairs">The data dictionary to add</param>
		public void SetSharedData( Dictionary<string, dynamic> pairs ) {
			foreach( var kvp in pairs ) {
				SharedData[kvp.Key] = kvp.Value;
			}

			BaseScript.TriggerClientEvent( "Session.UpdateSharedData", NetId, JsonConvert.SerializeObject( pairs ) );
		}

		/// <summary>
		/// Gets all shared values associated with this session, and shared between all clients.
		/// </summary>
		/// <returns>The dictionary of values stored.</returns>
		public IReadOnlyDictionary<string, dynamic> GetSharedData() {
			return new Dictionary<string, dynamic>( SharedData );
		}

		/// <summary>
		/// Gets the shared value associated with this session, and shared between all clients.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public dynamic GetSharedData( string key, dynamic defaultValue = null ) {
			if( SharedData.ContainsKey( key ) ) {
				return SharedData[key];
			}

			if( defaultValue != null ) {
				SetSharedData( key, defaultValue );
			}

			return defaultValue;
		}

		public void Drop( string reason ) {
			Player.Drop( reason );
		}

		public void TriggerEvent( string eventName, params object[] args ) {
			Player.TriggerEvent( eventName, args );
		}
	}
}
