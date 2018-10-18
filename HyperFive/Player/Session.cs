using System;
using System.Collections.Generic;
using CitizenFX.Core.Native;
using Hyperfive.Server.SharedModels;
using HyperFive.Util;
using Newtonsoft.Json;

namespace HyperFive.Player
{
	public class Session
	{
		public int NetId { get; }
		public CitizenFX.Core.Player Player => new CitizenFX.Core.Player( API.GetPlayerFromServerId( NetId ) );
		/// <summary>
		/// A display name which is allowed to be changed by the server. By default it will be the player's FiveM name.
		/// </summary>
		public string Name { get; protected internal set; }

		private readonly Dictionary<string, dynamic> _protectedData = new Dictionary<string, dynamic>();
		private readonly Dictionary<string, dynamic> _sharedData = new Dictionary<string, dynamic>();

		/// <summary>
		/// The time the session was created in UTC. Not accurate to the exact time they connected as this is dependent
		/// on the time it takes for them to load in.
		/// </summary>
		public readonly DateTime JoinTime = DateTime.UtcNow;

		public Session( int netId, SessionDataModel model ) {
			NetId = netId;

			Name = model.Name;

			foreach( var kvp in model.ProtectedData ) {
				_protectedData[kvp.Key] = kvp.Value;
			}

			foreach( var kvp in model.SharedData ) {
				_sharedData[kvp.Key] = kvp.Value;
			}
		}

		/// <summary>
		/// Gets data which is shared between all sessions on the server.
		/// </summary>
		/// <param name="key">The key associated with the data.</param>
		/// <param name="defaultValue">The default value to return if the key was not found.</param>
		/// <returns>The value paired with the key, or the default value if not found.</returns>
		public dynamic GetSharedData( string key, dynamic defaultValue = null ) {
			return _sharedData.ContainsKey( key ) ? _sharedData[key] : defaultValue;
		}

		protected internal void UpdateSharedData( string data ) {
			try {
				var dict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>( data );
				foreach( var kvp in dict ) {
					_sharedData[kvp.Key] = kvp.Value;
				}
			}
			catch( Exception ex ) {
				Log.Error( ex );
			}
		}

		/// <summary>
		/// Gets data which is only shared between this Client and the server.
		/// </summary>
		/// <param name="key">The key associated with the data.</param>
		/// <param name="defaultValue">The default value to return if the key was not found.</param>
		/// <returns>The value paired with the key, or the default value if not found.</returns>
		public dynamic GetProtectedData( string key, dynamic defaultValue = null ) {
			return _protectedData.ContainsKey( key ) ? _protectedData[key] : defaultValue;
		}

		protected internal void UpdateProtectedData( string data ) {
			try {
				var dict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>( data );
				foreach( var kvp in dict ) {
					_protectedData[kvp.Key] = kvp.Value;
				}
			}
			catch( Exception ex ) {
				Log.Error( ex );
			}
		}

	}
}
