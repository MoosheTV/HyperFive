using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using HyperFive.Util;

namespace HyperFive.Game
{
	public class PlayerController : ClientAccessor
	{
		public Ped PlayerPed { get; private set; }

		private Vector3 _pos = Vector3.Zero;
		/// <summary>
		/// The position of the player character.
		/// </summary>
		public Vector3 Position
		{
			get => _pos;
			set {
				PlayerPed.Position = value;
				_pos = value;
			}
		}

		private int _voiceChannel;
		/// <summary>
		/// The voice channel the player is in.
		/// </summary>
		public int VoiceChannel
		{
			get => _voiceChannel;
			set {
				if( value <= 0 ) {
					Function.Call( Hash.NETWORK_CLEAR_VOICE_CHANNEL );
				}
				else {
					Function.Call( Hash.NETWORK_SET_VOICE_CHANNEL, _voiceChannel );
				}
				_voiceChannel = Math.Max( 0, value );
			}
		}

		public PlayerController( Client client ) : base( client ) {
			client.RegisterTickHandler( CacheTick );
		}

		private async Task CacheTick() {
			try {
				PlayerPed = CitizenFX.Core.Game.PlayerPed;
				_pos = PlayerPed.Position;
			}
			catch( Exception ex ) {
				Log.Error( ex );
				await BaseScript.Delay( 1000 );
			}
		}
	}


	internal class CachedPlayerPosition : CachedValue<Vector3>
	{

		public CachedPlayerPosition() : base( 1 ) {
		}

		protected override Vector3 Update() {
			return CitizenFX.Core.Game.PlayerPed?.Position ?? Vector3.Zero;
		}
	}

	internal class CachedPlayerPed : CachedValue<Ped>
	{
		public CachedPlayerPed() : base( 1 ) {
		}

		protected override Ped Update() {
			return CitizenFX.Core.Game.PlayerPed;
		}
	}
}
