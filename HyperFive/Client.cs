using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using HyperFive.Game;
using HyperFive.Player;

// ReSharper disable once ClassNeverInstantiated.Global
namespace HyperFive
{
	public class Client : BaseScript
	{
		public static Client ActiveInstance { get; private set; }

		public DecorController Decors { get; }
		public GameController Game { get; }
		public PlayerController Player { get; }
		public WorldController World { get; }
		public SessionManager Sessions { get; }

		public Client() {
			if( ActiveInstance != null ) return; // Only instantiate once

			Decors = new DecorController( this );
			Game = new GameController( this );
			Player = new PlayerController( this );
			World = new WorldController( this );
			Sessions = new SessionManager( this );

			ActiveInstance = this;
		}

		public void RegisterEventHandler( string eventName, Delegate action ) {
			EventHandlers[eventName] += action;
		}

		public void RegisterTickHandler( Func<Task> tick ) {
			Tick += tick;
		}

		public void DeregisterTickHandler( Func<Task> tick ) {
			Tick -= tick;
		}

		public void RegisterExport( string exportName, Delegate callback ) {
			Exports.Add( exportName, callback );
		}

		public dynamic GetExport( string resourceName ) {
			return Exports[resourceName];
		}
	}
}
